using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Interfaces;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.Visualizations;

public abstract class VisualizationBase<TConfig> : IVisualization
    where TConfig : class, IVisualizationConfig
{
    private bool _isDisposed;

    protected Rect DrawingArea { get; private set; }
    protected double RenderScaling { get; private set; } = 1;
    protected TConfig Config { get; }

    public abstract bool HasActiveAnimation { get; }
    public virtual int MaximumFrameRate => int.MaxValue;

    #region CONSTRUCTOR

    protected VisualizationBase(TConfig config)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        Config.PropertyChanged += Config_PropertyChanged;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Config.PropertyChanged -= Config_PropertyChanged;
        DisposeCore();
        _isDisposed = true;
    }

    protected virtual void DisposeCore()
    {
    }

    #endregion CONSTRUCTOR

    #region PROPERTY CHANGES

    protected virtual void OnConfigChanged(string? propertyName)
    {
    }

    protected virtual void OnGraphicsChanged()
    {
    }

    private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnConfigChanged(e.PropertyName);
    }

    #endregion PROPERTY CHANGES

    public abstract void Draw(DrawingContext drawingContext, IBrush brush);
    public abstract void PreCalculate(in VisualizationFrame frame);
    public abstract void Reset();

    public void UpdateGraphics(Rect drawingArea, double renderScaling)
    {
        renderScaling = double.IsFinite(renderScaling) && renderScaling > 0
            ? renderScaling
            : 1;

        if (DrawingArea == drawingArea && RenderScaling == renderScaling)
            return;

        DrawingArea = drawingArea;
        RenderScaling = renderScaling;
        OnGraphicsChanged();
    }
}
