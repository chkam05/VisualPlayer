using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

public partial class InfoBarControl : UserControl
{
    #region CONST

    private static readonly TimeSpan AUTO_HIDE_ANIMATION_DURATION = TimeSpan.FromMilliseconds(220);

    #endregion CONST

    private Border? _contentHost;
    private double _expandedHeight;
    private readonly TranslateTransform _hideTransform;
    private readonly DispatcherTimer _hideTimer;
    private bool _isAutoHideInitialized;
    private bool _isHidden;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> AutoHideProperty
        = AvaloniaProperty.Register<InfoBarControl, bool>(nameof(AutoHide));

    public static readonly StyledProperty<int> DisplayDurationProperty
        = AvaloniaProperty.Register<InfoBarControl, int>(nameof(DisplayDuration), 5);
    
    public static readonly DirectProperty<InfoBarControl, bool> IsHiddenProperty
        = AvaloniaProperty.RegisterDirect<InfoBarControl, bool>(nameof(IsHidden), control => control.IsHidden);

    public static readonly StyledProperty<bool> IsManuallyHiddenProperty
        = AvaloniaProperty.Register<InfoBarControl, bool>(nameof(IsManuallyHidden));

    public static readonly StyledProperty<double> RevealSizeProperty
        = AvaloniaProperty.Register<InfoBarControl, double>(nameof(RevealSize), Dimensions.AutoHideRevealSize);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool AutoHide
    {
        get => GetValue(AutoHideProperty);
        set => SetValue(AutoHideProperty, value);
    }

    public int DisplayDuration
    {
        get => GetValue(DisplayDurationProperty);
        set => SetValue(DisplayDurationProperty, value);
    }

    public bool IsHidden
    {
        get => _isHidden;
        private set => SetAndRaise(IsHiddenProperty, ref _isHidden, value);
    }

    public bool IsManuallyHidden
    {
        get => GetValue(IsManuallyHiddenProperty);
        set => SetValue(IsManuallyHiddenProperty, value);
    }

    public double RevealSize
    {
        get => GetValue(RevealSizeProperty);
        set => SetValue(RevealSizeProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InfoBarControl()
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

        _hideTimer = new DispatcherTimer();
        _hideTimer.Tick += HideTimer_Tick;

        Transitions =
        [
            new DoubleTransition
            {
                Property = HeightProperty,
                Duration = AUTO_HIDE_ANIMATION_DURATION
            }
        ];

        PointerEntered += InfoBarControl_PointerEntered;
        PointerExited += InfoBarControl_PointerExited;

        InitializeComponent();
        InitializeElements();
    }

    #endregion CONSTRUCTORS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DisplayDurationProperty && _hideTimer.IsEnabled)
        {
            ShowTemporarily();
            return;
        }

        if (change.Property == IsManuallyHiddenProperty)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (IsManuallyHidden)
                        HideControl();
                    else
                        ShowControl();
                },
                DispatcherPriority.Loaded);
        }
        else if (change.Property == AutoHideProperty || change.Property == RevealSizeProperty)
            Dispatcher.UIThread.Post(ApplyAutoHideState, DispatcherPriority.Loaded);
    }

    #endregion PROPERTY CHANGE HANDLING

    #region SHOW HIDE

    private void ApplyAutoHideState()
    {
        if (!_isAutoHideInitialized)
            return;

        if (IsManuallyHidden)
        {
            _hideTimer.Stop();
            HideControl();
        }
        else if (!AutoHide || IsPointerOver)
        {
            _hideTimer.Stop();
            ShowControl();
        }
        else
        {
            HideControl();
        }
    }

    private void HideControl()
    {
        if ((!AutoHide && !IsManuallyHidden)
            || !_isAutoHideInitialized
            || _contentHost is null
            || IsHidden)
            return;

        _hideTimer.Stop();
        IsHidden = true;
        Height = Math.Max(1, RevealSize);
        _hideTransform.Y = -_expandedHeight;
    }

    private void HideTimer_Tick(object? sender, EventArgs e)
    {
        _hideTimer.Stop();

        if (!IsPointerOver)
            HideControl();
    }

    private void InfoBarControl_PointerEntered(object? sender, PointerEventArgs e)
    {
        _hideTimer.Stop();
        ShowControl();
    }

    private void InfoBarControl_PointerExited(object? sender, PointerEventArgs e)
    {
        HideControl();
    }

    private void InitializeAutoHide()
    {
        if (_contentHost is null)
            return;

        var expandedHeight = Math.Max(Bounds.Height, DesiredSize.Height);

        if (expandedHeight <= 0)
            return;

        _expandedHeight = expandedHeight;
        _contentHost.RenderTransform = _hideTransform;
        Height = expandedHeight;
        _isAutoHideInitialized = true;

        ApplyAutoHideState();
    }

    public void ShowTemporarily()
    {
        if (IsManuallyHidden || !AutoHide || !_isAutoHideInitialized)
            return;

        ShowControl();
        _hideTimer.Stop();
        _hideTimer.Interval = TimeSpan.FromSeconds(Math.Max(1, DisplayDuration));
        _hideTimer.Start();
    }

    private void ShowControl()
    {
        if (IsManuallyHidden || !_isAutoHideInitialized || _contentHost is null)
            return;

        IsHidden = false;
        Height = _expandedHeight;
        _hideTransform.Y = 0;
    }

    #endregion SHOW HIDE

    #region ELEMENT INITIALIZATION

    private void InitializeElements()
    {

        _contentHost = this.FindControl<Border>("ContentHost");
        _isAutoHideInitialized = false;

        Dispatcher.UIThread.Post(InitializeAutoHide, DispatcherPriority.Loaded);
    }

    #endregion ELEMENT INITIALIZATION

    #region VISUAL TREE

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _hideTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE
}
