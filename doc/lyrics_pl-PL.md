# System tekstów piosenek

[English](lyrics.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

## Źródła i priorytet

`LyricsCore` zarządza lokalną biblioteką tekstów i wybiera dokument dla bieżącego utworu. Przypisanie z lokalnej biblioteki ma pierwszeństwo przed danymi osadzonymi lub sąsiednim plikiem. Spośród dostępnych treści preferowana jest dokładniejsza synchronizacja: słowa, linie, a następnie zwykły tekst.

Obsługiwane źródła:

- Lokalne dokumenty tekstów VisualPlayer.
- Sąsiednie pliki `.lrc` oraz ręcznie importowane pliki LRC lub tekstowe.
- Metadane plików audio odczytywane przez TagLibSharp.
- Wyszukiwanie online w LRCLIB.

Ustawienia określają używanie i automatyczny import tekstów osadzonych oraz automatyczne pobieranie zgodnych tekstów online.

## Rodzaje tekstu

`LyricsType` opisuje trzy poziomy:

- `Plain`: linie bez czasu odtwarzania.
- `LineSynced`: każda linia ma czas rozpoczęcia.
- `WordSynced`: poszczególne słowa lub sylaby mają własne czasy.

Parser LRC obsługuje czas linii, na przykład `[00:12.40]`, i rozszerzone znaczniki, na przykład `<00:13.10>`. Segmenty rozszerzone są rozwijane do osobnych wierszy edytora, dzięki czemu każdy czas można zmienić niezależnie.

## Dane lokalne

Dokumenty znajdują się w katalogu danych aplikacji `Lyrics`. Każdy przechowuje nazwę, źródło, typ, opcjonalną ścieżkę utworu, metadane LRC i linie czasowe. Podkatalog `Cache` zawiera odpowiedzi LRCLIB ważne przez 24 godziny.

## Metadane osadzone

`EmbeddedLyricsReader` korzysta z TagLibSharp. Obsługuje zwykłe pole tekstu oraz ramki ID3v2 `SYLT` z absolutnym czasem w milisekundach. Wpisy SYLT są konwertowane na dokument synchronizowany liniami. Zwykłe pole jest również przekazywane przez parser LRC, dlatego osadzony tekst LRC może zachować synchronizację.

## Dostawca internetowy

`LrcLibLyricsProvider` implementuje `ILyricsProvider`. Obsługuje zapytania tekstowe oraz wyszukiwanie po tytule, artyście i albumie. Żądania mają limit 15 sekund, identyfikują aplikację w user-agent, zapisują poprawne odpowiedzi w cache i obsługują ograniczenie liczby zapytań.

## Edytor

`InternalViewLyricsEditor` udostępnia:

- Wybór typu oraz wiersze tekstu z czasem.
- Precyzyjne pole pozycji.
- Konfigurowalne krótkie, średnie i długie interwały przewijania.
- Odtwarzanie, zatrzymanie, przewijanie, punkt wznowienia i zmianę szybkości.
- Walidację czasu i widoczne informacje o błędach.
- Cofanie i ponawianie.
- Wykrywanie zmian oraz potwierdzenie zamknięcia.

Zwykły tekst jest eksportowany jako `.txt`, a tekst synchronizowany jako `.lrc`. Import korzysta z tego samego parsera i automatycznie rozpoznaje typ dokumentu.

## Nakładka podczas odtwarzania

`LyricsControl` respektuje `LyricsConfig.DisplayLyrics`. Tekst czasowy jest prezentowany w poziomym pasku nad odtwarzaczem jako treść poprzednia, bieżąca i następna. Zwykły tekst wykorzystuje większy obszar pomiędzy paskiem informacji, panelem bocznym, odtwarzaczem i prawą krawędzią. Rozmiary tekstu i przezroczystość gradientowego tła są konfigurowalne.
