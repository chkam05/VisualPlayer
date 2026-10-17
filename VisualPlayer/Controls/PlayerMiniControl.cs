using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Resources;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Controls;

public class PlayerMiniControl : PlayerControl
{
    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<PlayerMiniControl, bool> IsCoverVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(nameof(IsCoverVisible), control => control.IsCoverVisible);

    public static readonly DirectProperty<PlayerMiniControl, bool> IsTitlesVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(nameof(IsTitlesVisible), control => control.IsTitlesVisible);

    public static readonly DirectProperty<PlayerMiniControl, bool> IsMiniVisualizationVisibleProperty
        = AvaloniaProperty.RegisterDirect<PlayerMiniControl, bool>(
            nameof(IsMiniVisualizationVisible),
            control => control.IsMiniVisualizationVisible);

    #endregion AVALONIA PROPERTIES

    private bool _isCoverVisible;
    private bool _isMiniVisualizationVisible;
    private bool _isTitlesVisible;
    private ContextMenu? _contextMenu;
    private MenuItem? _closeApplicationMenuItem;
    private MenuItem? _fullPlayerMenuItem;
    private MenuItem? _nextMenuItem;
    private MenuItem? _pauseMenuItem;
    private MenuItem? _playMenuItem;
    private MenuItem? _previousMenuItem;
    private MenuItem? _stopMenuItem;
    private MenuItem[] _visualizationMenuItems = [];
    private VisualizationGlobalConfig? _visualizationGlobalConfig;

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

    public bool IsTitlesVisible
    {
        get => _isTitlesVisible;
        private set => SetAndRaise(IsTitlesVisibleProperty, ref _isTitlesVisible, value);
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

    public PlayerMiniControl()
    {
        SizeChanged += PlayerMiniControl_SizeChanged;

        AddHandler(
            InputElement.PointerPressedEvent,
            PlayerMiniControl_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);
    }

    #endregion CONSTRUCTORS

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ApplicationContext.Instance.VisualizationConfig.PropertyChanged += VisualizationConfig_PropertyChanged;
        SubscribeVisualizationGlobalConfig();
        UpdateResponsiveVisibility(Bounds.Width);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ApplicationContext.Instance.VisualizationConfig.PropertyChanged -= VisualizationConfig_PropertyChanged;

        if (_visualizationGlobalConfig is not null)
            _visualizationGlobalConfig.PropertyChanged -= VisualizationGlobalConfig_PropertyChanged;

        _visualizationGlobalConfig = null;
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachContextMenuHandlers();
        base.OnApplyTemplate(e);

        _contextMenu = e.NameScope.Find<ContextMenu>("PART_ContextMenu");
        _closeApplicationMenuItem = e.NameScope.Find<MenuItem>("PART_CloseApplicationMenuItem");
        _fullPlayerMenuItem = e.NameScope.Find<MenuItem>("PART_FullPlayerMenuItem");
        _nextMenuItem = e.NameScope.Find<MenuItem>("PART_NextMenuItem");
        _pauseMenuItem = e.NameScope.Find<MenuItem>("PART_PauseMenuItem");
        _playMenuItem = e.NameScope.Find<MenuItem>("PART_PlayMenuItem");
        _previousMenuItem = e.NameScope.Find<MenuItem>("PART_PreviousMenuItem");
        _stopMenuItem = e.NameScope.Find<MenuItem>("PART_StopMenuItem");
        _visualizationMenuItems =
        [
            e.NameScope.Find<MenuItem>("PART_RegularVisualizationMenuItem")!,
            e.NameScope.Find<MenuItem>("PART_UpsideDownVisualizationMenuItem")!,
            e.NameScope.Find<MenuItem>("PART_SingleVisualizationMenuItem")!,
            e.NameScope.Find<MenuItem>("PART_CenterFilledVisualizationMenuItem")!,
            e.NameScope.Find<MenuItem>("PART_CenterSingleVisualizationMenuItem")!
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

    #endregion TEMPLATE

    #region ACTIONS

    private void CloseApplicationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        CloseApplicationRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ContextMenu_Opened(object? sender, RoutedEventArgs e)
    {
        var selectedType = ApplicationContext.Instance.VisualizationConfig.Global.MiniVisualizationType;

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
            ApplicationContext.Instance.VisualizationConfig.Global.MiniVisualizationType = type;
        }
    }

    private void PlayerMiniControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.Source is not Visual source
            || source.FindAncestorOfType<WindowResizeBorder>(includeSelf: true) is not null
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

    private void VisualizationConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(VisualizationConfigurationContext.Global))
            return;

        SubscribeVisualizationGlobalConfig();
        UpdateResponsiveVisibility(Bounds.Width);
    }

    private void VisualizationGlobalConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualizationGlobalConfig.ShowMiniVisualization))
            UpdateResponsiveVisibility(Bounds.Width);
    }

    private void SubscribeVisualizationGlobalConfig()
    {
        if (_visualizationGlobalConfig is not null)
            _visualizationGlobalConfig.PropertyChanged -= VisualizationGlobalConfig_PropertyChanged;

        _visualizationGlobalConfig = ApplicationContext.Instance.VisualizationConfig.Global;
        _visualizationGlobalConfig.PropertyChanged += VisualizationGlobalConfig_PropertyChanged;
    }

    private void UpdateResponsiveVisibility(double width)
    {
        IsTitlesVisible = width >= Dimensions.PlayerMiniTitlesMinimumWidth;
        IsCoverVisible = width >= Dimensions.PlayerMiniCoverMinimumWidth;
        IsMiniVisualizationVisible = _visualizationGlobalConfig?.ShowMiniVisualization == true
            && width >= Dimensions.PlayerMiniVisualizationMinimumWidth;
    }

    #endregion ACTIONS

    protected override PlayerDataContext? ResolvePlayerDataContext(object? dataContext)
    {
        return dataContext is PlayerMiniDataContext miniDataContext
            ? miniDataContext.Player
            : base.ResolvePlayerDataContext(dataContext);
    }
}
