using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VisualPlayer.Enums;

namespace VisualPlayer.Converters;

public class VisualizationTypeToTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            VisualizationType.None => "Wyłączona",
            VisualizationType.CircleStripes => "Circle - Stripes",
            VisualizationType.CircleWaveform => "Circle - Waveform",
            VisualizationType.Oscilloscope => "Oscilloscope",
            VisualizationType.OscilloscopeCenter => "Oscilloscope - Center",
            VisualizationType.OscilloscopeDouble => "Oscilloscope - Double",
            VisualizationType.OscilloscopeFilled => "Oscilloscope - Filled",
            VisualizationType.Peaks => "Peaks",
            VisualizationType.PeaksFloating => "Peaks - Floating",
            VisualizationType.PeaksFloatingMirrored => "Peaks - Floating Mirrored",
            VisualizationType.PeaksMirrored => "Peaks - Mirrored",
            VisualizationType.Stripes => "Stripes",
            VisualizationType.StripesMirrored => "Stripes - Mirrored",
            VisualizationType.Waveform => "Waveform",
            VisualizationType.WaveformMirrored => "Waveform - Mirrored",
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
