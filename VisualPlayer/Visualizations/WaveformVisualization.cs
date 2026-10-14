using System;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations;

public class WaveformVisualization : VisualizationBase<WaveformVisualizationConfig>
{
    private readonly VisualizationColorConfig _colorConfig;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private float[] _levels = [];

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled;

    public WaveformVisualization(
        WaveformVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length < 2 || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
            return;

        var originY = Config.Direction == VisualizationDirection.BottomUp
            ? DrawingArea.Bottom
            : DrawingArea.Top;
        var direction = Config.Direction == VisualizationDirection.BottomUp ? -1 : 1;
        var startPoint = new Point(DrawingArea.Left, originY);
        var endPoint = new Point(DrawingArea.Right, originY);
        var points = new Point[_levels.Length];

        for (var index = 0; index < points.Length; index++)
        {
            var position = index / (double)(points.Length - 1);
            var level = CommonUtils.TransformWaveformLevel(
                _levels[index],
                position,
                Config.Transformation,
                Config.TransformationMinimum / 100d);

            points[index] = new Point(
                DrawingArea.Left + position * DrawingArea.Width,
                originY + direction * level * DrawingArea.Height);
        }

        var waveGeometry = CommonUtils.CreateSmoothGeometry(points);
        StreamGeometry? fillGeometry = null;

        if (Config.FillEnabled)
        {
            fillGeometry = CreateWaveGeometry(points, endPoint, startPoint);
        }

        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var brush = CreateBrush(fallbackColor);

        using (drawingContext.PushClip(DrawingArea))
        {
            if (fillGeometry is not null)
            {
                using (drawingContext.PushOpacity(Config.Opacity))
                    drawingContext.DrawGeometry(brush, null, fillGeometry);
            }

            if (Config.BeamEffect == BeamEffect.Beam)
            {
                using (drawingContext.PushOpacity(0.22))
                {
                    drawingContext.DrawGeometry(
                        null,
                        new Pen(brush, Config.LineThickness * RenderScaling * 4),
                        waveGeometry);
                }
            }

            drawingContext.DrawGeometry(
                null,
                new Pen(brush, Config.LineThickness * RenderScaling),
                waveGeometry);
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

    private static StreamGeometry CreateWaveGeometry(Point[] points, Point endPoint, Point startPoint)
    {
        var geometry = new StreamGeometry();

        using var geometryContext = geometry.Open();
        geometryContext.BeginFigure(points[0], true);
        CommonUtils.AppendSmoothPath(geometryContext, points);
        geometryContext.LineTo(endPoint);
        geometryContext.LineTo(startPoint);
        geometryContext.EndFigure(true);
        return geometry;
    }

}
