using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations;

public class SignalVisualization : VisualizationBase<SignalVisualizationConfig>
{
    private const int ARC_SEGMENT_COUNT = 64;
    private readonly VisualizationColorConfig _colorConfig;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private float[] _levels = [];
    private float[] _vibrationLevels = [];

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled
        || _levels.Any(level => level > 0.001f);

    public SignalVisualization(
        SignalVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length == 0 || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
            return;

        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var thickness = Math.Max(1, Config.LineThickness * RenderScaling);
        var spacing = DrawingArea.Height / (_levels.Length + 2d);
        var center = new Point(
            DrawingArea.Center.X,
            Config.Direction == VisualizationDirection.TopDown
                ? DrawingArea.Top + spacing
                : DrawingArea.Bottom - spacing);
        var brush = CreateBrush(fallbackColor);
        var dotRadius = Math.Max(thickness, Math.Min(spacing * 0.22, thickness * 2));

        using (drawingContext.PushClip(DrawingArea))
        {
            DrawDot(drawingContext, brush, center, dotRadius);

            for (var radiusIndex = 0; radiusIndex < _levels.Length; radiusIndex++)
            {
                var level = Math.Clamp(_levels[radiusIndex], 0, 1);
                if (level <= 0.001)
                    continue;

                var radius = (radiusIndex + 1) * spacing;
                var middleAngle = Config.Direction == VisualizationDirection.TopDown
                    ? Math.PI / 2
                    : Math.PI * 1.5;
                DrawArc(drawingContext, brush, thickness, CreateArcGeometry(
                    center, radius, spacing, level, radiusIndex, middleAngle));
            }
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != Config.StripesCount)
            _levels = new float[Config.StripesCount];

        var vibrationLevelCount = Config.StripesCount * Config.VibrationRange;
        if (_vibrationLevels.Length != vibrationLevelCount)
            _vibrationLevels = new float[vibrationLevelCount];

        var elapsed = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var spacing = DrawingArea.Height / (_levels.Length + 2d);
        var maximumRadius = Math.Max(1, _levels.Length * spacing);
        var normalizedDescent = Config.DescentRate / maximumRadius * elapsed;
        var spectrum = frame.Spectrum.Span;

        CommonUtils.UpdateColorAnimation(
            _colorConfig,
            elapsed,
            DrawingArea,
            ref _gradientOffsetX,
            ref _gradientOffsetY);

        for (var radiusIndex = 0; radiusIndex < _levels.Length; radiusIndex++)
        {
            // The stationary dot is the high-frequency end of the scale.
            var spectrumIndex = _levels.Length - 1 - radiusIndex;
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(spectrum, spectrumIndex, _levels.Length)
                : 0;

            _levels[radiusIndex] = target >= _levels[radiusIndex]
                ? (float)target
                : (float)Math.Max(0, _levels[radiusIndex] - normalizedDescent);

            for (var vibrationIndex = 0; vibrationIndex < Config.VibrationRange; vibrationIndex++)
            {
                var flatIndex = radiusIndex * Config.VibrationRange + vibrationIndex;
                var vibrationSpectrumIndex = Config.ReverseVibration
                    ? radiusIndex
                    : spectrumIndex;
                var vibrationTarget = frame.IsPlaying && Config.VibrationEnabled
                    ? GetSpectrumSubLevel(
                        spectrum,
                        vibrationSpectrumIndex,
                        _levels.Length,
                        vibrationIndex,
                        Config.VibrationRange)
                    : 0;
                _vibrationLevels[flatIndex] = vibrationTarget >= _vibrationLevels[flatIndex]
                    ? (float)vibrationTarget
                    : (float)Math.Max(0, _vibrationLevels[flatIndex] - normalizedDescent);
            }
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        Array.Clear(_vibrationLevels);
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    private StreamGeometry CreateArcGeometry(
        Point center,
        double radius,
        double spacing,
        double level,
        int radiusIndex,
        double middleAngle)
    {
        var halfSweep = level * Math.PI / 2;
        var startAngle = middleAngle - halfSweep;
        var pointCount = Math.Max(2, (int)Math.Ceiling(ARC_SEGMENT_COUNT * level));
        var points = new List<Point>(pointCount + 1);
        var vibrationAmplitude = Config.VibrationEnabled ? spacing / 2 : 0;
        var vibrationStart = radiusIndex * Config.VibrationRange;
        var vibrationMaximum = 0d;

        if (Config.VibrationEnabled)
        {
            for (var index = 0; index < Config.VibrationRange; index++)
                vibrationMaximum = Math.Max(
                    vibrationMaximum,
                    _vibrationLevels[vibrationStart + index]);
        }

        for (var pointIndex = 0; pointIndex <= pointCount; pointIndex++)
        {
            var position = pointIndex / (double)pointCount;
            var angle = startAngle + halfSweep * 2 * position;
            var vibration = vibrationAmplitude * GetVibrationOffset(
                vibrationStart,
                Config.VibrationRange,
                position,
                vibrationMaximum);
            var pointRadius = Math.Max(0, radius + vibration);
            points.Add(new Point(
                center.X + Math.Cos(angle) * pointRadius,
                center.Y + Math.Sin(angle) * pointRadius));
        }

        var geometry = new StreamGeometry();
        using var context = geometry.Open();
        context.BeginFigure(points[0], false);
        CommonUtils.AppendSmoothPath(context, points);
        context.EndFigure(false);
        return geometry;
    }

    private void DrawArc(
        DrawingContext drawingContext,
        IBrush brush,
        double thickness,
        StreamGeometry geometry)
    {
        if (Config.BeamEffect == BeamEffect.Beam)
        {
            using (drawingContext.PushOpacity(0.22))
                drawingContext.DrawGeometry(null, new Pen(brush, thickness * 4), geometry);
        }

        drawingContext.DrawGeometry(
            null,
            new Pen(brush, thickness)
            {
                LineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            },
            geometry);
    }

    private void DrawDot(DrawingContext context, IBrush brush, Point center, double radius)
    {
        if (Config.BeamEffect == BeamEffect.Beam)
        {
            using (context.PushOpacity(0.22))
                context.DrawEllipse(brush, null, center, radius * 2.5, radius * 2.5);
        }

        context.DrawEllipse(brush, null, center, radius, radius);
    }

    private double GetVibrationOffset(int start, int count, double position, double maximum)
    {
        if (!Config.VibrationEnabled || count < 2 || maximum <= 0.001)
            return 0;

        var samplePosition = position * (count - 1);
        var left = Math.Min((int)samplePosition, count - 1);
        var right = Math.Min(left + 1, count - 1);
        var fraction = samplePosition - left;
        var sample = _vibrationLevels[start + left]
            + (_vibrationLevels[start + right] - _vibrationLevels[start + left]) * fraction;

        // Normalize within the FFT band assigned to this arc. Raw neighbouring
        // bins are usually very close to one another, so subtracting their mean
        // makes the waveform visually disappear. The strongest local peak now
        // reaches the outer edge of the lane and silence remains flat.
        return Math.Clamp(sample / maximum * 2 - 1, -1, 1);
    }

    private static double GetSpectrumSubLevel(
        ReadOnlySpan<float> spectrum,
        int bandIndex,
        int bandCount,
        int subIndex,
        int subCount)
    {
        if (spectrum.IsEmpty)
            return 0;

        var bandStart = bandIndex * spectrum.Length / (double)bandCount;
        var bandEnd = (bandIndex + 1) * spectrum.Length / (double)bandCount;
        var start = Math.Clamp((int)Math.Floor(
            bandStart + (bandEnd - bandStart) * subIndex / subCount), 0, spectrum.Length - 1);
        var end = Math.Clamp((int)Math.Ceiling(
            bandStart + (bandEnd - bandStart) * (subIndex + 1) / subCount), start + 1, spectrum.Length);
        var maximum = 0f;

        for (var index = start; index < end; index++)
            maximum = Math.Max(maximum, spectrum[index]);

        return Math.Clamp(maximum, 0, 1);
    }

    private IBrush CreateBrush(Color fallbackColor)
    {
        var stretch = Math.Max(0.1, _colorConfig.Stretch / 100d);

        return _colorConfig.Mode switch
        {
            VisualizationColorMode.Solid => new SolidColorBrush(_colorConfig.SolidColor.ToColor()),
            VisualizationColorMode.Gradient => GradientBrushUtils.CreateAbsolute(
                _colorConfig.Gradient,
                DrawingArea,
                _gradientOffsetX,
                _gradientOffsetY,
                stretch),
            VisualizationColorMode.Rainbow => GradientBrushUtils.CreateAbsoluteRainbow(
                DrawingArea,
                _gradientOffsetX,
                _gradientOffsetY,
                stretch,
                _colorConfig.RainbowAngle,
                _colorConfig.RainbowGradientType),
            _ => new SolidColorBrush(fallbackColor)
        };
    }
}
