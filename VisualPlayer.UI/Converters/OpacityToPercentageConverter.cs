using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VisualPlayer.Converters;

public class OpacityToPercentageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            double opacity => opacity.ToString("P0", culture),
            int percentage => $"{percentage}%",
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
