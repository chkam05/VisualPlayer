using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class ColorizationLevelToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ColorizationLevel level)
            return string.Empty;

        return level switch
        {
            ColorizationLevel.Poorly => "Oszczędna",
            ColorizationLevel.Buttons => "Przyciski",
            ColorizationLevel.Whole => "Całość",
            _ => level.ToString()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
