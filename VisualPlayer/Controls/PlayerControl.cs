using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Player;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

[TemplatePart("PART_ContentHost", typeof(Border), IsRequired = true)]
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_PlayPauseButton", typeof(Button))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_StopButton", typeof(Button))]
[TemplatePart("PART_RepeatButton", typeof(Button))]
[TemplatePart("PART_SpeedButton", typeof(Button))]
[TemplatePart("PART_VolumeButton", typeof(Button))]
public class PlayerControl : TemplatedControl
{
    #region CONST

    private static readonly TimeSpan AUTO_HIDE_ANIMATION_DURATION = TimeSpan.FromMilliseconds(220);

    #endregion CONST

    private Border? _contentHost;
    private double _expandedHeight;
    private readonly TranslateTransform _hideTransform;
    private bool _isAutoHideInitialized;
    private bool _isHidden;
    private bool _isPointerInteractionActive;
    private Button? _nextButton;
    private Button? _playPauseButton;
    private Slider? _positionSlider;
    private Button? _previousButton;
    private Button? _repeatButton;
    private Button? _speedButton;
    private Button? _stopButton;
    private Button? _volumeButton;
    private bool _isSeeking;
    private PlayerDataContext? _playerDataContext;
    private double _speedPanelLeft;
    private double _volumePanelLeft;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> AutoHideProperty
        = AvaloniaProperty.Register<PlayerControl, bool>(nameof(AutoHide));
    
    public static readonly DirectProperty<PlayerControl, bool> IsHiddenProperty
        = AvaloniaProperty.RegisterDirect<PlayerControl, bool>(nameof(IsHidden), control => control.IsHidden);

    public static readonly StyledProperty<bool> KeepOpenProperty
        = AvaloniaProperty.Register<PlayerControl, bool>(nameof(KeepOpen));

    public static readonly StyledProperty<bool> IsMiniModeProperty
        = AvaloniaProperty.Register<PlayerControl, bool>(nameof(IsMiniMode), defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<PlayerControl, double> ExpandedHeightProperty
        = AvaloniaProperty.RegisterDirect<PlayerControl, double>(nameof(ExpandedHeight), control => control.ExpandedHeight);

    public static readonly StyledProperty<double> PositionProperty
        = AvaloniaProperty.Register<PlayerControl, double>(nameof(Position), 0);

    public static readonly StyledProperty<double> RevealSizeProperty
        = AvaloniaProperty.Register<PlayerControl, double>(nameof(RevealSize), Dimensions.AutoHideRevealSize);

    public static readonly StyledProperty<double> SpeedPanelWidthProperty
        = AvaloniaProperty.Register<PlayerControl, double>(nameof(SpeedPanelWidth));

    public static readonly DirectProperty<PlayerControl, double> SpeedPanelLeftProperty
        = AvaloniaProperty.RegisterDirect<PlayerControl, double>(
            nameof(SpeedPanelLeft),
            control => control.SpeedPanelLeft);

    public static readonly DirectProperty<PlayerControl, double> VolumePanelLeftProperty
        = AvaloniaProperty.RegisterDirect<PlayerControl, double>(
            nameof(VolumePanelLeft),
            control => control.VolumePanelLeft);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public static readonly RoutedEvent<RoutedEventArgs> NextRequestedEvent 
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(NextRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> PlayPauseRequestedEvent 
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(PlayPauseRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> PositionSeekRequestedEvent
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(PositionSeekRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> PreviousRequestedEvent
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(PreviousRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> StopRequestedEvent 
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(StopRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> SpeedRequestedEvent
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(SpeedRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> VolumeRequestedEvent
        = RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(VolumeRequested), RoutingStrategies.Bubble);


    public event EventHandler<RoutedEventArgs>? NextRequested
    {
        add => AddHandler(NextRequestedEvent, value);
        remove => RemoveHandler(NextRequestedEvent, value);
    }
    
    public event EventHandler<RoutedEventArgs>? PlayPauseRequested
    {
        add => AddHandler(PlayPauseRequestedEvent, value);
        remove => RemoveHandler(PlayPauseRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? PositionSeekRequested
    {
        add => AddHandler(PositionSeekRequestedEvent, value);
        remove => RemoveHandler(PositionSeekRequestedEvent, value);
    }
    
    public event EventHandler<RoutedEventArgs>? PreviousRequested
    {
        add => AddHandler(PreviousRequestedEvent, value);
        remove => RemoveHandler(PreviousRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? StopRequested
    {
        add => AddHandler(StopRequestedEvent, value);
        remove => RemoveHandler(StopRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? SpeedRequested
    {
        add => AddHandler(SpeedRequestedEvent, value);
        remove => RemoveHandler(SpeedRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? VolumeRequested
    {
        add => AddHandler(VolumeRequestedEvent, value);
        remove => RemoveHandler(VolumeRequestedEvent, value);
    }

    #endregion EVENTS

    #region PROPERTIES

    public bool AutoHide
    {
        get => GetValue(AutoHideProperty);
        set => SetValue(AutoHideProperty, value);
    }

    public bool IsHidden
    {
        get => _isHidden;
        private set => SetAndRaise(IsHiddenProperty, ref _isHidden, value);
    }

    public bool KeepOpen
    {
        get => GetValue(KeepOpenProperty);
        set => SetValue(KeepOpenProperty, value);
    }

    public bool IsMiniMode
    {
        get => GetValue(IsMiniModeProperty);
        set => SetValue(IsMiniModeProperty, value);
    }

    public double ExpandedHeight
    {
        get => _expandedHeight;
        private set => SetAndRaise(ExpandedHeightProperty, ref _expandedHeight, value);
    }

    public double Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public double RevealSize
    {
        get => GetValue(RevealSizeProperty);
        set => SetValue(RevealSizeProperty, value);
    }

    public double SpeedPanelWidth
    {
        get => GetValue(SpeedPanelWidthProperty);
        set => SetValue(SpeedPanelWidthProperty, value);
    }

    public double SpeedPanelLeft
    {
        get => _speedPanelLeft;
        private set => SetAndRaise(SpeedPanelLeftProperty, ref _speedPanelLeft, value);
    }

    public double VolumePanelLeft
    {
        get => _volumePanelLeft;
        private set => SetAndRaise(VolumePanelLeftProperty, ref _volumePanelLeft, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PlayerControl()
    {
        _hideTransform = new TranslateTransform
        {
            Transitions =
            [
                new DoubleTransition
                {
                    Property = TranslateTransform.YProperty,
                    Duration = AUTO_HIDE_ANIMATION_DURATION
                }
            ]
        };

        Transitions =
        [
            new DoubleTransition
            {
                Property = HeightProperty,
                Duration = AUTO_HIDE_ANIMATION_DURATION
            }
        ];

        DataContextChanged += PlayerControl_DataContextChanged;
        LayoutUpdated += PlayerControl_LayoutUpdated;
        PointerEntered += PlayerControl_PointerEntered;
        PointerExited += PlayerControl_PointerExited;

        AddHandler(
            InputElement.PointerPressedEvent,
            PlayerControl_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);

        AddHandler(
            InputElement.PointerReleasedEvent,
            PlayerControl_PointerReleased,
            RoutingStrategies.Bubble,
            handledEventsToo: true);

        AddHandler(
            InputElement.PointerCaptureLostEvent,
            PlayerControl_PointerCaptureLost,
            RoutingStrategies.Bubble,
            handledEventsToo: true);
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void NextButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(NextRequestedEvent));
    }

    private void PlayPauseButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(PlayPauseRequestedEvent));
    }

    private void PreviousButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(PreviousRequestedEvent));
    }

    private void RepeatButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_playerDataContext is null)
            return;

        _playerDataContext.RepeatMode = _playerDataContext.RepeatMode switch
        {
            RepeatMode.None => RepeatMode.All,
            RepeatMode.All => RepeatMode.One,
            _ => RepeatMode.None
        };
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(StopRequestedEvent));
    }

    private void SpeedButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(SpeedRequestedEvent));
    }

    private void VolumeButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(VolumeRequestedEvent));
    }

    public bool IsVolumeButtonSource(Visual? source)
    {
        return _volumeButton is not null
            && source is not null
            && (ReferenceEquals(source, _volumeButton) || _volumeButton.IsVisualAncestorOf(source));
    }

    public bool IsSpeedButtonSource(Visual? source)
    {
        return _speedButton is not null
            && source is not null
            && (ReferenceEquals(source, _speedButton) || _speedButton.IsVisualAncestorOf(source));
    }

    #endregion ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AutoHideProperty
            || change.Property == KeepOpenProperty
            || change.Property == IsMiniModeProperty
            || change.Property == RevealSizeProperty)
        {
            Dispatcher.UIThread.Post(ApplyAutoHideState, DispatcherPriority.Loaded);
        }
        else if (change.Property == SpeedPanelWidthProperty)
        {
            Dispatcher.UIThread.Post(UpdateVolumePanelPosition, DispatcherPriority.Loaded);
        }
    }

    private void PlayerDataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(PlayerDataContext.Position) or nameof(PlayerDataContext.Duration))
            UpdatePositionFromContext();
    }

    private void PlayerControl_DataContextChanged(object? sender, EventArgs e)
    {
        if (_playerDataContext is not null)
            _playerDataContext.PropertyChanged -= PlayerDataContext_PropertyChanged;

        _playerDataContext = DataContext as PlayerDataContext;

        if (_playerDataContext is not null)
            _playerDataContext.PropertyChanged += PlayerDataContext_PropertyChanged;

        UpdatePositionFromContext();
    }

    private void PlayerControl_LayoutUpdated(object? sender, EventArgs e)
    {
        if (!_isAutoHideInitialized && IsEffectivelyVisible)
            InitializeAutoHide();

        UpdateVolumePanelPosition();
    }

    private void UpdatePositionFromContext()
    {
        if (_isSeeking || _playerDataContext is null)
            return;

        Position = Math.Clamp(_playerDataContext.Position, 0, Math.Max(0, _playerDataContext.Duration));
    }

    #endregion PROPERTY CHANGE HANDLING

    #region SHOW HIDE

    private void ApplyAutoHideState()
    {
        if (!_isAutoHideInitialized)
            return;

        if (IsMiniMode || !AutoHide || KeepOpen || IsPointerOver)
            ShowControl();
        else
            HideControl();
    }

    private void CompletePointerInteraction()
    {
        _isPointerInteractionActive = false;

        Dispatcher.UIThread.Post(
            () =>
            {
                if (!IsPointerOver)
                    HideControl();
            },
            DispatcherPriority.Input);
    }

    private void HideControl()
    {
        if (IsMiniMode || !AutoHide || KeepOpen || !_isAutoHideInitialized || _contentHost is null || IsHidden)
            return;

        IsHidden = true;
        Height = Math.Max(1, RevealSize);
        _hideTransform.Y = _expandedHeight;
    }

    private void InitializeAutoHide()
    {
        if (_contentHost is null)
            return;

        var expandedHeight = Math.Max(Bounds.Height, DesiredSize.Height);

        if (expandedHeight <= 0)
            return;

        ExpandedHeight = expandedHeight;
        _contentHost.RenderTransform = _hideTransform;
        Height = expandedHeight;
        _isAutoHideInitialized = true;

        ApplyAutoHideState();
    }

    private void PlayerControl_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CompletePointerInteraction();
    }

    private void PlayerControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        ShowControl();
    }

    private void PlayerControl_PointerExited(object? sender, PointerEventArgs e)
    {
        if (_isPointerInteractionActive || e.Pointer.Captured is not null)
            return;

        HideControl();
    }

    private void PlayerControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isPointerInteractionActive = true;
    }

    private void PlayerControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CompletePointerInteraction();
    }

    private void ShowControl()
    {
        if (!_isAutoHideInitialized || _contentHost is null)
            return;

        IsHidden = false;
        Height = _expandedHeight;
        _hideTransform.Y = 0;
    }

    #endregion SHOW HIDE

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();

        base.OnApplyTemplate(e);

        _contentHost = e.NameScope.Find<Border>("PART_ContentHost");
        _nextButton = e.NameScope.Find<Button>("PART_NextButton");
        _playPauseButton = e.NameScope.Find<Button>("PART_PlayPauseButton");
        _positionSlider = e.NameScope.Find<Slider>("PART_PositionSlider");
        _previousButton = e.NameScope.Find<Button>("PART_PreviousButton");
        _repeatButton = e.NameScope.Find<Button>("PART_RepeatButton");
        _speedButton = e.NameScope.Find<Button>("PART_SpeedButton");
        _stopButton = e.NameScope.Find<Button>("PART_StopButton");
        _volumeButton = e.NameScope.Find<Button>("PART_VolumeButton");

        if (_nextButton is not null)
            _nextButton.Click += NextButton_Click;

        if (_playPauseButton is not null)
            _playPauseButton.Click += PlayPauseButton_Click;

        if (_positionSlider is not null)
        {
            _positionSlider.AddHandler(InputElement.KeyDownEvent, PositionSlider_KeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
            _positionSlider.AddHandler(InputElement.KeyUpEvent, PositionSlider_KeyUp, RoutingStrategies.Bubble, handledEventsToo: true);
            _positionSlider.AddHandler(InputElement.PointerPressedEvent, PositionSlider_PointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
            _positionSlider.AddHandler(InputElement.PointerReleasedEvent, PositionSlider_PointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
            _positionSlider.AddHandler(InputElement.PointerCaptureLostEvent, PositionSlider_PointerCaptureLost, RoutingStrategies.Bubble, handledEventsToo: true);
            
            _positionSlider.LostFocus += PositionSlider_LostFocus;
        }
        
        if (_previousButton is not null)
            _previousButton.Click += PreviousButton_Click;
        
        if (_repeatButton is not null)
            _repeatButton.Click += RepeatButton_Click;

        if (_stopButton is not null)
            _stopButton.Click += StopButton_Click;

        if (_speedButton is not null)
            _speedButton.Click += SpeedButton_Click;

        if (_volumeButton is not null)
            _volumeButton.Click += VolumeButton_Click;

        UpdatePositionFromContext();
        UpdateVolumePanelPosition();
        Dispatcher.UIThread.Post(InitializeAutoHide, DispatcherPriority.Loaded);
    }

    private void DetachHandlers()
    {
        if (_nextButton is not null)
            _nextButton.Click -= NextButton_Click;
        
        if (_playPauseButton is not null)
            _playPauseButton.Click -= PlayPauseButton_Click;
        
        if (_positionSlider is not null)
        {
            _positionSlider.RemoveHandler(InputElement.KeyDownEvent, PositionSlider_KeyDown);
            _positionSlider.RemoveHandler(InputElement.KeyUpEvent, PositionSlider_KeyUp);
            _positionSlider.RemoveHandler(InputElement.PointerPressedEvent, PositionSlider_PointerPressed);
            _positionSlider.RemoveHandler(InputElement.PointerReleasedEvent, PositionSlider_PointerReleased);
            _positionSlider.RemoveHandler(InputElement.PointerCaptureLostEvent, PositionSlider_PointerCaptureLost);
            
            _positionSlider.LostFocus -= PositionSlider_LostFocus;
        }

        if (_previousButton is not null)
            _previousButton.Click -= PreviousButton_Click;

        if (_repeatButton is not null)
            _repeatButton.Click -= RepeatButton_Click;

        if (_stopButton is not null)
            _stopButton.Click -= StopButton_Click;

        if (_speedButton is not null)
            _speedButton.Click -= SpeedButton_Click;

        if (_volumeButton is not null)
            _volumeButton.Click -= VolumeButton_Click;

        _isSeeking = false;
        _contentHost = null;
        _isAutoHideInitialized = false;
    }

    #endregion TEMPLATE

    #region VOLUME CONTROL

    private void UpdateVolumePanelPosition()
    {
        if (_speedButton is not null && _speedButton.Bounds.Width > 0)
        {
            var speedButtonCenter = new Point(_speedButton.Bounds.Width / 2, 0);
            var speedPosition = _speedButton.TranslatePoint(speedButtonCenter, this);

            if (speedPosition is not null)
                SpeedPanelLeft = speedPosition.Value.X - SpeedPanelWidth / 2;
        }

        if (_volumeButton is not null && _volumeButton.Bounds.Width > 0)
        {
            var volumeButtonCenter = new Point(_volumeButton.Bounds.Width / 2, 0);
            var volumePosition = _volumeButton.TranslatePoint(volumeButtonCenter, this);

            if (volumePosition is not null)
                VolumePanelLeft = volumePosition.Value.X - VolumeControl.WIDTH / 2;
        }
    }

    #endregion VOLUME CONTROL

    #region TRACK SLIDER METHODS

    private void CompletePositionSeek()
    {
        if (!_isSeeking)
            return;

        _isSeeking = false;
        RaiseEvent(new RoutedEventArgs(PositionSeekRequestedEvent));
    }

    private static bool IsPositionKey(Key key)
    {
        return key is Key.Left or Key.Right or Key.Up or Key.Down
            or Key.PageUp or Key.PageDown or Key.Home or Key.End;
    }

    private void PositionSlider_KeyDown(object? sender, KeyEventArgs e)
    {
        if (IsPositionKey(e.Key))
            _isSeeking = true;
    }

    private void PositionSlider_KeyUp(object? sender, KeyEventArgs e)
    {
        if (IsPositionKey(e.Key))
            CompletePositionSeek();
    }

    private void PositionSlider_LostFocus(object? sender, RoutedEventArgs e)
    {
        CompletePositionSeek();
    }

    private void PositionSlider_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CompletePositionSeek();
    }

    private void PositionSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isSeeking = true;
    }

    private void PositionSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CompletePositionSeek();
    }
    
    #endregion TRACK SLIDER METHODS
}
