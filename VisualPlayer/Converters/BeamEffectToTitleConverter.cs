using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class BeamEffectToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            BeamEffect.None => "Brak",
            BeamEffect.Beam => "Promień",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
