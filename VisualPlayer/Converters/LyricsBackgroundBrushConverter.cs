using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace VisualPlayer.Converters;

public class LyricsBackgroundBrushConverter : IMultiValueConverter
{
    public static LyricsBackgroundBrushConverter Instance { get; } = new();

    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2
            || values[0] is not SolidColorBrush source
            || values[1] is not double opacity)
        {
            return AvaloniaProperty.UnsetValue;
        }

        var alpha = (byte)Math.Round(255 * Math.Clamp(opacity, 0, 1));
        var center = Color.FromArgb(alpha, source.Color.R, source.Color.G, source.Color.B);
        var transparent = Color.FromArgb(0, source.Color.R, source.Color.G, source.Color.B);

        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops =
            [
                new GradientStop(transparent, 0),
                new GradientStop(center, 0.22),
                new GradientStop(center, 0.78),
                new GradientStop(transparent, 1)
            ]
        };
    }
}
