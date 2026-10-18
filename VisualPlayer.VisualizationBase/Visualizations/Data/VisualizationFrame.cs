using System;

namespace VisualPlayer.Visualizations.Data;

/// <summary>
/// Audio data for the current UI-thread frame. The host may reuse the underlying
/// buffers after PreCalculate returns; copy their contents to retain a snapshot.
/// </summary>
public readonly record struct VisualizationFrame(
    TimeSpan Elapsed,
    ReadOnlyMemory<float> Spectrum,
    ReadOnlyMemory<float> Waveform,
    bool IsPlaying);
