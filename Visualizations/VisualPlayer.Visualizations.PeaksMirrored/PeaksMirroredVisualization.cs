using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.PeaksMirrored.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.PeaksMirrored;

public class PeaksMirroredVisualization : VisualizationBase<PeaksMirroredVisualizationConfig>
{
    #region CONST

    private const double MINIMUM_LEVEL = 0.25;

    #endregion CONST

    private readonly VisualizationColorConfig _colorConfig;
    private readonly Dictionary<Color, SolidColorBrush> _frameBrushes = [];

    private double[] _floatingLevels = [];
    private bool[] _floatingIsFalling = [];
    private double[] _levels = [];
    private double _peakWidth;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    
    private bool IsFloatingOnly
    {
        get => false;
    }

    private bool IsMirrored
    {
        get => true;
    }

    private bool UsesCenterOrigin
    {
        get => IsMirrored;
    }

    public override bool HasActiveAnimation
    {
        get => _levels.Any(level => level > MINIMUM_LEVEL)
            || _floatingLevels.Any(level => level > MINIMUM_LEVEL)
            || _colorConfig.IsAnimationEnabled;
    }

    #region CONSTRUCTOR

    public PeaksMirroredVisualization(
        PeaksMirroredVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig ?? throw new ArgumentNullException(nameof(colorConfig));
        RebuildPeaks();
    }

    #endregion CONSTRUCTOR

    #region DRAW

    public override void Draw(DrawingContext drawingContext, IBrush brush)
    {
        if (DrawingArea.Width <= 0 || DrawingArea.Height <= 0 || _peakWidth <= 0)
            return;

        var horizontalSpace = Config.HorizontalBarSpace * RenderScaling;
        var barHeight = Config.BarHeight * RenderScaling;
        var barSpace = Config.VerticalBarSpace * RenderScaling;
        var segmentSize = Math.Max(1, barHeight + barSpace);
        var fallbackColor = brush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;

        _frameBrushes.Clear();

        using (drawingContext.PushClip(DrawingArea))
        {
            for (var peakIndex = 0; peakIndex < _levels.Length; peakIndex++)
            {
                var x = DrawingArea.X + peakIndex * (_peakWidth + horizontalSpace);

                if (!IsFloatingOnly)
                    DrawPeak(drawingContext, x, peakIndex, barHeight, segmentSize, fallbackColor);

                if (Config.ShowFloatingBar || IsFloatingOnly)
                    DrawPeaksFloating(drawingContext, x, peakIndex, barHeight, fallbackColor);
            }
        }
    }

    private void DrawPeaksFloating(
        DrawingContext drawingContext,
        double x,
        int peakIndex,
        double barHeight,
        Color fallbackColor)
    {
        var level = _floatingLevels[peakIndex];
        var floatingSpace = Config.VerticalBarSpace * RenderScaling;
        var centerHalfSpace = floatingSpace / 2;

        if (level <= MINIMUM_LEVEL)
            return;

        if (IsMirrored)
        {
            if (Config.Direction == MirroredVisualizationDirection.Outside)
            {
                var centerY = DrawingArea.Center.Y;
                DrawSegment(drawingContext, new Rect(x, centerY - centerHalfSpace - level - floatingSpace - barHeight, _peakWidth, barHeight), fallbackColor);
                DrawSegment(drawingContext, new Rect(x, centerY + centerHalfSpace + level + floatingSpace, _peakWidth, barHeight), fallbackColor);
            }
            else
            {
                DrawSegment(drawingContext, new Rect(x, DrawingArea.Top + level + floatingSpace, _peakWidth, barHeight), fallbackColor);
                DrawSegment(drawingContext, new Rect(x, DrawingArea.Bottom - level - floatingSpace - barHeight, _peakWidth, barHeight), fallbackColor);
            }

            return;
        }

        var y = Config.Direction == MirroredVisualizationDirection.Inside
            ? DrawingArea.Bottom - level - floatingSpace - barHeight
            : DrawingArea.Top + level + floatingSpace;

        DrawSegment(drawingContext, new Rect(x, y, _peakWidth, barHeight), fallbackColor);
    }

    private void DrawPeak(
        DrawingContext drawingContext,
        double x,
        int peakIndex,
        double barHeight,
        double segmentSize,
        Color fallbackColor)
    {
        var segmentCount = (int)Math.Floor(_levels[peakIndex] / segmentSize);
        var centerY = DrawingArea.Center.Y;
        var centerHalfSpace = Math.Max(0, segmentSize - barHeight) / 2;

        for (var segment = 0; segment < segmentCount; segment++)
        {
            if (IsMirrored)
            {
                var isInsideOut = Config.Direction == MirroredVisualizationDirection.Outside;
                var upperY = isInsideOut
                    ? centerY - centerHalfSpace - barHeight - segment * segmentSize
                    : DrawingArea.Top + segment * segmentSize;
                var lowerY = isInsideOut
                    ? centerY + centerHalfSpace + segment * segmentSize
                    : DrawingArea.Bottom - barHeight - segment * segmentSize;
                DrawSegment(drawingContext, new Rect(x, upperY, _peakWidth, barHeight), fallbackColor);
                DrawSegment(drawingContext, new Rect(x, lowerY, _peakWidth, barHeight), fallbackColor);
                continue;
            }

            var distance = barHeight + segment * segmentSize;
            var y = Config.Direction == MirroredVisualizationDirection.Inside
                ? DrawingArea.Bottom - distance
                : DrawingArea.Top + distance - barHeight;
            DrawSegment(drawingContext, new Rect(x, y, _peakWidth, barHeight), fallbackColor);
        }
    }

    private void DrawSegment(DrawingContext drawingContext, Rect rectangle, Color fallbackColor)
    {
        var normalizedX = (rectangle.Center.X - DrawingArea.X) / Math.Max(1, DrawingArea.Width);
        var normalizedY = (rectangle.Center.Y - DrawingArea.Y) / Math.Max(1, DrawingArea.Height);
        var color = VisualizationColorSampler.GetColor(
            _colorConfig,
            normalizedX,
            normalizedY,
            _gradientOffsetX,
            _gradientOffsetY,
            fallbackColor);

        if (!_frameBrushes.TryGetValue(color, out var brush))
        {
            brush = new SolidColorBrush(color);
            _frameBrushes.Add(color, brush);
        }

        drawingContext.DrawRectangle(brush, null, rectangle);
    }

    #endregion DRAW

    #region PROPERTY CHANGES

    protected override void OnConfigChanged(string? propertyName)
    {
        if (propertyName == nameof(PeaksMirroredVisualizationConfig.PeaksCount))
            RebuildPeaks();

        if (propertyName == nameof(PeaksMirroredVisualizationConfig.BarHeight)
            || propertyName == nameof(PeaksMirroredVisualizationConfig.HorizontalBarSpace)
            || propertyName == nameof(PeaksMirroredVisualizationConfig.VerticalBarSpace))
        {
            OnGraphicsChanged();
        }
    }

    protected override void OnGraphicsChanged()
    {
        var horizontalSpace = Config.HorizontalBarSpace * RenderScaling;
        _peakWidth = CommonUtils.CalculateColumnWidth(DrawingArea.Width, _levels.Length, horizontalSpace);
    }

    #endregion PROPERTY CHANGES

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != Config.PeaksCount)
            RebuildPeaks();

        var elapsedSeconds = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var spectrum = frame.Spectrum.Span;
        var maximumHeight = GetMaximumLevel();

        CommonUtils.UpdateColorAnimation(_colorConfig, elapsedSeconds, DrawingArea, ref _gradientOffsetX, ref _gradientOffsetY);

        for (var peakIndex = 0; peakIndex < _levels.Length; peakIndex++)
        {
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(spectrum, peakIndex, _levels.Length) * maximumHeight
                : 0;

            _levels[peakIndex] = target >= _levels[peakIndex]
                ? target
                : Math.Max(0, _levels[peakIndex] - Config.DescentRate * elapsedSeconds);

            UpdateFloatingLevel(peakIndex, target, elapsedSeconds, maximumHeight);
        }
    }

    public override void Reset()
    {
        Array.Clear(_levels);
        Array.Clear(_floatingLevels);
        Array.Clear(_floatingIsFalling);
        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }


    private double GetMaximumLevel()
    {
        var barHeight = Config.BarHeight * RenderScaling;
        var verticalSpace = Config.VerticalBarSpace * RenderScaling;
        var availableHeight = IsMirrored
            ? DrawingArea.Height / 2 - verticalSpace / 2
            : DrawingArea.Height;

        if (Config.ShowFloatingBar || IsFloatingOnly)
            availableHeight -= barHeight + verticalSpace;

        return Math.Max(0, availableHeight);
    }


    private void RebuildPeaks()
    {
        _levels = new double[Config.PeaksCount];
        _floatingLevels = new double[Config.PeaksCount];
        _floatingIsFalling = new bool[Config.PeaksCount];
        OnGraphicsChanged();
    }

    private void UpdateFloatingLevel(int index, double target, double elapsedSeconds, double maximumHeight)
    {
        var usesMirroredGravity = IsMirrored
            && !IsFloatingOnly
            && Config.Direction == MirroredVisualizationDirection.Outside;
        var usesRegularGravity = !IsMirrored && !IsFloatingOnly;
        var usesGravity = Config.UseFloatingBarGravity
            && (usesRegularGravity || usesMirroredGravity);

        if (usesGravity)
        {
            if (_floatingIsFalling[index] && _levels[index] >= _floatingLevels[index])
            {
                _floatingLevels[index] = _levels[index];
                _floatingIsFalling[index] = false;
            }

            if (!_floatingIsFalling[index])
            {
                if (target >= _floatingLevels[index])
                {
                    _floatingLevels[index] = target;
                    return;
                }

                _floatingIsFalling[index] = true;
            }

            var movement = Config.FloatingBarDescentRate * elapsedSeconds;
            var movesAwayFromOrigin = usesMirroredGravity
                || usesRegularGravity && Config.Direction == MirroredVisualizationDirection.Outside;
            _floatingLevels[index] += movesAwayFromOrigin
                ? movement
                : -movement;

            var floatingBarHeight = Config.BarHeight * RenderScaling;
            var leftDrawingArea = movesAwayFromOrigin
                ? _floatingLevels[index] > maximumHeight + floatingBarHeight
                : _floatingLevels[index] < -(floatingBarHeight + Config.VerticalBarSpace * RenderScaling);

            if (leftDrawingArea)
            {
                _floatingLevels[index] = Math.Clamp(_levels[index], 0, maximumHeight);
                _floatingIsFalling[index] = false;
            }

            return;
        }

        _floatingIsFalling[index] = false;

        if (target >= _floatingLevels[index])
        {
            _floatingLevels[index] = target;
            return;
        }

        _floatingLevels[index] = Math.Max(0, _floatingLevels[index] - Config.FloatingBarDescentRate * elapsedSeconds);
    }
}
