using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class VisualizationColorModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            VisualizationColorMode.Solid => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONCOLORMODE_SOLID"],
            VisualizationColorMode.Gradient => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONCOLORMODE_GRADIENT"],
            VisualizationColorMode.Rainbow => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONCOLORMODE_RAINBOW"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
