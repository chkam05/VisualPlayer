using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class RotationDirectionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            RotationDirection.Clockwise => ApplicationContext.Instance.Translations["ENUM_ROTATIONDIRECTION_CLOCKWISE"],
            RotationDirection.CounterClockwise => ApplicationContext.Instance.Translations["ENUM_ROTATIONDIRECTION_COUNTERCLOCKWISE"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
