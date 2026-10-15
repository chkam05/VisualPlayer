using System;
using System.Linq;
using Avalonia.Media;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Utils;

public static class VisualizationColorSampler
{
    public static Color GetColor(
        VisualizationColorConfig config,
        double x,
        double y,
        double offsetX,
        double offsetY,
        Color fallbackColor)
    {
        x = Math.Clamp(x, 0, 1);
        y = Math.Clamp(y, 0, 1);

        return config.Mode switch
        {
            VisualizationColorMode.Solid => config.SolidColor.ToColor(),
            VisualizationColorMode.Gradient => SampleGradient(config, x, y, offsetX, offsetY),
            VisualizationColorMode.Rainbow => SampleRainbow(config, x, y, offsetX, offsetY),
            _ => fallbackColor
        };
    }

    private static Color SampleGradient(
        VisualizationColorConfig config,
        double x,
        double y,
        double offsetX,
        double offsetY)
    {
        var definition = config.Gradient;
        var stretch = Math.Max(0.1, config.Stretch / 100d);
        var position = definition.Type switch
        {
            GradientType.Radial => GetRadialPosition(x, y, offsetX, offsetY, stretch),
            GradientType.Conic => GetConicPosition(x, y, offsetX, offsetY, definition.Angle, stretch),
            _ => GetLinearPosition(x, y, offsetX, offsetY, definition.Angle, stretch)
        };

        var sampledPosition = definition.Type == GradientType.Radial && offsetX == 0 && offsetY == 0
            ? Math.Clamp(position, 0, 1)
            : Wrap(position);

        return Interpolate(definition, sampledPosition);
    }

    private static Color SampleRainbow(
        VisualizationColorConfig config,
        double x,
        double y,
        double offsetX,
        double offsetY)
    {
        var stretch = Math.Max(0.1, config.Stretch / 100d);
        var position = config.RainbowGradientType switch
        {
            GradientType.Radial => GetRadialPosition(x, y, offsetX, offsetY, stretch),
            GradientType.Conic => GetConicPosition(
                x,
                y,
                offsetX,
                offsetY,
                config.RainbowAngle,
                stretch),
            _ => GetLinearPosition(
                x,
                y,
                offsetX,
                offsetY,
                config.RainbowAngle,
                stretch)
        };

        position = Wrap(position);
        var hue = (int)Math.Round(position * AHSLColor.HUE_MAX);
        return new AHSLColor(byte.MaxValue, hue, 100, 50).ToColor();
    }

    private static double GetLinearPosition(
        double x,
        double y,
        double offsetX,
        double offsetY,
        double angle,
        double stretch)
    {
        var radians = angle * Math.PI / 180;
        var directionX = Math.Cos(radians);
        var directionY = Math.Sin(radians);
        var relativeX = x - 0.5 - offsetX;
        var relativeY = y - 0.5 - offsetY;
        return 0.5 + (relativeX * directionX + relativeY * directionY) / stretch;
    }

    private static double GetRadialPosition(
        double x,
        double y,
        double offsetX,
        double offsetY,
        double stretch)
    {
        var relativeX = x - 0.5;
        var relativeY = y - 0.5;
        return Math.Sqrt(relativeX * relativeX + relativeY * relativeY) * 2 / stretch
            - GradientAnimationUtils.GetPhase(offsetX, offsetY, stretch);
    }

    private static double GetConicPosition(
        double x,
        double y,
        double offsetX,
        double offsetY,
        double angle,
        double stretch)
    {
        var relativeX = x - 0.5;
        var relativeY = y - 0.5;
        var turn = Math.Atan2(relativeY, relativeX) / (2 * Math.PI);
        return (turn - angle / 360d) / stretch
            - GradientAnimationUtils.GetPhase(offsetX, offsetY, stretch);
    }

    private static Color Interpolate(GradientDefinition definition, double position)
    {
        var points = definition.Points.OrderBy(point => point.Position).ToArray();

        if (points.Length == 0)
            return Colors.Transparent;

        if (points.Length == 1 || position <= points[0].Position)
            return points[0].Color.ToColor();

        if (position >= points[^1].Position)
            return points[^1].Color.ToColor();

        for (var index = 0; index < points.Length - 1; index++)
        {
            var start = points[index];
            var end = points[index + 1];

            if (position > end.Position)
                continue;

            var range = Math.Max(double.Epsilon, end.Position - start.Position);
            var amount = (position - start.Position) / range;
            return Interpolate(start.Color.ToColor(), end.Color.ToColor(), amount);
        }

        return points[^1].Color.ToColor();
    }

    private static Color Interpolate(Color start, Color end, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);

        return Color.FromArgb(
            Interpolate(start.A, end.A, amount),
            Interpolate(start.R, end.R, amount),
            Interpolate(start.G, end.G, amount),
            Interpolate(start.B, end.B, amount));
    }

    private static byte Interpolate(byte start, byte end, double amount)
    {
        return (byte)Math.Clamp((int)Math.Round(start + (end - start) * amount), 0, 255);
    }

    private static double Wrap(double value)
    {
        value %= 1;
        return value < 0 ? value + 1 : value;
    }

}
