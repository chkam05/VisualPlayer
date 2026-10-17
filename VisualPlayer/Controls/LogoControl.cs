using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Core;
using VisualPlayer.Resources;

namespace VisualPlayer.Controls;

public class LogoControl : TemplatedControl
{
    #region CONST

    public const double DEFAULT_HEIGHT = 261;
    public const double DEFAULT_WIDTH = 300;
    public const double DEFAULT_REFERENCE_HEIGHT = 720;
    public const double DEFAULT_REFERENCE_WIDTH = 1280;
    public const double INFO_HEIGHT = 240;
    public const double INFO_WIDTH = 278;

    #endregion CONST

    private readonly DispatcherTimer _animationTimer;
    private readonly ScaleTransform _scaleTransform = new();

    private double _beatLevel;
    private double _currentScale = 1;
    private long _lastAnimationTimestamp;
    private double _renderedLogoHeight = DEFAULT_HEIGHT;
    private double _renderedLogoWidth = DEFAULT_WIDTH;
    private Viewbox? _logoViewbox;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<int> AnimationContractionProperty
        = AvaloniaProperty.Register<LogoControl, int>(nameof(AnimationContraction), 6);

    public static readonly StyledProperty<int> AnimationMaximumFrequencyProperty
        = AvaloniaProperty.Register<LogoControl, int>(nameof(AnimationMaximumFrequency), 180);

    public static readonly StyledProperty<int> AnimationMinimumFrequencyProperty
        = AvaloniaProperty.Register<LogoControl, int>(nameof(AnimationMinimumFrequency), 90);

    public static readonly StyledProperty<double> AnimationScaleProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(AnimationScale), 50);

    public static readonly StyledProperty<double> AnimationSensitivityProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(AnimationSensitivity), 30);

    public static readonly StyledProperty<bool> EnableAnimationProperty
        = AvaloniaProperty.Register<LogoControl, bool>(nameof(EnableAnimation), true);

    public static readonly StyledProperty<double> LogoHeightProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(LogoHeight), DEFAULT_HEIGHT);

    public static readonly StyledProperty<double> LogoWidthProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(LogoWidth), DEFAULT_WIDTH);

    public static readonly StyledProperty<IBrush?> PlayerBorderProperty
        = AvaloniaProperty.Register<LogoControl, IBrush?>(nameof(PlayerBorder), new SolidColorBrush(Colors.Black));

    public static readonly StyledProperty<IBrush?> PlayerFillProperty
        = AvaloniaProperty.Register<LogoControl, IBrush?>(nameof(PlayerFill), new SolidColorBrush(AccentColors.Blue.ToColor()));

    public static readonly StyledProperty<double> ReferenceHeightProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(ReferenceHeight), DEFAULT_REFERENCE_HEIGHT);

    public static readonly StyledProperty<double> ReferenceWidthProperty
        = AvaloniaProperty.Register<LogoControl, double>(nameof(ReferenceWidth), DEFAULT_REFERENCE_WIDTH);

    public static readonly DirectProperty<LogoControl, double> RenderedLogoHeightProperty
        = AvaloniaProperty.RegisterDirect<LogoControl, double>(nameof(RenderedLogoHeight), control => control.RenderedLogoHeight);

    public static readonly DirectProperty<LogoControl, double> RenderedLogoWidthProperty
        = AvaloniaProperty.RegisterDirect<LogoControl, double>(nameof(RenderedLogoWidth), control => control.RenderedLogoWidth);

    public static readonly StyledProperty<IBrush?> VisualBorderProperty
        = AvaloniaProperty.Register<LogoControl, IBrush?>(nameof(VisualBorder), new SolidColorBrush(Colors.Black));

    public static readonly StyledProperty<IBrush?> VisualFillProperty
        = AvaloniaProperty.Register<LogoControl, IBrush?>(nameof(VisualFill), new SolidColorBrush(AccentColors.Red.ToColor()));

    public static readonly StyledProperty<VisualizationCore?> VisualizationCoreProperty
        = AvaloniaProperty.Register<LogoControl, VisualizationCore?>(nameof(VisualizationCore));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public int AnimationContraction
    {
        get => GetValue(AnimationContractionProperty);
        set => SetValue(AnimationContractionProperty, value);
    }

    public int AnimationMaximumFrequency
    {
        get => GetValue(AnimationMaximumFrequencyProperty);
        set => SetValue(AnimationMaximumFrequencyProperty, value);
    }

    public int AnimationMinimumFrequency
    {
        get => GetValue(AnimationMinimumFrequencyProperty);
        set => SetValue(AnimationMinimumFrequencyProperty, value);
    }

    public double AnimationScale
    {
        get => GetValue(AnimationScaleProperty);
        set => SetValue(AnimationScaleProperty, value);
    }

    public double AnimationSensitivity
    {
        get => GetValue(AnimationSensitivityProperty);
        set => SetValue(AnimationSensitivityProperty, value);
    }

    public bool EnableAnimation
    {
        get => GetValue(EnableAnimationProperty);
        set => SetValue(EnableAnimationProperty, value);
    }

    public double LogoHeight
    {
        get => GetValue(LogoHeightProperty);
        set => SetValue(LogoHeightProperty, value);
    }

    public double LogoWidth
    {
        get => GetValue(LogoWidthProperty);
        set => SetValue(LogoWidthProperty, value);
    }

    public IBrush? PlayerBorder
    {
        get => GetValue(PlayerBorderProperty);
        set => SetValue(PlayerBorderProperty, value);
    }

    public IBrush? PlayerFill
    {
        get => GetValue(PlayerFillProperty);
        set => SetValue(PlayerFillProperty, value);
    }

    public double ReferenceHeight
    {
        get => GetValue(ReferenceHeightProperty);
        set => SetValue(ReferenceHeightProperty, value);
    }

    public double ReferenceWidth
    {
        get => GetValue(ReferenceWidthProperty);
        set => SetValue(ReferenceWidthProperty, value);
    }

    public double RenderedLogoHeight
    {
        get => _renderedLogoHeight;
        private set => SetAndRaise(RenderedLogoHeightProperty, ref _renderedLogoHeight, value);
    }

    public double RenderedLogoWidth
    {
        get => _renderedLogoWidth;
        private set => SetAndRaise(RenderedLogoWidthProperty, ref _renderedLogoWidth, value);
    }

    public IBrush? VisualBorder
    {
        get => GetValue(VisualBorderProperty);
        set => SetValue(VisualBorderProperty, value);
    }

    public IBrush? VisualFill
    {
        get => GetValue(VisualFillProperty);
        set => SetValue(VisualFillProperty, value);
    }

    public VisualizationCore? VisualizationCore
    {
        get => GetValue(VisualizationCoreProperty);
        set => SetValue(VisualizationCoreProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public LogoControl()
    {
        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000d / 60)
        };

        _animationTimer.Tick += AnimationTimer_Tick;
    }

    #endregion CONSTRUCTOR

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _logoViewbox = e.NameScope.Find<Viewbox>("PART_Logo");

        if (_logoViewbox is null)
            return;

        _logoViewbox.RenderTransform = _scaleTransform;
        _logoViewbox.RenderTransformOrigin = RelativePoint.Center;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        UpdateLogoSize(finalSize);
        return base.ArrangeOverride(finalSize);
    }

    private void UpdateLogoSize(Size availableSize)
    {
        if (availableSize.Width <= 0 || availableSize.Height <= 0)
            return;

        var widthScale = availableSize.Width / Math.Max(ReferenceWidth, 1);
        var heightScale = availableSize.Height / Math.Max(ReferenceHeight, 1);
        var sizeScale = Math.Min(widthScale, heightScale);

        RenderedLogoWidth = Math.Max(1, LogoWidth * sizeScale);
        RenderedLogoHeight = Math.Max(1, LogoHeight * sizeScale);
    }

    #endregion TEMPLATE

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _lastAnimationTimestamp = Stopwatch.GetTimestamp();
        _animationTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _animationTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE

    #region ANIMATION

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        var currentTimestamp = Stopwatch.GetTimestamp();
        var elapsedSeconds = _lastAnimationTimestamp == 0
            ? 0
            : (double)(currentTimestamp - _lastAnimationTimestamp) / Stopwatch.Frequency;

        _lastAnimationTimestamp = currentTimestamp;
        elapsedSeconds = Math.Clamp(elapsedSeconds, 0, 0.1);

        if (!EnableAnimation || VisualizationCore?.CurrentVisualization is null)
        {
            _beatLevel = 0;
            _currentScale = 1;
            _scaleTransform.ScaleX = 1;
            _scaleTransform.ScaleY = 1;
            return;
        }

        var level = VisualizationCore.GetFrequencyLevel(
            AnimationMinimumFrequency,
            AnimationMaximumFrequency);

        UpdateBeatLevel(level, elapsedSeconds);

        var animationScale = Math.Max(0, AnimationScale) / 100d;
        _currentScale = 1 + _beatLevel * animationScale;

        _scaleTransform.ScaleX = _currentScale;
        _scaleTransform.ScaleY = _currentScale;
    }

    private void UpdateBeatLevel(double spectrumLevel, double elapsedSeconds)
    {
        if (!EnableAnimation)
        {
            _beatLevel = Math.Max(0, _beatLevel - AnimationContraction * elapsedSeconds);
            return;
        }

        var sensitivity = Math.Clamp(AnimationSensitivity, 0, 100) / 100d;

        if (sensitivity <= 0)
        {
            _beatLevel = Math.Max(0, _beatLevel - AnimationContraction * elapsedSeconds);
            return;
        }

        var limiter = 1 - sensitivity;
        var beat = spectrumLevel > limiter
            ? (spectrumLevel - limiter) / sensitivity
            : 0;

        _beatLevel = beat > _beatLevel
            ? Math.Min(beat, 1)
            : Math.Max(0, _beatLevel - AnimationContraction * elapsedSeconds);
    }

    #endregion ANIMATION
}
