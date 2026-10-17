# Konfiguracja i interfejs

[English](configuration-and-ui.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

## Pliki konfiguracji

VisualPlayer używa czytelnego JSON z nazwami właściwości camelCase i enumeracjami zapisanymi jako tekst. Pliki znajdują się poniżej:

```text
Environment.SpecialFolder.ApplicationData/VisualPlayer
```

Dokładny katalog bazowy zależy od platformy. Najważniejsze pozycje:

| Pozycja | Zawartość |
|---|---|
| `settings.json` | Wygląd, klawiatura, biblioteka, logo, teksty, mini odtwarzacz, odtwarzacz i okno. |
| `visualizations.json` | Globalne ustawienia wizualizacji i profile. |
| `equalizer.json` | Profile equalizera i nazwa aktywnego profilu. |
| `now_playing.vpl` | Opcjonalnie zapamiętana kolejka „Teraz odtwarzane”. |
| `Backgrounds/` | Obrazy tła skopiowane do danych aplikacji. |
| `Lyrics/` | Lokalna biblioteka tekstów. |
| `Lyrics/Cache/` | Cache wyników internetowych. |
| `PlayLists/` | Zapisane playlisty. |

Błędny JSON powoduje użycie wartości domyślnych dla danego głównego obiektu konfiguracji. Wbudowane profile wizualizacji i equalizera są dodatkowo walidowane i odtwarzane niezależnie.

## Zapisywanie ustawień

Konfiguracja jest zapisywana podczas zamykania aplikacji. Widoki ustawień i wizualizacji mają również jawne przyciski zapisu. Przyciski resetowania zastępują daną sekcję nowym obiektem z wartościami domyślnymi.

## Wygląd

Ustawienia wyglądu obejmują:

- Motyw jasny, ciemny lub systemowy.
- Kolor akcentu i ostatnio używane kolory.
- Poziom koloryzacji wybranych przycisków i powierzchni głównych paneli.
- Przezroczystość interfejsu i okna.
- Tło jednolite, obraz albo tło przezroczyste.
- Automatyczne ukrywanie odtwarzacza, paska informacji i panelu bocznego.
- Tryb oraz czas wyświetlania paska informacji.

`AppearanceConfig` oblicza pędzle motywu, akcentu, najechania, naciśnięcia, zaznaczenia, wyłączenia i powierzchni głównych kontrolek. XAML korzysta z bindingów, dlatego zmiany nie wymagają przebudowania kontrolek.

## Mini odtwarzacz

Preferencje mini odtwarzacza są zebrane w `ConfigurationContext.MiniPlayer` i zapisywane w obiekcie `miniPlayer` pliku `settings.json`. `MiniPlayerConfig` przechowuje:

- utrzymywanie okna na wierzchu podczas działania trybu mini,
- widoczność okładki i wizualizacji,
- rodzaj mini wizualizacji,
- niezależną przezroczystość tła ograniczoną do zakresu `0,75–1,00`.

Stan „zawsze na wierzchu” ustawiany przez `MainWindow` jest koniunkcją zapisanej preferencji i `IsMiniMode`, dlatego pełny widok nigdy nie jest wymuszany ponad innymi oknami. Układ ma dwie bazowe szerokości minimalne: `512` bez wizualizacji i `640` z wizualizacją. Włączona wizualizacja nigdy nie znika z powodu szerokości. Dodatkowa przestrzeń odsłania najpierw informacje o utworze, a następnie opcjonalną okładkę.

`PlayerMiniControl` korzysta z `PlayerMiniDataContext`, który udostępnia jednocześnie `PlayerDataContext` i `InfoBarContext`. Dzięki temu stan odtwarzania i metadane utworu mogą współistnieć bez zastępowania kontekstu danych kontrolki.

## Motywy i szablony

`Themes/Generic.axaml` jest centralnym słownikiem zasobów. Zasób musi być dołączony po swoich zależnościach. Główny podział:

- `Themes/Base`: standardowe kontrolki Avalonia.
- `Themes/Components`: współdzielone komponenty projektu.
- `Themes/Controls`: kontrolki aplikacji.
- `Themes/Messages`: dialogi wewnętrzne.
- `Themes/Views`: zasoby wspólne i przeznaczone dla konkretnych widoków.

Klucze motywów i szablonów używają w odpowiednich miejscach notacji z nazwą właściciela, na przykład `PlayerControl.IconButtonTheme` oraz `PlayerControl.SeparatorBorder`.

## Skróty klawiszowe

Ustawienia klawiatury używają `KeyBindControl` i zapisują gesty jako tekst. Domyślne skróty obejmują:

- `Ctrl+L`, `Ctrl+I`, `Ctrl+M` i `Ctrl+P` dla playlisty i paneli.
- Systemowe klawisze poprzedniego utworu, odtwarzania/pauzy i następnego utworu.
- `Ctrl+Shift+L/P/E/T/V/C` dla biblioteki, playlist, equalizera, tekstów, wizualizacji i ustawień.
- `Ctrl+Left` i `Ctrl+Right` dla profili wizualizacji.

Systemowe polecenia multimedialne mają obecnie jawną integrację na macOS przez `MacOSMediaCommandService`.

## Główne kontrolki

- `SidePanelControl` łączy nawigację z playlistą „Teraz odtwarzane”.
- `InfoBarControl` pokazuje metadane i czas odtwarzania.
- `PlayerControl` zawiera sterowanie i suwak pozycji.
- `PlayerMiniControl` zapewnia responsywny mini odtwarzacz, menu kontekstowe i miejsce na mini wizualizację.
- `MiniVisualizationControl` rysuje siedmiopasmową, segmentową wizualizację w stylu VFD.
- `SpeedControl` i `VolumeControl` są wysuwanymi panelami powiązanymi z przyciskami odtwarzacza.
- `LyricsControl` wyświetla aktywny tekst.
- `LogoControl` renderuje i animuje logo SVG.
- `VisualizationSurfaceControl` rysuje wizualizację pod interfejsem.
- `InternalViewController` oraz `InternalMessageController` wyświetlają widoki aplikacji i dialogi.

## Typografia i osadzone fonty

Zasoby fontów są rejestrowane w `Themes/Generic.axaml` i osadzane przez regułę zasobów Avalonia `Assets/**` z pliku projektu:

- **Oxanium** jest domyślnym krojem aplikacji.
- **LED Sled** jest używany w mini odtwarzaczu i licznikach czasu.
- **LED Counter 7** jest używany dla metadanych w `InfoBarControl`.

Metadane wyświetlane ograniczonymi fontami przechodzą przez `LatinTransliterationConverter`; źródłowe tagi pozostają bez zmian. Autorzy i odnośniki do stron fontów znajdują się w widoku informacji aplikacji. Oryginalne pliki licencyjne/readme są zapisane obok fontów w `Assets/Fonts`.
