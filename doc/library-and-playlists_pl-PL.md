# Biblioteka i playlisty

[English](library-and-playlists.md) · [Indeks dokumentacji](../README_pl-PL.md#dokumentacja-techniczna)

## Obsługiwane pliki audio

Wspólny filtr plików akceptuje:

`.aac`, `.aif`, `.aiff`, `.asf`, `.flac`, `.m4a`, `.mp3`, `.mp4`, `.ogg`, `.opus`, `.wav` i `.wma`.

Rzeczywista możliwość dekodowania zależy również od backendu MiniAudioEx dostępnego na danej platformie.

## Biblioteka muzyki

Katalogi biblioteki są ustawiane w sekcji Ogólne. Każdy katalog może uwzględniać tylko bezpośrednie pliki albo również wszystkie podkatalogi. Dodanie, usunięcie, odświeżenie lub zmiana rekurencji może uruchomić ponowne skanowanie.

`LibraryCore` skanuje katalogi w zadaniu roboczym, pomija niedostępne ścieżki, usuwa duplikaty ścieżek, odczytuje tagi i okładki, a następnie publikuje utwory posortowane po tytule. Okładki trafiają do `CoverCache` i są współdzielone przez bibliotekę i playlisty.

Menedżer biblioteki ma cztery wejścia:

- Utwory.
- Albumy.
- Artyści.
- Katalogi.

Po wybraniu artysty można najpierw pokazać albumy albo od razu utwory. Widok katalogu łączy podkatalogi i pliki audio. Układ jest zapamiętywany niezależnie dla utworów, albumów, artystów i katalogów. Dostępne są szczegóły, lista kolumnowa i kilka rozmiarów ikon.

## Teraz odtwarzane

`PlayListCore` zarządza aktywną kolejką. Obsługuje dodawanie plików, usuwanie i zmianę kolejności, sortowanie, tryb zaznaczania, nawigację losową i stan bieżącego utworu. Kolejka może być przywracana z `now_playing.vpl` przy starcie aplikacji.

Playlista w panelu bocznym może otworzyć plik `.vpl` albo wyeksportować bieżącą kolejkę przez systemowe okno wyboru pliku.

## Zapisane playlisty

`PlayListLibraryCore` przechowuje nazwane pliki `.vpl` w katalogu danych aplikacji `PlayLists`. Menedżer playlist obsługuje:

- Tworzenie i duplikowanie playlist.
- Jawne zapisywanie z potwierdzeniem niezapisanych zmian.
- Zastępowanie bieżącej kolejki lub dodawanie na jej końcu.
- Importowanie plików audio.
- Wyszukiwanie i pogrupowane sortowanie.
- Zmianę kolejności przez przeciąganie.
- Zaznaczanie wielu pozycji, także zakresu z klawiszem Shift.
- Kopiowanie utworów do innej playlisty.
- Eksportowanie i usuwanie playlist.

Specjalna pozycja „Teraz odtwarzane” istnieje w pamięci i nie jest traktowana jako zapisana playlista biblioteczna.

## Metadane

`TrackMetadataUtils` odczytuje tytuł, album, artystę, czas trwania i osadzoną okładkę przez TagLibSharp. Gdy brakuje metadanych, właściwości wyświetlane korzystają z wartości wyprowadzonych ze ścieżki pliku.
