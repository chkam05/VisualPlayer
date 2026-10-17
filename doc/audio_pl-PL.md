# System audio

[English](audio.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

## Tor odtwarzania

`PlayerCore` zarządza silnikiem `MaEngine` biblioteki MiniAudioEx. Utwory są otwierane jako dekodowane strumienie i reprezentowane przez `MaSound`. Stan odtwarzacza jest dostępny przez `PlayerDataContext`. Odtwarzacz obsługuje odtwarzanie, pauzę, zatrzymanie, przewijanie, następny i poprzedni utwór, losowanie, powtarzanie, głośność, wyciszenie oraz szybkość realizowaną przez zmianę pitch.

Silnik używa zarządzanego callbacku danych:

1. MiniAudio renderuje ramki PCM z silnika.
2. `AudioEqualizerProcessor` modyfikuje próbki w miejscu, jeżeli aktywny profil nie jest płaski.
3. `AudioAnalysisCore` zapisuje reprezentację mono w cyklicznym buforze próbek.
4. Przetworzone ramki wracają do urządzenia audio.

Okres urządzenia jest ustawiony na 2048 ramek, aby tolerować krótkie zatrzymania UI lub garbage collectora. Praca wewnątrz callbacku musi mieć ograniczony czas i nie może tworzyć alokacji.

## Płynne przejście

Po włączeniu crossfade następny `MaSound` rozpoczyna od zerowej głośności, a poprzedni pozostaje aktywny. Aktualizacja oparta na stoperze stopniowo zamienia ich głośności przez skonfigurowany czas od jednej do pięciu sekund. Poprzedni dźwięk jest zatrzymywany i zwalniany po zakończeniu przejścia.

Timer pozycji „Teraz odtwarzane” aktualizuje przejście co 100 ms.

## Analiza dźwięku

`AudioAnalysisCore` przechowuje ostatnie próbki mono w cyklicznym buforze o stałym rozmiarze 16 384 próbek. Wizualizacja nie przejmuje bufora wyjściowego urządzenia.

System udostępnia:

- Waveform z 1024 próbek.
- Widmo FFT o rozmiarze zależnym od konfiguracji.
- Okna Hann, Hamming, Blackman, Blackman-Harris i prostokątne.
- Kanały lewy, prawy, mid, side, stereo i mixed.

`VisualizationCore` mapuje próbki FFT do wybranego zakresu częstotliwości, przelicza amplitudę na znormalizowane decybele, stosuje czasy narastania i opadania oraz przekazuje aktywnej wizualizacji widoki danych klatki.

## Equalizer

Equalizer zawiera preamp oraz dziesięć filtrów z częstotliwościami:

`60 Hz`, `100 Hz`, `200 Hz`, `500 Hz`, `1 kHz`, `1,8 kHz`, `3,4 kHz`, `5,7 kHz`, `9 kHz` i `13 kHz`.

Każde pasmo ma zakres od -10 dB do +10 dB. Współczynniki filtrów są przebudowywane po zmianie profilu, wzmocnienia, częstotliwości lub preampu. Stan przetwarzania jest przechowywany osobno dla każdego kanału i pasma.

Brakujące lub niezgodne presety wbudowane są odtwarzane. Presety użytkownika można klonować i edytować. Stan equalizera znajduje się w `equalizer.json`.

## Zasady wydajności

- W `EngineDataCallback` nie wolno tworzyć obiektów, czytać plików, aktualizować UI ani wykonywać operacji sieciowych.
- Renderowanie może pomijać klatki; audio nie może czekać na wizualizację.
- Bufory analizy powinny mieć stały rozmiar, a tablice FFT powinny być używane ponownie.
- Podmieniane obiekty `MaSound` muszą być zwalniane, a zdarzenia konfiguracji odpinane.
- Błąd odczytu silnika powinien generować ciszę zamiast niezainicjalizowanego bufora wyjściowego.
