# Audio system

[Polski](audio_pl-PL.md) · [Documentation index](../README.md#technical-documentation)

## Playback pipeline

`PlayerCore` owns a MiniAudioEx `MaEngine`. Tracks are opened as decoded streams and represented by `MaSound`. The player exposes playback state through `PlayerDataContext` and supports play, pause, stop, seek, next, previous, shuffle, repeat, volume, mute, and pitch-based playback speed.

The engine uses a managed data callback:

1. MiniAudio renders PCM frames from the engine.
2. `AudioEqualizerProcessor` modifies samples in place when the active profile is not flat.
3. `AudioAnalysisCore` copies a mono representation into its lock-free circular sample buffer.
4. The processed frames are returned to the audio device.

The device period is configured to 2048 frames to tolerate short UI or garbage collection pauses. Work inside the callback must remain allocation-free and bounded.

## Crossfade

When crossfade is enabled, the next `MaSound` starts at zero volume while the previous sound remains active. A stopwatch-driven update gradually exchanges their volumes over the configured one-to-five-second duration. The old sound is stopped and disposed when the transition finishes.

The now-playing position timer updates crossfade progress every 100 ms.

## Audio analysis

`AudioAnalysisCore` stores recent mono samples in a fixed 16,384-sample circular buffer. Visualization reads never own the audio output buffer.

The analysis system exposes:

- A 1024-sample waveform.
- FFT magnitudes sized according to the configured FFT size.
- Hann, Hamming, Blackman, Blackman-Harris, and rectangular windows.
- Left, right, mid, side, stereo, and mixed channel modes.

`VisualizationCore` maps FFT bins into the configured frequency range, converts magnitudes to normalized decibel values, applies attack and release smoothing, and supplies immutable frame views to the active visualization.

## Equalizer

The equalizer contains a preamp and ten peak filters centered at:

`60 Hz`, `100 Hz`, `200 Hz`, `500 Hz`, `1 kHz`, `1.8 kHz`, `3.4 kHz`, `5.7 kHz`, `9 kHz`, and `13 kHz`.

Each band ranges from -10 dB to +10 dB. Filter coefficients are rebuilt when the selected profile, band gain, frequency, or preamp changes. Processing state is kept separately for each output channel and band.

Built-in presets are regenerated when missing or incompatible. User presets can be cloned and edited. Equalizer state is stored in `equalizer.json`.

## Performance rules

- Do not allocate objects, access files, update controls, or perform network work inside `EngineDataCallback`.
- Rendering may drop frames; audio output must never wait for visualization rendering.
- Keep analysis buffers fixed-size and reuse FFT arrays.
- Dispose replaced `MaSound` instances and unsubscribe configuration events.
- A failed engine read must produce silence instead of returning an uninitialized output buffer.
