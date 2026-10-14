# Building VisualPlayer

[Polski](build_pl-PL.md) · [Documentation index](../README.md#documentation)

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

VisualPlayer does not currently define a repository-level packaging pipeline. A framework-dependent build can be published with:

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

- Avalonia desktop, Fluent theme, and Inter font.
- CommunityToolkit.Mvvm.
- JAJ.Packages.MiniAudioEx.
- Lucide.Avalonia.
- TagLibSharp.
