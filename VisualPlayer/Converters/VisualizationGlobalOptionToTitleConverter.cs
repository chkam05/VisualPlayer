using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class VisualizationGlobalOptionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            FftSize size => ((int)size).ToString(culture),
            FftWindowType.BlackmanHarris => "Blackman-Harris",
            FftWindowType.Rectangular => "Prostokątne",
            FftWindowType window => window.ToString(),
            ScalingStrategy.Linear => "Liniowe",
            ScalingStrategy.Logarithmic => "Logarytmiczne",
            ScalingStrategy.Mel => "Mel",
            ScalingStrategy.Bark => "Bark",
            ScalingStrategy.Octave => "Oktawowe",
            ScalingStrategy.ThirdOctave => "Tercjowe",
            ChannelMode.Mixed => "Miks kanałów",
            ChannelMode.Left => "Lewy",
            ChannelMode.Right => "Prawy",
            ChannelMode.Stereo => "Stereo",
            ChannelMode.Mid => "Mid",
            ChannelMode.Side => "Side",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
