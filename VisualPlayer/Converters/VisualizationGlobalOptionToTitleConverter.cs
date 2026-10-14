using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class VisualizationGlobalOptionToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            FftSize size => ((int)size).ToString(culture),
            FftWindowType.BlackmanHarris => ApplicationContext.Instance.Translations["ENUM_FFTWINDOWTYPE_BLACKMANHARRIS"],
            FftWindowType.Rectangular => ApplicationContext.Instance.Translations["ENUM_FFTWINDOWTYPE_RECTANGULAR"],
            FftWindowType window => window.ToString(),
            ScalingStrategy.Linear => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_LINEAR"],
            ScalingStrategy.Logarithmic => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_LOGARITHMIC"],
            ScalingStrategy.Mel => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_MEL"],
            ScalingStrategy.Bark => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_BARK"],
            ScalingStrategy.Octave => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_OCTAVE"],
            ScalingStrategy.ThirdOctave => ApplicationContext.Instance.Translations["ENUM_SCALINGSTRATEGY_THIRDOCTAVE"],
            ChannelMode.Mixed => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_MIXED"],
            ChannelMode.Left => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_LEFT"],
            ChannelMode.Right => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_RIGHT"],
            ChannelMode.Stereo => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_STEREO"],
            ChannelMode.Mid => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_MID"],
            ChannelMode.Side => ApplicationContext.Instance.Translations["ENUM_CHANNELMODE_SIDE"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
