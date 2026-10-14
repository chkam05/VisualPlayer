using System;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.Interfaces;

public interface IVisualization : IDisposable
{
    bool HasActiveAnimation { get; }
    int MaximumFrameRate { get; }

    void Draw(DrawingContext drawingContext, IBrush brush);
    void PreCalculate(in VisualizationFrame frame);
    void Reset();
    void UpdateGraphics(Rect drawingArea, double renderScaling);
}
