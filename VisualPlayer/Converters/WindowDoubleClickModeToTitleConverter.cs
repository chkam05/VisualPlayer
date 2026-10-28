using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class WindowDoubleClickModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            WindowDoubleClickMode.Maximize => ApplicationContext.Instance.Translations["ENUM_WINDOWDOUBLECLICKMODE_MAXIMIZE"],
            WindowDoubleClickMode.FullScreen => ApplicationContext.Instance.Translations["ENUM_WINDOWDOUBLECLICKMODE_FULLSCREEN"],
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
