using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.WaveformMirrored.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.WaveformMirrored;

public class WaveformMirroredVisualization : VisualizationBase<WaveformMirroredVisualizationConfig>
{
    private readonly VisualizationColorConfig _colorConfig;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private float[] _levels = [];

    public override bool HasActiveAnimation
    {
        get => _colorConfig.IsAnimationEnabled;
    }

    public WaveformMirroredVisualization(
        WaveformMirroredVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length < 2 || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
            return;

        var upperPoints = new Point[_levels.Length];
        var lowerPoints = new Point[_levels.Length];
        var halfHeight = DrawingArea.Height / 2;
        var centerY = DrawingArea.Center.Y;

        for (var index = 0; index < _levels.Length; index++)
        {
            var position = index / (double)(_levels.Length - 1);
            var lowerIndex = Config.Shift ? _levels.Length - 1 - index : index;
            var upperLevel = CommonUtils.TransformWaveformLevel(
                _levels[index],
                position,
                Config.Transformation,
                Config.TransformationMinimum / 100d) * halfHeight;
            var lowerLevel = CommonUtils.TransformWaveformLevel(
                _levels[lowerIndex],
                position,
                Config.Transformation,
                Config.TransformationMinimum / 100d) * halfHeight;
            var x = DrawingArea.Left + position * DrawingArea.Width;

            if (Config.Direction == MirroredVisualizationDirection.Inside)
            {
                upperPoints[index] = new Point(x, DrawingArea.Top + upperLevel);
                lowerPoints[index] = new Point(x, DrawingArea.Bottom - lowerLevel);
            }
            else
            {
                upperPoints[index] = new Point(x, centerY - upperLevel);
                lowerPoints[index] = new Point(x, centerY + lowerLevel);
            }
        }

        var upperGeometry = CommonUtils.CreateSmoothGeometry(upperPoints);
        var lowerGeometry = CommonUtils.CreateSmoothGeometry(lowerPoints);
        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var brush = CreateBrush(fallbackColor);

        using (drawingContext.PushClip(DrawingArea))
        {
            if (Config.FillEnabled)
            {
                using (drawingContext.PushOpacity(Config.Opacity))
                {
                    if (Config.Direction == MirroredVisualizationDirection.Inside)
                    {
                        var upperFillGeometry = CreateEdgeFillGeometry(
                            upperPoints,
                            DrawingArea.Top);
                        var lowerFillGeometry = CreateEdgeFillGeometry(
                            lowerPoints,
                            DrawingArea.Bottom);

                        drawingContext.DrawGeometry(brush, null, upperFillGeometry);
                        drawingContext.DrawGeometry(brush, null, lowerFillGeometry);
                    }
                    else
                    {
                        var fillGeometry = CreateFillGeometry(upperPoints, lowerPoints);
                        drawingContext.DrawGeometry(brush, null, fillGeometry);
                    }
                }
            }

            if (Config.BeamEffect == BeamEffect.Beam)
            {
                using (drawingContext.PushOpacity(0.22))
                {
                    var beamPen = new Pen(brush, Config.LineThickness * RenderScaling * 4);
                    drawingContext.DrawGeometry(null, beamPen, upperGeometry);
                    drawingContext.DrawGeometry(null, beamPen, lowerGeometry);
                }
            }

            var pen = new Pen(brush, Config.LineThickness * RenderScaling);
            drawingContext.DrawGeometry(null, pen, upperGeometry);
            drawingContext.DrawGeometry(null, pen, lowerGeometry);
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        var spectrum = frame.Spectrum.Span;

        if (_levels.Length != Config.PeaksCount)
            _levels = new float[Config.PeaksCount];

        var elapsedSeconds = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var normalizedDescent = Config.DescentRate
            / Math.Max(1, DrawingArea.Height)
            * elapsedSeconds;

        CommonUtils.UpdateColorAnimation(
            _colorConfig,
            elapsedSeconds,
            DrawingArea,
            ref _gradientOffsetX,
            ref _gradientOffsetY);

        for (var index = 0; index < _levels.Length; index++)
        {
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(spectrum, index, _levels.Length)
                : 0;
            _levels[index] = target >= _levels[index]
                ? (float)target
                : (float)Math.Max(0, _levels[index] - normalizedDescent);
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    private IBrush CreateBrush(Color fallbackColor)
    {
        var stretch = Math.Max(0.1, _colorConfig.Stretch / 100d);

        return _colorConfig.Mode switch
        {
            VisualizationColorMode.Solid => new SolidColorBrush(_colorConfig.SolidColor.ToColor()),
            VisualizationColorMode.Gradient => GradientBrushUtils.Create(
                _colorConfig.Gradient,
                _gradientOffsetX,
                _gradientOffsetY,
                stretch),
            VisualizationColorMode.Rainbow => GradientBrushUtils.CreateRainbow(
                _gradientOffsetX,
                _gradientOffsetY,
                stretch,
                _colorConfig.RainbowAngle,
                _colorConfig.RainbowGradientType),
            _ => new SolidColorBrush(fallbackColor)
        };
    }

    private static StreamGeometry CreateFillGeometry(Point[] upperPoints, Point[] lowerPoints)
    {
        var reversedLowerPoints = lowerPoints.Reverse().ToArray();
        var geometry = new StreamGeometry();

        using var context = geometry.Open();
        context.BeginFigure(upperPoints[0], true);
        CommonUtils.AppendSmoothPath(context, upperPoints);
        context.LineTo(reversedLowerPoints[0]);
        CommonUtils.AppendSmoothPath(context, reversedLowerPoints);
        context.EndFigure(true);
        return geometry;
    }

    private static StreamGeometry CreateEdgeFillGeometry(Point[] points, double edgeY)
    {
        var geometry = new StreamGeometry();

        using var context = geometry.Open();
        context.BeginFigure(points[0], true);
        CommonUtils.AppendSmoothPath(context, points);
        context.LineTo(new Point(points[^1].X, edgeY));
        context.LineTo(new Point(points[0].X, edgeY));
        context.EndFigure(true);
        return geometry;
    }
}
