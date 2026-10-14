# VisualPlayer

[English](README.md)

VisualPlayer jest wieloplatformowym odtwarzaczem muzyki dla komputerów, zbudowanym przy użyciu .NET i Avalonia. Łączy odtwarzanie lokalnych plików audio z wizualizacjami działającymi w czasie rzeczywistym, biblioteką muzyki, zapisanymi playlistami, dziesięciopasmowym equalizerem, synchronizowanymi tekstami piosenek, konfigurowalnymi skrótami klawiszowymi oraz interfejsem dopasowującym się do wybranego motywu i koloru akcentu.

> VisualPlayer jest obecnie aktywnie rozwijany. Interfejsy, formaty konfiguracji i zachowanie aplikacji mogą zmienić się przed stabilnym wydaniem.

## Najważniejsze możliwości

- Odtwarzanie lokalnych plików AAC, AIFF, ASF, FLAC, M4A, MP3, MP4, Ogg, Opus, WAV i WMA.
- Zarządzanie kolejką, zapisane playlisty `.vpl`, przeciąganie utworów, wyszukiwanie, sortowanie i zaznaczanie wielu pozycji.
- Biblioteka podzielona na utwory, albumy, artystów i katalogi, z opcjonalnym skanowaniem podkatalogów.
- Zmiana szybkości odtwarzania, tryb losowy, powtarzanie, głośność, wyciszenie, konfigurowalne płynne przejścia oraz obsługa klawiszy multimedialnych.
- Dziesięciopasmowy equalizer z preampem, chronionymi presetami domyślnymi i edytowalnymi presetami użytkownika.
- Profile wizualizacji reagujących na dźwięk, z kolorem jednolitym, gradientem, tęczą i animacją gradientu.
- Teksty zwykłe, synchronizowane liniami oraz słowami, wraz z biblioteką, edytorem, importem, eksportem, metadanymi audio i wyszukiwaniem LRCLIB.
- Animowane logo SVG reagujące na dźwięk, z ustawieniami kolorów, przezroczystości, pasma, czułości, skali i szybkości kurczenia.
- Motyw jasny, ciemny i systemowy, kolory akcentu, poziomy koloryzacji paneli, konfigurowalne tło, przezroczystość oraz automatyczne ukrywanie paneli.
- Konfigurowalne skróty klawiszowe i integracja z systemowymi poleceniami multimedialnymi macOS.

## Wizualizacje

VisualPlayer zawiera obecnie:

- Peaks, wariant pływający, lustrzany oraz pływający lustrzany.
- Stripes oraz Stripes Mirrored.
- Oscyloskop w wariancie standardowym, środkowym, podwójnym i wypełnionym.
- Waveform i Waveform Mirrored.
- Circle Stripes i Circle Waveform.

Każdy profil przechowuje wyłącznie konfigurację właściwą dla wybranego rodzaju wizualizacji. Brakujące lub uszkodzone profile systemowe są odtwarzane, a profile użytkownika można tworzyć na podstawie bieżących ustawień.

## Teksty piosenek

Teksty mogą pochodzić z lokalnej biblioteki, sąsiednich plików LRC, importowanych plików LRC lub tekstowych, metadanych audio oraz LRCLIB. Aplikacja obsługuje:

- Zwykły tekst.
- Synchronizację linii, na przykład `[00:12.40]`.
- Rozszerzony LRC z czasem słów lub sylab, na przykład `<00:13.10>`.
- Edytor z precyzyjnym ustawianiem pozycji, interwałami przewijania, punktem wznowienia, cofaniem i ponawianiem oraz wykrywaniem niezapisanych zmian.
- Nakładkę podczas odtwarzania z osobnym układem dla tekstów czasowych i zwykłych.

## Dokumentacja

| Temat | Polski | English |
|---|---|---|
| Budowanie i uruchamianie | [Budowanie](doc/build_pl-PL.md) | [Build guide](doc/build.md) |
| Architektura projektu | [Architektura](doc/architecture_pl-PL.md) | [Architecture](doc/architecture.md) |
| Audio i equalizer | [System audio](doc/audio_pl-PL.md) | [Audio system](doc/audio.md) |
| Wizualizacje | [System wizualizacji](doc/visualizations_pl-PL.md) | [Visualization system](doc/visualizations.md) |
| Teksty | [System tekstów](doc/lyrics_pl-PL.md) | [Lyrics system](doc/lyrics.md) |
| Biblioteka i playlisty | [Biblioteka i playlisty](doc/library-and-playlists_pl-PL.md) | [Library and playlists](doc/library-and-playlists.md) |
| Konfiguracja i UI | [Konfiguracja i UI](doc/configuration-and-ui_pl-PL.md) | [Configuration and UI](doc/configuration-and-ui.md) |

## Technologie

- .NET 10
- Avalonia 12
- MiniAudioEx
- TagLibSharp
- CommunityToolkit.Mvvm
- Lucide.Avalonia

## Autor

Copyright © Kamil Karpiński.
