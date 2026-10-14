using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class ImageFillModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ImageFillMode.Normal => "Normalnie",
            ImageFillMode.Center => "Wyśrodkuj",
            ImageFillMode.Stretch => "Rozciągnij",
            ImageFillMode.Uniform => "Dopasuj",
            ImageFillMode.UniformToFill => "Wypełnij",
            ImageFillMode.Tile => "Kafelki",
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
