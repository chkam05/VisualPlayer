using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Visualizations.Utils;

public static class CommonUtils
{
    public static double TransformWaveformLevel(
        double level,
        double position,
        WaveformTransformation transformation,
        double minimum)
    {
        var normalizedMinimum = Math.Clamp(minimum, 0, 1);
        var waveform = transformation switch
        {
            WaveformTransformation.Sin => Math.Sin(position * Math.PI),
            WaveformTransformation.Cos => (Math.Cos(position * Math.PI * 2) + 1) / 2,
            _ => 1
        };
        var factor = transformation == WaveformTransformation.None
            ? 1
            : normalizedMinimum + (1 - normalizedMinimum) * waveform;

        return level * factor;
    }

    public static StreamGeometry CreateSmoothGeometry(IReadOnlyList<Point> points)
    {
        var geometry = new StreamGeometry();

        if (points.Count == 0)
            return geometry;

        using var context = geometry.Open();
        context.BeginFigure(points[0], false);
        AppendSmoothPath(context, points);
        context.EndFigure(false);
        return geometry;
    }

    public static void AppendSmoothPath(StreamGeometryContext context, IReadOnlyList<Point> points)
    {
        for (var index = 1; index < points.Count - 1; index++)
        {
            var current = points[index];
            var next = points[index + 1];
            var midpoint = new Point(
                (current.X + next.X) / 2,
                (current.Y + next.Y) / 2);

            context.QuadraticBezierTo(current, midpoint);
        }

        if (points.Count > 1)
            context.LineTo(points[^1]);
    }

    public static double CalculateColumnWidth(double areaWidth, int columnCount, double horizontalSpace)
    {
        var spacesWidth = horizontalSpace * Math.Max(0, columnCount - 1);
        return Math.Max(0, (areaWidth - spacesWidth) / Math.Max(1, columnCount));
    }

    public static double GetSpectrumLevel(ReadOnlySpan<float> spectrum, int index, int count)
    {
        if (spectrum.IsEmpty)
            return 0;

        var normalizedStart = (double)index / count;
        var normalizedEnd = (double)(index + 1) / count;
        var start = Math.Clamp((int)(normalizedStart * spectrum.Length), 0, spectrum.Length - 1);
        var end = Math.Clamp((int)Math.Ceiling(normalizedEnd * spectrum.Length), start + 1, spectrum.Length);
        var value = 0f;

        for (var spectrumIndex = start; spectrumIndex < end; spectrumIndex++)
            value = Math.Max(value, spectrum[spectrumIndex]);

        return Math.Clamp(value, 0, 1);
    }

    public static void UpdateColorAnimation(
        VisualizationColorConfig config,
        double elapsedSeconds,
        Rect drawingArea,
        ref double offsetX,
        ref double offsetY)
    {
        if (!config.IsAnimationEnabled)
            return;

        offsetX += config.HorizontalAnimationSpeed * elapsedSeconds / Math.Max(1, drawingArea.Width);
        offsetY += config.VerticalAnimationSpeed * elapsedSeconds / Math.Max(1, drawingArea.Height);
    }
}
