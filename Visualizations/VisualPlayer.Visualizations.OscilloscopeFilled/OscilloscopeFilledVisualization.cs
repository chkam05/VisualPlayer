using System;
using Avalonia.Media;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.OscilloscopeFilled.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.OscilloscopeFilled;

public class OscilloscopeFilledVisualization : VisualizationBase<OscilloscopeFilledVisualizationConfig>
{
    private readonly VisualizationColorConfig _colorConfig;
    private double _offsetX;
    private double _offsetY;
    private ReadOnlyMemory<float> _waveform;

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled;
    public override int MaximumFrameRate => 30;

    public OscilloscopeFilledVisualization(
        OscilloscopeFilledVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext context, IBrush fallbackBrush)
    {
        var fallback = fallbackBrush is ISolidColorBrush solid ? solid.Color : Colors.White;
        using (context.PushClip(DrawingArea))
        {
            if (_waveform.Span.IsEmpty || DrawingArea.Width <= 0 || DrawingArea.Height <= 0)
                return;

            var columnCount = Math.Max(
                2,
                Math.Min(256, Math.Min(_waveform.Span.Length, (int)Math.Ceiling(DrawingArea.Width / 6))));
            var columnWidth = DrawingArea.Width / columnCount + 0.5;

            for (var index = 0; index < columnCount; index++)
            {
                var normalizedX = (double)index / (columnCount - 1);
                var firstSample = OscilloscopeUtils.GetSample(_waveform.Span, normalizedX, Config.SmoothLine);
                var secondSample = -OscilloscopeUtils.GetSample(_waveform.Span, 1 - normalizedX, Config.SmoothLine);
                var firstY = Math.Clamp(0.5 - firstSample * 0.45, 0, 1);
                var secondY = Math.Clamp(0.5 - secondSample * 0.45, 0, 1);
                var top = Math.Min(firstY, secondY);
                var bottom = Math.Max(firstY, secondY);

                OscilloscopeUtils.DrawFilledColumn(
                    context,
                    DrawingArea,
                    _colorConfig,
                    fallback,
                    _offsetX,
                    _offsetY,
                    normalizedX,
                    DrawingArea.X + normalizedX * DrawingArea.Width,
                    DrawingArea.Y + top * DrawingArea.Height,
                    columnWidth,
                    (bottom - top) * DrawingArea.Height,
                    Config.Opacity);
            }

            OscilloscopeUtils.DrawDouble(
                context,
                DrawingArea,
                _waveform.Span,
                Config.LineThickness * RenderScaling,
                _colorConfig,
                fallback,
                _offsetX,
                _offsetY,
                Config.BeamEffect,
                Config.SmoothLine);
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        _waveform = frame.Waveform;
        CommonUtils.UpdateColorAnimation(
            _colorConfig,
            frame.Elapsed.TotalSeconds,
            DrawingArea,
            ref _offsetX,
            ref _offsetY);
    }

    public override void Reset()
    {
        _waveform = ReadOnlyMemory<float>.Empty;
        _offsetX = 0;
        _offsetY = 0;
    }
}
