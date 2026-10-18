using System;
using System.Linq;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.CircleStripes.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.CircleStripes;

public class CircleStripesVisualization : VisualizationBase<CircleStripesVisualizationConfig>
{
    private readonly VisualizationColorConfig _colorConfig;
    private double _animatedRotation;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private float[] _levels = [];

    public override bool HasActiveAnimation
    {
        get => Config.RotationAnimationEnabled
            || _colorConfig.IsAnimationEnabled
            || _levels.Any(level => level > 0.001f);
    }

    public CircleStripesVisualization(
        CircleStripesVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length < 2 || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
            return;

        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var requestedThickness = Config.LineThickness * RenderScaling;
        var outerRadius = CircleVisualizationUtils.GetOuterRadius(DrawingArea, requestedThickness);
        var innerRadius = Math.Clamp(Config.InnerRadius * RenderScaling, 0, outerRadius);
        var lineThickness = Math.Min(
            requestedThickness,
            CircleVisualizationUtils.GetMaximumLineThickness(innerRadius, _levels.Length));
        var availableRadius = Math.Max(0, outerRadius - innerRadius);
        var rotation = (Config.Rotation + _animatedRotation) * Math.PI / 180;
        var center = new Avalonia.Point(
            DrawingArea.Center.X + Config.CenterOffsetX * RenderScaling,
            DrawingArea.Center.Y + Config.CenterOffsetY * RenderScaling);
        var radialBrush = CreateRadialBrush(center, outerRadius);

        using (drawingContext.PushClip(DrawingArea))
        {
            for (var index = 0; index < _levels.Length; index++)
            {
                var position = index / (double)_levels.Length;
                var angle = rotation + position * Math.PI * 2;
                var length = _levels[index] * availableRadius;
                var startRadius = Config.Direction == RadialVisualizationDirection.InsideOut
                    ? innerRadius
                    : outerRadius;
                var endRadius = Config.Direction == RadialVisualizationDirection.InsideOut
                    ? innerRadius + length
                    : outerRadius - length;
                var start = CircleVisualizationUtils.GetPoint(center, startRadius, angle);
                var end = CircleVisualizationUtils.GetPoint(center, endRadius, angle);
                var brush = radialBrush ?? CreateStripeBrush(start, end, fallbackColor);

                if (Config.BeamEffect == BeamEffect.Beam)
                {
                    using (drawingContext.PushOpacity(0.22))
                    {
                        drawingContext.DrawLine(
                            new Pen(brush, lineThickness * 4)
                            {
                                LineCap = PenLineCap.Round
                            },
                            start,
                            end);
                    }
                }

                drawingContext.DrawLine(
                    new Pen(brush, lineThickness)
                    {
                        LineCap = PenLineCap.Round
                    },
                    start,
                    end);
            }
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != Config.StripesCount)
            _levels = new float[Config.StripesCount];

        var elapsedSeconds = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var normalizedDescent = Config.DescentRate
            / Math.Max(1, DrawingArea.Height)
            * elapsedSeconds;
        var spectrum = frame.Spectrum.Span;

        _animatedRotation = CircleVisualizationUtils.GetAnimatedRotation(
            _animatedRotation,
            Config.RotationAnimationEnabled,
            Config.RotationDirection,
            Config.RotationSpeed,
            elapsedSeconds);

        CommonUtils.UpdateColorAnimation(
            _colorConfig,
            elapsedSeconds,
            DrawingArea,
            ref _gradientOffsetX,
            ref _gradientOffsetY);

        for (var index = 0; index < _levels.Length; index++)
        {
            var position = index / (double)_levels.Length;
            var spectrumPosition = CircleVisualizationUtils.GetMirroredPosition(position, Config.Mirror);
            var spectrumIndex = Math.Clamp(
                (int)Math.Round(spectrumPosition * (_levels.Length - 1)),
                0,
                _levels.Length - 1);
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(spectrum, spectrumIndex, _levels.Length)
                : 0;

            _levels[index] = target >= _levels[index]
                ? (float)target
                : (float)Math.Max(0, _levels[index] - normalizedDescent);
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        _animatedRotation = 0;
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    private IBrush? CreateRadialBrush(Avalonia.Point center, double outerRadius)
    {
        var isGradientRadial = _colorConfig.Mode == VisualizationColorMode.Gradient
            && _colorConfig.Gradient.Type == GradientType.Radial;
        var isRainbowRadial = _colorConfig.Mode == VisualizationColorMode.Rainbow
            && _colorConfig.RainbowGradientType == GradientType.Radial;

        if (!isGradientRadial && !isRainbowRadial)
        {
            return null;
        }

        var stretch = Math.Max(0.1, _colorConfig.Stretch / 100d);

        return isRainbowRadial
            ? GradientBrushUtils.CreateAbsoluteRadialRainbow(
                center,
                outerRadius,
                DrawingArea,
                _gradientOffsetX,
                _gradientOffsetY,
                stretch)
            : GradientBrushUtils.CreateAbsoluteRadial(
                _colorConfig.Gradient,
                center,
                outerRadius,
                DrawingArea,
                _gradientOffsetX,
                _gradientOffsetY,
                stretch);
    }

    private IBrush CreateStripeBrush(
        Avalonia.Point start,
        Avalonia.Point end,
        Color fallbackColor)
    {
        var midpoint = new Avalonia.Point(
            (start.X + end.X) / 2,
            (start.Y + end.Y) / 2);
        var normalizedX = (midpoint.X - DrawingArea.Left) / Math.Max(1, DrawingArea.Width);
        var normalizedY = (midpoint.Y - DrawingArea.Top) / Math.Max(1, DrawingArea.Height);
        var color = VisualizationColorSampler.GetColor(
            _colorConfig,
            normalizedX,
            normalizedY,
            _gradientOffsetX,
            _gradientOffsetY,
            fallbackColor);

        return new SolidColorBrush(color);
    }
}
