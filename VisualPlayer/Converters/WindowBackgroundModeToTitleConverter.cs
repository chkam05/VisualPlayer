using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class WindowBackgroundModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            WindowBackgroundMode.Transparent => "Przezroczyste",
            WindowBackgroundMode.SolidColor => "Jednolity kolor",
            WindowBackgroundMode.Image => "Obraz",
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
