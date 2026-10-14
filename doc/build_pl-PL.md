# Budowanie VisualPlayer

[English](build.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja)

## Wymagania

- Obsługiwane środowisko graficzne Windows, macOS lub Linux.
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) albo nowszy SDK obsługujący `net10.0`.
- Natywne narzędzia kompilacji zwykle nie są potrzebne, ponieważ pakiet NuGet MiniAudioEx dostarcza biblioteki dla obsługiwanych platform.

## Przywracanie pakietów i kompilacja

W katalogu głównym repozytorium wykonaj:

```bash
dotnet restore VisualPlayer.slnx
dotnet build VisualPlayer/VisualPlayer.csproj --configuration Debug
```

Kompilacja wydania:

```bash
dotnet build VisualPlayer/VisualPlayer.csproj --configuration Release
```

Wynik znajduje się w `VisualPlayer/bin/<Konfiguracja>/net10.0/`.

## Uruchamianie ze źródeł

```bash
dotnet run --project VisualPlayer/VisualPlayer.csproj
```

Ścieżki plików audio można przekazać również jako argumenty programu. Zachowanie po starcie zależy od ustawień odtwarzacza.

## Publikowanie

Repozytorium nie posiada obecnie osobnego procesu tworzenia instalatorów. Wydanie zależne od zainstalowanego środowiska .NET można utworzyć poleceniem:

```bash
dotnet publish VisualPlayer/VisualPlayer.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained false
```

Identyfikator środowiska można zmienić między innymi na `win-x64`, `linux-x64`, `osx-x64` lub `osx-arm64`. Użyj `--self-contained true`, jeżeli komputer docelowy nie powinien wymagać osobno zainstalowanego .NET.

## Diagnostyka Avalonia

Kompilacje Debug włączają narzędzia deweloperskie Avalonia w `Program.BuildAvaloniaApp()`. Pakiet `AvaloniaUI.DiagnosticsSupport` jest wyłączany dla kompilacji innych niż Debug.

Przy błędzie XAML należy zacząć od pierwszego pliku i numeru linii zgłoszonego przez `dotnet build`. Błąd `StaticResource` może wystąpić dopiero podczas uruchamiania, jeżeli zasób jest ładowany przed zależnością. Kolejność zasobów definiuje `Themes/Generic.axaml`.

## Główne zależności

Wersje są zapisane w `VisualPlayer/VisualPlayer.csproj`:

- Avalonia Desktop, Fluent Theme i czcionka Inter.
- CommunityToolkit.Mvvm.
- JAJ.Packages.MiniAudioEx.
- Lucide.Avalonia.
- TagLibSharp.
