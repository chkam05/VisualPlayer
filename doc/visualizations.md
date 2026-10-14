# Visualization system

[Polski](visualizations_pl-PL.md) · [Documentation index](../README.md#documentation)

## Data flow

`VisualizationSurfaceControl` requests frames with an Avalonia dispatcher timer. It calculates the drawable area after accounting for the information bar, side panel, player, and the global fill-screen option.

`VisualizationCore` then:

1. Reads spectrum and waveform data from `AudioAnalysisCore`.
2. Maps FFT data to the configured frequency scale.
3. Applies decibel normalization plus attack and release smoothing.
4. Creates a `VisualizationFrame` containing elapsed time, spectrum, waveform, and playback state.
5. Calls `PreCalculate` and `Draw` on the selected implementation.

Only one visualization instance is active. It is disposed when the profile or visualization type changes.

## Global settings

Global visualization configuration controls:

- FFT size and window.
- Channel mode.
- Linear, logarithmic, Mel, or Bark frequency scaling.
- Minimum and maximum frequency.
- Minimum and maximum decibel range.
- Attack and release time.
- Frame-rate limit.
- Whether visualizations may use the whole window.

## Profiles

Each `VisualisationProfile` contains:

- A unique name.
- A `VisualizationType` discriminator.
- One configuration object appropriate for that type.
- An optional shared color configuration.
- An `IsDefault` flag protecting built-in profiles in the UI.

Serialization is handled by `VisualisationProfileJsonConverter`. Built-in profiles are defined in `Resources/DefaultVisualizationProfiles.cs`. On load, missing profiles are restored and incompatible built-in entries are replaced.

## Color system

`VisualizationColorConfig` supports:

- Solid color.
- User-defined linear or radial gradients.
- Linear or radial rainbow generation.
- Horizontal and vertical animation speed.
- Gradient stretch.

`VisualizationColorSampler` samples the selected color at normalized drawing coordinates. Animated offsets are accumulated by visualization implementations and wrapped so the end of a repeating gradient joins its beginning.

## Implementations

Current visualization families are:

- Peaks: standard, mirrored, floating, and floating mirrored.
- Stripes: standard and mirrored.
- Oscilloscope: standard, center, double, and filled.
- Waveform: standard and mirrored.
- Circle: stripes and waveform.

Every visualization has an independent configuration class and settings view. Shared algorithms belong in `Visualizations/Utils` only when multiple implementations use them.

## Rendering and performance

Visualization rendering runs on the UI thread. `VisualizationSurfaceControl` does not enqueue another invalidation while one is pending. Oscilloscopes are capped at 30 FPS and limit the number of drawn points or filled columns. Solid oscilloscope colors reuse a pen for the complete trace.

Audio playback has priority over smooth animation. New visualization implementations should reuse arrays and drawing objects where practical, avoid per-pixel allocations, bound geometry complexity, and tolerate dropped frames.
