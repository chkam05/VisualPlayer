using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class BeamEffectToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            BeamEffect.None => ApplicationContext.Instance.Translations["ENUM_BEAMEFFECT_NONE"],
            BeamEffect.Beam => ApplicationContext.Instance.Translations["ENUM_BEAMEFFECT_BEAM"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
