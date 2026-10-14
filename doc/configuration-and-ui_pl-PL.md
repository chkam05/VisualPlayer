# Konfiguracja i interfejs

[English](configuration-and-ui.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja)

## Pliki konfiguracji

VisualPlayer używa czytelnego JSON z nazwami właściwości camelCase i enumeracjami zapisanymi jako tekst. Pliki znajdują się poniżej:

```text
Environment.SpecialFolder.ApplicationData/VisualPlayer
```

Dokładny katalog bazowy zależy od platformy. Najważniejsze pozycje:

| Pozycja | Zawartość |
|---|---|
| `settings.json` | Wygląd, klawiatura, biblioteka, logo, teksty, odtwarzacz i okno. |
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
- `SpeedControl` i `VolumeControl` są wysuwanymi panelami powiązanymi z przyciskami odtwarzacza.
- `LyricsControl` wyświetla aktywny tekst.
- `LogoControl` renderuje i animuje logo SVG.
- `VisualizationSurfaceControl` rysuje wizualizację pod interfejsem.
- `InternalViewController` oraz `InternalMessageController` wyświetlają widoki aplikacji i dialogi.
