using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class MirroredVisualizationDirectionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            MirroredVisualizationDirection.Inside => ApplicationContext.Instance.Translations["ENUM_MIRROREDVISUALIZATIONDIRECTION_INSIDE"],
            MirroredVisualizationDirection.Outside => ApplicationContext.Instance.Translations["ENUM_MIRROREDVISUALIZATIONDIRECTION_OUTSIDE"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
