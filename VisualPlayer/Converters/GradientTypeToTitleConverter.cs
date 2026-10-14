using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class GradientTypeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            GradientType.Linear => ApplicationContext.Instance.Translations["ENUM_GRADIENTTYPE_LINEAR"],
            GradientType.Radial => ApplicationContext.Instance.Translations["ENUM_GRADIENTTYPE_RADIAL"],
            GradientType.Conic => ApplicationContext.Instance.Translations["ENUM_GRADIENTTYPE_CONIC"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
