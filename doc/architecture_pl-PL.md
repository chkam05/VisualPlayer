# Architektura projektu

[English](architecture.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja)

## Start i cykl życia

`Program` tworzy aplikację Avalonia i uruchamia klasyczny cykl życia aplikacji desktopowej. `ApplicationContext` jest głównym miejscem składania zależności. Ładuje konfigurację oraz tworzy usługi playlist, biblioteki, odtwarzacza, equalizera, tekstów i wizualizacji.

`ApplicationContext.Dispose()` zapisuje stan trwały i zwalnia usługi podczas zamykania aplikacji.

## Główne katalogi

| Katalog | Odpowiedzialność |
|---|---|
| `Assets` | Logo, ikony i dołączone zasoby tła. |
| `Attributes` | Metadane powiadomień właściwości. |
| `Commands` | Implementacje poleceń używane przez ViewModele. |
| `Components` | Małe elementy UI, między innymi przyciski z ikoną i pola numeryczne. |
| `Contexts` | Stan aplikacji, główne konfiguracje i koordynacja funkcji. |
| `Controls` | Kontrolki szablonowe, między innymi odtwarzacz, panel boczny, tekst i powierzchnia wizualizacji. |
| `Core` | Odtwarzanie, analiza, biblioteka, playlisty, teksty, equalizer i wizualizacje. |
| `Enums` | Wspólne enumeracje domeny i interfejsu. |
| `Models` | Typy danych i przetwarzania pogrupowane według funkcji. |
| `Providers` | Dostawcy danych zewnętrznych, obecnie między innymi LRCLIB. |
| `Resources` | Profile domyślne, wymiary, kolory i informacje o aplikacji. |
| `Themes` | Motywy, szablony kontrolek i współdzielone zasoby widoków Avalonia. |
| `Utils` | Bezstanowe narzędzia oraz integracje platformowe. |
| `ViewModels` | Konfiguracja i stan funkcji dostępny dla bindingów. |
| `Views` | Wewnętrzne widoki aplikacji i edytory. |
| `Visualizations` | Implementacje i konfiguracje wizualizacji, dane klatki oraz narzędzia rysujące. |
| `Windows` | Okna najwyższego poziomu i koordynacja interakcji. |

## Budowa interfejsu

`MainWindow` zawiera powierzchnię wizualizacji, a nad nią panel boczny, pasek informacji, odtwarzacz, kontrolki szybkości i głośności, logo, tekst piosenki oraz kontroler widoków wewnętrznych. Panel boczny łączy menu hamburgerowe z playlistą „Teraz odtwarzane”.

Główne funkcje mają osobne widoki wewnętrzne:

- Menedżer biblioteki.
- Menedżer playlist.
- Equalizer.
- Biblioteka i edytor tekstów.
- Profile wizualizacji.
- Ustawienia.

## Stan i powiadomienia

Klasy dostępne dla bindingów dziedziczą po `ViewModelBase`. Właściwości używają `UpdateProperty`, a własny atrybut `NotifyPropertyUpdate` wspiera mechanizm powiadomień projektu. Ustawienia trwałe są pogrupowane w kontekstach konfiguracji zamiast znajdować się bezpośrednio w kontrolkach.

## Granice modułów

- `PlayerCore` zarządza silnikiem MiniAudio i bieżącym dźwiękiem.
- `PlayListCore` zarządza kolejką „Teraz odtwarzane” w pamięci.
- `PlayListLibraryCore` zapisuje nazwane playlisty.
- `LibraryCore` skanuje skonfigurowane katalogi i tworzy listę utworów.
- `AudioAnalysisCore` przechwytuje próbki waveform i oblicza FFT.
- `VisualizationCore` tworzy klatki wizualizacji z danych analizy.
- `LyricsCore` wyszukuje, zapisuje, pobiera i synchronizuje teksty.
- `EqualizerConfigurationContext` oraz `AudioEqualizerProcessor` zarządzają profilami i przetwarzaniem equalizera.

Usługi komunikują się przez jawne wywołania metod, powiadomienia właściwości oraz niewielkie zdarzenia aplikacyjne.
