using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace VisualPlayer.Converters;

public class BrushOpacityConverter : IMultiValueConverter
{
    public static BrushOpacityConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2
            || values[0] is not SolidColorBrush brush
            || values[1] is not double opacity)
        {
            return AvaloniaProperty.UnsetValue;
        }

        double? minimumOpacity = values.Count > 2 && values[2] is double minimum
            ? Math.Clamp(minimum, 0, 1)
            : null;

        var effectiveOpacity = Math.Clamp(opacity, 0, 1);

        if (minimumOpacity is not null)
            effectiveOpacity = Math.Max(effectiveOpacity, minimumOpacity.Value);

        return new SolidColorBrush(brush.Color, brush.Opacity * effectiveOpacity);
    }
}
