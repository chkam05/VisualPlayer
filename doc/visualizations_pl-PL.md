# System wizualizacji

[English](visualizations.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja)

## Przepływ danych

`VisualizationSurfaceControl` żąda klatek przy użyciu timera dispatchera Avalonia. Wylicza obszar rysowania z uwzględnieniem paska informacji, panelu bocznego, odtwarzacza i globalnej opcji wypełniania ekranu.

Następnie `VisualizationCore`:

1. Pobiera widmo i waveform z `AudioAnalysisCore`.
2. Mapuje FFT do skonfigurowanej skali częstotliwości.
3. Normalizuje decybele i stosuje czasy narastania oraz opadania.
4. Tworzy `VisualizationFrame` z czasem klatki, widmem, waveformem i stanem odtwarzania.
5. Wywołuje `PreCalculate` oraz `Draw` aktywnej implementacji.

Aktywna jest tylko jedna instancja wizualizacji. Po zmianie profilu lub rodzaju jest zwalniana.

## Ustawienia globalne

Globalna konfiguracja obejmuje:

- Rozmiar i okno FFT.
- Tryb kanałów.
- Liniową, logarytmiczną, Mel albo Bark skalę częstotliwości.
- Minimalną i maksymalną częstotliwość.
- Minimalny i maksymalny poziom decybeli.
- Czas narastania i opadania.
- Limit klatek.
- Możliwość wykorzystania całego okna.

## Profile

Każdy `VisualisationProfile` zawiera:

- Unikalną nazwę.
- Dyskryminator `VisualizationType`.
- Jeden obiekt konfiguracji właściwy dla danego typu.
- Opcjonalną wspólną konfigurację koloru.
- Flagę `IsDefault`, która zabezpiecza profil wbudowany w UI.

Serializacją zarządza `VisualisationProfileJsonConverter`. Profile wbudowane definiuje `Resources/DefaultVisualizationProfiles.cs`. Podczas ładowania brakujące profile są odtwarzane, a niezgodne pozycje wbudowane zastępowane poprawnymi.

## System koloru

`VisualizationColorConfig` obsługuje:

- Kolor jednolity.
- Liniowe i promieniste gradienty użytkownika.
- Liniową i promienistą tęczę.
- Poziomą i pionową szybkość animacji.
- Rozciągnięcie gradientu.

`VisualizationColorSampler` próbkuje kolor we współrzędnych znormalizowanych. Przesunięcia animacji są aktualizowane przez wizualizację i zawijane tak, aby koniec powtarzanego gradientu łączył się z jego początkiem.

## Implementacje

Obecne rodziny wizualizacji:

- Peaks: standardowa, lustrzana, pływająca i pływająca lustrzana.
- Stripes: standardowa i lustrzana.
- Oscyloskop: standardowy, środkowy, podwójny i wypełniony.
- Waveform: standardowy i lustrzany.
- Circle: paski i waveform.

Każda wizualizacja ma niezależną klasę konfiguracji i widok ustawień. Wspólne algorytmy trafiają do `Visualizations/Utils` wyłącznie wtedy, gdy korzysta z nich więcej niż jedna implementacja.

## Renderowanie i wydajność

Wizualizacje są rysowane na wątku UI. `VisualizationSurfaceControl` nie dodaje kolejnego żądania odświeżenia, jeżeli poprzednie nadal oczekuje. Oscyloskopy mają limit 30 FPS oraz ograniczoną liczbę punktów i kolumn wypełnienia. Przy kolorze jednolitym używają jednego obiektu pióra dla całego przebiegu.

Odtwarzanie audio ma pierwszeństwo przed płynną animacją. Nowe wizualizacje powinny ponownie używać tablic i obiektów rysujących, unikać alokacji dla każdego piksela, ograniczać złożoność geometrii i poprawnie działać przy pominiętych klatkach.
