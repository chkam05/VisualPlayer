using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public sealed class ColorizationBrushConverter : IMultiValueConverter
{
    public static ColorizationBrushConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3 || values[0] is not ColorizationLevel level)
            return values.Count > 1 ? values[1] : null;

        return level == ColorizationLevel.Poorly
            ? values[1]
            : values[2];
    }
}
