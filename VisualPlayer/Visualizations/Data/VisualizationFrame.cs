using System;
using VisualPlayer.Enums;

namespace VisualPlayer.Visualizations.Data;

public readonly record struct VisualizationFrame(
    TimeSpan Elapsed,
    ReadOnlyMemory<float> Spectrum,
    ReadOnlyMemory<float> Waveform,
    bool IsPlaying,
    int MinimumFrequency,
    int MaximumFrequency,
    ScalingStrategy ScalingStrategy);
