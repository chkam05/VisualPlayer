# VisualPlayer

[Polski](README_pl-PL.md)

VisualPlayer is a cross-platform desktop music player built with .NET and Avalonia. It combines local audio playback with real-time visualizations, a media library, saved playlists, a ten-band equalizer, synchronized lyrics, configurable keyboard shortcuts, and an interface that can adapt to the selected theme and accent color.

> VisualPlayer is currently under active development. Interfaces, configuration formats, and behavior may change before a stable release.

## Highlights

- Local playback of AAC, AIFF, ASF, FLAC, M4A, MP3, MP4, Ogg, Opus, WAV, and WMA files.
- Queue management, saved `.vpl` playlists, drag and drop ordering, search, sorting, and multi-selection.
- A library organized by tracks, albums, artists, and folders, with optional recursive folder scanning.
- Playback speed control, shuffle, repeat, volume, mute, configurable crossfade, and media key support.
- A ten-band equalizer with preamp control, protected built-in presets, and editable user presets.
- Audio-reactive visualization profiles with solid colors, gradients, rainbow coloring, and gradient animation.
- Plain, line-synchronized, and word-synchronized lyrics with a library, editor, import, export, embedded metadata support, and LRCLIB search.
- An audio-reactive SVG logo with configurable colors, opacity, frequency range, sensitivity, scale, and contraction speed.
- Light, dark, and system themes, accent colors, panel colorization levels, configurable backgrounds, opacity, and auto-hiding interface panels.
- Configurable keyboard shortcuts and macOS system media command integration.

## Visualizations

VisualPlayer currently includes:

- Peaks, floating peaks, mirrored peaks, and mirrored floating peaks.
- Stripes and mirrored stripes.
- Oscilloscope, center, double, and filled variants.
- Waveform and mirrored waveform.
- Circular stripes and circular waveform.

Each visualization profile stores only the configuration appropriate for its visualization type. Built-in profiles are restored when missing or invalid, while user profiles can be created from the current settings.

## Lyrics

Lyrics can be loaded from the local lyrics library, adjacent LRC files, imported LRC or text files, embedded audio metadata, or LRCLIB. The application supports:

- Plain text.
- Line timestamps such as `[00:12.40]`.
- Enhanced LRC word or syllable timestamps such as `<00:13.10>`.
- An editor with precise playback positioning, seek intervals, playback anchors, undo and redo, and unsaved-change handling.
- A playback overlay with separate layouts for timed and plain lyrics.

## Documentation

| Topic | English | Polski |
|---|---|---|
| Build and run | [Build guide](doc/build.md) | [Budowanie](doc/build_pl-PL.md) |
| Project architecture | [Architecture](doc/architecture.md) | [Architektura](doc/architecture_pl-PL.md) |
| Audio and equalizer | [Audio system](doc/audio.md) | [System audio](doc/audio_pl-PL.md) |
| Visualizations | [Visualization system](doc/visualizations.md) | [System wizualizacji](doc/visualizations_pl-PL.md) |
| Lyrics | [Lyrics system](doc/lyrics.md) | [System tekstów](doc/lyrics_pl-PL.md) |
| Library and playlists | [Library and playlists](doc/library-and-playlists.md) | [Biblioteka i playlisty](doc/library-and-playlists_pl-PL.md) |
| Configuration and UI | [Configuration and UI](doc/configuration-and-ui.md) | [Konfiguracja i UI](doc/configuration-and-ui_pl-PL.md) |

## Technology

- .NET 10
- Avalonia 12
- MiniAudioEx
- TagLibSharp
- CommunityToolkit.Mvvm
- Lucide.Avalonia

## Author

Copyright © Kamil Karpiński.
