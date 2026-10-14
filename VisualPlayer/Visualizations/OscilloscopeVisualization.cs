using System;
using Avalonia.Media;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations;

public class OscilloscopeVisualization : VisualizationBase<OscilloscopeVisualizationConfig>
{
    private readonly VisualizationColorConfig _colorConfig;
    private double _offsetX;
    private double _offsetY;
    private ReadOnlyMemory<float> _waveform;

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled;
    public override int MaximumFrameRate => 30;

    public OscilloscopeVisualization(OscilloscopeVisualizationConfig config, VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext context, IBrush fallbackBrush)
    {
        var fallback = fallbackBrush is ISolidColorBrush solid ? solid.Color : Colors.White;
        using (context.PushClip(DrawingArea))
        {
            OscilloscopeUtils.DrawStandard(
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
        CommonUtils.UpdateColorAnimation(_colorConfig, frame.Elapsed.TotalSeconds, DrawingArea, ref _offsetX, ref _offsetY);
    }

    public override void Reset()
    {
        _waveform = ReadOnlyMemory<float>.Empty;
        _offsetX = 0;
        _offsetY = 0;
    }
}
