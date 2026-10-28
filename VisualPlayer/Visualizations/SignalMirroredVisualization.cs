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

public class SignalMirroredVisualization : VisualizationBase<SignalMirroredVisualizationConfig>
{
    private const int ARC_SEGMENT_COUNT = 64;

    private readonly VisualizationColorConfig _colorConfig;
    private double _animatedRotation;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private float[] _levels = [];
    private float[] _vibrationLevels = [];

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled
        || Config.RotationAnimationEnabled
        || _levels.Any(level => level > 0.001f);

    public SignalMirroredVisualization(
        SignalMirroredVisualizationConfig config,
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
        var rotation = GetRotationRadians();
        var spacing = GetSpacing(rotation);
        var center = DrawingArea.Center;
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
                DrawArc(drawingContext, brush, thickness, CreateArcGeometry(
                    center, radius, spacing, level, radiusIndex, rotation, false));
                DrawArc(drawingContext, brush, thickness, CreateArcGeometry(
                    center, radius, spacing, level, radiusIndex, rotation + Math.PI, true));
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
        _animatedRotation = CircleVisualizationUtils.GetAnimatedRotation(
            _animatedRotation,
            Config.RotationAnimationEnabled,
            Config.RotationDirection,
            Config.RotationSpeed,
            elapsed);

        var maximumRadius = Math.Max(
            1,
            _levels.Length * GetSpacing(GetRotationRadians()));
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
        _animatedRotation = 0;
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    private StreamGeometry CreateArcGeometry(
        Point center,
        double radius,
        double spacing,
        double level,
        int radiusIndex,
        double middleAngle,
        bool reverseVibration)
    {
        var maximumBackwardSweep = GetMaximumSweep(radius, middleAngle, -1);
        var maximumForwardSweep = GetMaximumSweep(radius, middleAngle, 1);
        var backwardSweep = maximumBackwardSweep * level;
        var forwardSweep = maximumForwardSweep * level;
        var startAngle = middleAngle - backwardSweep;
        var totalSweep = backwardSweep + forwardSweep;
        var pointCount = Math.Max(
            2,
            (int)Math.Ceiling(ARC_SEGMENT_COUNT * totalSweep / Math.PI));
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
            var angle = startAngle + totalSweep * position;
            var vibration = vibrationAmplitude * GetVibrationOffset(
                vibrationStart,
                Config.VibrationRange,
                reverseVibration ? 1 - position : position,
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

    private double GetMaximumSweep(double radius, double middleAngle, int direction)
    {
        const double maximumSweep = Math.PI / 2;
        var insideSweep = 0d;
        var outsideSweep = double.NaN;

        // A rotated circle can leave the rectangle and enter it again before a
        // quarter turn is completed. Locate the first exit before refining the
        // intersection, rather than checking only the final point.
        for (var sample = 1; sample <= ARC_SEGMENT_COUNT; sample++)
        {
            var candidate = maximumSweep * sample / ARC_SEGMENT_COUNT;
            if (IsPointInsideDrawingArea(radius, middleAngle + direction * candidate))
            {
                insideSweep = candidate;
                continue;
            }

            outsideSweep = candidate;
            break;
        }

        if (double.IsNaN(outsideSweep))
            return maximumSweep;

        // The point at the middle angle is guaranteed to be inside because the
        // radius spacing is calculated from the distance to the rectangle edge.
        // Find the first intersection of this circle with the drawing area.
        for (var iteration = 0; iteration < 24; iteration++)
        {
            var candidate = (insideSweep + outsideSweep) / 2;
            if (IsPointInsideDrawingArea(radius, middleAngle + direction * candidate))
                insideSweep = candidate;
            else
                outsideSweep = candidate;
        }

        return insideSweep;
    }

    private bool IsPointInsideDrawingArea(double radius, double angle)
    {
        const double tolerance = 0.0001;
        var point = new Point(
            DrawingArea.Center.X + Math.Cos(angle) * radius,
            DrawingArea.Center.Y + Math.Sin(angle) * radius);

        return point.X >= DrawingArea.Left - tolerance
            && point.X <= DrawingArea.Right + tolerance
            && point.Y >= DrawingArea.Top - tolerance
            && point.Y <= DrawingArea.Bottom + tolerance;
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

    private double GetRotationRadians()
    {
        return (Config.Rotation + _animatedRotation) * Math.PI / 180;
    }

    private double GetSpacing(double rotation)
    {
        var halfWidth = DrawingArea.Width / 2;
        var halfHeight = DrawingArea.Height / 2;
        var horizontalComponent = Math.Abs(Math.Cos(rotation));
        var verticalComponent = Math.Abs(Math.Sin(rotation));
        var horizontalDistance = horizontalComponent <= 0.000001
            ? double.PositiveInfinity
            : halfWidth / horizontalComponent;
        var verticalDistance = verticalComponent <= 0.000001
            ? double.PositiveInfinity
            : halfHeight / verticalComponent;
        var distanceToEdge = Math.Min(horizontalDistance, verticalDistance);

        return distanceToEdge / (_levels.Length + 1d);
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
