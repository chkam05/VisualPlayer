using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Controls;

public partial class PlayerMiniControl : PlayerControl
{
    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<PlayerMiniControl, bool> IsCoverVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(nameof(IsCoverVisible), control => control.IsCoverVisible);

    public static readonly DirectProperty<PlayerMiniControl, bool> AreMetadataLabelsVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(
            nameof(AreMetadataLabelsVisible),
            control => control.AreMetadataLabelsVisible);

    public static readonly DirectProperty<PlayerMiniControl, bool> IsMiniVisualizationVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(nameof(IsMiniVisualizationVisible), control => control.IsMiniVisualizationVisible);

    #endregion AVALONIA PROPERTIES

    private ContextMenu? _contextMenu;
    private MenuItem? _closeApplicationMenuItem;
    private MenuItem? _fullPlayerMenuItem;
    private bool _isCoverVisible;
    private bool _isMiniVisualizationVisible;
    private bool _areMetadataLabelsVisible;
    private MiniPlayerConfig? _miniPlayerConfig;
    private MenuItem? _nextMenuItem;
    private MenuItem? _pauseMenuItem;
    private MenuItem? _playMenuItem;
    private MenuItem? _previousMenuItem;
    private MenuItem? _stopMenuItem;
    private MenuItem[] _visualizationMenuItems = [];
    private Border? _resizeBorder;
    private IPointer? _resizePointer;
    private Window? _resizeWindow;
    private bool _isResizingFromLeft;
    private double _resizeScreenUnitsPerDip;
    private PixelPoint _resizeStartPointerPosition;
    private PixelPoint _resizeStartWindowPosition;
    private double _resizeStartWidth;

    #region EVENTS

    public event EventHandler<PointerPressedEventArgs>? DragRequested;
    public event EventHandler? CloseApplicationRequested;

    #endregion EVENTS

    #region PROPERTIES

    public bool IsCoverVisible
    {
        get => _isCoverVisible;
        private set => SetAndRaise(IsCoverVisibleProperty, ref _isCoverVisible, value);
    }

    public bool AreMetadataLabelsVisible
    {
        get => _areMetadataLabelsVisible;
        private set => SetAndRaise(
            AreMetadataLabelsVisibleProperty,
            ref _areMetadataLabelsVisible,
            value);
    }

    public bool IsMiniVisualizationVisible
    {
        get => _isMiniVisualizationVisible;
        private set => SetAndRaise(
            IsMiniVisualizationVisibleProperty,
            ref _isMiniVisualizationVisible,
            value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PlayerMiniControl() : base(false)
    {
        SizeChanged += PlayerMiniControl_SizeChanged;

        AddHandler(
            InputElement.PointerPressedEvent,
            PlayerMiniControl_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);

        InitializeComponent();
        InitializePlayerElements();
        InitializeMiniElements();
    }

    #endregion CONSTRUCTORS

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeMiniPlayerConfig();
        UpdateResponsiveVisibility(Bounds.Width);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        EndResize();

        if (_miniPlayerConfig is not null)
            _miniPlayerConfig.PropertyChanged -= MiniPlayerConfig_PropertyChanged;

        _miniPlayerConfig = null;
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE

    #region ELEMENT INITIALIZATION

    private void InitializeMiniElements()
    {
        DetachContextMenuHandlers();

        _contextMenu = this.FindControl<ContextMenu>("PlayerContextMenu");
        _closeApplicationMenuItem = this.FindControl<MenuItem>("CloseApplicationMenuItem");
        _fullPlayerMenuItem = this.FindControl<MenuItem>("FullPlayerMenuItem");
        _nextMenuItem = this.FindControl<MenuItem>("NextMenuItem");
        _pauseMenuItem = this.FindControl<MenuItem>("PauseMenuItem");
        _playMenuItem = this.FindControl<MenuItem>("PlayMenuItem");
        _previousMenuItem = this.FindControl<MenuItem>("PreviousMenuItem");
        _stopMenuItem = this.FindControl<MenuItem>("StopMenuItem");
        _visualizationMenuItems =
        [
            this.FindControl<MenuItem>("RegularVisualizationMenuItem")!,
            this.FindControl<MenuItem>("UpsideDownVisualizationMenuItem")!,
            this.FindControl<MenuItem>("SingleVisualizationMenuItem")!,
            this.FindControl<MenuItem>("CenterFilledVisualizationMenuItem")!,
            this.FindControl<MenuItem>("CenterSingleVisualizationMenuItem")!
        ];

        if (_contextMenu is not null)
            _contextMenu.Opened += ContextMenu_Opened;
        if (_closeApplicationMenuItem is not null)
            _closeApplicationMenuItem.Click += CloseApplicationMenuItem_Click;
        if (_fullPlayerMenuItem is not null)
            _fullPlayerMenuItem.Click += FullPlayerMenuItem_Click;
        if (_nextMenuItem is not null)
            _nextMenuItem.Click += NextMenuItem_Click;
        if (_pauseMenuItem is not null)
            _pauseMenuItem.Click += PlayPauseMenuItem_Click;
        if (_playMenuItem is not null)
            _playMenuItem.Click += PlayPauseMenuItem_Click;
        if (_previousMenuItem is not null)
            _previousMenuItem.Click += PreviousMenuItem_Click;
        if (_stopMenuItem is not null)
            _stopMenuItem.Click += StopMenuItem_Click;

        foreach (var item in _visualizationMenuItems)
            item.Click += VisualizationMenuItem_Click;
    }

    private void DetachContextMenuHandlers()
    {
        if (_contextMenu is not null)
            _contextMenu.Opened -= ContextMenu_Opened;
        if (_closeApplicationMenuItem is not null)
            _closeApplicationMenuItem.Click -= CloseApplicationMenuItem_Click;
        if (_fullPlayerMenuItem is not null)
            _fullPlayerMenuItem.Click -= FullPlayerMenuItem_Click;
        if (_nextMenuItem is not null)
            _nextMenuItem.Click -= NextMenuItem_Click;
        if (_pauseMenuItem is not null)
            _pauseMenuItem.Click -= PlayPauseMenuItem_Click;
        if (_playMenuItem is not null)
            _playMenuItem.Click -= PlayPauseMenuItem_Click;
        if (_previousMenuItem is not null)
            _previousMenuItem.Click -= PreviousMenuItem_Click;
        if (_stopMenuItem is not null)
            _stopMenuItem.Click -= StopMenuItem_Click;

        foreach (var item in _visualizationMenuItems)
            item.Click -= VisualizationMenuItem_Click;

        _visualizationMenuItems = [];
    }

    #endregion ELEMENT INITIALIZATION

    #region ACTIONS

    private void PlayerMiniControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.Source is not Visual source
            || source.GetSelfAndVisualAncestors().Any(visual =>
                ReferenceEquals(visual, LeftResizeHandle) || ReferenceEquals(visual, RightResizeHandle))
            || source.FindAncestorOfType<Button>(includeSelf: true) is not null
            || source.FindAncestorOfType<Slider>(includeSelf: true) is not null
            || source.FindAncestorOfType<MiniVisualizationControl>(includeSelf: true) is not null)
        {
            return;
        }

        DragRequested?.Invoke(this, e);
    }

    private void PlayerMiniControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateResponsiveVisibility(e.NewSize.Width);
    }

    private void SubscribeMiniPlayerConfig()
    {
        if (_miniPlayerConfig is not null)
            _miniPlayerConfig.PropertyChanged -= MiniPlayerConfig_PropertyChanged;

        _miniPlayerConfig = ApplicationContext.Instance.Config.MiniPlayer;
        _miniPlayerConfig.PropertyChanged += MiniPlayerConfig_PropertyChanged;
    }

    private void UpdateResponsiveVisibility(double width)
    {
        var showVisualization = _miniPlayerConfig?.ShowVisualization == true;
        var minimumWidth = showVisualization
            ? Dimensions.PlayerMiniControlWithVisualizationMinimumWidth
            : Dimensions.PlayerMiniControlMinimumWidth;

        MinWidth = minimumWidth;
        IsMiniVisualizationVisible = showVisualization;
        IsCoverVisible = _miniPlayerConfig?.ShowCover == true
            && width >= minimumWidth + Dimensions.PlayerMiniCoverAdditionalWidth;
        AreMetadataLabelsVisible = width >= minimumWidth
            + Dimensions.PlayerMiniMetadataLabelsAdditionalWidth
            + (IsCoverVisible ? Dimensions.PlayerMiniCoverOccupiedWidth : 0);
    }

    #endregion ACTIONS

    #region WINDOW RESIZING

    private void ResizeBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border
            || _resizeBorder is not null
            || !e.GetCurrentPoint(border).Properties.IsLeftButtonPressed
            || TopLevel.GetTopLevel(border) is not Window window)
            return;

        _resizeBorder = border;
        _resizePointer = e.Pointer;
        _resizeWindow = window;
        _isResizingFromLeft = ReferenceEquals(border, LeftResizeHandle);

        var screenOrigin = window.PointToScreen(default);
        var screenSample = window.PointToScreen(new Point(100, 0));
        _resizeScreenUnitsPerDip = Math.Abs(screenSample.X - screenOrigin.X) / 100d;
        if (!double.IsFinite(_resizeScreenUnitsPerDip) || _resizeScreenUnitsPerDip <= 0)
            _resizeScreenUnitsPerDip = 1;

        _resizeStartPointerPosition = window.PointToScreen(e.GetPosition(window));
        _resizeStartWindowPosition = window.Position;
        _resizeStartWidth = window.ClientSize.Width;
        e.Pointer.Capture(border);
        e.Handled = true;
    }

    private void ResizeBorder_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_resizeWindow is not { } window
            || !ReferenceEquals(sender, _resizeBorder)
            || !ReferenceEquals(e.Pointer, _resizePointer)
            || e.Pointer.Captured != _resizeBorder)
            return;

        var pointerPosition = window.PointToScreen(e.GetPosition(window));
        var pointerDelta = (pointerPosition.X - _resizeStartPointerPosition.X) / _resizeScreenUnitsPerDip;
        var requestedWidth = _isResizingFromLeft
            ? _resizeStartWidth - pointerDelta
            : _resizeStartWidth + pointerDelta;
        var width = Math.Clamp(requestedWidth, window.MinWidth, window.MaxWidth);

        window.Width = width;

        if (_isResizingFromLeft)
        {
            var actualDelta = _resizeStartWidth - width;
            window.Position = new PixelPoint(
                _resizeStartWindowPosition.X + (int)Math.Round(actualDelta * _resizeScreenUnitsPerDip),
                _resizeStartWindowPosition.Y);
        }

        e.Handled = true;
    }

    private void ResizeBorder_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!ReferenceEquals(sender, _resizeBorder) || !ReferenceEquals(e.Pointer, _resizePointer))
            return;

        EndResize();
        e.Handled = true;
    }

    private void ResizeBorder_OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (ReferenceEquals(sender, _resizeBorder) && ReferenceEquals(e.Pointer, _resizePointer))
            EndResize();
    }

    private void EndResize()
    {
        var border = _resizeBorder;
        var pointer = _resizePointer;
        _resizeBorder = null;
        _resizePointer = null;
        _resizeWindow = null;

        if (border is not null && pointer?.Captured == border)
            pointer.Capture(null);
    }

    #endregion WINDOW RESIZING

    #region BUTTON ACTIONS

    private void CloseApplicationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        CloseApplicationRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ContextMenu_Opened(object? sender, RoutedEventArgs e)
    {
        var selectedType = ApplicationContext.Instance.Config.MiniPlayer.VisualizationType;

        foreach (var item in _visualizationMenuItems)
        {
            item.IsChecked = item.Tag is string value
                && Enum.TryParse<MiniVisualizationType>(value, out var type)
                && type == selectedType;
        }
    }

    private void FullPlayerMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        IsMiniMode = false;
    }

    private void NextMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(NextRequestedEvent));
    }

    private void PlayPauseMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(PlayPauseRequestedEvent));
    }

    private void PreviousMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(PreviousRequestedEvent));
    }

    private void StopMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(StopRequestedEvent));
    }

    private void VisualizationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { Tag: string value }
            && Enum.TryParse<MiniVisualizationType>(value, out var type))
        {
            ApplicationContext.Instance.Config.MiniPlayer.VisualizationType = type;
        }
    }

    #endregion BUTTON ACTIONS

    #region DATA CONTEXT

    protected override PlayerDataContext? ResolvePlayerDataContext(object? dataContext)
    {
        return dataContext is PlayerMiniDataContext miniDataContext
            ? miniDataContext.Player
            : base.ResolvePlayerDataContext(dataContext);
    }

    #endregion DATA CONTEXT

    #region PROPERTY CHANGES

    private void MiniPlayerConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MiniPlayerConfig.ShowCover)
            or nameof(MiniPlayerConfig.ShowVisualization))
        {
            UpdateResponsiveVisibility(Bounds.Width);
        }
    }

    #endregion PROPERTY CHANGES

}
