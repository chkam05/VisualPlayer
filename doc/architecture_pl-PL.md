# Architektura projektu

[English](architecture.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

## Start i cykl życia

`Program` tworzy aplikację Avalonia i uruchamia klasyczny cykl życia aplikacji desktopowej. `ApplicationContext` jest głównym miejscem składania zależności. Ładuje konfigurację oraz tworzy usługi playlist, biblioteki, odtwarzacza, equalizera, tekstów i wizualizacji.

`ApplicationContext.Dispose()` zapisuje stan trwały i zwalnia usługi podczas zamykania aplikacji.

## Główne katalogi

| Katalog | Odpowiedzialność |
|---|---|
| `Assets` | Logo, ikony i dołączone zasoby tła. |
| `Attributes` | Metadane powiadomień właściwości. |
| `Commands` | Implementacje poleceń używane przez ViewModele. |
| `Components` | Współdzielone elementy UI, między innymi przyciski z ikoną, pola numeryczne i szablonowe kontrolki ustawień. |
| `Contexts` | Stan aplikacji, główne konfiguracje i koordynacja funkcji. |
| `Controls` | Kontrolki `UserControl`, między innymi odtwarzacz, panel boczny, tekst i powierzchnia wizualizacji; kontrolery wewnętrzne pozostają w `InternalViews`. |
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

`MainWindow` zawiera powierzchnię wizualizacji, a nad nią panel boczny, pasek informacji, pełny odtwarzacz, mini odtwarzacz, kontrolki szybkości i głośności, logo, tekst piosenki oraz kontroler widoków wewnętrznych. Panel boczny łączy menu hamburgerowe z playlistą „Teraz odtwarzane”. Przełączanie trybu zachowuje osobne rozmiary widoku pełnego i mini, zmienia dekoracje okna oraz włącza stan „zawsze na wierzchu” wyłącznie wtedy, gdy aktywny jest tryb mini i zapisana preferencja.

`PlayerMiniControl` jest responsywną alternatywą dla `PlayerControl`. Obowiązkowe grupy sterowania i opcji wyznaczają bazową szerokość minimalną. Włączenie mini wizualizacji wybiera większe minimum; wizualizacja pozostaje wtedy stała, a wraz ze wzrostem dostępnego miejsca pojawiają się informacje o utworze i opcjonalna okładka. `PlayerMiniDataContext` łączy stan odtwarzania z `PlayerDataContext` oraz metadane z `InfoBarContext`.

Główne funkcje mają osobne widoki wewnętrzne:

- Menedżer biblioteki.
- Menedżer playlist.
- Equalizer.
- Biblioteka i edytor tekstów.
- Profile wizualizacji.
- Ustawienia.

## Stan i powiadomienia

Klasy dostępne dla bindingów dziedziczą po `ViewModelBase`. Właściwości używają `UpdateProperty`, a własny atrybut `NotifyPropertyUpdate` wspiera mechanizm powiadomień projektu. Ustawienia trwałe są pogrupowane w kontekstach konfiguracji zamiast znajdować się bezpośrednio w kontrolkach. `MiniPlayerConfig` przechowuje preferencje dotyczące pozostawania na wierzchu, okładki, wizualizacji, jej typu i przezroczystości kompaktowego odtwarzacza.

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
