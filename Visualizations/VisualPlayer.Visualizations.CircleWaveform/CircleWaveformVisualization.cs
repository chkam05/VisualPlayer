using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.CircleWaveform.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.CircleWaveform;

public class CircleWaveformVisualization : VisualizationBase<CircleWaveformVisualizationConfig>
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

    public CircleWaveformVisualization(
        CircleWaveformVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length < 3 || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
            return;

        var thickness = Config.LineThickness * RenderScaling;
        var outerRadius = CircleVisualizationUtils.GetOuterRadius(DrawingArea, thickness);
        var innerRadius = Math.Clamp(Config.InnerRadius * RenderScaling, 0, outerRadius);
        var availableRadius = Math.Max(0, outerRadius - innerRadius);
        var rotation = (Config.Rotation + _animatedRotation) * Math.PI / 180;
        var center = new Point(
            DrawingArea.Center.X + Config.CenterOffsetX * RenderScaling,
            DrawingArea.Center.Y + Config.CenterOffsetY * RenderScaling);
        var waveformPoints = new Point[_levels.Length];

        for (var index = 0; index < _levels.Length; index++)
        {
            var position = index / (double)_levels.Length;
            var angle = rotation + position * Math.PI * 2;
            var transformedLevel = _levels[index];
            var radius = innerRadius + transformedLevel * availableRadius;

            waveformPoints[index] = CircleVisualizationUtils.GetPoint(center, radius, angle);
        }

        var waveformGeometry = CircleVisualizationUtils.CreateClosedSmoothGeometry(waveformPoints);
        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var brush = CreateBrush(fallbackColor, center, outerRadius);

        using (drawingContext.PushClip(DrawingArea))
        {
            if (Config.FillEnabled)
            {
                using (drawingContext.PushOpacity(Config.Opacity))
                    drawingContext.DrawGeometry(brush, null, waveformGeometry);
            }

            if (Config.BeamEffect == BeamEffect.Beam)
            {
                using (drawingContext.PushOpacity(0.22))
                {
                    drawingContext.DrawGeometry(
                        null,
                        new Pen(brush, thickness * 4),
                        waveformGeometry);
                }
            }

            drawingContext.DrawGeometry(
                null,
                new Pen(brush, thickness),
                waveformGeometry);
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != Config.PeaksCount)
            _levels = new float[Config.PeaksCount];

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

    private IBrush CreateBrush(
        Color fallbackColor,
        Point center,
        double outerRadius)
    {
        var stretch = Math.Max(0.1, _colorConfig.Stretch / 100d);

        return _colorConfig.Mode switch
        {
            VisualizationColorMode.Solid => new SolidColorBrush(_colorConfig.SolidColor.ToColor()),
            VisualizationColorMode.Gradient => _colorConfig.Gradient.Type == GradientType.Radial
                ? GradientBrushUtils.CreateAbsoluteRadial(
                    _colorConfig.Gradient,
                    center,
                    outerRadius,
                    DrawingArea,
                    _gradientOffsetX,
                    _gradientOffsetY,
                    stretch)
                : GradientBrushUtils.Create(
                    _colorConfig.Gradient,
                    _gradientOffsetX,
                    _gradientOffsetY,
                    stretch),
            VisualizationColorMode.Rainbow => _colorConfig.RainbowGradientType == GradientType.Radial
                ? GradientBrushUtils.CreateAbsoluteRadialRainbow(
                    center,
                    outerRadius,
                    DrawingArea,
                    _gradientOffsetX,
                    _gradientOffsetY,
                    stretch)
                : GradientBrushUtils.CreateRainbow(
                    _gradientOffsetX,
                    _gradientOffsetY,
                    stretch,
                    _colorConfig.RainbowAngle,
                    _colorConfig.RainbowGradientType),
            _ => new SolidColorBrush(fallbackColor)
        };
    }
}
