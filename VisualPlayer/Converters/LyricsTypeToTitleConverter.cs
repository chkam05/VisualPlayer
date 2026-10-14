using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class LyricsTypeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            LyricsType.Plain => "Zwykły tekst",
            LyricsType.LineSynced => "Synchronizacja liniami",
            LyricsType.WordSynced => "Synchronizacja słowami",
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
