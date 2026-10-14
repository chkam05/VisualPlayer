using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class ApplicationThemeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ApplicationTheme.System => ApplicationContext.Instance.Translations["ENUM_APPLICATIONTHEME_SYSTEM"],
            ApplicationTheme.Light => ApplicationContext.Instance.Translations["ENUM_APPLICATIONTHEME_LIGHT"],
            ApplicationTheme.Dark => ApplicationContext.Instance.Translations["ENUM_APPLICATIONTHEME_DARK"],
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
