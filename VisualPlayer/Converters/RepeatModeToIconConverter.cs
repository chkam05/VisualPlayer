using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Lucide.Avalonia;
using VisualPlayer.Enums.Player;

namespace VisualPlayer.Converters;

public class RepeatModeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not RepeatMode repeatMode)
            return LucideIconKind.Repeat;

        switch (repeatMode)
        {
            case RepeatMode.None:
                return LucideIconKind.RepeatOff;
            
            case RepeatMode.All:
                return LucideIconKind.Repeat;
            
            case RepeatMode.One:
                return LucideIconKind.Repeat1;
        }

        return LucideIconKind.RepeatOff;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}