# Configuration and UI

[Polski](configuration-and-ui_pl-PL.md) · [Documentation index](../README.md#documentation)

## Configuration files

VisualPlayer uses indented JSON with camel-case property names and string enum values. Files are stored below:

```text
Environment.SpecialFolder.ApplicationData/VisualPlayer
```

The exact base directory is platform-dependent. Important entries are:

| Entry | Contents |
|---|---|
| `settings.json` | Appearance, keyboard, library, logo, lyrics, player, and window settings. |
| `visualizations.json` | Global visualization settings and profiles. |
| `equalizer.json` | Equalizer profiles and active profile name. |
| `now_playing.vpl` | Optional persisted now-playing queue. |
| `Backgrounds/` | Background images copied into application storage. |
| `Lyrics/` | Local lyrics library. |
| `Lyrics/Cache/` | Cached online lyrics results. |
| `PlayLists/` | Saved playlist files. |

Malformed JSON causes the corresponding configuration root to fall back to defaults. Built-in visualization and equalizer profiles are validated and restored separately.

## Saving settings

Configuration is saved during application shutdown. Settings and visualization screens also expose explicit save buttons. Reset buttons replace a section with a new default configuration object.

## Appearance

Appearance settings include:

- Light, dark, or system theme.
- Accent color and recent colors.
- Colorization level for selected buttons and main panel surfaces.
- Interface and window opacity controls.
- Solid, image, or transparent window background.
- Auto-hiding player, information bar, and side panel behavior.
- Information-bar display mode and duration.

`AppearanceConfig` calculates brushes for theme, accent, hover, pressed, selected, disabled, and main control surfaces. XAML uses bindings to these brushes so changes are applied without rebuilding controls.

## Themes and templates

`Themes/Generic.axaml` is the central resource dictionary. Resources must be included after their dependencies. The hierarchy is broadly divided into:

- `Themes/Base`: standard Avalonia controls.
- `Themes/Components`: reusable project components.
- `Themes/Controls`: application controls.
- `Themes/Messages`: internal dialogs.
- `Themes/Views`: shared and view-specific resources.

Theme and template keys use a dotted owner convention where appropriate, for example `PlayerControl.IconButtonTheme` and `PlayerControl.SeparatorBorder`.

## Keyboard shortcuts

Keyboard settings use `KeyBindControl` and persist textual key gestures. Defaults include:

- `Ctrl+L`, `Ctrl+I`, `Ctrl+M`, and `Ctrl+P` for playlist and panel behavior.
- System media keys for previous, play/pause, and next.
- `Ctrl+Shift+L/P/E/T/V/C` for library, playlists, equalizer, lyrics, visualizations, and settings.
- `Ctrl+Left` and `Ctrl+Right` for visualization profiles.

System media commands are currently integrated explicitly on macOS through `MacOSMediaCommandService`.

## Main controls

- `SidePanelControl` combines navigation and the now-playing playlist.
- `InfoBarControl` displays track metadata and playback time.
- `PlayerControl` owns transport controls and the position slider.
- `SpeedControl` and `VolumeControl` are transient panels anchored to player buttons.
- `LyricsControl` displays the active lyrics.
- `LogoControl` renders and animates the SVG logo.
- `VisualizationSurfaceControl` draws the active visualization behind the interface.
- `InternalViewController` and `InternalMessageController` host application views and dialogs.
