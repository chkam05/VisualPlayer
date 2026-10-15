namespace VisualPlayer.Enums;

public enum VisualizationType
{
    // Keep existing numeric IDs stable when inserting new visualization types.
    None,
    CircleStripes,
    CircleWaveform,
    Oscilloscope,
    OscilloscopeCenter,
    OscilloscopeDouble,
    OscilloscopeFilled,
    Peaks,
    PeaksBouncing = 15,
    PeaksFloating = 8,
    PeaksFloatingMirrored,
    PeaksMirrored,
    Stripes,
    StripesBouncing = 16,
    StripesMirrored = 12,
    Waveform,
    WaveformMirrored,
}
