using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Utils;

public static class CircleVisualizationUtils
{
    public static StreamGeometry CreateClosedSmoothGeometry(IReadOnlyList<Point> points)
    {
        var geometry = new StreamGeometry();

        if (points.Count < 2)
            return geometry;

        var start = Midpoint(points[^1], points[0]);

        using var context = geometry.Open();
        context.BeginFigure(start, true);

        for (var index = 0; index < points.Count; index++)
        {
            var current = points[index];
            var next = points[(index + 1) % points.Count];
            context.QuadraticBezierTo(current, Midpoint(current, next));
        }

        context.EndFigure(true);
        return geometry;
    }

    public static double GetAnimatedRotation(
        double currentRotation,
        bool enabled,
        RotationDirection direction,
        double speed,
        double elapsedSeconds)
    {
        if (!enabled)
            return currentRotation;

        var directionMultiplier = direction == RotationDirection.Clockwise ? 1 : -1;
        var result = currentRotation + directionMultiplier * speed * elapsedSeconds;
        return (result % 360 + 360) % 360;
    }

    public static double GetMaximumLineThickness(double innerRadius, int count)
    {
        return Math.Max(1, 2 * innerRadius * Math.Sin(Math.PI / Math.Max(2, count)) * 0.75);
    }

    public static double GetMirroredPosition(double position, bool mirror)
    {
        if (!mirror)
            return position;

        return position <= 0.5
            ? position * 2
            : (1 - position) * 2;
    }

    public static double GetOuterRadius(Rect area, double lineThickness)
    {
        return Math.Max(0, Math.Min(area.Width, area.Height) / 2 - lineThickness / 2);
    }

    public static Point GetPoint(Point center, double radius, double angleRadians)
    {
        return new Point(
            center.X + Math.Cos(angleRadians) * radius,
            center.Y + Math.Sin(angleRadians) * radius);
    }

    private static Point Midpoint(Point first, Point second)
    {
        return new Point(
            (first.X + second.X) / 2,
            (first.Y + second.Y) / 2);
    }

}
