using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class WindowBackgroundModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            WindowBackgroundMode.Transparent => ApplicationContext.Instance.Translations["ENUM_WINDOWBACKGROUNDMODE_TRANSPARENT"],
            WindowBackgroundMode.SolidColor => ApplicationContext.Instance.Translations["ENUM_WINDOWBACKGROUNDMODE_SOLIDCOLOR"],
            WindowBackgroundMode.Image => ApplicationContext.Instance.Translations["ENUM_WINDOWBACKGROUNDMODE_IMAGE"],
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
