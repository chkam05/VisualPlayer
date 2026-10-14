using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Utils;

public static class GradientBrushUtils
{
    public static IBrush Create(
        GradientDefinition definition,
        double offsetX = 0,
        double offsetY = 0,
        double stretch = 1)
    {
        var stops = CreateStops(definition);

        return Create(definition.Type, definition.Angle, stops, offsetX, offsetY, stretch);
    }

    public static IBrush CreateRainbow(
        double offsetX = 0,
        double offsetY = 0,
        double stretch = 1,
        double angle = 0,
        GradientType type = GradientType.Linear)
    {
        var stops = new List<GradientStop>
        {
            new(Colors.Red, 0),
            new(Colors.Yellow, 1d / 6),
            new(Colors.Lime, 2d / 6),
            new(Colors.Cyan, 3d / 6),
            new(Colors.Blue, 4d / 6),
            new(Colors.Magenta, 5d / 6),
            new(Colors.Red, 1)
        };

        return Create(type, angle, stops, offsetX, offsetY, stretch);
    }

    public static IBrush CreateAbsoluteRadial(
        GradientDefinition definition,
        Point center,
        double radius,
        Rect drawingArea,
        double offsetX = 0,
        double offsetY = 0,
        double stretch = 1)
    {
        return CreateAbsoluteRadial(
            CreateStops(definition),
            center,
            radius,
            drawingArea,
            offsetX,
            offsetY,
            stretch);
    }

    public static IBrush CreateAbsoluteRadialRainbow(
        Point center,
        double radius,
        Rect drawingArea,
        double offsetX = 0,
        double offsetY = 0,
        double stretch = 1)
    {
        var stops = new List<GradientStop>
        {
            new(Colors.Red, 0),
            new(Colors.Yellow, 1d / 6),
            new(Colors.Lime, 2d / 6),
            new(Colors.Cyan, 3d / 6),
            new(Colors.Blue, 4d / 6),
            new(Colors.Magenta, 5d / 6),
            new(Colors.Red, 1)
        };

        return CreateAbsoluteRadial(
            stops,
            center,
            radius,
            drawingArea,
            offsetX,
            offsetY,
            stretch);
    }

    private static IBrush CreateAbsoluteRadial(
        IEnumerable<GradientStop> stops,
        Point center,
        double radius,
        Rect drawingArea,
        double offsetX,
        double offsetY,
        double stretch)
    {
        stretch = Math.Clamp(stretch, 0.1, 4);

        var animatedCenter = new RelativePoint(
            center.X + offsetX * drawingArea.Width,
            center.Y + offsetY * drawingArea.Height,
            RelativeUnit.Absolute);
        var absoluteRadius = Math.Max(1, radius * stretch);
        var brush = new RadialGradientBrush
        {
            Center = animatedCenter,
            GradientOrigin = animatedCenter,
            RadiusX = new RelativeScalar(absoluteRadius, RelativeUnit.Absolute),
            RadiusY = new RelativeScalar(absoluteRadius, RelativeUnit.Absolute),
            SpreadMethod = GradientSpreadMethod.Pad
        };

        foreach (var stop in stops)
            brush.GradientStops.Add(stop);

        return brush;
    }

    private static IBrush Create(
        GradientType type,
        double angle,
        IEnumerable<GradientStop> stops,
        double offsetX,
        double offsetY,
        double stretch)
    {
        stretch = Math.Clamp(stretch, 0.1, 4);
        var center = new RelativePoint(0.5 + offsetX, 0.5 + offsetY, RelativeUnit.Relative);
        GradientBrush brush;

        switch (type)
        {
            case GradientType.Radial:
                brush = new RadialGradientBrush
                {
                    Center = center,
                    GradientOrigin = center,
                    RadiusX = new RelativeScalar(0.5 * stretch, RelativeUnit.Relative),
                    RadiusY = new RelativeScalar(0.5 * stretch, RelativeUnit.Relative)
                };
                break;
            case GradientType.Conic:
                brush = new ConicGradientBrush
                {
                    Angle = angle,
                    Center = center
                };
                break;
            default:
                var radians = angle * Math.PI / 180;
                var halfX = Math.Cos(radians) * stretch / 2;
                var halfY = Math.Sin(radians) * stretch / 2;

                brush = new LinearGradientBrush
                {
                    StartPoint = new RelativePoint(0.5 - halfX + offsetX, 0.5 - halfY + offsetY, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(0.5 + halfX + offsetX, 0.5 + halfY + offsetY, RelativeUnit.Relative)
                };
                break;
        }

        brush.SpreadMethod = type == GradientType.Radial
            ? GradientSpreadMethod.Pad
            : GradientSpreadMethod.Repeat;

        foreach (var stop in stops)
            brush.GradientStops.Add(stop);

        return brush;
    }

    private static IEnumerable<GradientStop> CreateStops(GradientDefinition definition)
    {
        return definition.Points
            .OrderBy(point => point.Position)
            .Select(point => new GradientStop(point.Color.ToColor(), point.Position));
    }
}
