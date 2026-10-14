using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class WaveformTransformationToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            WaveformTransformation.None => ApplicationContext.Instance.Translations["ENUM_BEAMEFFECT_NONE"],
            WaveformTransformation.Sin => ApplicationContext.Instance.Translations["ENUM_WAVEFORMTRANSFORMATION_SIN"],
            WaveformTransformation.Cos => ApplicationContext.Instance.Translations["ENUM_WAVEFORMTRANSFORMATION_COS"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
