using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using VisualPlayer.Enums.UI;

namespace VisualPlayer.Components;

/// <summary>
/// A single line of text scrolling inside a clipped border at a constant speed.
/// By default, only text wider than the viewport is animated.
/// </summary>
[TemplatePart("PART_Viewport", typeof(Canvas), IsRequired = true)]
[TemplatePart("PART_Text", typeof(TextBlock), IsRequired = true)]
public class ScrollTextBlock : TemplatedControl
{
    private readonly DispatcherTimer _animationTimer;
    private readonly Stopwatch _stopwatch = new();
    private readonly TranslateTransform _translation = new();

    private bool _isAttached;
    private TextBlock? _text;
    private Canvas? _viewport;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> AnimateIfTextTooLargeProperty
        = AvaloniaProperty.Register<ScrollTextBlock, bool>(nameof(AnimateIfTextTooLarge), true);

    public static readonly StyledProperty<TimeSpan> DisplayDurationProperty
        = AvaloniaProperty.Register<ScrollTextBlock, TimeSpan>(nameof(DisplayDuration), TimeSpan.FromSeconds(2), validate: value => value >= TimeSpan.Zero);

    public static readonly StyledProperty<ScrollTextType> ScrollTypeProperty
        = AvaloniaProperty.Register<ScrollTextBlock, ScrollTextType>(nameof(ScrollType), ScrollTextType.ToLeft, validate: value => Enum.IsDefined(value));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<ScrollTextBlock, string?>(nameof(Text));

    public static readonly StyledProperty<double> VelocityProperty
        = AvaloniaProperty.Register<ScrollTextBlock, double>(nameof(Velocity), 40, validate: value => double.IsFinite(value) && value > 0);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    /// <summary>When true, animate only text that exceeds the available viewport width.</summary>
    public bool AnimateIfTextTooLarge
    {
        get => GetValue(AnimateIfTextTooLargeProperty);
        set => SetValue(AnimateIfTextTooLargeProperty, value);
    }

    /// <summary>
    /// Stationary display time for To/From modes; blank time between full passes
    /// for RightToLeft and LeftToRight. May be zero.
    /// </summary>
    public TimeSpan DisplayDuration
    {
        get => GetValue(DisplayDurationProperty);
        set => SetValue(DisplayDurationProperty, value);
    }

    public ScrollTextType ScrollType
    {
        get => GetValue(ScrollTypeProperty);
        set => SetValue(ScrollTypeProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Scrolling speed in logical pixels per second; must be finite and positive.</summary>
    public double Velocity
    {
        get => GetValue(VelocityProperty);
        set => SetValue(VelocityProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public ScrollTextBlock()
    {
        _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1d / 60) };
        _animationTimer.Tick += AnimationTimer_Tick;
    }

    #endregion CONSTRUCTORS

    #region ANIMATION

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        if (!IsEffectivelyVisible)
        {
            _stopwatch.Restart();
            return;
        }

        UpdateFrame(_stopwatch.Elapsed.TotalSeconds);
    }

    // Derive position from elapsed time instead of frame count so the speed is
    // independent of text length, viewport width and timer scheduling delays.
    internal static (double Offset, bool IsVisible) CalculateFrame(
        ScrollTextType type, double elapsedSeconds, double viewportWidth,
        double textWidth, double velocity, TimeSpan displayDuration)
    {
        if (viewportWidth <= 0 || textWidth <= 0)
            return (0, false);

        var holdSeconds = displayDuration.TotalSeconds;
        var distance = type switch
        {
            ScrollTextType.RightToLeft or ScrollTextType.LeftToRight => viewportWidth + textWidth,
            ScrollTextType.ToLeft => textWidth,
            _ => viewportWidth
        };
        
        var travelSeconds = distance / velocity;
        var phase = elapsedSeconds % (travelSeconds + holdSeconds);

        return type switch
        {
            ScrollTextType.RightToLeft => (viewportWidth - Math.Min(phase, travelSeconds) * velocity, phase < travelSeconds),
            ScrollTextType.LeftToRight => (-textWidth + Math.Min(phase, travelSeconds) * velocity, phase < travelSeconds),
            ScrollTextType.ToLeft => (-Math.Max(0, phase - holdSeconds) * velocity, true),
            ScrollTextType.ToRight => (Math.Max(0, phase - holdSeconds) * velocity, true),
            ScrollTextType.FromLeft => (-textWidth + Math.Min(phase, travelSeconds) * velocity, true),
            ScrollTextType.FromRight => (viewportWidth - Math.Min(phase, travelSeconds) * velocity, true),
            _ => (0, true)
        };
    }

    private void RestartAnimation()
    {
        // Property changes can occur while the base constructor is running.
        if (_animationTimer is null)
            return;

        _animationTimer.Stop();
        _stopwatch.Reset();
        UpdateFrame(0);
        if (_isAttached && ShouldAnimate)
        {
            _stopwatch.Start();
            _animationTimer.Start();
        }
    }

    private bool ShouldAnimate => !string.IsNullOrEmpty(Text)
        && _viewport?.Bounds.Width > 0 && _text?.Bounds.Width > 0
        && (!AnimateIfTextTooLarge || _text.Bounds.Width > _viewport.Bounds.Width);

    private void UpdateFrame(double elapsedSeconds)
    {
        if (_viewport is null || _text is null)
            return;

        if (!ShouldAnimate)
        {
            _translation.X = 0;
            _text.Opacity = 1;
            return;
        }

        var frame = CalculateFrame(ScrollType, elapsedSeconds, _viewport.Bounds.Width,
            _text.Bounds.Width, Velocity, DisplayDuration);
        _translation.X = frame.Offset;
        _text.Opacity = frame.IsVisible ? 1 : 0;
    }

    #endregion ANIMATION

    #region PROPERTY CHANGES

    private void Content_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        RestartAnimation();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == AnimateIfTextTooLargeProperty)
        {
            RestartAnimation();
        }
        else if (change.Property == TextProperty || change.Property == ScrollTypeProperty
            || change.Property == DisplayDurationProperty || change.Property == VelocityProperty
            || change.Property == FontFamilyProperty || change.Property == FontSizeProperty
            || change.Property == FontWeightProperty || change.Property == FontStyleProperty
            || change.Property == FontStretchProperty)
        {
            InvalidateMeasure();
            RestartAnimation();
        }
    }

    #endregion PROPERTY CHANGES

    #region TEMPLATE AND LAYOUT

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_text is not null && _viewport is not null)
            Canvas.SetTop(_text, Math.Max(0, (_viewport.Bounds.Height - _text.Bounds.Height) / 2));
        return size;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _text?.Measure(Size.Infinity);
        var size = base.MeasureOverride(availableSize);
        if (_text is null)
            return size;

        // Canvas measures the text without a horizontal limit, but contributes
        // no desired size itself. Preserve the text's natural height and width.
        var horizontalInset = Padding.Left + Padding.Right + BorderThickness.Left + BorderThickness.Right;
        var verticalInset = Padding.Top + Padding.Bottom + BorderThickness.Top + BorderThickness.Bottom;
        return new Size(
            Math.Min(availableSize.Width, Math.Max(size.Width, _text.DesiredSize.Width + horizontalInset)),
            Math.Min(availableSize.Height, Math.Max(size.Height, _text.DesiredSize.Height + verticalInset)));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _animationTimer.Stop();
        if (_viewport is not null)
            _viewport.SizeChanged -= Content_SizeChanged;
        if (_text is not null)
            _text.SizeChanged -= Content_SizeChanged;

        base.OnApplyTemplate(e);
        _viewport = e.NameScope.Find<Canvas>("PART_Viewport");
        _text = e.NameScope.Find<TextBlock>("PART_Text");

        if (_viewport is not null)
            _viewport.SizeChanged += Content_SizeChanged;
        if (_text is not null)
        {
            _text.RenderTransform = _translation;
            _text.SizeChanged += Content_SizeChanged;
        }

        RestartAnimation();
    }

    #endregion TEMPLATE AND LAYOUT

    #region VISUAL TREE
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        RestartAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _animationTimer.Stop();
        _stopwatch.Reset();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE

    
}
