using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations;

public class PeaksBouncingVisualization : BouncingVisualizationBase<PeaksBouncingVisualizationConfig>
{
    private readonly Dictionary<Color, SolidColorBrush> _frameBrushes = [];
    protected override int ColumnCount => Config.PeaksCount;

    public PeaksBouncingVisualization(PeaksBouncingVisualizationConfig config, VisualizationColorConfig colorConfig)
        : base(config, colorConfig) { }

    protected override IBrush PrepareBrush(IBrush brush)
    {
        _frameBrushes.Clear();
        return brush;
    }

    protected override void DrawBand(DrawingContext context, IBrush brush,
        double x, double width, double innerLevel, double outerLevel)
    {
        var barHeight = Config.BarHeight * RenderScaling;
        var segmentSize = barHeight + Config.VerticalBarSpace * RenderScaling;
        var firstSegment = (int)Math.Floor(innerLevel / segmentSize);
        var segmentCount = (int)Math.Ceiling(outerLevel / segmentSize);
        for (var segment = firstSegment; segment < segmentCount; segment++)
        {
            var inner = segment * segmentSize;
            var outer = inner + barHeight;
            // Keep the whole block until the band boundary has passed its far edge.
            if (outer <= innerLevel || inner >= outerLevel)
                continue;

            DrawSegment(context, brush, new Rect(x, DrawingArea.Center.Y - outer, width, barHeight));
            DrawSegment(context, brush, new Rect(x, DrawingArea.Center.Y + inner, width, barHeight));
        }
    }

    private void DrawSegment(DrawingContext context, IBrush fallback, Rect rectangle)
    {
        var color = VisualizationColorSampler.GetColor(ColorConfig,
            (rectangle.Center.X - DrawingArea.X) / Math.Max(1, DrawingArea.Width),
            (rectangle.Center.Y - DrawingArea.Y) / Math.Max(1, DrawingArea.Height),
            GradientOffsetX, GradientOffsetY,
            fallback is ISolidColorBrush solid ? solid.Color : Colors.White);
        if (!_frameBrushes.TryGetValue(color, out var brush))
        {
            brush = new SolidColorBrush(color);
            _frameBrushes.Add(color, brush);
        }
        context.DrawRectangle(brush, null, rectangle);
    }
}
