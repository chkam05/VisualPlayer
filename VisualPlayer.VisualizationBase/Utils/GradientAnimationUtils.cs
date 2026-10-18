using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace VisualPlayer.Utils;

public static class GradientAnimationUtils
{
    public static double GetPhase(double offsetX, double offsetY, double stretch)
    {
        var phase = (offsetX + offsetY) / stretch % 1;
        return phase < 0 ? phase + 1 : phase;
    }

    public static IEnumerable<GradientStop> ShiftStops(IEnumerable<GradientStop> stops, double phase)
    {
        var source = stops.OrderBy(stop => stop.Offset).ToList();
        if (source.Count == 0 || phase == 0)
            return source;

        // Extend missing endpoints before moving the repeating color sequence.
        if (source[0].Offset > 0)
            source.Insert(0, new GradientStop(source[0].Color, 0));
        if (source[^1].Offset < 1)
            source.Add(new GradientStop(source[^1].Color, 1));

        var boundaryPosition = 1 - phase;
        var boundaryColor = source[^1].Color;
        for (var index = 1; index < source.Count; index++)
        {
            if (boundaryPosition > source[index].Offset)
                continue;

            var start = source[index - 1];
            var end = source[index];
            var amount = (boundaryPosition - start.Offset) / Math.Max(double.Epsilon, end.Offset - start.Offset);
            boundaryColor = Color.FromArgb(
                Lerp(start.Color.A, end.Color.A, amount), Lerp(start.Color.R, end.Color.R, amount),
                Lerp(start.Color.G, end.Color.G, amount), Lerp(start.Color.B, end.Color.B, amount));
            break;
        }

        var shifted = new List<GradientStop> { new(boundaryColor, 0) };
        // Keep both colors at a wrap seam when the original endpoints differ.
        for (var cycle = -1; cycle <= 0; cycle++)
        {
            foreach (var stop in source)
            {
                var position = stop.Offset + phase + cycle;
                if (position > 0 && position < 1)
                    shifted.Add(new GradientStop(stop.Color, position));
            }
        }
        shifted.Add(new GradientStop(boundaryColor, 1));
        return shifted.OrderBy(stop => stop.Offset);
    }

    private static byte Lerp(byte start, byte end, double amount)
        => (byte)Math.Clamp((int)Math.Round(start + (end - start) * amount), 0, 255);
}
