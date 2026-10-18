using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.PeaksFloatingMirrored.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.PeaksFloatingMirrored;

public class PeaksFloatingMirroredVisualization : VisualizationBase<PeaksFloatingMirroredVisualizationConfig>
{
    #region CONST

    private const double MINIMUM_LEVEL = 0.25;

    #endregion CONST

    private readonly VisualizationColorConfig _colorConfig;
    private readonly Dictionary<Color, SolidColorBrush> _frameBrushes = [];
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private double[] _levels = [];
    private double _peakWidth;

    public override bool HasActiveAnimation => _levels.Any(level => level > MINIMUM_LEVEL) || _colorConfig.IsAnimationEnabled;

    #region CONSTRUCTOR

    public PeaksFloatingMirroredVisualization(PeaksFloatingMirroredVisualizationConfig config, VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig ?? throw new ArgumentNullException(nameof(colorConfig));
        RebuildPeaks();
    }

    #endregion CONSTRUCTOR

    #region DRAW

    public override void Draw(DrawingContext context, IBrush fallbackBrush)
    {
        var space = Config.HorizontalBarSpace * RenderScaling;
        var height = Config.BarHeight * RenderScaling;
        var centerHalfSpace = Config.VerticalBarSpace * RenderScaling / 2;
        var fallbackColor = fallbackBrush is ISolidColorBrush solid ? solid.Color : Colors.White;
        _frameBrushes.Clear();

        using (context.PushClip(DrawingArea))
        {
            for (var index = 0; index < _levels.Length; index++)
            {
                if (_levels[index] <= MINIMUM_LEVEL) continue;
                var x = DrawingArea.X + index * (_peakWidth + space);
                var upperY = Config.Direction == MirroredVisualizationDirection.Outside
                    ? DrawingArea.Center.Y - centerHalfSpace - _levels[index] - height
                    : DrawingArea.Top + _levels[index];
                var lowerY = Config.Direction == MirroredVisualizationDirection.Outside
                    ? DrawingArea.Center.Y + centerHalfSpace + _levels[index]
                    : DrawingArea.Bottom - _levels[index] - height;
                DrawPeak(context, new Rect(x, upperY, _peakWidth, height), fallbackColor);
                DrawPeak(context, new Rect(x, lowerY, _peakWidth, height), fallbackColor);
            }
        }
    }

    private void DrawPeak(DrawingContext context, Rect rectangle, Color fallbackColor)
    {
        var color = VisualizationColorSampler.GetColor(_colorConfig,
            (rectangle.Center.X - DrawingArea.X) / Math.Max(1, DrawingArea.Width),
            (rectangle.Center.Y - DrawingArea.Y) / Math.Max(1, DrawingArea.Height),
            _gradientOffsetX, _gradientOffsetY, fallbackColor);
        if (!_frameBrushes.TryGetValue(color, out var brush)) _frameBrushes[color] = brush = new SolidColorBrush(color);
        context.DrawRectangle(brush, null, rectangle);
    }

    #endregion DRAW

    #region PROPERTY CHANGES

    protected override void OnConfigChanged(string? propertyName)
    {
        if (propertyName == nameof(PeaksFloatingMirroredVisualizationConfig.PeaksCount)) RebuildPeaks();
        if (propertyName == nameof(PeaksFloatingMirroredVisualizationConfig.HorizontalBarSpace)) OnGraphicsChanged();
    }

    protected override void OnGraphicsChanged()
    {
        var space = Config.HorizontalBarSpace * RenderScaling;
        _peakWidth = CommonUtils.CalculateColumnWidth(DrawingArea.Width, _levels.Length, space);
    }

    #endregion PROPERTY CHANGES

    public override void PreCalculate(in VisualizationFrame frame)
    {
        var elapsed = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        CommonUtils.UpdateColorAnimation(_colorConfig, elapsed, DrawingArea, ref _gradientOffsetX, ref _gradientOffsetY);

        for (var index = 0; index < _levels.Length; index++)
        {
            var barHeight = Config.BarHeight * RenderScaling;
            var centerHalfSpace = Config.VerticalBarSpace * RenderScaling / 2;
            var maximumLevel = Math.Max(0, DrawingArea.Height / 2 - centerHalfSpace - barHeight);
            var target = frame.IsPlaying ? CommonUtils.GetSpectrumLevel(frame.Spectrum.Span, index, _levels.Length) * maximumLevel : 0;
            _levels[index] = target >= _levels[index] ? target : Math.Max(0, _levels[index] - Config.DescentRate * elapsed);
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    

    private void RebuildPeaks()
    {
        _levels = new double[Config.PeaksCount];
        OnGraphicsChanged();
    }
}
