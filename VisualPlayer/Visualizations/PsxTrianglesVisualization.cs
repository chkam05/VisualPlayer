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

public class PsxTrianglesVisualization : VisualizationBase<PsxTrianglesVisualizationConfig>
{
    private const double TRIANGLE_GAP_RATIO = 0.5;

    private readonly VisualizationColorConfig _colorConfig;
    private double _gradientOffsetX;
    private double _gradientOffsetY;
    private int _maximumFrequency;
    private int _minimumFrequency;
    private float[][] _levels = [];
    private ScalingStrategy _scalingStrategy;

    private static readonly (double Minimum, double Maximum)[] FrequencyBands =
    [
        (50, 120),
        (120, 250),
        (250, 500),
        (500, 2000),
        (2000, 6000),
        (6000, 24000)
    ];

    public override bool HasActiveAnimation => _colorConfig.IsAnimationEnabled
        || HasVisibleLevels();

    public PsxTrianglesVisualization(
        PsxTrianglesVisualizationConfig config,
        VisualizationColorConfig colorConfig) : base(config)
    {
        _colorConfig = colorConfig;
    }

    public override void Draw(DrawingContext drawingContext, IBrush fallbackBrush)
    {
        if (_levels.Length != PsxTrianglesVisualizationConfig.GROUP_COUNT
            || DrawingArea.Width <= 0
            || DrawingArea.Height <= 0)
        {
            return;
        }

        var fallbackColor = fallbackBrush is ISolidColorBrush solidBrush
            ? solidBrush.Color
            : Colors.White;
        var centerY = DrawingArea.Center.Y;
        var maximumHeight = DrawingArea.Height / 2;
        var offset = Config.OffsetX * RenderScaling;

        using (drawingContext.PushClip(DrawingArea))
        {
            foreach (var groupIndex in Config.LayerOrder)
            {
                var brush = CreateGroupBrush(groupIndex, fallbackColor);
                DrawGroup(
                    drawingContext,
                    brush,
                    brush,
                    groupIndex,
                    centerY,
                    maximumHeight,
                    -offset,
                    offset);
            }

            using (drawingContext.PushOpacity(Config.CenterLineOpacity))
            {
                drawingContext.DrawLine(
                    new Pen(Brushes.Black, RenderScaling),
                    new Point(DrawingArea.Left, centerY),
                    new Point(DrawingArea.Right, centerY));
            }
        }
    }

    public override void PreCalculate(in VisualizationFrame frame)
    {
        EnsureLevelArrays();

        var elapsed = Math.Clamp(frame.Elapsed.TotalSeconds, 0, 0.1);
        var normalizedDescent = Config.DescentRate
            / Math.Max(1, DrawingArea.Height / 2)
            * elapsed;
        var spectrum = frame.Spectrum.Span;
        _minimumFrequency = frame.MinimumFrequency;
        _maximumFrequency = frame.MaximumFrequency;
        _scalingStrategy = frame.ScalingStrategy;

        CommonUtils.UpdateColorAnimation(
            _colorConfig,
            elapsed,
            DrawingArea,
            ref _gradientOffsetX,
            ref _gradientOffsetY);

        for (var groupIndex = 0;
             groupIndex < PsxTrianglesVisualizationConfig.GROUP_COUNT;
             groupIndex++)
        {
            var groupLevels = _levels[groupIndex];
            var sharedTarget = frame.IsPlaying
                ? GetBandAverage(
                    spectrum,
                    groupIndex,
                    PsxTrianglesVisualizationConfig.GROUP_COUNT)
                : 0;

            for (var triangleIndex = 0; triangleIndex < groupLevels.Length; triangleIndex++)
            {
                var target = frame.IsPlaying && Config.SplitBand
                    ? GetSubBandAverage(
                        spectrum,
                        groupIndex,
                        PsxTrianglesVisualizationConfig.GROUP_COUNT,
                        triangleIndex,
                        groupLevels.Length)
                    : sharedTarget;

                groupLevels[triangleIndex] = target >= groupLevels[triangleIndex]
                    ? (float)target
                    : (float)Math.Max(0, groupLevels[triangleIndex] - normalizedDescent);
            }
        }
    }

    public override void Reset()
    {
        foreach (var group in _levels)
            Array.Clear(group);

        _gradientOffsetX = 0;
        _gradientOffsetY = 0;
    }

    private void DrawGroup(
        DrawingContext drawingContext,
        IBrush brush,
        IBrush borderBrush,
        int groupIndex,
        double centerY,
        double maximumHeight,
        double topOffsetX,
        double bottomOffsetX)
    {
        var levels = _levels[groupIndex];
        if (levels.Length == 0)
            return;

        var groupOpacity = Config.GetLayerOpacity(groupIndex);
        DrawGroupHalf(
            drawingContext,
            brush,
            borderBrush,
            levels,
            centerY,
            maximumHeight,
            topOffsetX,
            -1,
            groupOpacity);
        DrawGroupHalf(
            drawingContext,
            brush,
            borderBrush,
            levels,
            centerY,
            maximumHeight,
            bottomOffsetX,
            1,
            groupOpacity);
    }

    private void DrawGroupHalf(
        DrawingContext drawingContext,
        IBrush brush,
        IBrush borderBrush,
        float[] levels,
        double centerY,
        double maximumHeight,
        double horizontalInset,
        int verticalDirection,
        double groupOpacity)
    {
        var clampedInset = Math.Clamp(
            horizontalInset,
            -DrawingArea.Width,
            DrawingArea.Width);
        var availableLeft = DrawingArea.Left + Math.Max(0, clampedInset);
        var availableWidth = Math.Max(0, DrawingArea.Width - Math.Abs(clampedInset));
        if (availableWidth <= 0)
            return;

        var maximumWidth = Config.MaximumTriangleWidth * RenderScaling;
        var widthForFullRow = availableWidth
            / (levels.Length + (levels.Length + 1) * TRIANGLE_GAP_RATIO);
        var triangleWidth = Math.Min(maximumWidth, widthForFullRow);
        var gap = triangleWidth * TRIANGLE_GAP_RATIO;
        var rowWidth = levels.Length * triangleWidth
            + Math.Max(0, levels.Length - 1) * gap;
        var firstCenterX = availableLeft
            + availableWidth / 2
            - rowWidth / 2
            + triangleWidth / 2;

        for (var triangleIndex = 0; triangleIndex < levels.Length; triangleIndex++)
        {
            var level = Math.Clamp(levels[triangleIndex], 0, 1);
            if (level <= 0.001)
                continue;

            var centerX = firstCenterX + triangleIndex * (triangleWidth + gap);
            var height = level * maximumHeight;
            DrawTriangle(
                drawingContext,
                brush,
                borderBrush,
                CreateTriangle(
                    centerX,
                    centerY,
                    triangleWidth,
                    height,
                    verticalDirection),
                groupOpacity);
        }
    }

    private void DrawTriangle(
        DrawingContext drawingContext,
        IBrush fillBrush,
        IBrush borderBrush,
        StreamGeometry geometry,
        double fillOpacity)
    {
        using (drawingContext.PushOpacity(fillOpacity))
            drawingContext.DrawGeometry(fillBrush, null, geometry);

        if (!Config.BorderEnabled)
            return;

        var borderOpacity = fillOpacity * Config.BorderOpacity;
        using (drawingContext.PushOpacity(borderOpacity))
        {
            if (Config.BorderEffect == BeamEffect.Beam)
            {
                using (drawingContext.PushOpacity(0.22))
                {
                    drawingContext.DrawGeometry(
                        null,
                        new Pen(borderBrush, RenderScaling * 4),
                        geometry);
                }
            }

            drawingContext.DrawGeometry(
                null,
                new Pen(borderBrush, RenderScaling),
                geometry);
        }
    }

    private void EnsureLevelArrays()
    {
        if (_levels.Length != PsxTrianglesVisualizationConfig.GROUP_COUNT)
            _levels = new float[PsxTrianglesVisualizationConfig.GROUP_COUNT][];

        for (var groupIndex = 0; groupIndex < _levels.Length; groupIndex++)
        {
            var count = Config.GetTriangleCount(groupIndex);
            if (_levels[groupIndex] is null || _levels[groupIndex].Length != count)
                _levels[groupIndex] = new float[count];
        }
    }

    private bool HasVisibleLevels()
    {
        foreach (var group in _levels)
        {
            if (group is null)
                continue;

            foreach (var level in group)
            {
                if (level > 0.001f)
                    return true;
            }
        }

        return false;
    }

    private static StreamGeometry CreateTriangle(
        double centerX,
        double centerY,
        double width,
        double height,
        int verticalDirection)
    {
        var geometry = new StreamGeometry();
        using var context = geometry.Open();
        context.BeginFigure(new Point(centerX - width / 2, centerY), true);
        context.LineTo(new Point(centerX, centerY + verticalDirection * height));
        context.LineTo(new Point(centerX + width / 2, centerY));
        context.EndFigure(true);
        return geometry;
    }

    private double GetBandAverage(
        ReadOnlySpan<float> spectrum,
        int bandIndex,
        int bandCount)
    {
        return GetSubBandAverage(spectrum, bandIndex, bandCount, 0, 1);
    }

    private double GetSubBandAverage(
        ReadOnlySpan<float> spectrum,
        int bandIndex,
        int bandCount,
        int subBandIndex,
        int subBandCount)
    {
        if (spectrum.IsEmpty)
            return 0;

        var frequencyBand = FrequencyBands[Math.Clamp(bandIndex, 0, FrequencyBands.Length - 1)];
        var subBandFrequencyWidth = (frequencyBand.Maximum - frequencyBand.Minimum)
            / Math.Max(1, subBandCount);
        var subBandMinimum = frequencyBand.Minimum
            + subBandFrequencyWidth * subBandIndex;
        var subBandMaximum = frequencyBand.Minimum
            + subBandFrequencyWidth * (subBandIndex + 1);
        var bandStart = GetSpectrumPosition(subBandMinimum) * spectrum.Length;
        var bandEnd = GetSpectrumPosition(subBandMaximum) * spectrum.Length;
        if (bandEnd <= 0 || bandStart >= spectrum.Length || bandEnd <= bandStart)
            return 0;

        var start = Math.Clamp(
            (int)Math.Floor(bandStart),
            0,
            spectrum.Length - 1);
        var end = Math.Clamp(
            (int)Math.Ceiling(bandEnd),
            start + 1,
            spectrum.Length);
        var sum = 0d;

        for (var index = start; index < end; index++)
            sum += Math.Clamp(spectrum[index], 0, 1);

        return sum / Math.Max(1, end - start);
    }

    private double GetSpectrumPosition(double frequency)
    {
        var minimum = Math.Clamp(_minimumFrequency, 0, 23999);
        var maximum = Math.Clamp(_maximumFrequency, minimum + 1, 24000);
        frequency = Math.Clamp(frequency, minimum, maximum);

        if (_scalingStrategy == ScalingStrategy.Linear)
            return (frequency - minimum) / (maximum - minimum);

        if (_scalingStrategy == ScalingStrategy.Mel)
        {
            var minimumMel = 2595 * Math.Log10(1 + minimum / 700d);
            var maximumMel = 2595 * Math.Log10(1 + maximum / 700d);
            var frequencyMel = 2595 * Math.Log10(1 + frequency / 700d);
            return (frequencyMel - minimumMel) / Math.Max(double.Epsilon, maximumMel - minimumMel);
        }

        if (_scalingStrategy == ScalingStrategy.Bark)
        {
            var minimumBark = 6 * Math.Asinh(minimum / 600d);
            var maximumBark = 6 * Math.Asinh(maximum / 600d);
            var frequencyBark = 6 * Math.Asinh(frequency / 600d);
            return (frequencyBark - minimumBark) / Math.Max(double.Epsilon, maximumBark - minimumBark);
        }

        var safeMinimum = Math.Max(1, minimum);
        return Math.Log(Math.Max(safeMinimum, frequency) / safeMinimum)
            / Math.Max(double.Epsilon, Math.Log(maximum / (double)safeMinimum));
    }

    private IBrush CreateGroupBrush(int groupIndex, Color fallbackColor)
    {
        if (_colorConfig.Mode == VisualizationColorMode.Solid)
            return new SolidColorBrush(_colorConfig.SolidColor.ToColor());

        var stretch = Math.Max(0.1, _colorConfig.Stretch / 100d);
        var segmentStart = groupIndex / (double)PsxTrianglesVisualizationConfig.GROUP_COUNT;
        var segmentEnd = (groupIndex + 1d) / PsxTrianglesVisualizationConfig.GROUP_COUNT;
        var definition = new GradientDefinition
        {
            Angle = _colorConfig.Mode == VisualizationColorMode.Gradient
                ? _colorConfig.Gradient.Angle
                : _colorConfig.RainbowAngle,
            Type = _colorConfig.Mode == VisualizationColorMode.Gradient
                ? _colorConfig.Gradient.Type
                : _colorConfig.RainbowGradientType,
            Points = []
        };

        const int stopCount = 8;
        for (var stopIndex = 0; stopIndex <= stopCount; stopIndex++)
        {
            var position = stopIndex / (double)stopCount;
            var sourcePosition = segmentStart + (segmentEnd - segmentStart) * position;
            definition.Points.Add(new GradientPoint
            {
                Color = new(
                    VisualizationColorSampler.GetColorAtPosition(
                        _colorConfig,
                        sourcePosition,
                        fallbackColor),
                    string.Empty),
                Position = position
            });
        }

        return GradientBrushUtils.CreateAbsolute(
            definition,
            DrawingArea,
            _gradientOffsetX,
            _gradientOffsetY,
            stretch);
    }
}
