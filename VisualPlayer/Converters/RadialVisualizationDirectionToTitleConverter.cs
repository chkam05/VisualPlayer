using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class RadialVisualizationDirectionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            RadialVisualizationDirection.InsideOut => "Od środka na zewnątrz",
            RadialVisualizationDirection.OutsideIn => "Od zewnątrz do środka",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
