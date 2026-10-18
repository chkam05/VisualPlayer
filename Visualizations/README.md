# System visualization projects

All built-in visualization projects live under this directory. Shared SDK, UI and
runtime libraries remain at the solution root. All 16 implementations have their
own DLL/package as of stage E; the application has no concrete visualization types.

## Build and package

Each `VisualPlayer.Visualizations.<Name>` references only SDK/UI and contains its own
implementation, config, editor, English defaults, Polish JSON, manifest and preset.
It has no reference to the executable, runtime or any other concrete visualization.

```powershell
dotnet build Visualizations/VisualPlayer.Visualizations.Peaks -c Release -p:UsedAvaloniaProducts=
```

Outputs under `VisualPlayer.Visualizations.Peaks/bin/Release/net10.0/`:

- `Package/visualplayer.peaks/`: DLL, `.deps.json`, `manifest.json`, `Lang/pl-PL.json`
  and required private dependencies (currently MicroCom.Runtime).
- `visualplayer.peaks-1.0.0.zip`: package contents at the ZIP root, ready for the
  installer implemented in stage F.

`scripts/VisualizationPackage.targets` creates these outputs. It removes stale
generated package files, preserves private dependency subdirectories and excludes
SDK/UI/Avalonia/Lucide replacements. `scripts/BuildSystemVisualizations.targets`
discovers projects under `Visualizations/*/*.csproj`, restores/builds them and copies
their packages into the host's `Plugins/` output and publish directory. There is no
host ProjectReference or assembly reference to a concrete visualization. Normal
solution builds and direct application builds/publishes include all packages automatically.

The manifest/plugin version is currently numeric `1.0.0`, the config version is 1,
and supported API is 1.0 on .NET 10 / Avalonia 12.1.2. Change package manifest and
descriptor together when changing the plugin version. API 1.0 is exercised by all
16 plugins; package versions remain experimental until the external NuGet and
cross-platform validation stages are complete.

## Runtime deployment

Windows/Linux use `<application>/Plugins/visualplayer.peaks/`; the macOS packaging
script moves `Plugins` to `Contents/Resources/Plugins`. DLLs stay external to a
single-file host. User packages use the per-platform paths documented in
`VisualPlayer.Visualizations/README.md` and activate after restart.

Every visualization uses an independent translation scope. The main application's
English or Polish dictionaries are not required by its editor. Missing language JSON falls
back to English compiled into the plugin. Common colors/enum labels come from UI.

Run the headless loader, UI, migration and process-restart checks in
`tests/VisualPlayer.VisualizationChecks`. Their README includes a check against
packages from an actual publish output. macOS/Linux GUI/signing and ZIP installation
remain later-stage validation.

The projects are CircleStripes, CircleWaveform, Oscilloscope, OscilloscopeCenter,
OscilloscopeDouble, OscilloscopeFilled, Peaks, PeaksFloating, PeaksFloatingMirrored,
PeaksMirrored, PeaksBouncing, Stripes, StripesMirrored, StripesBouncing, Waveform and
WaveformMirrored. None depends on another concrete visualization DLL. Common drawing,
bouncing and configuration helpers belong to the SDK; the shared opacity converter
and other editor controls belong to UI.

Default preset ID remains `default`. Its English persisted name and values retain
the pre-migration fixture; localized display names use plugin-local preset keys.
`VisualizationType` is now used only by the legacy migration adapter/tests.
Removing a package leaves its profiles unavailable while preserving their JSON.
The remaining packages load and draw independently; tests exercise removal of each
of the 16 packages in turn.
