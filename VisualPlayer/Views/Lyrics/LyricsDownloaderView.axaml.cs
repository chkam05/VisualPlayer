using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VisualPlayer.Contexts;
using VisualPlayer.Providers.Lyrics;
using VisualPlayer.ViewModels.Lyrics;

namespace VisualPlayer.Views.Lyrics;

public partial class LyricsDownloaderView : UserControl
{
    private string _searchPhrase = string.Empty;
    private string _status = "Wyszukaj tekst w LRCLIB.";
    private CancellationTokenSource? _searchCancellation;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<LyricsDownloaderView, string> SearchPhraseProperty
        = AvaloniaProperty.RegisterDirect<LyricsDownloaderView, string>(nameof(SearchPhrase), view => view.SearchPhrase, (view, value) => view.SearchPhrase = value);

    public static readonly DirectProperty<LyricsDownloaderView, string> StatusProperty
        = AvaloniaProperty.RegisterDirect<LyricsDownloaderView, string>(nameof(Status), view => view.Status);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public ObservableCollection<LyricsSearchResultViewModel> Results { get; } = [];

    public string SearchPhrase
    {
        get => _searchPhrase;
        set => SetAndRaise(SearchPhraseProperty, ref _searchPhrase, value ?? string.Empty);
    }

    public string Status
    {
        get => _status;
        private set => SetAndRaise(StatusProperty, ref _status, value);
    }

    #endregion AVALONIA PROPERTIES

    #region CONSTRUCTORS

    public LyricsDownloaderView()
    {
        InitializeComponent();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private async void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        await SearchAsync(cancellationToken => ApplicationContext.Instance.LyricsCore.SearchOnlineAsync(
            SearchPhrase,
            cancellationToken));
    }

    private async void SearchCurrentTrackButton_Click(object? sender, RoutedEventArgs e)
    {
        var track = ApplicationContext.Instance.PlayerCore.CurrentTrack;

        if (track is null)
        {
            Status = "Brak aktualnie odtwarzanego nagrania.";
            return;
        }

        SearchPhrase = $"{track.Artist} {track.DisplayTitle}".Trim();

        await SearchAsync(cancellationToken => ApplicationContext.Instance.LyricsCore.SearchOnlineAsync(
            track,
            cancellationToken));
    }

    private void DownloadButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: LyricsSearchResultViewModel result })
            return;

        var document = ApplicationContext.Instance.LyricsCore.DownloadOnlineResult(result.Result);
        Status = $"Pobrano „{document.SuggestedName}” do lokalnej biblioteki.";
    }

    private async Task SearchAsync(
        Func<CancellationToken, Task<System.Collections.Generic.IReadOnlyList<LyricsProviderResult>>> search)
    {
        _searchCancellation?.Cancel();
        _searchCancellation?.Dispose();
        _searchCancellation = new CancellationTokenSource();
        Results.Clear();
        Status = "Wyszukiwanie w LRCLIB…";

        try
        {
            var results = await search(_searchCancellation.Token);

            foreach (var result in results.Where(result => !result.Instrumental
                         && (!string.IsNullOrWhiteSpace(result.SyncedLyrics)
                             || !string.IsNullOrWhiteSpace(result.PlainLyrics))))
            {
                Results.Add(new LyricsSearchResultViewModel(result));
            }

            Status = Results.Count == 0
                ? "Nie znaleziono kompatybilnego tekstu."
                : $"Znaleziono wyników: {Results.Count}.";
        }
        catch (OperationCanceledException)
        {
            // A newer search replaced this request.
        }
        catch (HttpRequestException exception)
        {
            Status = $"Nie udało się pobrać wyników: {exception.Message}";
        }

    }

    #endregion ACTIONS
}
