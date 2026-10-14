using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class ApplicationThemeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ApplicationTheme.System => "Systemowy",
            ApplicationTheme.Light => "Jasny",
            ApplicationTheme.Dark => "Ciemny",
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
