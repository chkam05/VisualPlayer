using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using VisualPlayer.Resources;

namespace VisualPlayer.Converters;

public class LyricsControlMarginConverter : IMultiValueConverter
{
    public static LyricsControlMarginConverter Instance { get; } = new();

    public object Convert(
        IList<object?> values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (values.Count < 3
            || values[0] is not double left
            || values[1] is not double playerHeight
            || values[2] is not bool isPlayerHidden)
        {
            return AvaloniaProperty.UnsetValue;
        }

        var bottom = isPlayerHidden
            ? 8
            : Math.Max(0, playerHeight - Dimensions.AutoHideRevealSize);
        return new Thickness(left, 0, 0, bottom);
    }
}
