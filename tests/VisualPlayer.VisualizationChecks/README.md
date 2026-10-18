# Visualization regression checks (stages A–E)

Run from the repository root:

```powershell
dotnet run --project tests/VisualPlayer.VisualizationChecks -c Release -p:UsedAvaloniaProducts=
```

`UsedAvaloniaProducts` is empty only to skip Avalonia build telemetry in restricted
environments. It does not change application rendering or the plugin API.

The console process returns a nonzero exit code on failure. It uses Avalonia.Headless,
opens no desktop window, accesses no audio device and does not read/write the user's
configuration. It verifies:

- All 16 built-in visualizations can calculate and record drawing operations for
  playback/pause, three render scales and reset.
- The 17 default profiles match a fixture captured before source extraction.
- All 17 legacy string/numeric types migrate to their stable plugin IDs, preserving
  configuration, colors, user names, selection and fill-screen settings.
- Version 2 settings use registry presets and plugin serialization. Migration keeps
  a byte-identical `.pre-plugins.bak`; repeated saves preserve that backup. A failed
  serialization leaves the previous settings file intact.
- Missing/malformed/unsupported plugin configs and unknown JSON fields survive copy,
  overwrite, save/reload and registry removal/restoration. Default reconciliation does
  not replace edited values or alter user parameters.
- A registered SDK-only plugin with an ID absent from the historical enum runs through
  the extracted runtime. Injected audio, frame limits, graphics scaling, reset,
  disposal, live config updates and rendering failures are exercised.
- All 16 concrete editors use SDK contexts, live colors and scoped translations.
  Local enum selections update plugin config; selected labels/open popups change
  language without resetting editor expansion or selected enum values.
- The dynamic type selector keeps the saved ID while a plugin is absent, restores
  selection on re-registration and can switch to another registered factory.
- Contracts, MVVM and shared models have one assembly identity, and the SDK has no
  reverse references to application/runtime/UI.
- A separately built sample plugin references only the SDK, can serialize versioned
  config, create an editor and visualization, and detaches config events on disposal.
- The application-side UI adapter updates appearance/colorization/opacity and shared
  language resources, follows replacement appearance settings and detaches on disposal.

Stage B also adds `tests/VisualPlayer.UiChecks`, whose independent executable does
not reference the application or runtime. Its UI/editor/popup tests are described
in `VisualPlayer.UI/README.md`.

Stage D builds and copies the Peaks package through the application build targets;
the test executable also has no Peaks assembly reference. Checks cover actual
`AssemblyLoadContext` loading, shared SDK/UI/Avalonia identity, a packaged private
dependency, deterministic duplicate selection, built-in fallback, manifest and
constructor/binary failures, plugin-local translations and settings/color editing.
Two child processes verify saving and restoring an external-plugin profile across
an actual process restart. No live audio device or user configuration is accessed.

Stage E loads all 16 system plugins exclusively from packages, with no host or
shared-library reference to any concrete plugin. A separate process removes each
package's manifest in turn from its temporary copy, loads/draws all other 15,
loads/saves/reloads the missing plugin's selected legacy profile and checks that
its raw config survives. Host files and real installed packages are never changed.
All 16 original configs/colors/preset names are compared with the pre-extraction
fixture, and all 16 DLL-local editors are tested in English and Polish.

To repeat those package checks against a publish result:

```powershell
dotnet run --project tests/VisualPlayer.VisualizationChecks --no-build -c Release -- --plugin-published <publish-directory>/Plugins
```

Manual ZIP/package inspection also verified the manifest, `.deps.json`, language
file and absence of SDK/UI/Avalonia replacements. Full self-contained Windows
publish was checked in stage D. Single-file publish could not be restored in the
offline environment because `Microsoft.NET.ILLink.Tasks` was not cached; Linux/macOS
execution and signing remain future validation.

This is not a pixel/screenshot comparison, live audio playback test or validation on
macOS/Linux. Those checks remain in the later migration stages.

Do not regenerate `Fixtures/default-profiles.json` as part of a source move. The
baseline was captured before the stage A extraction; the runner no longer provides
a fixture regeneration command. Intentional default-setting changes need a separate
review of the fixture diff.
