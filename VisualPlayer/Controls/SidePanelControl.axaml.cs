using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using VisualPlayer.Contexts;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

public partial class SidePanelControl : UserControl
{
    #region CONST

    private static readonly TimeSpan PANEL_ANIMATION_DURATION = Dimensions.SidePanelAnimationDuration;

    #endregion CONST

    private double _availableWidth = double.PositiveInfinity;
    private HamburgerMenuControl? _hamburgerMenu;
    private int _hideTransitionVersion;
    private Thickness _infoBarMargin;
    private Transitions? _infoBarTransitions;
    private int _instantWidthChangeVersion;
    private bool _isMenuCollapsing;
    private bool _isPanelHidden;
    private bool _isPlayListCollapsing;
    private bool _isResizing;
    private int _menuTransitionVersion;
    private double _occupiedWidth;
    private Border? _panelHost;
    private PlayListControl? _playList;
    private Border? _playListHost;
    private double _playListHostWidth;
    private double _resizeStartPointerX;
    private double _resizeStartWidth;
    private Thumb? _resizeThumb;
    private readonly TranslateTransform _slideTransform;
    private double _visualizationLeftInset;

    #region EVENTS

    public event EventHandler<RoutedEventArgs>? AddFileRequested
    {
        add => AddHandler(PlayListControl.AddFileRequestedEvent, value);
        remove => RemoveHandler(PlayListControl.AddFileRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? ClearPlaylistRequested
    {
        add => AddHandler(PlayListControl.ClearPlaylistRequestedEvent, value);
        remove => RemoveHandler(PlayListControl.ClearPlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? OpenPlaylistRequested
    {
        add => AddHandler(PlayListControl.OpenPlaylistRequestedEvent, value);
        remove => RemoveHandler(PlayListControl.OpenPlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? SavePlaylistRequested
    {
        add => AddHandler(PlayListControl.SavePlaylistRequestedEvent, value);
        remove => RemoveHandler(PlayListControl.SavePlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? ExportPlaylistRequested
    {
        add => AddHandler(PlayListControl.ExportPlaylistRequestedEvent, value);
        remove => RemoveHandler(PlayListControl.ExportPlaylistRequestedEvent, value);
    }

    #endregion EVENTS

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> AutoHideProperty
        = AvaloniaProperty.Register<SidePanelControl, bool>(nameof(AutoHide));

    public static readonly DirectProperty<SidePanelControl, Thickness> InfoBarMarginProperty
        = AvaloniaProperty.RegisterDirect<SidePanelControl, Thickness>(nameof(InfoBarMargin), control => control.InfoBarMargin);

    public static readonly DirectProperty<SidePanelControl, Transitions?> InfoBarTransitionsProperty
        = AvaloniaProperty.RegisterDirect<SidePanelControl, Transitions?>(nameof(InfoBarTransitions), control => control.InfoBarTransitions);

    public static readonly StyledProperty<bool> IsMenuOpenProperty
        = AvaloniaProperty.Register<SidePanelControl, bool>(nameof(IsMenuOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsManuallyHiddenProperty
        = AvaloniaProperty.Register<SidePanelControl, bool>(nameof(IsManuallyHidden));

    public static readonly StyledProperty<bool> IsPlayListOpenProperty
        = AvaloniaProperty.Register<SidePanelControl, bool>(nameof(IsPlayListOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<HamburgerMenuContext?> MenuContextProperty
        = AvaloniaProperty.Register<SidePanelControl, HamburgerMenuContext?>(nameof(MenuContext));

    public static readonly StyledProperty<double> MaximumPlayListWidthProperty
        = AvaloniaProperty.Register<SidePanelControl, double>(nameof(MaximumPlayListWidth), 768);

    public static readonly StyledProperty<double> MinimumPlayListWidthProperty
        = AvaloniaProperty.Register<SidePanelControl, double>(nameof(MinimumPlayListWidth), 420);

    public static readonly StyledProperty<double> MinimumRemainingWidthProperty
        = AvaloniaProperty.Register<SidePanelControl, double>(nameof(MinimumRemainingWidth), Dimensions.InfoBarMinimumWidth);

    public static readonly DirectProperty<SidePanelControl, double> OccupiedWidthProperty
        = AvaloniaProperty.RegisterDirect<SidePanelControl, double>(nameof(OccupiedWidth), control => control.OccupiedWidth);

    public static readonly StyledProperty<object?> PlayListDataContextProperty
        = AvaloniaProperty.Register<SidePanelControl, object?>(nameof(PlayListDataContext));

    public static readonly DirectProperty<SidePanelControl, double> PlayListHostWidthProperty
        = AvaloniaProperty.RegisterDirect<SidePanelControl, double>(nameof(PlayListHostWidth), control => control.PlayListHostWidth);

    public static readonly StyledProperty<double> PlayListWidthProperty
        = AvaloniaProperty.Register<SidePanelControl, double>(nameof(PlayListWidth), 640, defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> RevealSizeProperty
        = AvaloniaProperty.Register<SidePanelControl, double>(nameof(RevealSize), Dimensions.AutoHideRevealSize);

    public static readonly DirectProperty<SidePanelControl, double> VisualizationLeftInsetProperty
        = AvaloniaProperty.RegisterDirect<SidePanelControl, double>(nameof(VisualizationLeftInset), control => control.VisualizationLeftInset);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool AutoHide
    {
        get => GetValue(AutoHideProperty);
        set => SetValue(AutoHideProperty, value);
    }

    public Thickness InfoBarMargin
    {
        get => _infoBarMargin;
        private set => SetAndRaise(InfoBarMarginProperty, ref _infoBarMargin, value);
    }

    public Transitions? InfoBarTransitions
    {
        get => _infoBarTransitions;
        private set => SetAndRaise(InfoBarTransitionsProperty, ref _infoBarTransitions, value);
    }

    public bool IsMenuOpen
    {
        get => GetValue(IsMenuOpenProperty);
        set => SetValue(IsMenuOpenProperty, value);
    }

    public bool IsManuallyHidden
    {
        get => GetValue(IsManuallyHiddenProperty);
        set => SetValue(IsManuallyHiddenProperty, value);
    }

    public bool IsPlayListOpen
    {
        get => GetValue(IsPlayListOpenProperty);
        set => SetValue(IsPlayListOpenProperty, value);
    }

    public HamburgerMenuContext? MenuContext
    {
        get => GetValue(MenuContextProperty);
        set => SetValue(MenuContextProperty, value);
    }

    public double MaximumPlayListWidth
    {
        get => GetValue(MaximumPlayListWidthProperty);
        set => SetValue(MaximumPlayListWidthProperty, value);
    }

    public double MinimumPlayListWidth
    {
        get => GetValue(MinimumPlayListWidthProperty);
        set => SetValue(MinimumPlayListWidthProperty, value);
    }

    public double MinimumRemainingWidth
    {
        get => GetValue(MinimumRemainingWidthProperty);
        set => SetValue(MinimumRemainingWidthProperty, value);
    }

    public double OccupiedWidth
    {
        get => _occupiedWidth;
        private set
        {
            SetAndRaise(OccupiedWidthProperty, ref _occupiedWidth, value);
            var leftMargin = value <= 0
                ? 0
                : value;
            InfoBarMargin = new Thickness(leftMargin, 0, 0, 0);
        }
    }

    public object? PlayListDataContext
    {
        get => GetValue(PlayListDataContextProperty);
        set => SetValue(PlayListDataContextProperty, value);
    }

    public double PlayListHostWidth
    {
        get => _playListHostWidth;
        private set => SetAndRaise(PlayListHostWidthProperty, ref _playListHostWidth, value);
    }

    public double PlayListWidth
    {
        get => GetValue(PlayListWidthProperty);
        set => SetValue(PlayListWidthProperty, value);
    }

    public double RevealSize
    {
        get => GetValue(RevealSizeProperty);
        set => SetValue(RevealSizeProperty, value);
    }

    public double VisualizationLeftInset
    {
        get => _visualizationLeftInset;
        private set => SetAndRaise(VisualizationLeftInsetProperty, ref _visualizationLeftInset, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public SidePanelControl()
    {
        _slideTransform = new TranslateTransform();
        InfoBarTransitions = CreateInfoBarTransitions();

        PointerEntered += SidePanelControl_PointerEntered;
        PointerExited += SidePanelControl_PointerExited;
        SizeChanged += SidePanelControl_SizeChanged;

        InitializeComponent();
        InitializeElements();
    }

    #endregion CONSTRUCTOR

    #region ACTIONS

    public void ConstrainWidth(double availableWidth)
    {
        if (!double.IsFinite(availableWidth) || availableWidth <= 0)
            return;

        _availableWidth = availableWidth;
        SetPlayListWidthInstantly(GetConstrainedPlayListWidth(PlayListWidth));
    }

    public void TogglePlayList()
    {
        SetCurrentValue(IsMenuOpenProperty, false);

        if (IsPlayListOpen)
            ClosePlayList(false);
        else
        {
            ++_menuTransitionVersion;
            _isPlayListCollapsing = false;
            SetCurrentValue(IsPlayListOpenProperty, true);
        }
    }

    private double GetConstrainedPlayListWidth(double width)
    {
        var availableForPlayList = Math.Max(
            0,
            _availableWidth
            - Dimensions.SidePanelCollapsedWidth
            - Dimensions.SidePanelResizeThumbWidth
            - MinimumRemainingWidth);
        var maximum = Math.Min(MaximumPlayListWidth, availableForPlayList);
        var minimum = Math.Min(MinimumPlayListWidth, maximum);
        return Math.Clamp(width, minimum, maximum);
    }

    private static Transitions CreateInfoBarTransitions()
    {
        return
        [
            new ThicknessTransition
            {
                Property = MarginProperty,
                Duration = PANEL_ANIMATION_DURATION
            }
        ];
    }

    private static Transitions CreatePlayListTransitions()
    {
        return
        [
            new DoubleTransition
            {
                Property = WidthProperty,
                Duration = PANEL_ANIMATION_DURATION
            }
        ];
    }

    private void DisableWidthTransitions()
    {
        ++_instantWidthChangeVersion;
        InfoBarTransitions = null;

        if (_playListHost is not null)
            _playListHost.Transitions = null;
    }

    private async void RestoreWidthTransitionsAfterLayout()
    {
        var transitionVersion = _instantWidthChangeVersion;
        await Task.Delay(TimeSpan.FromMilliseconds(50));

        if (transitionVersion != _instantWidthChangeVersion || _isResizing)
            return;

        InfoBarTransitions = CreateInfoBarTransitions();

        if (_playListHost is not null)
            _playListHost.Transitions = CreatePlayListTransitions();
    }

    private void SetPlayListWidthInstantly(double width)
    {
        DisableWidthTransitions();
        SetCurrentValue(PlayListWidthProperty, width);
        RestoreWidthTransitionsAfterLayout();
    }

    private void MenuItem_Invoked(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { DataContext: { } item }
            || MenuContext is null)
        {
            return;
        }

        if (ReferenceEquals(item, MenuContext.NowPlayingItem))
        {
            if (IsPlayListOpen)
                ClosePlayList(false);
            else if (IsMenuOpen || _hamburgerMenu?.IsOpen == true)
                CloseMenuAndOpenPlayList();
            else
                TogglePlayList();
        }
        else if (ReferenceEquals(item, MenuContext.MainMenuItem))
        {
            if (IsPlayListOpen)
                ClosePlayList(true);
            else
            {
                ++_menuTransitionVersion;
                SetCurrentValue(IsMenuOpenProperty, !IsMenuOpen);
            }
        }
        else
        {
            ++_menuTransitionVersion;
            SetCurrentValue(IsMenuOpenProperty, false);
        }
    }

    private async void CloseMenuAndOpenPlayList()
    {
        var transitionVersion = ++_menuTransitionVersion;
        _isMenuCollapsing = true;
        SetCurrentValue(IsMenuOpenProperty, false);
        _hamburgerMenu?.SetCurrentValue(HamburgerMenuControl.IsOpenProperty, false);

        await Task.Delay(PANEL_ANIMATION_DURATION);

        if (transitionVersion != _menuTransitionVersion)
            return;

        _isMenuCollapsing = false;
        SetCurrentValue(IsPlayListOpenProperty, true);
    }

    private async void ClosePlayList(bool openMenuAfter)
    {
        var transitionVersion = ++_menuTransitionVersion;
        _isPlayListCollapsing = true;
        SetCurrentValue(IsMenuOpenProperty, false);
        SetCurrentValue(IsPlayListOpenProperty, false);

        await Task.Delay(PANEL_ANIMATION_DURATION);

        if (transitionVersion != _menuTransitionVersion || IsPlayListOpen)
            return;

        _isPlayListCollapsing = false;

        if (openMenuAfter)
            SetCurrentValue(IsMenuOpenProperty, true);
        else
            UpdateAutoHideState();
    }

    private void PlayList_CloseRequested(object? sender, RoutedEventArgs e)
    {
        ClosePlayList(false);
        e.Handled = true;
    }

    private void ResizeThumb_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _isResizing = false;
        RestoreWidthTransitionsAfterLayout();
    }

    private void ResizeThumb_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isResizing || TopLevel.GetTopLevel(this) is not { } topLevel)
            return;

        var pointerX = e.GetPosition(topLevel).X;
        SetCurrentValue(
            PlayListWidthProperty,
            GetConstrainedPlayListWidth(_resizeStartWidth + pointerX - _resizeStartPointerX));
        e.Handled = true;
    }

    private void ResizeThumb_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_resizeThumb is null
            || !e.GetCurrentPoint(_resizeThumb).Properties.IsLeftButtonPressed
            || TopLevel.GetTopLevel(this) is not { } topLevel)
        {
            return;
        }

        _isResizing = true;
        DisableWidthTransitions();
        _resizeStartWidth = PlayListWidth;
        _resizeStartPointerX = e.GetPosition(topLevel).X;
    }

    private void ResizeThumb_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isResizing = false;
        RestoreWidthTransitionsAfterLayout();
    }

    #endregion ACTIONS

    #region PROPERTY CHANGES

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsPlayListOpenProperty || change.Property == PlayListWidthProperty)
            PlayListHostWidth = IsPlayListOpen ? GetConstrainedPlayListWidth(PlayListWidth) : 0;

        if (change.Property == MaximumPlayListWidthProperty
            || change.Property == MinimumPlayListWidthProperty
            || change.Property == MinimumRemainingWidthProperty)
        {
            SetPlayListWidthInstantly(GetConstrainedPlayListWidth(PlayListWidth));
        }

        if (change.Property == IsMenuOpenProperty
            || change.Property == IsPlayListOpenProperty
            || change.Property == PlayListWidthProperty)
        {
            UpdateOccupiedWidth();
        }

        if (change.Property == IsManuallyHiddenProperty)
        {
            if (IsManuallyHidden)
                HidePanel();
            else
                ShowPanel();
        }
        else if (change.Property == AutoHideProperty
            || change.Property == IsPlayListOpenProperty
            || change.Property == RevealSizeProperty)
        {
            UpdateAutoHideState();
        }
    }

    #endregion PROPERTY CHANGES

    #region SHOW HIDE

    private void SidePanelControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        ShowPanel();
    }

    private void SidePanelControl_PointerExited(object? sender, PointerEventArgs e)
    {
        if (IsPlayListOpen)
            return;

        var isMenuExpanded = IsMenuOpen || _hamburgerMenu?.IsOpen == true;
        if (isMenuExpanded)
        {
            _isMenuCollapsing = true;
            SetCurrentValue(IsMenuOpenProperty, false);
            _hamburgerMenu?.SetCurrentValue(HamburgerMenuControl.IsOpenProperty, false);
            HidePanelAfterMenuCollapse();
            return;
        }

        if (_isMenuCollapsing || _isPlayListCollapsing)
            return;

        UpdateAutoHideState();
    }

    private void ShowPanel()
    {
        if (IsManuallyHidden)
            return;

        ++_hideTransitionVersion;
        _isMenuCollapsing = false;
        _isPanelHidden = false;
        _slideTransform.X = 0;
        VisualizationLeftInset = Dimensions.SidePanelCollapsedWidth;
        UpdateOccupiedWidth();
    }

    private async void HidePanelAfterMenuCollapse()
    {
        var transitionVersion = ++_hideTransitionVersion;
        await Task.Delay(PANEL_ANIMATION_DURATION);

        if (transitionVersion == _hideTransitionVersion)
        {
            _isMenuCollapsing = false;
            UpdateAutoHideState();
        }
    }

    private void HidePanel()
    {
        if (_panelHost is null)
            return;

        _isPanelHidden = true;
        _slideTransform.X = -(_panelHost.Bounds.Width + Dimensions.SidePanelOuterLeftMargin);
        VisualizationLeftInset = 0;
        UpdateOccupiedWidth();
    }

    private void UpdateAutoHideState()
    {
        if (IsManuallyHidden)
        {
            HidePanel();
            return;
        }

        if (_isMenuCollapsing || _isPlayListCollapsing)
        {
            _isPanelHidden = false;
            _slideTransform.X = 0;
            UpdateOccupiedWidth();
            return;
        }

        if (!AutoHide
            || IsPointerOver
            || IsMenuOpen
            || IsPlayListOpen)
        {
            ShowPanel();
            return;
        }

        HidePanel();
    }

    private void UpdateOccupiedWidth()
    {
        if (_isPanelHidden)
        {
            OccupiedWidth = 0;
            return;
        }

        if (IsPlayListOpen)
        {
            OccupiedWidth = Dimensions.SidePanelCollapsedWidth
                + PlayListHostWidth
                + Dimensions.SidePanelResizeThumbWidth;
            return;
        }

        OccupiedWidth = IsMenuOpen
            ? Dimensions.SidePanelExpandedMenuWidth
            : Dimensions.SidePanelCollapsedWidth;
    }

    #endregion SHOW HIDE

    #region ELEMENT INITIALIZATION

    private void InitializeElements()
    {
        DetachHandlers();

        _hamburgerMenu = this.FindControl<HamburgerMenuControl>("HamburgerMenu");
        _panelHost = this.FindControl<Border>("PanelHost");
        _playList = this.FindControl<PlayListControl>("PlayList");
        _playListHost = this.FindControl<Border>("PlayListHost");
        _resizeThumb = this.FindControl<Thumb>("ResizeThumb");

        if (_panelHost is not null)
        {
            _panelHost.RenderTransform = _slideTransform;
            _slideTransform.Transitions =
            [
                new DoubleTransition
                {
                    Property = TranslateTransform.XProperty,
                    Duration = PANEL_ANIMATION_DURATION
                }
            ];
        }

        _hamburgerMenu?.AddHandler(HamburgerMenuControl.ItemInvokedEvent, MenuItem_Invoked);
        _playList?.AddHandler(PlayListControl.CloseRequestedEvent, PlayList_CloseRequested);

        if (_playListHost is not null && !_isResizing)
            _playListHost.Transitions = CreatePlayListTransitions();

        if (_resizeThumb is not null)
        {
            _resizeThumb.AddHandler(PointerPressedEvent, ResizeThumb_PointerPressed, RoutingStrategies.Tunnel, true);
            _resizeThumb.AddHandler(PointerMovedEvent, ResizeThumb_PointerMoved, RoutingStrategies.Tunnel, true);
            _resizeThumb.AddHandler(PointerReleasedEvent, ResizeThumb_PointerReleased, RoutingStrategies.Tunnel, true);
            _resizeThumb.PointerCaptureLost += ResizeThumb_PointerCaptureLost;
        }

        Dispatcher.UIThread.Post(UpdateAutoHideState);
    }

    private void DetachHandlers()
    {
        _hamburgerMenu?.RemoveHandler(HamburgerMenuControl.ItemInvokedEvent, MenuItem_Invoked);
        _playList?.RemoveHandler(PlayListControl.CloseRequestedEvent, PlayList_CloseRequested);

        if (_resizeThumb is not null)
        {
            _resizeThumb.RemoveHandler(PointerPressedEvent, ResizeThumb_PointerPressed);
            _resizeThumb.RemoveHandler(PointerMovedEvent, ResizeThumb_PointerMoved);
            _resizeThumb.RemoveHandler(PointerReleasedEvent, ResizeThumb_PointerReleased);
            _resizeThumb.PointerCaptureLost -= ResizeThumb_PointerCaptureLost;
        }

        _hamburgerMenu = null;
        _panelHost = null;
        _playList = null;
        _playListHost = null;
        _resizeThumb = null;
    }

    #endregion ELEMENT INITIALIZATION

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        PlayListHostWidth = IsPlayListOpen ? GetConstrainedPlayListWidth(PlayListWidth) : 0;
        UpdateAutoHideState();
    }

    #endregion VISUAL TREE

    #region SIZE CHANGES

    private void SidePanelControl_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (_isPanelHidden && _panelHost is not null)
            _slideTransform.X = -(_panelHost.Bounds.Width + Dimensions.SidePanelOuterLeftMargin);
    }

    #endregion SIZE CHANGES
}
