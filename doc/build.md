# Building VisualPlayer

[Polski](build_pl-PL.md) · [Documentation index](../README.md#technical-documentation)

## Requirements

- A supported Windows, macOS, or Linux desktop environment.
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or a newer SDK capable of targeting `net10.0`.
- Native build tools are not normally required because MiniAudioEx provides runtime binaries through its NuGet package.

## Restore and build

Run these commands from the repository root:

```bash
dotnet restore VisualPlayer.slnx
dotnet build VisualPlayer/VisualPlayer.csproj --configuration Debug
```

For a release build:

```bash
dotnet build VisualPlayer/VisualPlayer.csproj --configuration Release
```

Build output is written below `VisualPlayer/bin/<Configuration>/net10.0/`.

## Run from source

```bash
dotnet run --project VisualPlayer/VisualPlayer.csproj
```

Audio file paths can also be passed as command-line arguments. Startup behavior depends on the player settings.

## Publish

To publish a self-contained Release build, run the script for your target platform from the repository root:

```text
Windows: build_windown.bat
macOS:   bash build_macos.sh
Linux:   bash build_linux.sh
```

The scripts detect the current architecture. You can specify a target explicitly, for example `build_windown.bat win-x64`, `bash build_macos.sh osx-arm64`, or `bash build_linux.sh linux-x64`. Each script also supports `--help`.

The output is written to `artifacts/publish/<runtime>/`. Windows and Linux produce portable folders: run `VisualPlayer.exe` or `./VisualPlayer` and distribute the **entire folder**, including native libraries and translations. The target computer does not need .NET installed; native desktop/audio dependencies are still required.

Fonts under `VisualPlayer/Assets/Fonts` are compiled as Avalonia resources, so target systems do not need Oxanium, LED Sled, or LED Counter 7 installed. Keep the bundled license/readme files with redistributed builds and review their terms separately from the project's MIT license, especially for commercial distribution.

On macOS, run the script on a Mac. It creates `artifacts/publish/<runtime>/VisualPlayer.app`, ready to open in Finder or copy to Applications. The bundle contains the runtime, native libraries, translations, a standard/Retina `.icns` icon generated from `Assets/logo.png`, and `Info.plist` metadata read from the project. The title, product name, version, authors, company and copyright stay synchronized with the `.csproj`. Bundle identity and minimum macOS version are configured with `MacOSBundleIdentifier` and `MacOSMinimumSystemVersion` in that file.

The standard macOS release version uses the first three components of `Version` (e.g. `7.1.1`); the full informational version (e.g. `7.1.1.0`) is preserved in the bundle information. By default the bundle build version matches the release version; override it with `MACOS_BUNDLE_VERSION=123 bash build_macos.sh` if needed. The script validates the property list and applies/verifies local ad-hoc code signatures. Developer ID signing and Apple notarization are separate distribution steps.

A framework-dependent build can also be published manually:

```bash
dotnet publish VisualPlayer/VisualPlayer.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained false
```

Replace the runtime identifier with the target platform, for example `win-x64`, `linux-x64`, `osx-x64`, or `osx-arm64`. Use `--self-contained true` when the target computer should not require a separately installed .NET runtime.

## Debugging Avalonia

Debug builds enable Avalonia developer tools through `Program.BuildAvaloniaApp()`. The project also references `AvaloniaUI.DiagnosticsSupport`, excluded from non-debug output.

If an Avalonia XAML error occurs, inspect the first file and line reported by `dotnet build`. Runtime `StaticResource` failures can still occur when a resource exists at compile time but is loaded before one of its dependencies; resource order is defined in `Themes/Generic.axaml`.

## Main dependencies

Versions are defined in `VisualPlayer/VisualPlayer.csproj`:

- Avalonia desktop, Fluent theme, and Inter fallback font.
- CommunityToolkit.Mvvm.
- JAJ.Packages.MiniAudioEx.
- Lucide.Avalonia.
- TagLibSharp.

The interface also embeds Oxanium, LED Sled, and LED Counter 7 as application assets; they are not NuGet dependencies.
