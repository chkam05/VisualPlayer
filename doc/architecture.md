# Project architecture

[Polski](architecture_pl-PL.md) · [Documentation index](../README.md#documentation)

## Startup and lifetime

`Program` creates the Avalonia application and starts the classic desktop lifetime. `ApplicationContext` is the application-wide composition root. It loads configuration and creates the playlist, library, player, equalizer, lyrics, and visualization services.

`ApplicationContext.Dispose()` saves persistent state and disposes services when the application closes.

## Main directories

| Directory | Responsibility |
|---|---|
| `Assets` | Logo, icons, and bundled background resources. |
| `Attributes` | Property notification metadata. |
| `Commands` | Command implementations used by view models. |
| `Components` | Small reusable UI primitives such as icon buttons and numeric input. |
| `Contexts` | Application state, configuration roots, and feature coordination. |
| `Controls` | Reusable templated controls such as the player, side panel, lyrics, and visualization surface. |
| `Core` | Playback, analysis, library, playlist, lyrics, equalizer, and visualization services. |
| `Enums` | Shared domain and UI enumerations. |
| `Models` | Data-only or processing types grouped by feature. |
| `Providers` | External data providers, currently including LRCLIB. |
| `Resources` | Default profiles, dimensions, colors, and application metadata. |
| `Themes` | Avalonia control themes, templates, and shared view resources. |
| `Utils` | Stateless helpers and platform integrations. |
| `ViewModels` | Bindable configuration and feature state. |
| `Views` | Internal application views and feature editors. |
| `Visualizations` | Visualization implementations, configurations, frame data, and drawing utilities. |
| `Windows` | Top-level windows and interaction coordination. |

## UI composition

`MainWindow` hosts the visualization surface and overlays the side panel, information bar, player, speed, volume, logo, lyrics, and internal view controller. The side panel combines the hamburger menu with the now-playing playlist.

Major feature screens are implemented as internal views:

- Library manager.
- Playlist manager.
- Equalizer.
- Lyrics library and lyrics editor.
- Visualization profiles.
- Settings.

## State and notification model

Bindable classes derive from `ViewModelBase`. Properties use `UpdateProperty`, and the custom `NotifyPropertyUpdate` attribute is used by the project's notification mechanism. Persistent settings are grouped under configuration contexts instead of being stored directly in controls.

## Feature boundaries

- `PlayerCore` owns the MiniAudio engine and current sound.
- `PlayListCore` owns the in-memory now-playing queue.
- `PlayListLibraryCore` stores named playlists.
- `LibraryCore` scans configured folders and produces tracks.
- `AudioAnalysisCore` captures waveform samples and calculates FFT data.
- `VisualizationCore` turns analysis data into visualization frames.
- `LyricsCore` resolves, stores, downloads, and synchronizes lyrics.
- `EqualizerConfigurationContext` and `AudioEqualizerProcessor` manage equalizer profiles and sample processing.

These services communicate through explicit method calls, property notifications, and small application events.
