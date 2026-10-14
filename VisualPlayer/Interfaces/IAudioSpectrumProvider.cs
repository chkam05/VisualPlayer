using System;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Interfaces;

public interface IAudioSpectrumProvider
{
    int SpectrumSize { get; }
    int WaveformSize { get; }


    void Configure(VisualizationGlobalConfig config);

    bool TryGetSpectrum(Span<float> destination);
    bool TryGetWaveform(Span<float> destination);
}
