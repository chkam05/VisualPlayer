using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class LyricsTypeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            LyricsType.Plain => ApplicationContext.Instance.Translations["ENUM_LYRICSTYPE_PLAIN"],
            LyricsType.LineSynced => ApplicationContext.Instance.Translations["ENUM_LYRICSTYPE_LINESYNCED"],
            LyricsType.WordSynced => ApplicationContext.Instance.Translations["ENUM_LYRICSTYPE_WORDSYNCED"],
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
