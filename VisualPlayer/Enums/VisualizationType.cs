namespace VisualPlayer.Enums;

public enum VisualizationType
{
    // Keep existing numeric IDs stable when inserting new visualization types.
    None,
    CircleStripes,
    CircleWaveform,
    Oscilloscope,
    OscilloscopeCenter,
    // Legacy ID retained only for migration of saved profiles.
    OscilloscopeDouble = 5,
    OscilloscopeFilled = 6,
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
    Signal = 17,
    SignalMirrored = 18,
    PsxTriangles = 19,
}
