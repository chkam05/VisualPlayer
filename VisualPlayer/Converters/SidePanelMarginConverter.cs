using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using VisualPlayer.Resources;

namespace VisualPlayer.Converters;

public class SidePanelMarginConverter : IValueConverter
{
    public static SidePanelMarginConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var bottom = value is true
            ? -(Dimensions.AutoHideRevealSize - 8)
            : -Dimensions.AutoHideRevealSize;

        return new Thickness(0, 8, 0, bottom);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
