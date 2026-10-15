using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations;

public abstract class BouncingVisualizationBase<TConfig> : VisualizationBase<TConfig>
    where TConfig : BouncingVisualizationConfig
{
    private double[] _levels = [];
    private double[] _upperLevels = [];
    private double _columnWidth;
    protected readonly VisualizationColorConfig ColorConfig;
    protected double GradientOffsetX;
    protected double GradientOffsetY;
    protected abstract int ColumnCount { get; }

    public override bool HasActiveAnimation => _upperLevels.Any(level => level > 0)
        || ColorConfig.IsAnimationEnabled;

    protected BouncingVisualizationBase(TConfig config, VisualizationColorConfig colorConfig) : base(config)
    {
        ColorConfig = colorConfig ?? throw new ArgumentNullException(nameof(colorConfig));
        RebuildColumns();
    }

    public override void Draw(DrawingContext drawingContext, IBrush brush)
    {
        if (DrawingArea.Width <= 0 || DrawingArea.Height <= 0 || _columnWidth <= 0)
            return;

        brush = PrepareBrush(brush);
        var spacing = Config.HorizontalBarSpace * RenderScaling;
        using (drawingContext.PushClip(DrawingArea))
        {
            for (var index = 0; index < _levels.Length; index++)
            {
                if (_upperLevels[index] <= _levels[index])
                    continue;

                var x = DrawingArea.X + index * (_columnWidth + spacing);
                DrawBand(drawingContext, brush, x, _columnWidth, _levels[index], _upperLevels[index]);
            }
        }
    }

    protected virtual IBrush PrepareBrush(IBrush brush) => brush;

    protected abstract void DrawBand(DrawingContext context, IBrush brush,
        double x, double width, double innerLevel, double outerLevel);

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != ColumnCount)
            RebuildColumns();

        var elapsed = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var maximumHeight = Math.Max(0, DrawingArea.Height / 2);
        CommonUtils.UpdateColorAnimation(ColorConfig, elapsed, DrawingArea, ref GradientOffsetX, ref GradientOffsetY);

        for (var index = 0; index < _levels.Length; index++)
        {
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(frame.Spectrum.Span, index, _levels.Length) * maximumHeight
                : 0;
            _levels[index] = Math.Clamp(Math.Max(target, _levels[index] - Config.DescentRate * elapsed), 0, maximumHeight);
            // The peak envelope bounds the band and can never cross its inner edge.
            _upperLevels[index] = Math.Clamp(Math.Max(_levels[index],
                _upperLevels[index] - Config.UpperEdgeDescentRate * elapsed), 0, maximumHeight);
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        Array.Clear(_upperLevels);
        GradientOffsetX = GradientOffsetY = 0;
    }

    protected override void OnConfigChanged(string? propertyName)
    {
        if (_levels.Length != ColumnCount)
            RebuildColumns();
        else
            OnGraphicsChanged();
    }

    protected override void OnGraphicsChanged()
    {
        _columnWidth = CommonUtils.CalculateColumnWidth(DrawingArea.Width,
            _levels.Length, Config.HorizontalBarSpace * RenderScaling);
    }

    private void RebuildColumns()
    {
        _levels = new double[ColumnCount];
        _upperLevels = new double[ColumnCount];
        OnGraphicsChanged();
    }
}
