using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Core;
using VisualPlayer.Enums;

namespace VisualPlayer.Controls;

[TemplatePart("PART_Canvas", typeof(Canvas), IsRequired = true)]
public class MiniVisualizationControl : TemplatedControl
{
    #region CONST

    public const int FrequencyBandCount = 7;
    public const int LogicalLevelCount = 11;
    public const int SegmentCount = LogicalLevelCount * 2;
    public const int CenterMaximumDistance = LogicalLevelCount / 2;

    public const double EdgeBarHeight = 2;
    public const double EdgeDotSize = 2;
    public const double EdgeSpacing = 4;
    public const double EdgeWidth = 5;

    public const double MainDescentRate = 20;
    public const double FloatingBarDescentRate = 10;
    public const double RegularFloatingBarFallDelay = 0.2;
    public const double FloatingBarRestartDelay = 0.3;
    public const double MinimumVisibleLevel = 0.5;

    public const double InactiveSegmentOpacity = 0.10;
    public const double ActiveSegmentOpacity = 1.0;
    public const double GlowBlurRadius = 4;
    public const double GlowOpacity = 0.85;

    public const double SegmentHeight = 2;
    public const double SegmentHorizontalSpacing = 4;
    public const double SegmentVerticalSpacing = 1;
    public const double SegmentWidth = 10;

    #endregion CONST

    private readonly double[] _bandLevels = new double[FrequencyBandCount];
    private readonly double[] _floatingLevels = new double[FrequencyBandCount];
    private readonly double[] _mainLevels = new double[FrequencyBandCount];
    private readonly List<DropShadowEffect> _accentGlowEffects = [];
    private readonly List<Shape> _accentShapes = [];
    private readonly Rectangle[,] _segments = new Rectangle[FrequencyBandCount, SegmentCount];
    private readonly DispatcherTimer _renderTimer;
    private Canvas? _canvas;
    private bool _outwardFloatingGroupIsFalling;
    private double _outwardFloatingRestartDelayRemaining;
    private bool _regularFloatingGroupIsFalling;
    private double _regularFloatingFallDelayRemaining;

    #region AVALONIA PROPERTIES
    
    public static readonly StyledProperty<VisualizationCore?> CoreProperty
        = AvaloniaProperty.Register<MiniVisualizationControl, VisualizationCore?>(nameof(Core));

    public static readonly StyledProperty<MiniVisualizationType> VisualizationTypeProperty
        = AvaloniaProperty.Register<MiniVisualizationControl, MiniVisualizationType>(
            nameof(VisualizationType),
            MiniVisualizationType.Regular,
            defaultBindingMode: BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public VisualizationCore? Core
    {
        get => GetValue(CoreProperty);
        set => SetValue(CoreProperty, value);
    }

    public MiniVisualizationType VisualizationType
    {
        get => GetValue(VisualizationTypeProperty);
        set => SetValue(VisualizationTypeProperty, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public MiniVisualizationControl()
    {
        _renderTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1d / 60) };
        _renderTimer.Tick += RenderTimer_Tick;
    }

    #endregion CONSTRUCTOR

    #region PROPERTY CHANGES

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ForegroundProperty)
            UpdateAccentBrush();
        else if (change.Property == VisualizationTypeProperty)
            ResetAnimation();
    }

    #endregion PROPERTY CHANGES

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _canvas = e.NameScope.Find<Canvas>("PART_Canvas");
        BuildVisualization();
    }

    #endregion TEMPLATE

    #region VISUALIZATION

    private void BuildVisualization()
    {
        if (_canvas is null)
            return;

        _canvas.Children.Clear();
        _accentGlowEffects.Clear();
        _accentShapes.Clear();

        var visualisationHeight = SegmentCount * SegmentHeight
            + (SegmentCount - 1) * SegmentVerticalSpacing;
        var peaksWidth = FrequencyBandCount * SegmentWidth
            + (FrequencyBandCount - 1) * SegmentHorizontalSpacing;
        var rightEdgeX = EdgeWidth + EdgeSpacing + peaksWidth + EdgeSpacing;

        _canvas.Width = rightEdgeX + EdgeWidth;
        _canvas.Height = visualisationHeight;
        BuildEdge(0, visualisationHeight);
        BuildEdge(rightEdgeX, visualisationHeight);

        for (var bandIndex = 0; bandIndex < FrequencyBandCount; bandIndex++)
        {
            var x = EdgeWidth + EdgeSpacing
                + bandIndex * (SegmentWidth + SegmentHorizontalSpacing);

            for (var segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
            {
                var top = segmentIndex * (SegmentHeight + SegmentVerticalSpacing);
                var inactiveSegment = new Rectangle
                {
                    Fill = Foreground,
                    Height = SegmentHeight,
                    Opacity = InactiveSegmentOpacity,
                    Width = SegmentWidth
                };

                Canvas.SetLeft(inactiveSegment, x);
                Canvas.SetTop(inactiveSegment, top);
                _accentShapes.Add(inactiveSegment);
                _canvas.Children.Add(inactiveSegment);

                var segment = new Rectangle
                {
                    Effect = CreateGlowEffect(Foreground, _accentGlowEffects),
                    Fill = Foreground,
                    Height = SegmentHeight,
                    IsVisible = false,
                    Opacity = ActiveSegmentOpacity,
                    Width = SegmentWidth
                };

                Canvas.SetLeft(segment, x);
                Canvas.SetTop(segment, top);
                _segments[bandIndex, segmentIndex] = segment;
                _accentShapes.Add(segment);
                _canvas.Children.Add(segment);
            }
        }
    }

    private void BuildEdge(double x, double height)
    {
        if (_canvas is null)
            return;

        const int groupCount = 3;
        const int dotsPerGroup = 4;
        var barPositions = new[] { 0d, height / 3, height * 2 / 3, height - EdgeBarHeight };

        foreach (var y in barPositions)
        {
            var bar = new Rectangle
            {
                Effect = CreateGlowEffect(Brushes.Red),
                Fill = Brushes.Red,
                Height = EdgeBarHeight,
                Width = EdgeWidth
            };
            Canvas.SetLeft(bar, x);
            Canvas.SetTop(bar, y);
            _canvas.Children.Add(bar);
        }

        for (var group = 0; group < groupCount; group++)
        {
            var start = barPositions[group] + EdgeBarHeight;
            var end = barPositions[group + 1];
            var step = (end - start) / (dotsPerGroup + 1);

            for (var dotIndex = 0; dotIndex < dotsPerGroup; dotIndex++)
            {
                var dot = new Ellipse
                {
                    Effect = CreateGlowEffect(Foreground, _accentGlowEffects),
                    Fill = Foreground,
                    Height = EdgeDotSize,
                    Width = EdgeDotSize
                };
                Canvas.SetLeft(dot, x + (EdgeWidth - EdgeDotSize) / 2);
                Canvas.SetTop(dot, start + step * (dotIndex + 1) - EdgeDotSize / 2);
                _accentShapes.Add(dot);
                _canvas.Children.Add(dot);
            }
        }
    }

    private void RenderTimer_Tick(object? sender, EventArgs e)
    {
        if (!IsEffectivelyVisible || Core is null)
            return;

        if (_renderTimer.Interval != Core.FrameInterval)
            _renderTimer.Interval = Core.FrameInterval;

        var elapsed = Core.UpdateSpectrum();
        Core.GetRawFrequencyBands(_bandLevels);
        var elapsedSeconds = Math.Clamp(elapsed.TotalSeconds, 1d / 240, 0.1);
        UpdateMainLevels(elapsedSeconds, Core.IsPlaying);

        if (UsesFloatingBar())
            UpdateFloatingLevels(elapsedSeconds, Core.IsPlaying);

        UpdateSegments();
    }

    private void ResetAnimation()
    {
        Array.Clear(_floatingLevels);
        Array.Clear(_mainLevels);
        _outwardFloatingGroupIsFalling = false;
        _outwardFloatingRestartDelayRemaining = 0;
        _regularFloatingGroupIsFalling = false;
        _regularFloatingFallDelayRemaining = 0;

        if (_canvas is not null)
            UpdateSegments();
    }

    private void SetCenterFill(int bandIndex, int levelCount)
    {
        if (levelCount <= 0)
            return;

        SetLogicalPair(bandIndex, CenterMaximumDistance);

        for (var distance = 1; distance < levelCount; distance++)
        {
            SetLogicalPair(bandIndex, CenterMaximumDistance - distance);
            SetLogicalPair(bandIndex, CenterMaximumDistance + distance);
        }
    }

    private void SetCenterFloating(int bandIndex, int distance)
    {
        SetLogicalPair(bandIndex, CenterMaximumDistance - distance);

        if (distance > 0)
            SetLogicalPair(bandIndex, CenterMaximumDistance + distance);
    }

    private void SetLogicalPair(int bandIndex, int logicalLevel)
    {
        var first = logicalLevel * 2;
        _segments[bandIndex, first].IsVisible = true;
        _segments[bandIndex, first + 1].IsVisible = true;
    }

    private void SetRegularFill(int bandIndex, int levelCount, bool upsideDown)
    {
        for (var level = 0; level < levelCount; level++)
        {
            var first = upsideDown ? level * 2 : SegmentCount - 2 - level * 2;
            _segments[bandIndex, first].IsVisible = true;
            _segments[bandIndex, first + 1].IsVisible = true;
        }
    }

    private void SetRegularFloating(int bandIndex, int level, bool upsideDown)
    {
        var first = upsideDown ? level * 2 : SegmentCount - 2 - level * 2;
        _segments[bandIndex, first].IsVisible = true;
        _segments[bandIndex, first + 1].IsVisible = true;
    }

    private void UpdateFloatingLevels(double elapsedSeconds, bool isPlaying)
    {
        if (VisualizationType == MiniVisualizationType.Regular)
        {
            UpdateRegularFloatingGroup(elapsedSeconds);
            return;
        }

        if (VisualizationType is MiniVisualizationType.UpsideDown
            or MiniVisualizationType.CenterFilled)
        {
            UpdateOutwardFloatingGroup(elapsedSeconds, isPlaying);
        }
    }

    private void UpdateMainLevels(double elapsedSeconds, bool isPlaying)
    {
        for (var index = 0; index < FrequencyBandCount; index++)
        {
            var target = isPlaying
                ? Math.Clamp(_bandLevels[index], 0, 1) * (LogicalLevelCount - 1)
                : 0;
            _mainLevels[index] = target >= _mainLevels[index]
                ? target
                : Math.Max(target, _mainLevels[index] - MainDescentRate * elapsedSeconds);
        }
    }

    private void UpdateOutwardFloatingGroup(double elapsedSeconds, bool isPlaying)
    {
        if (_outwardFloatingRestartDelayRemaining > 0)
        {
            _outwardFloatingRestartDelayRemaining = Math.Max(
                0,
                _outwardFloatingRestartDelayRemaining - elapsedSeconds);
            return;
        }

        if (!_outwardFloatingGroupIsFalling)
        {
            if (!isPlaying)
            {
                Array.Clear(_floatingLevels);
                _outwardFloatingGroupIsFalling = true;
                return;
            }

            for (var index = 0; index < FrequencyBandCount; index++)
            {
                var target = VisualizationType == MiniVisualizationType.CenterFilled
                    ? GetCenterMainLevelCount(_mainLevels[index])
                    : Math.Clamp(_bandLevels[index], 0, 1) * (LogicalLevelCount - 1);

                if (target >= _floatingLevels[index])
                    _floatingLevels[index] = target;
                else if (_floatingLevels[index] > MinimumVisibleLevel)
                    _outwardFloatingGroupIsFalling = true;
            }

            return;
        }

        var maximumLevel = VisualizationType == MiniVisualizationType.CenterFilled
            ? CenterMaximumDistance
            : LogicalLevelCount - 1;
        var descentRate = VisualizationType == MiniVisualizationType.CenterFilled
            ? FloatingBarDescentRate / 2
            : FloatingBarDescentRate;
        var allOutside = true;

        for (var index = 0; index < FrequencyBandCount; index++)
        {
            _floatingLevels[index] += descentRate * elapsedSeconds;
            allOutside &= _floatingLevels[index] > maximumLevel;
        }

        if (!allOutside)
            return;

        Array.Clear(_floatingLevels);
        _outwardFloatingGroupIsFalling = false;
        _outwardFloatingRestartDelayRemaining = FloatingBarRestartDelay;
    }

    private void UpdateRegularFloatingGroup(double elapsedSeconds)
    {
        var wasPushed = false;
        var hasLevelAboveMainBar = false;

        for (var index = 0; index < FrequencyBandCount; index++)
        {
            var target = GetRegularMainLevelCount(_mainLevels[index]);

            if (target > _floatingLevels[index])
            {
                _floatingLevels[index] = target;
                wasPushed = true;
            }

            hasLevelAboveMainBar |= _floatingLevels[index] > target + 0.01;
        }

        if (wasPushed)
        {
            _regularFloatingGroupIsFalling = false;
            _regularFloatingFallDelayRemaining = RegularFloatingBarFallDelay;
            return;
        }

        if (!hasLevelAboveMainBar)
        {
            _regularFloatingGroupIsFalling = false;
            return;
        }

        if (!_regularFloatingGroupIsFalling)
        {
            _regularFloatingFallDelayRemaining = Math.Max(
                0,
                _regularFloatingFallDelayRemaining - elapsedSeconds);

            if (_regularFloatingFallDelayRemaining > 0)
                return;

            _regularFloatingGroupIsFalling = true;
        }

        var allRestingOnMainBars = true;

        for (var index = 0; index < FrequencyBandCount; index++)
        {
            var target = GetRegularMainLevelCount(_mainLevels[index]);
            _floatingLevels[index] = Math.Max(
                target,
                _floatingLevels[index] - FloatingBarDescentRate * elapsedSeconds);
            allRestingOnMainBars &= _floatingLevels[index] <= target + 0.01;
        }

        if (!allRestingOnMainBars)
            return;

        _regularFloatingGroupIsFalling = false;
        _regularFloatingFallDelayRemaining = RegularFloatingBarFallDelay;
    }

    private static int GetRegularMainLevelCount(double level)
    {
        return Math.Clamp(
            (int)Math.Ceiling(level / (LogicalLevelCount - 1) * (LogicalLevelCount - 1)),
            0,
            LogicalLevelCount - 1);
    }

    private static int GetCenterMainLevelCount(double level)
    {
        return Math.Clamp(
            (int)Math.Ceiling(level / (LogicalLevelCount - 1) * CenterMaximumDistance),
            0,
            CenterMaximumDistance);
    }

    private bool UsesFloatingBar()
    {
        return VisualizationType is MiniVisualizationType.Regular
            or MiniVisualizationType.UpsideDown
            or MiniVisualizationType.CenterFilled;
    }

    private void UpdateAccentBrush()
    {
        foreach (var shape in _accentShapes)
            shape.Fill = Foreground;

        var glowColor = GetBrushColor(Foreground);
        foreach (var effect in _accentGlowEffects)
            effect.Color = glowColor;
    }

    private static DropShadowEffect CreateGlowEffect(
        IBrush? brush,
        List<DropShadowEffect>? trackedEffects = null)
    {
        var effect = new DropShadowEffect
        {
            BlurRadius = GlowBlurRadius,
            Color = GetBrushColor(brush),
            OffsetX = 0,
            OffsetY = 0,
            Opacity = GlowOpacity
        };

        trackedEffects?.Add(effect);
        return effect;
    }

    private static Color GetBrushColor(IBrush? brush)
    {
        return brush is ISolidColorBrush solidColorBrush
            ? solidColorBrush.Color
            : Colors.White;
    }

    private void UpdateSegments()
    {
        for (var bandIndex = 0; bandIndex < FrequencyBandCount; bandIndex++)
        {
            for (var segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
                _segments[bandIndex, segmentIndex].IsVisible = false;

            var filledLevelCount = Math.Clamp(
                (int)Math.Ceiling(_mainLevels[bandIndex] / (LogicalLevelCount - 1) * LogicalLevelCount),
                0,
                LogicalLevelCount);
            var regularFilledLevelCount = GetRegularMainLevelCount(_mainLevels[bandIndex]);
            var centerFilledLevelCount = GetCenterMainLevelCount(_mainLevels[bandIndex]);
            var mainLevel = Math.Clamp(
                (int)Math.Round(_mainLevels[bandIndex]),
                0,
                LogicalLevelCount - 1);
            var floatingLevel = Math.Clamp(
                (int)Math.Round(_floatingLevels[bandIndex]),
                0,
                LogicalLevelCount - 1);
            var centerFloatingLevel = Math.Clamp(floatingLevel, 0, CenterMaximumDistance);
            var centerSingleLevel = GetCenterMainLevelCount(_mainLevels[bandIndex]);
            var floatingMaximumLevel = VisualizationType == MiniVisualizationType.CenterFilled
                ? CenterMaximumDistance
                : LogicalLevelCount - 1;
            var hasOutwardFloatingGroup = _outwardFloatingGroupIsFalling
                && VisualizationType is (MiniVisualizationType.UpsideDown
                    or MiniVisualizationType.CenterFilled);
            var hasFloatingLevel = (_floatingLevels[bandIndex] > MinimumVisibleLevel
                || hasOutwardFloatingGroup)
                && _floatingLevels[bandIndex] <= floatingMaximumLevel;

            switch (VisualizationType)
            {
                case MiniVisualizationType.Regular:
                    SetRegularFill(bandIndex, regularFilledLevelCount, false);
                    if (hasFloatingLevel)
                        SetRegularFloating(bandIndex, floatingLevel, false);
                    break;
                case MiniVisualizationType.UpsideDown:
                    SetRegularFill(bandIndex, filledLevelCount, true);
                    if (hasFloatingLevel)
                        SetRegularFloating(bandIndex, floatingLevel, true);
                    break;
                case MiniVisualizationType.Single:
                    if (_mainLevels[bandIndex] > MinimumVisibleLevel)
                        SetRegularFloating(bandIndex, mainLevel, false);
                    break;
                case MiniVisualizationType.CenterFilled:
                    SetCenterFill(bandIndex, centerFilledLevelCount);
                    if (hasFloatingLevel)
                        SetCenterFloating(bandIndex, centerFloatingLevel);
                    break;
                case MiniVisualizationType.CenterSingle:
                    if (_mainLevels[bandIndex] > MinimumVisibleLevel)
                        SetCenterFloating(bandIndex, centerSingleLevel);
                    break;
            }
        }
    }

    #endregion VISUALIZATION

    #region INPUT

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        VisualizationType = VisualizationType switch
        {
            MiniVisualizationType.Regular => MiniVisualizationType.UpsideDown,
            MiniVisualizationType.UpsideDown => MiniVisualizationType.Single,
            MiniVisualizationType.Single => MiniVisualizationType.CenterFilled,
            MiniVisualizationType.CenterFilled => MiniVisualizationType.CenterSingle,
            _ => MiniVisualizationType.Regular
        };

        e.Handled = true;
    }

    #endregion INPUT

    #region VISUAL TREE

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _renderTimer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _renderTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    #endregion VISUAL TREE
}
