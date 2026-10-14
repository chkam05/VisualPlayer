using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;

namespace VisualPlayer.Converters;

public class InfoBarDisplayModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            InfoBarDisplayMode.Manual => "Ręczne",
            InfoBarDisplayMode.PlaybackStarted => "Rozpoczynanie odtwarzania",
            InfoBarDisplayMode.PlaybackResumed => "Wznawianie odtwarzania",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
