using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Lucide.Avalonia;

namespace VisualPlayer.Converters;

public class PlayingToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true
            ? LucideIconKind.Pause
            : LucideIconKind.Play;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}