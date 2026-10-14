using System;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Utils;

public static class OscilloscopeUtils
{
    public static void DrawStandard(
        DrawingContext context,
        Rect area,
        ReadOnlySpan<float> samples,
        double thickness,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        BeamEffect effect,
        bool smoothLine)
    {
        DrawTrace(
            context, area, samples, thickness, colors, fallback, offsetX, offsetY, effect, smoothLine,
            static position => position,
            static (sample, _) => sample);
    }

    public static void DrawDouble(
        DrawingContext context,
        Rect area,
        ReadOnlySpan<float> samples,
        double thickness,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        BeamEffect effect,
        bool smoothLine)
    {
        DrawStandard(
            context,
            area,
            samples,
            thickness,
            colors,
            fallback,
            offsetX,
            offsetY,
            effect,
            smoothLine);

        DrawTrace(
            context,
            area,
            samples,
            thickness,
            colors,
            fallback,
            offsetX,
            offsetY,
            effect,
            smoothLine,
            static position => position,
            static (sample, _) => -sample);
    }

    public static void DrawDoubleFilled(
        DrawingContext context,
        Rect area,
        ReadOnlySpan<float> samples,
        double thickness,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        BeamEffect effect,
        bool smoothLine,
        double opacity)
    {
        if (samples.IsEmpty || area.Width <= 0 || area.Height <= 0)
            return;

        var columnCount = Math.Max(
            2,
            Math.Min(256, Math.Min(samples.Length, (int)Math.Ceiling(area.Width / 6))));
        var columnWidth = area.Width / columnCount + 0.5;

        for (var index = 0; index < columnCount; index++)
        {
            var normalizedX = (double)index / (columnCount - 1);
            var firstSample = GetSample(samples, normalizedX, smoothLine);
            var secondSample = -GetSample(samples, normalizedX, smoothLine);
            var firstY = Math.Clamp(0.5 - firstSample * 0.45, 0, 1);
            var secondY = Math.Clamp(0.5 - secondSample * 0.45, 0, 1);
            var top = Math.Min(firstY, secondY);
            var bottom = Math.Max(firstY, secondY);

            DrawFilledColumn(
                context,
                area,
                colors,
                fallback,
                offsetX,
                offsetY,
                normalizedX,
                area.X + normalizedX * area.Width,
                area.Y + top * area.Height,
                columnWidth,
                (bottom - top) * area.Height,
                opacity);
        }

        DrawDouble(
            context,
            area,
            samples,
            thickness,
            colors,
            fallback,
            offsetX,
            offsetY,
            effect,
            smoothLine);
    }

    public static void DrawCollision(
        DrawingContext context,
        Rect area,
        ReadOnlySpan<float> samples,
        double thickness,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        BeamEffect effect,
        bool smoothLine)
    {
        DrawTrace(
            context,
            area,
            samples,
            thickness,
            colors,
            fallback,
            offsetX,
            offsetY,
            effect,
            smoothLine,
            static position => Math.Abs(position - 0.5) * 2,
            static (sample, position) =>
            {
                var centerStrength = 1 - Math.Abs(position - 0.5) * 2;
                var verticalDirection = position <= 0.5 ? 1 : -1;
                return (float)(sample * centerStrength * verticalDirection);
            });
    }

    private static void DrawTrace(
        DrawingContext context,
        Rect area,
        ReadOnlySpan<float> samples,
        double thickness,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        BeamEffect effect,
        bool smoothLine,
        Func<double, double> samplePositionSelector,
        Func<float, double, float> sampleSelector)
    {
        if (samples.IsEmpty || area.Width <= 0 || area.Height <= 0)
            return;

        var pointCount = smoothLine
            ? Math.Max(2, Math.Min(512, Math.Min(samples.Length * 2, (int)Math.Ceiling(area.Width / 2))))
            : Math.Max(2, Math.Min(512, Math.Min(samples.Length, (int)Math.Ceiling(area.Width / 2))));
        Point? previous = null;
        Pen? solidPen = null;
        Pen? solidGlowPen = null;

        if (colors.Mode == VisualizationColorMode.Solid)
        {
            var solidColor = colors.SolidColor.ToColor();
            solidPen = new Pen(new SolidColorBrush(solidColor), thickness);

            if (effect == BeamEffect.Beam)
                solidGlowPen = new Pen(new SolidColorBrush(WithOpacity(solidColor, 0.22)), thickness * 4);
        }

        for (var index = 0; index < pointCount; index++)
        {
            var normalizedX = (double)index / (pointCount - 1);
            var samplePosition = samplePositionSelector(normalizedX);
            var sample = sampleSelector(GetSample(samples, samplePosition, smoothLine), normalizedX);
            var normalizedY = Math.Clamp(0.5 - sample * 0.45, 0, 1);
            var point = new Point(
                area.X + normalizedX * area.Width,
                area.Y + normalizedY * area.Height);

            if (previous is not null)
            {
                if (solidPen is not null)
                {
                    if (solidGlowPen is not null)
                        context.DrawLine(solidGlowPen, previous.Value, point);

                    context.DrawLine(solidPen, previous.Value, point);
                    previous = point;
                    continue;
                }

                var color = VisualizationColorSampler.GetColor(
                    colors, normalizedX, normalizedY, offsetX, offsetY, fallback);

                if (effect == BeamEffect.Beam)
                {
                    context.DrawLine(
                        new Pen(new SolidColorBrush(WithOpacity(color, 0.22)), thickness * 4),
                        previous.Value,
                        point);
                }

                context.DrawLine(new Pen(new SolidColorBrush(color), thickness), previous.Value, point);
            }

            previous = point;
        }
    }

    public static float GetSample(ReadOnlySpan<float> samples, double position, bool smoothLine)
    {
        var exactIndex = Math.Clamp(position, 0, 1) * (samples.Length - 1);

        if (!smoothLine)
            return samples[Math.Clamp((int)Math.Round(exactIndex), 0, samples.Length - 1)];

        var index = (int)Math.Floor(exactIndex);
        var amount = exactIndex - index;
        var point0 = GetSmoothedSample(samples, index - 1);
        var point1 = GetSmoothedSample(samples, index);
        var point2 = GetSmoothedSample(samples, index + 1);
        var point3 = GetSmoothedSample(samples, index + 2);
        var amountSquared = amount * amount;
        var amountCubed = amountSquared * amount;

        return (float)(0.5 * (
            2 * point1
            + (-point0 + point2) * amount
            + (2 * point0 - 5 * point1 + 4 * point2 - point3) * amountSquared
            + (-point0 + 3 * point1 - 3 * point2 + point3) * amountCubed));
    }

    private static float GetSmoothedSample(ReadOnlySpan<float> samples, int index)
    {
        const int radius = 4;
        var value = 0f;
        var weightSum = 0f;

        for (var offset = -radius; offset <= radius; offset++)
        {
            var sampleIndex = Math.Clamp(index + offset, 0, samples.Length - 1);
            var weight = radius + 1 - Math.Abs(offset);
            value += samples[sampleIndex] * weight;
            weightSum += weight;
        }

        return value / weightSum;
    }

    public static void DrawFilledColumn(
        DrawingContext context,
        Rect area,
        VisualizationColorConfig colors,
        Color fallback,
        double offsetX,
        double offsetY,
        double normalizedX,
        double left,
        double top,
        double width,
        double height,
        double opacity)
    {
        if (height <= 0)
            return;

        const int colorSteps = 4;
        var stepHeight = height / colorSteps;

        for (var step = 0; step < colorSteps; step++)
        {
            var stepTop = top + step * stepHeight;
            var normalizedY = Math.Clamp(
                (stepTop + stepHeight * 0.5 - area.Top) / Math.Max(1, area.Height),
                0,
                1);
            var color = VisualizationColorSampler.GetColor(
                colors,
                normalizedX,
                normalizedY,
                offsetX,
                offsetY,
                fallback);

            context.DrawRectangle(
                new SolidColorBrush(WithOpacity(color, opacity)),
                null,
                new Rect(left, stepTop, width, stepHeight + 0.5));
        }
    }

    private static Color WithOpacity(Color color, double opacity)
    {
        return Color.FromArgb(
            (byte)Math.Clamp(Math.Round(color.A * opacity), 0, byte.MaxValue),
            color.R,
            color.G,
            color.B);
    }
}
