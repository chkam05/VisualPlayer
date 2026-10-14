using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace VisualPlayer.Converters;

public class AnyTrueConverter : IMultiValueConverter
{
    public static AnyTrueConverter Instance { get; } = new();

    public object Convert(
        IList<object?> values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return values.Any(value => value is true);
    }
}
