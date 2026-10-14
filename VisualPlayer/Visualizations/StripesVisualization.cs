using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations;

public class StripesVisualization : VisualizationBase<StripesVisualizationConfig>
{
    #region CONST

    private const double MINIMUM_LEVEL = 0.25;

    #endregion CONST

    private readonly VisualizationColorConfig _colorConfig;
    private readonly VisualizationType _visualizationType;
    private double[] _floatingLevels = [];
    private bool[] _floatingIsFalling = [];
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private double[] _levels = [];
    private double _stripeWidth;

    private bool IsMirrored
    {
        get => _visualizationType == VisualizationType.StripesMirrored;
    }

    public override bool HasActiveAnimation
    {
        get => _levels.Any(level => level > MINIMUM_LEVEL)
            || _floatingLevels.Any(level => level > MINIMUM_LEVEL)
            || _colorConfig.IsAnimationEnabled;
    }

    #region CONSTRUCTOR

    public StripesVisualization(
        StripesVisualizationConfig config,
        VisualizationColorConfig colorConfig,
        VisualizationType visualizationType = VisualizationType.Stripes) : base(config)
    {
        _colorConfig = colorConfig ?? throw new ArgumentNullException(nameof(colorConfig));
        _visualizationType = visualizationType;
        RebuildStripes();
    }

    #endregion CONSTRUCTOR

    #region DRAWING

    public override void Draw(DrawingContext drawingContext, IBrush brush)
    {
        if (DrawingArea.Width <= 0 || DrawingArea.Height <= 0 || _stripeWidth <= 0)
            return;

        var horizontalSpace = Config.HorizontalBarSpace * RenderScaling;
        var floatingBarHeight = Config.FloatingBarHeight * RenderScaling;
        var floatingSpace = Config.VerticalBarSpace * RenderScaling;
        var centerHalfSpace = floatingSpace / 2;
        var visualizationBrush = CreateVisualizationBrush(brush);

        using (drawingContext.PushClip(DrawingArea))
        {
            for (var stripeIndex = 0; stripeIndex < _levels.Length; stripeIndex++)
            {
                var x = DrawingArea.X + stripeIndex * (_stripeWidth + horizontalSpace);
                var height = Math.Clamp(_levels[stripeIndex], 0, DrawingArea.Height);

                if (height > MINIMUM_LEVEL)
                {
                    if (IsMirrored)
                    {
                        if (Config.Direction == VisualizationDirection.BottomUp)
                        {
                            DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Center.Y - height, _stripeWidth, height));
                            DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Center.Y, _stripeWidth, height));
                        }
                        else
                        {
                            DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Top, _stripeWidth, height));
                            DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Bottom - height, _stripeWidth, height));
                        }
                    }
                    else
                    {
                        var y = Config.Direction == VisualizationDirection.BottomUp
                            ? DrawingArea.Bottom - height
                            : DrawingArea.Top;
                        DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, y, _stripeWidth, height));
                    }
                }

                if (!Config.ShowFloatingBar || _floatingLevels[stripeIndex] <= MINIMUM_LEVEL)
                    continue;

                if (IsMirrored)
                {
                    if (Config.Direction == VisualizationDirection.BottomUp)
                    {
                        DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Center.Y - centerHalfSpace - _floatingLevels[stripeIndex] - floatingSpace - floatingBarHeight, _stripeWidth, floatingBarHeight));
                        DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Center.Y + centerHalfSpace + _floatingLevels[stripeIndex] + floatingSpace, _stripeWidth, floatingBarHeight));
                    }
                    else
                    {
                        DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Top + _floatingLevels[stripeIndex] + floatingSpace, _stripeWidth, floatingBarHeight));
                        DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, DrawingArea.Bottom - _floatingLevels[stripeIndex] - floatingSpace - floatingBarHeight, _stripeWidth, floatingBarHeight));
                    }
                }
                else
                {
                    var floatingY = Config.Direction == VisualizationDirection.BottomUp
                        ? DrawingArea.Bottom - _floatingLevels[stripeIndex] - floatingSpace - floatingBarHeight
                        : DrawingArea.Top + _floatingLevels[stripeIndex] + floatingSpace;
                    DrawGradientSlice(drawingContext, visualizationBrush, new Rect(x, floatingY, _stripeWidth, floatingBarHeight));
                }
            }
        }
    }

    private void DrawGradientSlice(DrawingContext drawingContext, IBrush brush, Rect slice)
    {
        using (drawingContext.PushClip(slice))
            drawingContext.DrawRectangle(brush, null, DrawingArea);
    }

    #endregion DRAWING

    #region PROPERTY CHANGES

    protected override void OnConfigChanged(string? propertyName)
    {
        if (propertyName == nameof(StripesVisualizationConfig.StripesCount))
            RebuildStripes();
    }

    protected override void OnGraphicsChanged()
    {
        var horizontalSpace = Config.HorizontalBarSpace * RenderScaling;
        _stripeWidth = CommonUtils.CalculateColumnWidth(DrawingArea.Width, _levels.Length, horizontalSpace);
    }

    #endregion PROPERTY CHANGES

    public override void PreCalculate(in VisualizationFrame frame)
    {
        if (_levels.Length != Config.StripesCount)
            RebuildStripes();

        var elapsedSeconds = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var spectrum = frame.Spectrum.Span;
        var maximumHeight = GetMaximumLevel();

        CommonUtils.UpdateColorAnimation(_colorConfig, elapsedSeconds, DrawingArea, ref _gradientOffsetX, ref _gradientOffsetY);

        for (var stripeIndex = 0; stripeIndex < _levels.Length; stripeIndex++)
        {
            var target = frame.IsPlaying
                ? CommonUtils.GetSpectrumLevel(spectrum, stripeIndex, _levels.Length) * maximumHeight
                : 0;

            _levels[stripeIndex] = target >= _levels[stripeIndex]
                ? target
                : Math.Max(0, _levels[stripeIndex] - Config.DescentRate * elapsedSeconds);

            UpdateFloatingLevel(stripeIndex, target, elapsedSeconds, maximumHeight);
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
        var floatingHeight = Config.FloatingBarHeight * RenderScaling;
        var verticalSpace = Config.VerticalBarSpace * RenderScaling;
        var availableHeight = IsMirrored
            ? DrawingArea.Height / 2 - verticalSpace / 2
            : DrawingArea.Height;

        if (Config.ShowFloatingBar)
            availableHeight -= floatingHeight + verticalSpace;

        return Math.Max(0, availableHeight);
    }

    private void RebuildStripes()
    {
        _levels = new double[Config.StripesCount];
        _floatingLevels = new double[Config.StripesCount];
        _floatingIsFalling = new bool[Config.StripesCount];
        OnGraphicsChanged();
    }

    private void UpdateFloatingLevel(int index, double target, double elapsedSeconds, double maximumHeight)
    {
        var usesMirroredGravity = IsMirrored
            && Config.Direction == VisualizationDirection.TopDown;
        var usesRegularGravity = !IsMirrored;
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
                || usesRegularGravity && Config.Direction == VisualizationDirection.TopDown;
            _floatingLevels[index] += movesAwayFromOrigin
                ? movement
                : -movement;

            var floatingBarHeight = Config.FloatingBarHeight * RenderScaling;
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

    #region UTILITIES

    private IBrush CreateVisualizationBrush(IBrush fallbackBrush)
    {
        var stretch = _colorConfig.Stretch / 100d;

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
            _ => fallbackBrush
        };
    }

    #endregion UTILITIES
}
