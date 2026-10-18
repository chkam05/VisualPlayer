# Wydzielenie wizualizacji do pluginów

## Cel i sposób korzystania z instrukcji

Dokument jest planem przyszłej implementacji, przygotowanym na podstawie kodu projektu
i ustaleń z użytkownikiem. Samo utworzenie dokumentu nie oznacza wykonania migracji.
Przed rozpoczęciem prac sprawdź aktualny stan repozytorium i obowiązujące AGENTS.md.
Zachowaj niezwiązane zmiany użytkownika. Nie przebudowuj odtwarzania audio przy okazji
wydzielania pluginów.

Wykonuj etapy w podanej kolejności. W spisie zadań zmieniaj `[ ]` na `[x]` dopiero po
implementacji i sprawdzeniu kryteriów danego kroku. Dopisuj pod zadaniem krótką
informację o wyniku walidacji, a przy przeszkodzie pozostaw krok nieoznaczony.
Nie oznaczaj testu macOS lub Linuksa jako wykonanego wyłącznie na podstawie kompilacji
na Windows. Jeżeli kod zmienił się od czasu przygotowania planu, aktualizuj dokument
przed realizacją zależnego etapu.

## Uzgodniona architektura

Pozostają cztery projekty wspólne. Każda z 16 obecnych wizualizacji otrzymuje dodatkowo
własny projekt biblioteki i własny DLL. Nie twórz kolejnych bibliotek wspólnych bez
konkretnej potrzeby. Osobny projekt nie oznacza obowiązku publikowania NuGeta.

Projekty wizualizacji systemowych umieszczaj w podkatalogu
`Visualizations/VisualPlayer.Visualizations.<Nazwa>/` oraz folderze rozwiązania
`Visualizations`. Cztery projekty wspólne pozostają w katalogu głównym rozwiązania.

| Projekt | Odpowiedzialność |
| --- | --- |
| `VisualPlayer` | Okno aplikacji, odtwarzanie, integracja usług, lista dodatków w widoku informacji, prezentowanie edytora |
| `VisualPlayer.UI` | Komponenty, współdzielone kontrolki ustawień, motywy, templates, konwertery i zasoby UI |
| `VisualPlayer.Visualizations` | Loader, rejestr, runtime, profile, serializacja, przygotowanie danych wizualizacji |
| `VisualPlayer.VisualizationBase` | Publiczne API dodatków, klasy bazowe, dane klatki, konfiguracja współdzielona, kontrakty usług |
| `VisualPlayer.Visualizations.<Nazwa>` | Jedna konkretna wizualizacja, jej konfiguracja, widok ustawień, domyślne profile, tłumaczenia i zasoby |

Docelowe zależności:

```text
VisualPlayer ---------------------------------> VisualPlayer.Visualizations
     |                                                    |
     +----> VisualPlayer.UI -------------------+           |
                                             v           v
                                  VisualPlayer.VisualizationBase

VisualPlayer.Visualizations.Peaks ------------> VisualPlayer.VisualizationBase
     +---------------------------------------> VisualPlayer.UI (opcjonalnie)
```

`VisualizationBase` nie zależy od UI, runtime ani aplikacji. UI może zależeć od
`VisualizationBase`, ponieważ współdzielony edytor kolorów potrzebuje jego modeli.
Runtime nie zależy od `VisualPlayer.UI` ani konkretnych pluginów. Aplikacja nie
referencjonuje typów pluginów. Nie dodawaj ProjectReference do pluginów po to, aby
uruchamiały się w aplikacji: ich budowanie i kopiowanie organizuje proces budowania.

Publicznymi NuGetami będą początkowo `VisualPlayer.VisualizationBase` i
`VisualPlayer.UI`. Runtime pozostaje projektem rozwiązania. Pakiet instalacyjny
wizualizacji jest ZIP-em, nie NuGetem pobieranym przez odtwarzacz w czasie działania.

## Stan początkowy i miejsca wymagające zmian

Stan rozpoznany podczas przygotowania dokumentu: .NET 10, Avalonia 12.1.2.
Zweryfikuj wersje przed rozpoczęciem i utrzymuj zgodne wersje Avalonia we wszystkich
projektach. Nie podnoś ich wyłącznie na potrzeby tego refaktoringu.

| Obecne miejsce | Docelowa zmiana |
| --- | --- |
| `VisualPlayer/Interfaces/IVisualization.cs` | Przeniesienie kontraktu do `VisualizationBase` |
| `VisualPlayer/Interfaces/IVisualizationConfig.cs` | Publiczny kontrakt konfiguracji dodatku |
| `VisualPlayer/Visualizations/VisualizationBase.cs` | Przeniesienie i usunięcie zależności od typów aplikacji |
| `VisualPlayer/Visualizations/Data/VisualizationFrame.cs` | Wspólny kontrakt klatki |
| `VisualPlayer/Core/VisualizationCore.cs` | Runtime; usunięcie fabryki opartej na switch/enumeracji |
| `VisualPlayer/Contexts/VisualizationConfigurationContext.cs` | Profile i zapis w runtime, ścieżkę oraz zapis integruje host |
| `VisualPlayer/Visualizations/Utils/VisualisationProfileJsonConverter.cs` | Odczyt przez identyfikator i rejestr; migracja starych profili |
| `VisualPlayer/Visualizations/Config/Base/VisualisationProfile.cs` | Rozdzielenie danych profilu i publicznego kontekstu edytora |
| `VisualPlayer/Resources/DefaultVisualizationProfiles.cs` | Domyślne profile dostarczane przez pluginy; `None` pozostaje w hoście |
| `VisualPlayer/Views/InternalViewVisualizations.axaml.cs` | Dynamiczna lista dodatków i fabryka widoku zamiast switch |
| `VisualPlayer/Views/Visualisations/` | Edytory do właściwych pluginów; wspólne UI do `VisualPlayer.UI` |
| `VisualPlayer/Converters/VisualizationTypeToTitleConverter.cs` | Nazwy z deskryptorów i tłumaczeń dodatków |
| `VisualPlayer/Contexts/TranslationContext.cs` | Integracja niezależnych zakresów tłumaczeń |
| `VisualPlayer/Themes/Generic.axaml` | Import zasobów wydzielonej biblioteki UI |
| `VisualPlayer/Controls/VisualizationSurfaceControl.cs` | Użycie runtime przez publiczne API |
| `VisualPlayer/Controls/MiniVisualizationControl.cs`, `LogoControl.cs` | Zachowanie dostępu do danych analizy po przeniesieniu runtime |
| `build_windown.bat`, `build_linux.sh`, `build_macos.sh` | Budowanie i dystrybucja zewnętrznych paczek pluginów |

Przeszukaj całe rozwiązanie pod kątem `VisualizationType`, `GetConfig<T>`,
`DefaultVisualizationProfiles`, statycznych odwołań do konfiguracji i konkretnego typu
wizualizacji. Sama wymiana switch w `VisualizationCore` nie kończy migracji.
`MiniVisualizationType` opisuje oddzielną mini wizualizację: zachowaj go, dopóki nie ma
osobnego wymagania dotyczącego jego rozszerzalności.

## Publiczne API i odpowiedzialności

W `VisualizationBase` zdefiniuj następujące elementy. Nazwy nowych typów z tej sekcji
są docelową propozycją API; ujednolić je w prototypie, przed migracją 16 pluginów.

| Typ | Wymagane zachowanie |
| --- | --- |
| `IVisualization` | Zachowuje `Draw`, `PreCalculate`, `Reset`, `UpdateGraphics`, `HasActiveAnimation`, `MaximumFrameRate`, `Dispose` |
| `VisualizationBase<TConfig>` | Wspólna obsługa konfiguracji, geometrii i zwalniania zasobów |
| `VisualizationFrame` | Czas od poprzedniej klatki, widmo, waveform, stan odtwarzania |
| `IVisualizationConfig` | Konfiguracja pluginu z powiadomieniami `INotifyPropertyChanged` |
| `IVisualizationPlugin` | Dostarcza deskryptor, fabryki instancji i ustawień, domyślne profile, migrację konfiguracji i angielski słownik |
| `VisualizationDescriptor` | Trwały ID, wersja pluginu, wersja schematu konfiguracji, klucz nazwy/opisu, capabilities |
| `VisualizationEditorContext` | Konfiguracja konkretnego pluginu, ustawienia kolorów i tłumaczenia wymagane przez edytor |
| `ITranslationScope` | Indekser tłumaczeń, formatowanie, kod języka i powiadomienie o zmianie języka |
| `IVisualizationHostServices` | Ograniczony zestaw usług: lokalne/wspólne tłumaczenia i diagnostyka |

Plugin musi umieć utworzyć nową konfigurację, wczytać ją z JSON określonej wersji,
zapisać ją oraz utworzyć `Control?` edytora dla otrzymanego kontekstu. Dopuszczalny jest
brak własnego widoku ustawień. Domyślne profile muszą być deskryptorami/danymi z SDK,
a nie instancjami typu profilu zależnego od runtime. Każde wywołanie fabryki konfiguracji
tworzy niezależną instancję.

Deskryptor rejestruje capabilities, np. obsługę kolorów, waveform i ustawień wspólnych.
Usuń rozpoznawanie możliwości przez porównywanie ID z listą wbudowanych typów, m.in.
obecne `SupportsFloatingBarGravity`. Ustawienia specyficzne dla pluginu powinny należeć
do jego konfiguracji i edytora.

Kontrakty rysowania oraz edytora mogą zależeć od Avalonia. Nie jest wymagane tworzenie
abstrakcji innego silnika renderującego. Fabryki edytorów, aktywacja i rysowanie działają
na wątku UI. Odczyt metadanych nie powinien niepotrzebnie blokować interfejsu.

Pamięci `Spectrum` i `Waveform` są ważne podczas obsługi bieżącej klatki. Plugin nie
może zachować ich zawartości jako niezmiennego snapshotu na kolejne klatki; jeśli jej
potrzebuje, wykonuje kopię. Dispose musi odłączać subskrypcje i zwalniać zasoby pluginu.

Wspólne modele kolorów, gradientów, enumy i narzędzia matematyczne używane przez kilka
pluginów umieść w `VisualizationBase`. Obejmuje to zależności `VisualizationColorConfig`,
`GradientDefinition`, `GradientPoint`, potrzebne modele kolorów, `CommonUtils`,
`OscilloscopeUtils`, `CircleVisualizationUtils` oraz współdzielone klasy bouncing.
Nie przenoś tam funkcji zarządzania oknem, odtwarzaczem ani katalogami użytkownika.
Typy używane tylko przez jeden plugin pozostają w nim.

Obecny `ViewModelBase` i `NotifyPropertyUpdateAttribute` są zależnością konfiguracji.
Przenieś ich potrzebną, niezależną implementację do SDK lub zastąp ją jedną wspólną
bazą konfiguracji. Aplikacja może korzystać z tej samej implementacji. Nie utrzymuj
dwóch niezgodnych kopii atrybutu ani typu kontraktowego.

## Biblioteka UI

Komponenty, motywy, konwertery i zależne modele przenoś jako spójny zestaw. Publiczne
kontrolki ustawień, np. `SettingsControl`, edytor kolorów, gradientów, numeryczne pola,
przełączniki i selektory, muszą działać bez referencji do aplikacji. Kontrolki związane
z konkretnymi funkcjami odtwarzacza mogą pozostać w `VisualPlayer`.

Usuń z wydzielanej biblioteki użycie `ApplicationContext.Instance` i typów konfiguracji
aplikacji. Kolory, fonty, opacity i inne parametry wyglądu udostępnij przez zasoby
Avalonia, najlepiej `DynamicResource` dla wartości zmienianych podczas działania.
Biblioteka dostarcza wartości domyślne, aplikacja je nadpisuje. Unikaj bezpośrednich
setterów konkurowania z lokalnymi bindingami pluginu.

Własne widoki pluginów korzystają z przekazanego `VisualizationEditorContext` lub
własnego viewmodelu z typowaną konfiguracją. Nie bindują do profilu runtime ani
globalnego singletona aplikacji. Wspólny edytor kolorów z
`Views/Visualisations/Common/VisualisationColorConfig` przenieś do UI i dostosuj jego
DataContext. Zachowaj zmianę motywu i języka bez ponownego otwierania widoku.

Motywy UI mają jeden publiczny słownik wejściowy, importowany przez `avares://`.
Zachowaj obecne możliwości komponentów, w tym `ScrollTextBlock`. Sprawdź fonty, ikony,
obrazy, typy XAML i adresy zasobów po zmianie assembly. Specyficzne zasoby pluginu
rejestruj w zakresie jego edytora, aby nie nadpisywały globalnych zasobów aplikacji.

## Identyfikatory i migracja istniejących typów

ID jest niezależny od nazwy wyświetlanej, języka, nazwy profilu i nazwy DLL. Nie
zapisuj pełnej nazwy CLR typu jako trwałego identyfikatora wizualizacji. Dla pierwszej
wersji jedna paczka zawiera jedną wizualizację, a `pluginId` jest jej ID.

Poniższa tabela jest obowiązkową mapą migracji. Obsłuż liczby oraz stare nazwy enuma
(także camelCase zapisywane przez obecny serializer). Nazwy starych profili domyślnych
mogą być wskazówką wyłącznie dla formatu, w którym faktycznie brakowało pola `type`.

| Dawna wartość | Dawny enum | Nowy ID | Osobny DLL |
| --- | --- | --- | --- |
| 0 | None | `visualplayer.none` | Nie; wbudowana opcja braku wizualizacji |
| 1 | CircleStripes | `visualplayer.circle-stripes` | `VisualPlayer.Visualizations.CircleStripes.dll` |
| 2 | CircleWaveform | `visualplayer.circle-waveform` | `VisualPlayer.Visualizations.CircleWaveform.dll` |
| 3 | Oscilloscope | `visualplayer.oscilloscope` | `VisualPlayer.Visualizations.Oscilloscope.dll` |
| 4 | OscilloscopeCenter | `visualplayer.oscilloscope-center` | `VisualPlayer.Visualizations.OscilloscopeCenter.dll` |
| 5 | OscilloscopeDouble | `visualplayer.oscilloscope-double` | `VisualPlayer.Visualizations.OscilloscopeDouble.dll` |
| 6 | OscilloscopeFilled | `visualplayer.oscilloscope-filled` | `VisualPlayer.Visualizations.OscilloscopeFilled.dll` |
| 7 | Peaks | `visualplayer.peaks` | `VisualPlayer.Visualizations.Peaks.dll` |
| 8 | PeaksFloating | `visualplayer.peaks-floating` | `VisualPlayer.Visualizations.PeaksFloating.dll` |
| 9 | PeaksFloatingMirrored | `visualplayer.peaks-floating-mirrored` | `VisualPlayer.Visualizations.PeaksFloatingMirrored.dll` |
| 10 | PeaksMirrored | `visualplayer.peaks-mirrored` | `VisualPlayer.Visualizations.PeaksMirrored.dll` |
| 11 | Stripes | `visualplayer.stripes` | `VisualPlayer.Visualizations.Stripes.dll` |
| 12 | StripesMirrored | `visualplayer.stripes-mirrored` | `VisualPlayer.Visualizations.StripesMirrored.dll` |
| 13 | Waveform | `visualplayer.waveform` | `VisualPlayer.Visualizations.Waveform.dll` |
| 14 | WaveformMirrored | `visualplayer.waveform-mirrored` | `VisualPlayer.Visualizations.WaveformMirrored.dll` |
| 15 | PeaksBouncing | `visualplayer.peaks-bouncing` | `VisualPlayer.Visualizations.PeaksBouncing.dll` |
| 16 | StripesBouncing | `visualplayer.stripes-bouncing` | `VisualPlayer.Visualizations.StripesBouncing.dll` |

W czasie przejściowym enum i adapter mogą nadal istnieć, ale końcowo enum pozostaje
wyłącznie szczegółem migracji. Nie wolno zmieniać jego dawnych wartości liczbowych.

Nowy profil zapisuje co najmniej:

```json
{
  "name": "Moje słupki",
  "pluginId": "visualplayer.peaks",
  "configVersion": 1,
  "isDefault": false,
  "fillScreen": false,
  "color": {},
  "config": { "peaksCount": 64 }
}
```

To przykład struktury, nie kompletny poprawny profil kolorów. Wersję formatu całego
pliku profili zapisuj osobno od `configVersion` konkretnego pluginu. Przed pierwszym
zapisem zmigrowanego istniejącego pliku wykonaj jego kopię. Zapisuj atomowo przez plik
tymczasowy w tym samym katalogu; przy błędzie pozostaw ostatnią poprawną wersję.

Brak pluginu, nieobsługiwana wersja konfiguracji lub wyjątek jej odczytu nie mogą
powodować zastąpienia ustawień wartościami domyślnymi. Zachowaj surowy JSON profilu
i konfiguracji, także nieznane pola, np. przez `JsonElement.Clone()` i extension data.
Klonowanie profilu niedostępnego dodatku również nie może utracić jego danych.
Wyświetl profil jako niedostępny; nie uruchamiaj go i nie zmieniaj jego zapisanego ID.
Po ponownym zainstalowaniu pluginu ustawienia mają być odtwarzane.

Zastąp obecną normalizację `EnsureProfiles`, która może odtwarzać domyślne profile
według nazwy. Domyślne profile identyfikuj stabilnym ID pluginu i ID presetu, a nazwy
wyświetlaj przez tłumaczenia. Nazw profili użytkownika nie tłumacz. Nie przywracaj
automatycznie usuniętego pluginu przez sam odczyt profili.

## Tłumaczenia

Każdy plugin zawiera własny `Resources/EnglishTranslations.cs` kompilowany do DLL.
Własne `Lang/pl-PL.json` jest plikiem dołączanym do paczki. Dotyczy to również wszystkich
pluginów dostarczanych razem z aplikacją. Główny słownik aplikacji nie może być wymagany
do wyświetlania specyficznych ustawień nowego pluginu.

Docelowy układ paczki:

```text
visualplayer.peaks/
  manifest.json
  VisualPlayer.Visualizations.Peaks.dll
  VisualPlayer.Visualizations.Peaks.deps.json
  Lang/
    pl-PL.json
  Assets/
    preview.png                 (opcjonalnie)
```

Wybrany układ podstawowy to języki wewnątrz paczek. Alternatywa ustalona w rozmowie,
czyli wspólny `Lang/Peaks_pl-PL.json`, również jest możliwa, lecz nie jest wymagana
w pierwszym wydaniu. Jeśli zostanie dodana, manifest musi jawnie przypisywać prefiks
do stabilnego ID. Obecny loader języków odrzuca powtórzony kod języka; nie można
przekazać mu bez zmian wszystkich plików `*_pl-PL.json` jako języków aplikacji.

Format lokalnego słownika zachowuje obecną strukturę:

```json
{
  "header": { "code": "pl-PL", "name": "Polski" },
  "translations": {
    "NAME": "Słupki",
    "PEAKS_COUNT_TITLE": "Liczba słupków"
  }
}
```

Każdy plugin otrzymuje osobny `ITranslationScope`, więc identyczne klucze w różnych
pluginach nie kolidują. Dla lokalnego klucza fallback to język wybrany przez użytkownika,
następnie angielski słownik tego samego pluginu, na końcu klucz z diagnostyką ID dodatku.
Wspólne napisy typu Reset/Save i wspólnego edytora kolorów pobieraj z jawnego zakresu
wspólnego, nigdy przez przypadkowe wyszukiwanie w słownikach innych pluginów.

Wspólny katalog UI także musi działać bez aplikacji: słowniki wspólnych kontrolek
dostarcza biblioteka UI, host może udostępniać je przez wspólną usługę tłumaczeń.
Plugin nie musi kopiować tych tekstów. Jeden globalny wybór języka aktualizuje wszystkie
istniejące zakresy, nazwy pluginów, otwarte edytory i teksty generowane przez konwertery.
Aktualizacja języka nie powinna zmieniać wartości konfiguracji ani zapisywać profilu.

W trakcie migracji mogą tymczasowo istnieć kopie starych wpisów w słowniku aplikacji.
Usuń je dopiero po przełączeniu wszystkich odwołań. Rozszerz kontrolę tłumaczeń tak,
aby osobno walidowała aplikację, UI i każdy plugin: brakujące/zdublowane klucze, puste
wartości, zgodność placeholderów i klucze rzeczywiście używane przez właściwe widoki.

## Manifest, loader i zgodność

Przykładowy manifest wersji pierwszej:

```json
{
  "manifestVersion": 1,
  "id": "visualplayer.peaks",
  "version": "1.0.0",
  "apiMajor": 1,
  "minimumApiMinor": 0,
  "targetFramework": "net10.0",
  "avaloniaVersion": "12.1.2",
  "entryAssembly": "VisualPlayer.Visualizations.Peaks.dll",
  "entryType": "VisualPlayer.Visualizations.Peaks.PeaksPlugin",
  "defaultLanguage": "en-US",
  "languageDirectory": "Lang"
}
```

W prototypie ustal finalne nazwy i typy pól, waliduj je i opublikuj przykład.
`targetFramework` i wersje zależności nie są gwarancją kompatybilności same w sobie.
Pierwsza wersja wspiera jawnie określony TFM oraz zgodny, testowany zestaw SDK/Avalonia.
Przyjmuj zgodny major API i wystarczający minor hosta. Nie obiecuj zgodności dowolnych
wersji Avalonia. Numer wersji pluginu, API SDK i schematu konfiguracji to różne wartości.

Skanuj manifesty w katalogach pluginów przy starcie. Nie próbuj aktywować każdej
znalezionej biblioteki zależnej. Dla poprawnej paczki utwórz własny `AssemblyLoadContext`
i `AssemblyDependencyResolver` wskazujący jej bibliotekę wejściową. Zachowaj `.deps.json`
oraz wymagane prywatne zależności w wyniku budowania/publikowania paczki.

Biblioteki kontraktowe i UI muszą być współdzielone z hostem: `VisualizationBase`,
`VisualPlayer.UI`, Avalonia oraz inne typy bibliotek przekraczające granicę plugin/host,
np. Lucide, jeśli występuje w publicznym API UI. Nie ładuj ich drugiej kopii do
prywatnego kontekstu. Skonfiguruj ProjectReference/PackageReference i pakowanie tak,
aby kontraktów hosta nie dystrybuować jako prywatnych zamienników pluginu.
Prywatne zależności mogą być rozwiązywane we własnym kontekście. Nie dodawaj ogólnego
loadera opartego wyłącznie na `Assembly.LoadFrom` i nie korzystaj z `Assembly.Location`
hosta jako źródła ścieżek w wydaniu single-file.

Kolejność startu: ustawienia ogólne i język -> środowisko Avalonia/zasoby UI -> wykrycie
paczek i rejestr -> odczyt/migracja profili -> wybór aktywnego dodatku -> runtime i widoki.
Oddziel wykrycie metadanych od tworzenia edytora i instancji rysującej.

Użytkownik instaluje, aktualizuje i usuwa paczki ręcznie przy zamkniętej aplikacji;
zmiany są wykrywane przy następnym starcie.
Nie realizuj hot reload ani nie wymagaj natychmiastowego wyładowania assembly.
Odłączaj widoki i eventy po zmianie profilu. Konteksty mogą żyć do zamknięcia procesu.

Błędy konstruktora, fabryki, konfiguracji, PreCalculate, Draw i tworzenia ustawień
powinny wyłączać problematyczny dodatek oraz pozostawiać odtwarzacz działający, o ile
wyjątek da się bezpiecznie obsłużyć. Nie ponawiaj uszkodzonego rysowania w każdej klatce.
Zachowaj diagnostykę i profil. `AssemblyLoadContext` nie jest sandboxem: DLL działa
z uprawnieniami aplikacji. Nie deklaruj izolacji awarii natywnych, zawieszenia wątku UI
ani nieufnego kodu. Pełna izolacja procesowa nie należy do pierwszego wydania.

## Instalowanie i katalogi

Instalacja jest ręczna, opisana w `README.md` i `README_pl-PL.md`. Użytkownik zamyka
aplikację, rozpakowuje ZIP i przenosi kompletny folder paczki do `Plugins` wewnątrz
istniejącego katalogu ustawień aplikacji. Aplikacja skanuje rozpakowane paczki,
nie rozpakowuje ZIP-ów i nie udostępnia instalatora ani osobnego widoku zarządzania.

Jedyną podstawą ścieżki użytkownika jest `ConfigurationContext.DirectoryPath`:

```csharp
Path.Combine(ConfigurationContext.DirectoryPath, "Plugins")
```

Host przekazuje tę ścieżkę do runtime; runtime nie referencjonuje konfiguracji aplikacji
i nie wyznacza ponownie jej lokalizacji. Nie zmieniaj lokalizacji ustawień i profili.
Zastąp wcześniejsze niezależne ścieżki `%LOCALAPPDATA%` / `$XDG_DATA_HOME` ścieżką
wyprowadzoną z katalogu ustawień. Na Windows oznacza to `%APPDATA%/VisualPlayer/Plugins`;
opisy Linux/macOS w README mają odpowiadać rzeczywistej lokalizacji ustawień aplikacji,
a nie nowej, osobnej konwencji katalogów danych. Jeśli dotychczasowy katalog dodatków
użytkownika zawiera pliki, opisz ich ręczne przeniesienie do nowego miejsca.

Docelowy układ (manifest bez dodatkowego poziomu zagnieżdżenia):

```text
<katalog ustawień>/Plugins/
  visualplayer.peaks/
    manifest.json
    VisualPlayer.Visualizations.Peaks.dll
    VisualPlayer.Visualizations.Peaks.deps.json
    Lang/pl-PL.json
    ...prywatne zależności paczki
```

Wbudowane paczki są automatycznie dostarczane przez build/publish w katalogu aplikacji:
`Plugins` obok programu na Windows/Linux oraz `Contents/Resources/Plugins` w macOS.
Ta obsługa istnieje od etapów D/E; etap F ma ją zachować i sprawdzić, bez dodawania
nowej lokalizacji w `.app` ani kopiowania tam dodatków użytkownika. Nie kopiuj też
automatycznie paczek wbudowanych do katalogu ustawień. Użytkownik nie musi edytować
bundle ani katalogu instalacyjnego. Uwzględnij instalacje bez prawa zapisu do nich.

Przy powtórzonym ID wybierz dokładnie jedną paczkę przed załadowaniem assembly.
Poprawna, zgodna paczka użytkownika może zastąpić wbudowaną. Niezgodna/uszkodzona
paczka użytkownika daje diagnostykę; wbudowana może pozostać dostępna. Nie wybieraj
wyniku według przypadkowej kolejności plików. W katalogu użytkownika przechowuj jedną
aktywną wersję danego ID. Usunięcie nadpisującej paczki użytkownika ponownie udostępnia
wersję wbudowaną, jeśli jest dostarczona z aplikacją; opisz to w README.

Aktualizacja polega na ręcznej wymianie całego folderu paczki przy zamkniętej aplikacji,
bez mieszania plików dwóch wersji. Kopię poprzedniej wersji przechowuj poza skanowanym
`Plugins`. Usunięcie polega na ręcznym usunięciu folderu paczki użytkownika; nie usuwa
jej profili. Brakująca paczka nadal zachowuje zapisane ID i surową konfigurację.

Nie implementuj rozpakowywania ZIP, staging, kolejki operacji po restarcie, automatycznych
aktualizacji, rollbacku ani przełączników trwałego włączania/wyłączania. Walidacja
manifestów, zgodności i ścieżek oraz diagnostyka błędów przy starcie pozostają zadaniem
loadera. Częściowa lub błędna paczka nie może blokować pozostałych wizualizacji.

Listę pluginów pokaż jako sekcję istniejącego widoku informacji: nazwa/ID, wersja,
źródło i status ładowania oraz ewentualna diagnostyka. Lista służy do odczytu;
bez przycisków instalowania, aktualizacji, usuwania lub wyłączania. Wskaż też faktyczny
katalog `Plugins` użytkownika, aby ułatwić skorzystanie z instrukcji README.

## Budowanie, publikowanie i NuGet

Wszystkie pluginy C# mogą być zarządzanymi DLL na Windows, Linux i macOS. Nie generuj
plików `.o` dla pluginów .NET. Wtyczka oparta wyłącznie na przenośnym kodzie i Avalonia
może korzystać z jednego DLL; biblioteki natywne wymagają paczki dla konkretnego RID
i architektury oraz rozwiązania ścieżek natywnych przez loader. Na pierwszym etapie
16 wbudowanych pluginów nie powinno wprowadzać nowych natywnych zależności.

Zachowaj publikację self-contained i wyłączony trimming. Nie włączaj NativeAOT dla
hosta używającego dynamicznych zarządzanych pluginów. Single-file hosta na macOS może
pozostać, ale biblioteki pluginów i ich pliki to zewnętrzne paczki.

Proces budowania musi najpierw zbudować biblioteki wspólne oraz pluginy, a następnie
umieścić paczki w odpowiednim output/publish. Nie wystarcza zbudowanie rozwiązania:
przetestuj też `dotnet publish` i istniejące skrypty. `build_macos.sh` już przenosi
paczki wbudowane do `Contents/Resources/Plugins` przed podpisaniem. Zachowaj istniejący
mechanizm i sprawdź kompletność paczek; nie dodawaj drugiej lokalizacji ani instalowania
paczek użytkownika wewnątrz `.app`.

NuGet SDK/UI powinien zawierać potrzebne assemblies, zasoby i deklaracje zależności.
Przykładowy zewnętrzny plugin buduj z tych NuGetów, bez referencji do kodu aplikacji
i bez ścieżek lokalnych do repozytorium. Publikowanie NuGetów do publicznego feedu
nie jest warunkiem lokalnego testu: najpierw spakuj je do lokalnego feedu testowego.
Publiczną publikację wykonuj dopiero w ramach osobno zleconego wydania.

## Kolejność implementacji i lista zadań

### Etap A — stan bazowy i kontrakty

[x] 1. Sprawdź aktualny kod, instrukcje repozytorium, niezwiązane zmiany i wersje zależności; zaktualizuj ten plan przy rozbieżnościach.

Walidacja: brak AGENTS.md w repozytorium; .NET 10 i Avalonia 12.1.2 bez zmiany wersji.
Przed rozpoczęciem jedynym niezapisanym w Git plikiem był ten plan.

[x] 2. Zapisz stan bazowy: budowanie Release, działanie 16 wizualizacji, ich domyślne parametry oraz zestaw przykładowych istniejących profili liczbowych i tekstowych.

Walidacja: przed przenoszeniem źródeł Release przeszedł bez błędów i ostrzeżeń;
utrwalono 17 profili w `tests/VisualPlayer.VisualizationChecks/Fixtures/default-profiles.json`.
Test headless sprawdził 16 wizualizacji przy skalach 1/1.5/2, odtwarzanie/pauzę/reset
oraz tekstowe i liczbowe stare `type`. Nie jest to test wizualny ani odsłuch audio.

[x] 3. Utwórz projekty `VisualPlayer.VisualizationBase`, `VisualPlayer.UI`, `VisualPlayer.Visualizations` i dodaj je do `VisualPlayer.slnx` z opisanym kierunkiem zależności.

Walidacja: cztery projekty produkcyjne budują się w Release. UI i runtime są na etapie A
szkieletami zależnymi wyłącznie od SDK; przenoszenie ich implementacji pozostaje w B/C.

[x] 4. Przenieś kontrakty klatki, wizualizacji i konfiguracji, niezależną bazę MVVM oraz potrzebne wspólne modele; zapewnij jedną tożsamość typów w całym rozwiązaniu.

Walidacja: fizycznie przeniesiono współdzielone źródła do SDK, zachowując przestrzenie
nazw dla istniejącego kodu i XAML. Test sprawdza tożsamość kontraktów, modeli i MVVM.
Obejmuje modele kolorów/gradientów, ich enumy, AHSLColor oraz współdzielone narzędzia
rysowania i bouncing. Enum VisualizationType i konfiguracja globalna pozostają na razie
w aplikacji. Baza generyczna wymaga teraz IVisualizationConfig z INotifyPropertyChanged,
a nie konkretnego ViewModelBase.

[x] 5. Zdefiniuj API pluginu, deskryptor, kontekst edytora, domyślne presety, capabilities i wersjonowanie konfiguracji; opisz odpowiedzialność za Dispose i wątek UI.

Walidacja: API w `VisualPlayer.VisualizationBase/Plugins`, opis w README SDK.
Testowy `VisualPlayer.SamplePlugin` referencjonuje tylko SDK; testuje się niezależność
konfiguracji/presetów, JSON, odrzucenie nieznanej wersji, kontekst edytora i Dispose.
API 1.0 pozostaje eksperymentalne w pakiecie 0.1.0 do weryfikacji w etapie D.

[x] 6. Zdefiniuj kontrakty tłumaczeń oraz usług hosta, bez odwołań do aplikacji i konkretnych konfiguracji odtwarzacza.

Walidacja: ITranslationScope i IVisualizationHostServices są w SDK; aktualny
TranslationContext implementuje ITranslationScope jako adapter. Usługi rozdzielają
lokalne/wspólne tłumaczenia i diagnostykę. Wczytywanie katalogów pluginów oraz ich
fallback nie zostały wyprzedzająco wdrożone — należą do etapu B.

Kryterium etapu: kontrakty kompilują się niezależnie od projektu aplikacji i runtime;
dotychczasowa aplikacja nadal działa przez przejściowe adaptery.

### Etap B — UI i tłumaczenia

[x] 7. Wydziel komponenty oraz kontrolki potrzebne edytorom, wraz z ich zależnościami, motywami, konwerterami i zasobami.

Walidacja: Components, SettingsControl/SettingsExpandControl, GradientControl,
ColorPaletteControl, ich modele i zależności, motywy bazowe oraz fonty/licencje
przeniesiono do VisualPlayer.UI. Przestrzenie nazw zachowano dla istniejącego XAML.
Słownik wejściowy to `avares://VisualPlayer.UI/Themes/Generic.axaml`.
Samodzielny test sprawdza m.in. zasoby/fonty, liczby, paletę i ScrollTextBlock.

[x] 8. Zastąp w wydzielanym UI odwołania do `ApplicationContext.Instance` zasobami motywu i przekazywanymi usługami; zapewnij wartości domyślne.

Walidacja: UI nie referencjonuje ApplicationContext, AppearanceConfig ani profilu
runtime. Motywy używają DynamicResource, a Defaults.axaml dostarcza wartości domyślne.
Adapter aplikacji UiResourceBridge publikuje kolory, colorization i opacity; test
sprawdza zmianę zasobów, wymianę AppearanceConfig i odłączanie subskrypcji.

[x] 9. Przenieś wspólny edytor kolorów do UI i przełącz go z profilu runtime na publiczny kontekst edytora.

Walidacja: VisualPlayer.UI.Views.VisualisationColorEditor przyjmuje EditorContext SDK
i edytuje jego Color w miejscu. W aplikacji pozostał mały adapter starego profilu.
Zewnętrzny SampleUiPlugin buduje widok wyłącznie z SDK/UI. Test potwierdza edycję
gradientu, podmianę kontekstu bez zmiany trybu koloru i detach/reattach edytora.
GradientControl odłącza też stare kolekcje/punkty oraz eventy po usunięciu widoku.

[x] 10. Utwórz zakresy tłumaczeń pluginów i wspólnego UI, z angielskim fallbackiem oraz jednym wyborem języka zarządzanym przez aplikację.

Walidacja: TranslationService/TranslationCatalog w SDK rozdzielają katalogi po ID,
obsługują JSON, błędy ładowania, angielski fallback i diagnostykę nieznanych kluczy.
VisualPlayer.UI zawiera 47 własnych wpisów EN/PL; polski JSON jest zasobem biblioteki.
UiCatalogs/UiResources udostępniają wspólny zakres i dynamiczne teksty motywów.
ApplicationContext przekazuje globalną zmianę języka do serwisu katalogów. Stare kopie
wpisów w głównym słowniku pozostają do końcowego porządkowania. Audyt tłumaczeń
sprawdza osobno aplikację/UI i przyszłe projekty z własnymi katalogami.

[x] 11. Sprawdź działanie zasobów `avares://`, zmianę wyglądu i zmianę języka w otwartym edytorze, także w konwerterach oraz listach enumów.

Walidacja: `tests/VisualPlayer.UiChecks` przechodzi bez załadowania assembly aplikacji.
Sprawdza zmianę tekstów i motywu otwartego edytora, etykiety wybranego enuma oraz
otwartą listę gradientów, bez zmiany konfiguracji. LocalizedEnumValue niesie zakres
do popupu; MultiBinding obserwuje też kod języka. Dodano powiadomienie `Item` wymagane
przez indeksatory Avalonia, również w starym TranslationContext. Build Release:
zero błędów/ostrzeżeń. Nadal przechodzą testy 16 wizualizacji i 17 profili oraz
niezależny audyt 588 wpisów aplikacji i 47 wpisów UI. Testy są headless na Windows;
nie zastępują późniejszej walidacji wydań na macOS/Linux.

Kryterium etapu: prosty zewnętrzny widok ustawień działa z SDK/UI bez uruchamiania
singletona aplikacji. Zachowane są dotychczasowe funkcje wydzielonych komponentów.

### Etap C — runtime, rejestr i zapis

[x] 12. Utwórz rejestr deskryptorów po ID; dodaj wewnętrzną opcję `visualplayer.none` i przejściową rejestrację obecnych implementacji.

[x] 13. Przenieś runtime i analizę danych do `VisualPlayer.Visualizations`, wstrzykując dostawcę audio, stan odtwarzania, konfigurację i diagnostykę.

[x] 14. Zastąp switch fabryki w `VisualizationCore` wywołaniem zarejestrowanego pluginu; zachowaj limity FPS, reset, zmianę skali i sprzątanie zasobów.

[x] 15. Wprowadź ID pluginu oraz wersję konfiguracji do profilu i serializację przez plugin; oddziel dane profilu runtime od kontekstu widoku SDK.

[x] 16. Zaimplementuj migrację wszystkich starych wartości `type`, kopię zapasową i atomowy zapis; sprawdź zgodność z mapą w tym dokumencie.

[x] 17. Dodaj profile niedostępnych pluginów z zachowaniem surowych danych, kopiowania profilu i późniejszego przywrócenia ustawień.

[x] 18. Zastąp `EnsureProfiles` oraz fabrykę domyślnych profili rejestrem presetów o stabilnych ID; zachowaj parametry i nazwy profili użytkownika.

[x] 19. Przebuduj `InternalViewVisualizations`: dynamiczna lista typów, nazwy z lokalizacji, edytor z fabryki, komunikaty braku pluginu i capabilities zamiast switch.

Kryterium etapu: aplikacja obsługuje nowy rejestr i stare profile; zapis profilu
niedostępnego pluginu nie usuwa jego konfiguracji ani nie zmienia jego ID.

Walidacja etapu C:

- `VisualPlayer.Visualizations` zawiera runtime, przetwarzanie danych audio, globalną
  konfigurację, rejestr i zapis profili; zależy tylko od SDK. Dostawca audio, stan
  odtwarzania, ścieżka pliku i diagnostyka są przekazywane przez hosta.
- `BuiltInVisualizations` przejściowo rejestruje 16 implementacji z aplikacji.
  Runtime i `InternalViewVisualizations` wybierają fabryki przez rejestr. Opcja
  `visualplayer.none` jest zawsze dostępna. Rozszerzenie listy nie wymaga enuma.
- Profil ma `pluginId`, `configVersion`, `profileId` i opcjonalne `presetId`.
  Zapis formatu 2 wybiera profil po ID. Pierwszy zapis starego pliku tworzy
  `visualizations.json.pre-plugins.bak`, następnie zastępuje plik zapisem atomowym.
- Testy odczytują niezmieniony fixture z etapu A: wszystkie 17 dawnych wartości
  liczbowych i tekstowych zachowuje konfigurację, kolory oraz domyślne parametry.
  Sprawdzono także nazwy/profil użytkownika, wybór po ponownym odczycie, brak
  nadpisywania istniejących presetów i zachowanie nieznanych pól JSON.
- Brak pluginu, nieobsługiwana wersja i błędna konfiguracja nie zastępują danych
  wartościami domyślnymi. Sprawdzono kopiowanie, nadpisywanie, zapis/odczyt i powrót
  pluginu. Błąd serializacji pozostawia poprzedni plik bez zmian; kopia migracji
  pozostaje niezmieniona przy kolejnych zapisach.
- Wszystkie 16 edytorów przyjmuje `VisualizationEditorContext` z SDK, używa
  przekazanych tłumaczeń i wspólnego edytora kolorów. Testy headless obejmują zmianę
  języka przy otwartej liście, edycję enumów, zachowanie rozwiniętych sekcji oraz
  usunięcie/przywrócenie pozycji dynamicznej listy typów bez utraty zapisanego ID.
- Build całego rozwiązania Release: zero błędów i ostrzeżeń. Przeszły
  `VisualPlayer.VisualizationChecks`, niezależny `VisualPlayer.UiChecks` oraz audyt
  tłumaczeń: 591 par EN/PL aplikacji i 47 par UI. Testy obejmują 16 wizualizacji
  w trzech skalach, wstrzyknięte audio, FPS, reset, Dispose i błąd rysowania pluginu.

Szczegóły API runtime i zapisu: `VisualPlayer.Visualizations/README.md`.
Polecenia i zakres testów: `tests/VisualPlayer.VisualizationChecks/README.md`.
Wbudowane implementacje i ich lokalne słowniki pozostają jeszcze w aplikacji;
przeniesienie do DLL i loader to etapy D/E. Nie wykonano jeszcze testów rzeczywistego
odtwarzania ani wydań macOS/Linux; pozostają częścią końcowej walidacji.

### Etap D — pierwszy rzeczywisty plugin DLL

[x] 20. Zaimplementuj manifest, rozwiązywanie ścieżek, politykę zgodności i deterministyczny wybór jednej paczki dla ID.

[x] 21. Zaimplementuj loader przez `AssemblyLoadContext` i `AssemblyDependencyResolver`, współdzielenie SDK/UI/Avalonia oraz obsługę błędnych dodatków.

[x] 22. Utwórz projekt `Visualizations/VisualPlayer.Visualizations.Peaks` z implementacją, konfiguracją, edytorem i domyślnym presetem o dotychczasowych wartościach.

[x] 23. Dodaj angielski słownik w DLL Peaks, jego `Lang/pl-PL.json`, manifest i generowanie kompletnej paczki.

[x] 24. Uruchom Peaks wyłącznie z zewnętrznego DLL, bez referencji aplikacji do jego typów; sprawdź ustawienia, język, kolory, zapis i ponowny start.

Walidacja etapu D:

- `VisualPlayer.Visualizations/Loading` udostępnia schemat manifestu 1, katalogi
  hosta/użytkownika i loader uruchamiany przed odczytem profili. Sprawdza API 1.0,
  `net10.0`, Avalonia 12.1.2, ścieżki, ID i zgodność deskryptora z manifestem.
  Metadane są odkrywane przed aktywacją. Kolejność wyboru: użytkownik, wersja
  malejąco, ścieżka porządkowana ordinalnie; błędny kandydat pozwala na fallback.
- Peaks przeniesiono do `Visualizations/VisualPlayer.Visualizations.Peaks`.
  Aplikacja nie zawiera jego implementacji, konfiguracji ani widoku i nie ma
  referencji projektu/assembly do pluginu. Pozostałych 15 implementacji pozostaje
  przejściowo w aplikacji. Wszystkie przyszłe projekty etapu E trafiają do tego
  samego nadrzędnego katalogu `Visualizations`.
- Plugin ma 21 własnych wpisów EN/PL, lokalny preset zgodny z fixture etapu A
  i konfigurację schematu 1. Wspólny edytor kolorów oraz enumy korzystają z UI.
  Brak pliku języka zachowuje angielski fallback z DLL.
- `VisualizationPackage.targets` generuje katalog `Package/visualplayer.peaks`
  i `visualplayer.peaks-1.0.0.zip` w output projektu. Paczka obejmuje manifest,
  DLL, `.deps.json`, języki i prywatne zależności; nie dołącza zamienników SDK/UI,
  Avalonia ani Lucide. `BuildSystemVisualizations.targets` automatycznie buduje
  projekty pod `Visualizations` i kopiuje paczki do `Plugins` output/publish hosta.
- Testy headless sprawdzają tożsamość współdzielonych typów, prywatną zależność,
  wybór użytkownika/fallback, błędny manifest, wersję, typ, konstruktor i DLL.
  Dwa osobne procesy sprawdzają zapis i ponowne odtworzenie wyboru, konfiguracji,
  kolorów i FillScreen. Test edytora działa bez głównego słownika aplikacji.
- Build rozwiązania Release oraz bezpośrednie budowanie aplikacji bez wcześniejszych
  plików restore Peaks przechodzą bez błędów i ostrzeżeń. Sprawdzono publish
  framework-dependent oraz Windows win-x64 self-contained, a także test loadera
  bezpośrednio na paczce z publikacji. Nadal przechodzą testy wszystkich 16
  wizualizacji i migracji 17 typów oraz niezależne testy UI. Audyt: 591 par EN/PL
  aplikacji, 47 UI i 21 Peaks.

Opis pakowania: `Visualizations/README.md`; polityka loadera:
`VisualPlayer.Visualizations/README.md`; testy: `tests/VisualPlayer.VisualizationChecks/README.md`.
Single-file publish nie został zweryfikowany: offline brakuje pakietu
`Microsoft.NET.ILLink.Tasks`. Nie uruchomiono wydań macOS/Linux ani rzeczywistego
odtwarzania audio. Skrypt macOS przenosi już paczki do `Contents/Resources/Plugins`,
lecz walidacja bundle/podpisu i końcowy sposób udostępniania paczek pozostają w F/G.
Aktualny zakres F poniżej zastępuje wcześniejszy pomysł instalatora ZIP i zarządzania.

Kryterium etapu: pełna pionowa ścieżka działa dla jednego dodatku. Dopiero teraz
utrwal API i rozpocznij migrację pozostałych wizualizacji.

### Etap E — osobny DLL dla każdej pozostałej wizualizacji

Każdy poniższy krok obejmuje implementację, model, widok, domyślne presety, tłumaczenia,
manifest, pakowanie i sprawdzenie odczytu istniejącego profilu. Plugin nie może wymagać
obecności DLL innej konkretnej wizualizacji. Wspólna implementacja trafia do SDK.

[x] 25. Wydziel `CircleStripes` do `VisualPlayer.Visualizations.CircleStripes`.

[x] 26. Wydziel `CircleWaveform` do `VisualPlayer.Visualizations.CircleWaveform`.

[x] 27. Wydziel `Oscilloscope` do `VisualPlayer.Visualizations.Oscilloscope`.

[x] 28. Wydziel `OscilloscopeCenter` do `VisualPlayer.Visualizations.OscilloscopeCenter`.

[x] 29. Wydziel `OscilloscopeDouble` do `VisualPlayer.Visualizations.OscilloscopeDouble`.

[x] 30. Wydziel `OscilloscopeFilled` do `VisualPlayer.Visualizations.OscilloscopeFilled`.

[x] 31. Wydziel `PeaksFloating` do `VisualPlayer.Visualizations.PeaksFloating`.

[x] 32. Wydziel `PeaksFloatingMirrored` do `VisualPlayer.Visualizations.PeaksFloatingMirrored`.

[x] 33. Wydziel `PeaksMirrored` do `VisualPlayer.Visualizations.PeaksMirrored`.

[x] 34. Wydziel `Stripes` do `VisualPlayer.Visualizations.Stripes`.

[x] 35. Wydziel `StripesMirrored` do `VisualPlayer.Visualizations.StripesMirrored`.

[x] 36. Wydziel `Waveform` do `VisualPlayer.Visualizations.Waveform`.

[x] 37. Wydziel `WaveformMirrored` do `VisualPlayer.Visualizations.WaveformMirrored`.

[x] 38. Wydziel `PeaksBouncing` do `VisualPlayer.Visualizations.PeaksBouncing`.

[x] 39. Wydziel `StripesBouncing` do `VisualPlayer.Visualizations.StripesBouncing`.

Kryterium etapu: istnieje 16 niezależnych DLL/paczek; usunięcie jednej paczki nie
uniemożliwia startu aplikacji ani korzystania z pozostałych.

Walidacja etapu E:

- Wszystkie 16 projektów znajduje się pod `Visualizations/`, w tym 15 wydzielonych
  w tym etapie. Każdy ma własną implementację, model konfiguracji, edytor,
  preset `default`, manifest, słownik angielski w DLL oraz `Lang/pl-PL.json`.
  Referencje projektów prowadzą wyłącznie do SDK/UI, bez innej konkretnej wizualizacji.
- Usunięto przejściowe `BuiltInVisualizations` i `DefaultVisualizationProfiles`
  z aplikacji. Wszystkie typy wizualizacji i edytorów są tworzone przez loader;
  aplikacja i biblioteki wspólne nie referencjonują ich assembly. Stary enum
  pozostał tylko w adapterze migracji, bez zależności pluginów i profilu runtime.
- Wartości parametrów, kolorów i angielskie nazwy wszystkich 17 starych profili
  są zgodne z niezmienionym fixture z etapu A. Pluginy zachowują stabilne ID
  i schemat konfiguracji 1. Osobny lokalny klucz presetu pozwala tłumaczyć jego
  nazwę wyświetlaną, zachowując historyczną nazwę zapisaną.
- Dodatkową wspólną zależność edytorów, `OpacityToPercentageConverter`, przeniesiono
  do UI. Nie dodano kolejnej biblioteki wspólnej. Zachowano kod rysowania,
  skalowanie, grawitację/floating bars i współdzieloną bazę bouncing w SDK.
- Każdy edytor korzysta z lokalnego zakresu tłumaczeń. Usunięto 103 zbędne wpisy
  pluginów z głównych katalogów aplikacji. Audyt osobnych zakresów przechodzi:
  488 par EN/PL aplikacji, 47 UI oraz 283 wpisy/pary w 16 katalogach pluginów.
- Testy wszystkich 16 wizualizacji obejmują rysowanie podczas odtwarzania/pauzy,
  trzy skale, reset, odczyt dawnych wartości `type` i zachowanie parametrów.
  Wszystkie edytory z DLL działają bez singletona aplikacji, zmieniają język
  przy otwartych listach i zachowują edytowaną konfigurację/kolory.
- Osobny proces sprawdza kolejno brak manifestu każdej z 16 paczek: za każdym
  razem pozostałych 15 ładuje się i rysuje, wybór niedostępnego profilu zostaje
  zachowany, a jego konfiguracja przetrwa kopiowanie i zapis/ponowny odczyt.
  Test kontroluje też brak wzajemnych referencji DLL i brak konkretnych pluginów
  w domyślnym AssemblyLoadContext. Fixture jest tworzony wyłącznie w katalogu temp.
- Build rozwiązania Release: zero błędów i ostrzeżeń. Publish Windows win-x64
  self-contained zawiera komplet 16 katalogów paczek; testy loadera wykonano
  również bezpośrednio na tym wyniku publikacji. Sprawdzono 16 kompletnych ZIP-ów
  z manifestem, DLL, `.deps.json` i językiem, bez prywatnych kopii SDK/UI/Avalonia.
  Przeszły także niezależne testy UI oraz istniejące testy restartu i błędnych paczek.

Dokumentacja projektów i paczek: `Visualizations/README.md`.
Nie przeprowadzono jeszcze testów rzeczywistego odtwarzania ani uruchomienia wydań
Linux/macOS. Ograniczenie walidacji single-file z etapu D nadal obowiązuje.
Instrukcja ręcznego instalowania paczek i lista w widoku informacji pozostają
zadaniami etapu F, zgodnie z doprecyzowanym poniżej zakresem.

### Etap F — katalog użytkownika, instalacja ręczna, informacje i dystrybucja

[ ] 40. Ujednolić lokalizację paczek użytkownika jako `Path.Combine(ConfigurationContext.DirectoryPath, "Plugins")` na wszystkich systemach i przekazać ją z hosta do loadera. Zachować istniejące katalogi paczek wbudowanych z build/publish; nie dodawać nowej lokalizacji w `.app` ani automatycznego kopiowania do ustawień.

[ ] 41. Dodać do `README.md` i `README_pl-PL.md` instrukcję ręcznej instalacji: zamknąć aplikację, rozpakować ZIP, przenieść cały folder paczki do `<katalog ustawień>/Plugins/<pluginId>/` i ponownie uruchomić aplikację. Podać rzeczywiste ścieżki dla trzech systemów i przykład drzewa plików z manifestem, DLL, `.deps.json`, językami i zależnościami; sam ZIP nie jest wykrywany przez loader.

[ ] 42. Opisać i sprawdzić ręczną aktualizację oraz usuwanie przy zamkniętej aplikacji: wymiana kompletnego folderu, kopia poprzedniej wersji poza `Plugins`, usunięcie folderu bez usuwania profili. Wyjaśnić pierwszeństwo paczki użytkownika i powrót wersji wbudowanej po usunięciu nadpisania. Bez kolejki operacji, automatycznych aktualizacji/rollbacku i przełączników wyłączania.

[ ] 43. Dodać do istniejącego widoku informacji sekcję pluginów tylko do odczytu: nazwa/ID, wersja, źródło, status/błąd ładowania oraz faktyczny katalog paczek użytkownika. Bez osobnego widoku instalacji lub zarządzania i bez akcji zmieniających paczki.

[ ] 44. Uzupełnić teksty sekcji pluginów w widoku informacji w języku angielskim i polskim; zachować audyt osobnych słowników aplikacji, UI i wszystkich pluginów. Nie dodawać tekstów nieistniejącego instalatora/menedżera.

[ ] 45. Zweryfikować istniejący build/publish: 16 kompletnych paczek wbudowanych trafia automatycznie do output/publish, a skrypt macOS umieszcza je w już obsługiwanych zasobach bundle przed podpisaniem. Sprawdzić skrypty trzech systemów i zgodność instrukcji README z układem ZIP; nie wymagać od użytkownika ręcznego dodawania bibliotek wbudowanych do `.app`.

[ ] 46. Spakuj SDK oraz UI do lokalnego feedu NuGet i zbuduj z nich przykładowy plugin spoza rozwiązania, bez dostępu do typów aplikacji/runtime.

Kryterium etapu: zewnętrzny autor może zbudować paczkę, a użytkownik zainstalować ją
ręcznie według README w `Plugins` obok ustawień, bez zmiany `.app`, katalogu
instalacyjnego ani głównych plików językowych. Po restarcie paczka jest wykrywana,
a lista w widoku informacji pokazuje jej źródło i stan. Ręczne usunięcie zachowuje
profile. Wbudowane wizualizacje pojawiają się automatycznie po build/publish.
Etap nie obejmuje instalatora ZIP ani osobnego interfejsu zarządzania pluginami.

### Etap G — walidacja i zakończenie migracji

[ ] 47. Dodaj testy rejestru, manifestów, wersji API, duplikatów ID, brakujących zależności i dwóch pluginów z różnymi prywatnymi zależnościami.

[ ] 48. Dodaj testy migracji wszystkich 17 dawnych wartości (w tym None), wersjonowania konfiguracji, kopii profili i zachowania nieznanych danych przy braku/błędzie pluginu.

[ ] 49. Dodaj testy lokalizacji: brak polskiego wpisu, brak pliku języka, kolizje kluczy między pluginami, wspólne napisy, zmiana języka w otwartym edytorze.

[ ] 50. Dodać testy ręcznego umieszczenia, wymiany i usunięcia paczek między uruchomieniami: katalog ustawień/Plugins, poprawna i częściowo skopiowana paczka, błędny manifest/ścieżki/zależności, nadpisanie wersji wbudowanej i fallback po usunięciu. Sprawdzić zachowanie profili oraz zgodność listy w widoku informacji. Bez testów nieimplementowanego instalatora ZIP.

[ ] 51. Sprawdź błędy konstruktora, konfiguracji, edytora i rysowania pluginu; odtwarzanie audio ma działać nadal, a profil ma pozostać zapisany.

[ ] 52. Sprawdź wszystkie 16 wizualizacji i zmianę profili podczas odtwarzania, pauzę, stop, skalowanie DPI, limity FPS, zużycie pamięci i odłączanie eventów.

[ ] 53. Sprawdź regresje mini odtwarzacza, jego wizualizacji i logo po przeniesieniu dostępu do danych audio; sprawdź wygląd współdzielonych komponentów w pozostałych widokach.

[ ] 54. Uruchom wydanie publikowane na Windows: ręczne rozpakowanie/przeniesienie paczki do `Plugins` w katalogu ustawień, wykrywanie pluginów i lista informacji, ponowny start, aktualizacja/usunięcie ręczne oraz zachowanie profili.

[ ] 55. Uruchom wydanie publikowane na Linux: `Plugins` w rzeczywistym katalogu ustawień, start z katalogu instalacji bez prawa zapisu, ręczne dodanie/wymiana/usunięcie paczki, wykrywanie, lista informacji i restart.

[ ] 56. Uruchom gotowy `.app` na macOS: automatycznie dołączone wizualizacje wbudowane, DLL spoza single-file, podpis, ręczne dodanie/wymiana/usunięcie paczki w `Plugins` obok ustawień bez modyfikowania bundle, lista informacji i ponowny start.

[ ] 57. Usuń przejściowe fabryki i typowane zależności aplikacji od pluginów, stary enum poza adapterem migracji oraz nieużywane kopie konfiguracji, widoków i tłumaczeń.

[ ] 58. Uzupełnij dokumentację tworzenia dodatku, przykład projektu, politykę zgodności API i instrukcję pakowania ZIP oraz ręcznego rozpakowania/przeniesienia, aktualizacji i usuwania paczek według README.

[ ] 59. Wykonaj końcowy przegląd grafu zależności, budowanie rozwiązania/publish, audyt tłumaczeń i podsumuj rzeczywiście wykonane testy oraz pozostałe ograniczenia.

## Warunki uznania całości za gotową

Wszystkie kroki są wykonane albo ich niewykonanie jest jawnie opisane. Nową
wizualizację można dodać bez zmiany aplikacji, enuma, głównego słownika ani switcha
wybierającego edytor. Wszystkie obecne 16 wizualizacji ma osobny DLL i własne słowniki.
Host uruchamia się bez nich. Stare profile działają po migracji, a profile brakujących
pluginów zachowują dane. Przykład spoza repozytorium potwierdza użyteczność NuGetów.
Ręczna instalacja według README, aktualizacja/usuwanie i publikacja są sprawdzone
na wszystkich trzech systemach. Wbudowane paczki dołącza build/publish, a dodatki
użytkownika są w `Plugins` wewnątrz katalogu ustawień. Lista w widoku informacji
nie służy do zmieniania paczek.

Nie wliczaj do pierwszego wydania: hot reload, gwarantowane wyładowywanie assembly,
instalatora ZIP, osobnego menedżera pluginów, kolejek aktualizacji/usuwania, sklepu
dodatków, automatycznego pobierania z internetu, izolacji procesowej, obsługi
arbitralnych wersji Avalonia ani publikacji hosta przez NativeAOT.

## Dokumentacja referencyjna do implementacji

Przy wdrożeniu sprawdź aktualną dokumentację dla używanych wersji:

- [Ładowanie pluginów .NET i współdzielenie kontraktów](https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support)
- [Ładowanie bibliotek natywnych](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/native-library-loading)
- [Ograniczenia NativeAOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Biblioteki kontrolek i zasoby Avalonia](https://docs.avaloniaui.net/docs/custom-controls/custom-control-library)
- [Katalogi danych użytkownika na Linux](https://specifications.freedesktop.org/basedir/0.8/)
- [Application Support na macOS](https://developer.apple.com/documentation/foundation/url/applicationsupportdirectory)
