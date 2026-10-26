using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using VisualPlayer.Components;
using VisualPlayer.Resources;
using VisualPlayer.Utils;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Controls;

public partial class SpeedControl : UserControl
{
    private bool _hasPointerEntered;
    private bool _isSynchronizingSpeed;
    private Border? _contentHost;
    private double _expandedHeight;
    private Button? _preset1Button;
    private Button? _preset2Button;
    private Button? _preset3Button;
    private Button? _preset4Button;
    private Button? _resetButton;
    private int _speedPercent = PlayerConfig.PLAYBACK_SPEED_DEFAULT;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<SpeedControl, bool>(nameof(IsOpen));

    public static readonly StyledProperty<double> PlaybackSpeedProperty
        = AvaloniaProperty.Register<SpeedControl, double>(
            nameof(PlaybackSpeed),
            1,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> Preset1Property
        = AvaloniaProperty.Register<SpeedControl, int>(nameof(Preset1), PlayerConfig.PLAYBACK_SPEED_PRESET_1_DEFAULT);

    public static readonly StyledProperty<int> Preset2Property
        = AvaloniaProperty.Register<SpeedControl, int>(nameof(Preset2), PlayerConfig.PLAYBACK_SPEED_PRESET_2_DEFAULT);

    public static readonly StyledProperty<int> Preset3Property
        = AvaloniaProperty.Register<SpeedControl, int>(nameof(Preset3), PlayerConfig.PLAYBACK_SPEED_PRESET_3_DEFAULT);

    public static readonly StyledProperty<int> Preset4Property
        = AvaloniaProperty.Register<SpeedControl, int>(nameof(Preset4), PlayerConfig.PLAYBACK_SPEED_PRESET_4_DEFAULT);

    public static readonly DirectProperty<SpeedControl, int> SpeedPercentProperty
        = AvaloniaProperty.RegisterDirect<SpeedControl, int>(
            nameof(SpeedPercent),
            control => control.SpeedPercent,
            (control, value) => control.SpeedPercent = value);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public double PlaybackSpeed
    {
        get => GetValue(PlaybackSpeedProperty);
        set => SetValue(PlaybackSpeedProperty, value);
    }

    public int Preset1
    {
        get => GetValue(Preset1Property);
        set => SetValue(Preset1Property, value);
    }

    public int Preset2
    {
        get => GetValue(Preset2Property);
        set => SetValue(Preset2Property, value);
    }

    public int Preset3
    {
        get => GetValue(Preset3Property);
        set => SetValue(Preset3Property, value);
    }

    public int Preset4
    {
        get => GetValue(Preset4Property);
        set => SetValue(Preset4Property, value);
    }

    public int SpeedPercent
    {
        get => _speedPercent;
        set
        {
            var normalized = PlaybackSpeedUtils.NormalizePercent(value);

            if (!SetAndRaise(SpeedPercentProperty, ref _speedPercent, normalized)
                || _isSynchronizingSpeed)
            {
                return;
            }

            PlaybackSpeed = PlaybackSpeedUtils.ToPlaybackSpeed(normalized);
        }
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public SpeedControl()
    {
        ClipToBounds = true;
        Height = 0;
        IsHitTestVisible = false;

        Transitions =
        [
            new DoubleTransition
            {
                Property = HeightProperty,
                Duration = Dimensions.SidePanelAnimationDuration
            }
        ];

        PointerEntered += SpeedControl_PointerEntered;
        PointerExited += SpeedControl_PointerExited;

        InitializeComponent();
        InitializeElements();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    public void Close()
    {
        IsOpen = false;
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
    }

    private void Preset1Button_Click(object? sender, RoutedEventArgs e)
    {
        SpeedPercent = Preset1;
    }

    private void Preset2Button_Click(object? sender, RoutedEventArgs e)
    {
        SpeedPercent = Preset2;
    }

    private void Preset3Button_Click(object? sender, RoutedEventArgs e)
    {
        SpeedPercent = Preset3;
    }

    private void Preset4Button_Click(object? sender, RoutedEventArgs e)
    {
        SpeedPercent = Preset4;
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        SpeedPercent = PlayerConfig.PLAYBACK_SPEED_DEFAULT;
    }

    #endregion ACTIONS

    #region POINTER ACTIONS

    public bool Contains(Point point)
    {
        return new Rect(Bounds.Size).Contains(point);
    }

    private void SpeedControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        if (IsOpen)
            _hasPointerEntered = true;
    }

    private void SpeedControl_PointerExited(object? sender, PointerEventArgs e)
    {
        if (!_hasPointerEntered
            || e.Pointer.Captured is not null
            || Contains(e.GetPosition(this)))
        {
            return;
        }

        Close();
    }

    #endregion POINTER ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
        {
            UpdateOpenState();
        }
        else if (change.Property == PlaybackSpeedProperty)
        {
            _isSynchronizingSpeed = true;
            SpeedPercent = PlaybackSpeedUtils.ToPercent(PlaybackSpeed);
            _isSynchronizingSpeed = false;
        }
    }

    private void UpdateOpenState()
    {
        if (IsOpen)
        {
            _hasPointerEntered = false;
            IsHitTestVisible = true;
            Dispatcher.UIThread.Post(
                () =>
                {
                    UpdateExpandedSize();
                    Height = _expandedHeight;
                },
                DispatcherPriority.Render);
        }
        else
        {
            Height = 0;
            _hasPointerEntered = false;
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (!IsOpen)
                        IsHitTestVisible = false;
                },
                DispatcherPriority.Background);
        }
    }

    #endregion PROPERTY CHANGE HANDLING

    #region ELEMENT INITIALIZATION

    private void InitializeElements()
    {
        DetachHandlers();

        _contentHost = this.FindControl<Border>("ContentHost");
        _preset1Button = this.FindControl<Button>("Preset1Button");
        _preset2Button = this.FindControl<Button>("Preset2Button");
        _preset3Button = this.FindControl<Button>("Preset3Button");
        _preset4Button = this.FindControl<Button>("Preset4Button");
        _resetButton = this.FindControl<Button>("ResetButton");

        if (_preset1Button is not null)
            _preset1Button.Click += Preset1Button_Click;

        if (_preset2Button is not null)
            _preset2Button.Click += Preset2Button_Click;

        if (_preset3Button is not null)
            _preset3Button.Click += Preset3Button_Click;

        if (_preset4Button is not null)
            _preset4Button.Click += Preset4Button_Click;

        if (_resetButton is not null)
            _resetButton.Click += ResetButton_Click;

        Dispatcher.UIThread.Post(UpdateExpandedSize, DispatcherPriority.Loaded);
    }

    private void DetachHandlers()
    {
        if (_preset1Button is not null)
            _preset1Button.Click -= Preset1Button_Click;

        if (_preset2Button is not null)
            _preset2Button.Click -= Preset2Button_Click;

        if (_preset3Button is not null)
            _preset3Button.Click -= Preset3Button_Click;

        if (_preset4Button is not null)
            _preset4Button.Click -= Preset4Button_Click;

        if (_resetButton is not null)
            _resetButton.Click -= ResetButton_Click;

        _contentHost = null;
    }

    private void UpdateExpandedSize()
    {
        if (_contentHost is null)
            return;

        _contentHost.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        Width = _contentHost.DesiredSize.Width;
        _expandedHeight = _contentHost.DesiredSize.Height;
    }

    #endregion ELEMENT INITIALIZATION
}
