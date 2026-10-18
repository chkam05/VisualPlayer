# Visualization runtime — stages C–E

This .NET 10 library references the SDK only. It contains `VisualizationCore`, the
injected audio provider contract, global analysis settings, the registry, profiles
and persistence. Namespaces are preserved where existing application code uses them;
these types now belong to `VisualPlayer.Visualizations.dll`.

## Composition and lifecycle

Create `VisualizationRegistry`, register plugins with their scoped
`IVisualizationHostServices`, and load `VisualizationConfigurationContext` with an
explicit path. Construct `VisualizationCore` with those settings, an
`IAudioSpectrumProvider` and the playback-state function. The runtime neither opens
an audio device nor obtains an application singleton or application-data path.

The registry always contains `visualplayer.none`. Registration validates IDs,
duplicate IDs, API compatibility and preset metadata; preset JSON/colors are copied.
Operate the registry/settings/editors on the UI thread. All 16 implementations are
loaded only from external packages built under `Visualizations/`. The transitional
`BuiltInVisualizations` and application default-profile factory have been removed.

Factories create visualizations from the live profile config/color and scoped host
services. Profile changes dispose the previous instance. Graphics, analysis,
frame-rate limits and resets are retained. A drawing/factory/reset/graphics failure
is reported through the registered diagnostics service and disables that instance.
It is retried after the active profile/configuration changes. Dispose the runtime,
settings context, plugin loader, then registry when their host closes.

Editors receive `VisualizationEditorContext` from the SDK, never the runtime profile.
The context includes descriptor capabilities, including floating-bar gravity.
Registration entries notify localized name changes; preset `DisplayName` is localized
without changing the stored name or identity of a user profile. A selector can bind
two-way to profile `Registration`; null selection retains the saved `PluginId`.

## Persistence and migration

`VisualizationConfigurationContext.Load(path, registry)` reads the old settings file
and format version 2. `VisualizationProfileSerializer` handles individual profiles.
All historical numeric/string enum values map through `LegacyVisualizationIds` to
the IDs documented in `separate_visualizations.md`. Unknown types retain their raw
legacy value and remain unavailable. The enum exists only for compatibility with
legacy migration adapter; profiles and plugins no longer expose or use this enum.

Version 2 stores `pluginId`, `configVersion`, `profileId` and optional `presetId`.
Selection uses `selectedProfileId`; the old selected name is still written for
diagnostic/compatibility purposes. Defaults use stable plugin/preset identities;
reconciliation adds missing defaults without replacing existing profiles.

The plugin reads its own versioned config. Missing plugins, unsupported versions and
invalid config leave the original JSON available for save/copy/overwrite. Re-registering
the plugin retries reading. Unknown root, config, global-setting and color fields
are retained. Explicitly selecting a different type creates fresh plugin config.

The first save of a legacy file creates `<filename>.pre-plugins.bak` if absent, then
writes a flushed temporary file in the same directory and replaces the destination.
Serialization completes before any file is touched. Subsequent saves do not overwrite
the backup. Invalid whole-file JSON or a newer file format is rejected rather than
replaced with defaults. Restoring the backup also requires a compatible application;
the new profile schema is not intended to be edited by an older executable.

## Validation

Run `tests/VisualPlayer.VisualizationChecks` as described in its README, followed by
the independent `tests/VisualPlayer.UiChecks` and `scripts/Test-Translations.ps1`.
These are headless Windows checks, not live playback or macOS/Linux release tests.

## Startup package loader

Construct `VisualizationPluginLoader` with the registry, global `TranslationService`,
common UI scope, and host-owned shared assemblies (including the UI assembly). Call
`Load(roots)` before reading profiles. `Discover(roots)` reads metadata without
executing plugin code; a loader accepts only one Load call. `LoadedPackages` and
`Diagnostics` expose the result. Discovery never writes to scanned directories.

`VisualizationPluginPaths.ForCurrentHost()` returns built-in/user roots:

| System | Built-in | User |
| --- | --- | --- |
| Windows | `<base>/Plugins` | `%LOCALAPPDATA%/VisualPlayer/Plugins` |
| Linux | `<base>/Plugins` | `$XDG_DATA_HOME/VisualPlayer/Plugins`, otherwise `~/.local/share/VisualPlayer/Plugins` |
| macOS app | `Contents/Resources/Plugins` | `~/Library/Application Support/VisualPlayer/Plugins` |

Each immediate package directory needs `manifest.json`; dependent DLLs are not
discovered as plugins. Manifest schema 1 rejects missing/unknown/duplicate fields,
unsupported API/TFM/Avalonia, invalid IDs and paths, links, missing DLL or `.deps.json`.
For now .NET `net10.0`, Avalonia `12.1.2` and English `en-US` defaults are explicit
compatibility requirements; matching numeric major API and sufficient host minor
are also required. A plugin descriptor must agree with its manifest.

Candidates are ranked before activation: user over built-in, highest numeric plugin
version, then ordinal full path. Exactly one successful candidate is registered per
ID. Invalid metadata or activation errors produce diagnostics and allow the next
candidate, including a built-in fallback. IDs already registered by the host are
retained with diagnostics; normal system visualizations all use this loader now.

Each attempted package gets its own non-collectible `AssemblyLoadContext` and
`AssemblyDependencyResolver`. SDK/UI/Avalonia/Lucide resolve from the host with exact
assembly-version checks; packaged private managed/native dependencies resolve under
the package. A declared application/runtime reference is rejected when resolved.
Contexts remain for the process lifetime; Dispose detaches registrations/catalogs,
not code. DLL code has host privileges: hangs/native crashes are not isolated.

The plugin supplies its own English dictionary; the loader registers its local
scope and `Lang` files with the global language service. Errors reading language
files retain English fallback. Settings/rendering errors use the same diagnostics
collection. Installation, disabled-ID persistence, pending updates and a management
screen belong to stage F; use a restart after changing packages.
