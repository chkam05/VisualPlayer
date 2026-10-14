using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class RadialVisualizationDirectionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            RadialVisualizationDirection.InsideOut => ApplicationContext.Instance.Translations["ENUM_RADIALVISUALIZATIONDIRECTION_INSIDEOUT"],
            RadialVisualizationDirection.OutsideIn => ApplicationContext.Instance.Translations["ENUM_RADIALVISUALIZATIONDIRECTION_OUTSIDEIN"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
