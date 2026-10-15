using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;
using VisualPlayer.Contexts;

namespace VisualPlayer.Converters;

public class VisualizationTypeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            VisualizationType.None => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_NONE"],
            VisualizationType.CircleStripes => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_CIRCLESTRIPES"],
            VisualizationType.CircleWaveform => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_CIRCLEWAVEFORM"],
            VisualizationType.Oscilloscope => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPE"],
            VisualizationType.OscilloscopeCenter => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPECENTER"],
            VisualizationType.OscilloscopeDouble => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPEDOUBLE"],
            VisualizationType.OscilloscopeFilled => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_OSCILLOSCOPEFILLED"],
            VisualizationType.Peaks => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_PEAKS"],
            VisualizationType.PeaksBouncing => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_PEAKSBOUNCING"],
            VisualizationType.PeaksFloating => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_PEAKSFLOATING"],
            VisualizationType.PeaksFloatingMirrored => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_PEAKSFLOATINGMIRRORED"],
            VisualizationType.PeaksMirrored => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_PEAKSMIRRORED"],
            VisualizationType.Stripes => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_STRIPES"],
            VisualizationType.StripesBouncing => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_STRIPESBOUNCING"],
            VisualizationType.StripesMirrored => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_STRIPESMIRRORED"],
            VisualizationType.Waveform => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_WAVEFORM"],
            VisualizationType.WaveformMirrored => ApplicationContext.Instance.Translations["ENUM_VISUALIZATIONTYPE_WAVEFORMMIRRORED"],
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
