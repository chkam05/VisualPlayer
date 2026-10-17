# Budowanie VisualPlayer

[English](build.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

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

Gotowe wydanie Release z dołączonym środowiskiem .NET utworzysz skryptem uruchomionym z katalogu głównego repozytorium:

```text
Windows: build_windown.bat
macOS:   bash build_macos.sh
Linux:   bash build_linux.sh
```

Skrypty wykrywają bieżącą architekturę. Możesz też podać ją jawnie, np. `build_windown.bat win-x64`, `bash build_macos.sh osx-arm64` albo `bash build_linux.sh linux-x64`. Każdy skrypt obsługuje także `--help`.

Wynik znajdziesz w `artifacts/publish/<runtime>/`. Na Windows i Linux powstaje przenośny katalog: uruchom w nim `VisualPlayer.exe` albo `./VisualPlayer`. Do przeniesienia aplikacji skopiuj **cały katalog**, wraz z bibliotekami natywnymi i tłumaczeniami. Komputer docelowy nie wymaga zainstalowanego .NET; nadal potrzebuje natywnych zależności środowiska graficznego i audio.

Fonty z `VisualPlayer/Assets/Fonts` są kompilowane jako zasoby Avalonia, dlatego system docelowy nie musi mieć zainstalowanych fontów Oxanium, LED Sled ani LED Counter 7. Podczas redystrybucji należy zachować dołączone pliki licencyjne/readme i rozpatrywać ich warunki niezależnie od licencji MIT projektu, szczególnie przy dystrybucji komercyjnej.

Skrypt macOS uruchom na Macu. Utworzy `artifacts/publish/<runtime>/VisualPlayer.app`, który można otworzyć w Finderze lub skopiować do katalogu Aplikacje. Pakiet zawiera runtime, biblioteki natywne, tłumaczenia, ikonę `.icns` w rozmiarach standardowych i Retina generowaną z `Assets/logo.png` oraz metadane `Info.plist` odczytane z projektu. Tytuł, nazwa produktu, wersja, autorzy, firma i prawa autorskie pozostają zgodne z `.csproj`. Identyfikator pakietu i minimalną wersję macOS ustawisz w tym pliku przez `MacOSBundleIdentifier` i `MacOSMinimumSystemVersion`.

Standardowa wersja wydania macOS korzysta z pierwszych trzech części `Version` (np. `7.1.1`); pełna wersja informacyjna (np. `7.1.1.0`) również jest zapisana w pakiecie. Domyślnie numer kompilacji pakietu odpowiada wersji wydania; można go nadpisać przez `MACOS_BUNDLE_VERSION=123 bash build_macos.sh`. Skrypt sprawdza poprawność listy właściwości oraz tworzy i weryfikuje lokalne podpisy ad hoc. Podpis Developer ID i notaryzacja Apple są osobnymi etapami dystrybucji.

Wydanie zależne od zainstalowanego środowiska .NET można również utworzyć ręcznie:

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

- Avalonia Desktop, Fluent Theme i zapasowy font Inter.
- CommunityToolkit.Mvvm.
- JAJ.Packages.MiniAudioEx.
- Lucide.Avalonia.
- TagLibSharp.

Interfejs osadza również fonty Oxanium, LED Sled i LED Counter 7 jako zasoby aplikacji; nie są one zależnościami NuGet.
