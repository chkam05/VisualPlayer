using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class VisualizationDirectionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            VisualizationDirection.BottomUp => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONDIRECTION_BOTTOMUP"],
            VisualizationDirection.TopDown => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONDIRECTION_TOPDOWN"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
