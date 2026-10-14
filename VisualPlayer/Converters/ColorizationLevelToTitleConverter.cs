using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class ColorizationLevelToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ColorizationLevel level)
            return string.Empty;

        return level switch
        {
            ColorizationLevel.Poorly => ApplicationContext.Instance.Translations["ENUM_COLORIZATIONLEVEL_POORLY"],
            ColorizationLevel.Buttons => ApplicationContext.Instance.Translations["ENUM_COLORIZATIONLEVEL_BUTTONS"],
            ColorizationLevel.Whole => ApplicationContext.Instance.Translations["ENUM_COLORIZATIONLEVEL_WHOLE"],
            _ => level.ToString()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
