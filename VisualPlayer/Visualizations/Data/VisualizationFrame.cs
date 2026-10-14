using System;

namespace VisualPlayer.Visualizations.Data;

public readonly record struct VisualizationFrame(
    TimeSpan Elapsed,
    ReadOnlyMemory<float> Spectrum,
    ReadOnlyMemory<float> Waveform,
    bool IsPlaying);
