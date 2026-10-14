using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums.Config;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class InfoBarDisplayModeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            InfoBarDisplayMode.Manual => ApplicationContext.Instance.Translations["ENUM_INFOBARDISPLAYMODE_MANUAL"],
            InfoBarDisplayMode.PlaybackStarted => ApplicationContext.Instance.Translations["ENUM_INFOBARDISPLAYMODE_PLAYBACKSTARTED"],
            InfoBarDisplayMode.PlaybackResumed => ApplicationContext.Instance.Translations["ENUM_INFOBARDISPLAYMODE_PLAYBACKRESUMED"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
