using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualPlayer.Attributes;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Models.Player;
using VisualPlayer.Providers.Lyrics;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.ViewModels.Lyrics;
using VisualPlayer.Utils.Lyrics;

namespace VisualPlayer.Core;

public class LyricsCore : ViewModelBase, IDisposable
{
    #region CONST

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    #endregion CONST

    private LyricsDocument? _currentDocument;
    private LyricsLine? _currentLine;
    private Track? _currentTrack;
    private CancellationTokenSource? _automaticDownloadCancellation;
    private readonly ILyricsProvider _lyricsProvider;


    public LyricsConfig Config { get; }
    public ObservableCollection<LyricsDocument> Documents { get; } = [];

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public LyricsDocument? CurrentDocument
    {
        get => _currentDocument;
        private set
        {
            if (ReferenceEquals(_currentDocument, value))
                return;

            UpdateProperty(ref _currentDocument, value);
            NotifyPropertyChanged(nameof(HasCurrentTrackLyrics));
        }
    }

    [NotifyPropertyUpdate]
    public LyricsLine? CurrentLine
    {
        get => _currentLine;
        private set => UpdateProperty(ref _currentLine, value);
    }

    public bool HasCurrentTrackLyrics
    {
        get => CurrentDocument is not null;
    }

    #endregion PROPERTIES

    #region CONSTRUcTORS

    public LyricsCore(LyricsConfig config)
    {
        Config = config;
        _lyricsProvider = new LrcLibLyricsProvider(ConfigurationContext.LyricsCacheDirectoryPath);
        Config.PropertyChanged += Config_PropertyChanged;
        LoadLibrary();
    }

    #endregion CONSTRUcTORS

    #region CONFIGURATION

    private void Config_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_currentTrack is null)
            return;

        if (e.PropertyName == nameof(LyricsConfig.UseEmbeddedLyrics)
            || e.PropertyName == nameof(LyricsConfig.AutoImportEmbeddedLyrics)
            || e.PropertyName == nameof(LyricsConfig.AutoDownloadLyrics))
        {
            SetCurrentTrack(_currentTrack);
        }
    }

    #endregion CONFIGURATION

    public void AssignToCurrentTrack(LyricsDocument document)
    {
        if (_currentTrack is null)
            return;

        foreach (var item in Documents.Where(item => PathsEqual(item.TrackFilePath, _currentTrack.FilePath)))
        {
            item.TrackFilePath = null;
            Save(item);
        }

        document.TrackFilePath = _currentTrack.FilePath;
        foreach (var item in Documents)
            item.IsCurrentTrack = ReferenceEquals(item, document);

        CurrentDocument = document;
        Save(document);
    }

    public LyricsDocument CreateForCurrentTrack()
    {
        if (_currentTrack is null)
            throw new InvalidOperationException("Nie można utworzyć tekstu bez aktualnego nagrania.");

        var existing = CurrentDocument is not null
            && PathsEqual(CurrentDocument.TrackFilePath, _currentTrack.FilePath)
                ? CurrentDocument
                : FindForTrack(_currentTrack.FilePath);

        if (existing is not null)
            return existing;

        var document = new LyricsDocument
        {
            Name = _currentTrack.DisplayTitle,
            TrackFilePath = _currentTrack.FilePath,
            Type = Config.PreferredType
        };

        Documents.Add(document);
        CurrentDocument = document;
        Save(document);
        return document;
    }

    public LyricsDocument CreateNewForCurrentTrack(bool copyCurrentDocument = false)
    {
        if (_currentTrack is null)
            throw new InvalidOperationException("Nie można utworzyć tekstu bez aktualnego nagrania.");

        var previousDocument = CurrentDocument;

        foreach (var document in Documents.Where(document => PathsEqual(
                     document.TrackFilePath,
                     _currentTrack.FilePath)))
        {
            document.TrackFilePath = null;
            document.IsCurrentTrack = false;
            Save(document);
        }

        var newDocument = new LyricsDocument
        {
            Name = _currentTrack.DisplayTitle,
            TrackFilePath = _currentTrack.FilePath,
            Type = previousDocument?.Type ?? Config.PreferredType
        };

        if (copyCurrentDocument && previousDocument is not null)
        {
            foreach (var line in previousDocument.Lines)
            {
                newDocument.Lines.Add(new LyricsLine
                {
                    End = line.End,
                    Start = line.Start,
                    Text = line.Text,
                    Segments = new ObservableCollection<Models.Lyrics.LyricsSegment>(
                        line.Segments.Select(segment => new Models.Lyrics.LyricsSegment
                        {
                            End = segment.End,
                            Start = segment.Start,
                            Text = segment.Text
                        }))
                });
            }
        }

        newDocument.IsCurrentTrack = true;
        Documents.Add(newDocument);
        CurrentDocument = newDocument;
        CurrentLine = null;
        Save(newDocument);
        return newDocument;
    }

    public void Delete(LyricsDocument document)
    {
        Documents.Remove(document);

        var filePath = Path.Combine(
            ConfigurationContext.LyricsDirectoryPath,
            document.StorageFileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        if (ReferenceEquals(CurrentDocument, document))
        {
            CurrentDocument = null;
            CurrentLine = null;
        }
    }

    public string GetStoragePath(LyricsDocument document)
    {
        return Path.Combine(ConfigurationContext.LyricsDirectoryPath, document.StorageFileName);
    }

    public LyricsDocument? ImportLrc(string lrcPath, Track? track = null)
    {
        if (!File.Exists(lrcPath))
            return null;

        return ImportText(
            File.ReadAllText(lrcPath),
            Path.GetFileNameWithoutExtension(lrcPath),
            track);
    }

    public LyricsDocument? ImportText(
        string content,
        string fallbackName,
        Track? track = null)
    {
        var trackFilePath = track?.FilePath;
        var document = LrcParser.Parse(
            content,
            trackFilePath,
            track?.DisplayTitle ?? fallbackName);

        if (document.Lines.Count == 0)
            return null;

        if (track is not null)
        {
            foreach (var existing in Documents.Where(item => PathsEqual(item.TrackFilePath, track.FilePath)))
            {
                existing.TrackFilePath = null;
                existing.IsCurrentTrack = false;
                Save(existing);
            }

            document.IsCurrentTrack = PathsEqual(_currentTrack?.FilePath, track.FilePath);
        }

        Documents.Add(document);
        Save(document);

        if (document.IsCurrentTrack)
            CurrentDocument = document;

        return document;
    }

    public async Task<IReadOnlyList<LyricsProviderResult>> SearchOnlineAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return await _lyricsProvider.SearchAsync(query, cancellationToken);
    }

    public async Task<IReadOnlyList<LyricsProviderResult>> SearchOnlineAsync(
        Track track,
        CancellationToken cancellationToken = default)
    {
        return await _lyricsProvider.SearchAsync(track, cancellationToken);
    }

    public LyricsDocument DownloadOnlineResult(LyricsProviderResult result)
    {
        var providerId = result.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var existing = Documents.FirstOrDefault(document => document.ProviderName == _lyricsProvider.Name
            && document.ProviderId == providerId);

        if (existing is not null)
        {
            if (_currentTrack is not null && IsSafeMatch(result, _currentTrack))
                AssignToCurrentTrack(existing);

            return existing;
        }

        var currentTrack = _currentTrack;
        var assignToCurrentTrack = currentTrack is not null && IsSafeMatch(result, currentTrack);
        var document = CreateOnlineDocument(result, assignToCurrentTrack ? currentTrack : null);
        Documents.Add(document);

        if (assignToCurrentTrack)
            AssignToCurrentTrack(document);
        else
            Save(document);

        return document;
    }

    public void Save(LyricsDocument document)
    {
        Directory.CreateDirectory(ConfigurationContext.LyricsDirectoryPath);

        if (!Documents.Contains(document))
            Documents.Add(document);

        SortLines(document);

        var filePath = Path.Combine(
            ConfigurationContext.LyricsDirectoryPath,
            document.StorageFileName);
        var json = JsonSerializer.Serialize(document, JsonOptions);
        File.WriteAllText(filePath, json);
    }

    public void SetCurrentTrack(Track? track)
    {
        _automaticDownloadCancellation?.Cancel();
        _automaticDownloadCancellation?.Dispose();
        _automaticDownloadCancellation = null;
        _currentTrack = track;

        foreach (var document in Documents)
            document.IsCurrentTrack = track is not null && PathsEqual(document.TrackFilePath, track.FilePath);

        CurrentDocument = track is null
            ? null
            : FindForTrack(track.FilePath);

        if (track is not null && CurrentDocument is null)
            CurrentDocument = TryImportAdjacentLrc(track);

        if (track is not null && CurrentDocument is null && Config.UseEmbeddedLyrics)
            CurrentDocument = TryLoadEmbeddedLyrics(track);

        CurrentLine = null;

        if (track is not null && CurrentDocument is null && Config.AutoDownloadLyrics)
        {
            _automaticDownloadCancellation = new CancellationTokenSource();
            _ = TryAutomaticDownloadAsync(track, _automaticDownloadCancellation.Token);
        }
    }

    public void UpdatePosition(TimeSpan position)
    {
        var lines = CurrentDocument?.Lines
            .OrderBy(line => line.Start)
            .ToArray();

        if (lines is null || lines.Length == 0)
        {
            CurrentLine = null;
            return;
        }

        var low = 0;
        var high = lines.Length - 1;
        var result = -1;

        while (low <= high)
        {
            var middle = low + (high - low) / 2;

            if (lines[middle].Start <= position)
            {
                result = middle;
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        CurrentLine = result >= 0
            ? lines[result]
            : null;
    }


    private LyricsDocument? FindForTrack(string filePath)
    {
        return Documents
            .Where(document => PathsEqual(document.TrackFilePath, filePath))
            .OrderByDescending(document => document.Type)
            .FirstOrDefault();
    }

    private static string GetAdjacentLrcPath(string trackFilePath)
    {
        return Path.ChangeExtension(trackFilePath, ".lrc");
    }

    private LyricsDocument? TryImportAdjacentLrc(Track track)
    {
        var lrcPath = GetAdjacentLrcPath(track.FilePath);

        try
        {
            return ImportLrc(lrcPath, track);
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private LyricsDocument? TryLoadEmbeddedLyrics(Track track)
    {
        var document = EmbeddedLyricsReader.Read(track);

        if (document is null)
            return null;

        document.IsCurrentTrack = true;

        if (Config.AutoImportEmbeddedLyrics)
        {
            Documents.Add(document);
            Save(document);
        }

        return document;
    }

    private LyricsDocument CreateOnlineDocument(LyricsProviderResult result, Track? track)
    {
        var useSynced = Config.PreferredType != LyricsType.Plain
            && !string.IsNullOrWhiteSpace(result.SyncedLyrics);
        var content = useSynced
            ? result.SyncedLyrics!
            : result.PlainLyrics ?? result.SyncedLyrics ?? string.Empty;
        var document = LrcParser.Parse(content, track?.FilePath, result.TrackName);

        document.Album = result.AlbumName;
        document.Artist = result.ArtistName;
        document.Name = result.TrackName;
        document.ProviderId = result.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
        document.ProviderName = _lyricsProvider.Name;
        document.Source = LyricsSource.Online;
        document.TrackFilePath = track?.FilePath;

        if (!useSynced)
            document.Type = LyricsType.Plain;

        return document;
    }

    private async Task TryAutomaticDownloadAsync(Track track, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(track.Artist))
            return;

        try
        {
            var results = await _lyricsProvider.SearchAsync(track, cancellationToken);
            var match = results
                .Where(result => HasLyricsForPreferredType(result))
                .Where(result => IsSafeMatch(result, track))
                .OrderBy(result => GetMatchDistance(result, track))
                .FirstOrDefault();

            if (match is null
                || cancellationToken.IsCancellationRequested
                || !ReferenceEquals(_currentTrack, track)
                || CurrentDocument is not null)
            {
                return;
            }

            DownloadOnlineResult(match);
        }
        catch (OperationCanceledException)
        {
            // Changing the track cancels the previous automatic lookup.
        }
        catch (HttpRequestException)
        {
            // An automatic network failure must not interrupt playback.
        }
    }

    private bool HasLyricsForPreferredType(LyricsProviderResult result)
    {
        return Config.PreferredType switch
        {
            LyricsType.Plain => !string.IsNullOrWhiteSpace(result.PlainLyrics)
                || !string.IsNullOrWhiteSpace(result.SyncedLyrics),
            _ => !string.IsNullOrWhiteSpace(result.SyncedLyrics)
                || !string.IsNullOrWhiteSpace(result.PlainLyrics)
        };
    }

    private static bool IsSafeMatch(LyricsProviderResult result, Track track)
    {
        if (!string.Equals(NormalizeMatchText(result.TrackName), NormalizeMatchText(track.DisplayTitle), StringComparison.Ordinal)
            || !string.Equals(NormalizeMatchText(result.ArtistName), NormalizeMatchText(track.Artist), StringComparison.Ordinal))
        {
            return false;
        }

        if (track.Duration > TimeSpan.Zero && result.Duration > 0)
        {
            var tolerance = Math.Max(3, track.Duration.TotalSeconds * 0.02);
            return Math.Abs(result.Duration - track.Duration.TotalSeconds) <= tolerance;
        }

        return !string.IsNullOrWhiteSpace(track.Album)
            && string.Equals(NormalizeMatchText(result.AlbumName), NormalizeMatchText(track.Album), StringComparison.Ordinal);
    }

    private static double GetMatchDistance(LyricsProviderResult result, Track track)
    {
        return track.Duration > TimeSpan.Zero && result.Duration > 0
            ? Math.Abs(result.Duration - track.Duration.TotalSeconds)
            : 0;
    }

    private static string NormalizeMatchText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character)
                == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
                result.Append(char.ToLowerInvariant(character));
        }

        return result.ToString();
    }

    public void Dispose()
    {
        Config.PropertyChanged -= Config_PropertyChanged;
        _automaticDownloadCancellation?.Cancel();
        _automaticDownloadCancellation?.Dispose();

        if (_lyricsProvider is IDisposable disposableProvider)
            disposableProvider.Dispose();
    }

    private void LoadLibrary()
    {
        Directory.CreateDirectory(ConfigurationContext.LyricsDirectoryPath);

        foreach (var filePath in Directory.EnumerateFiles(
                     ConfigurationContext.LyricsDirectoryPath,
                     "*.lyrics.json"))
        {
            try
            {
                var document = JsonSerializer.Deserialize<LyricsDocument>(
                    File.ReadAllText(filePath),
                    JsonOptions);

                if (document is not null)
                    Documents.Add(document);
            }
            catch (JsonException)
            {
                // A damaged entry must not prevent the remaining library from loading.
            }
            catch (IOException)
            {
                // An unavailable entry can be loaded on the next application start.
            }
        }
    }

    private static bool PathsEqual(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
            return false;

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        return string.Equals(
            Path.GetFullPath(first),
            Path.GetFullPath(second),
            comparison);
    }

    private static void SortLines(LyricsDocument document)
    {
        var ordered = document.Lines
            .OrderBy(line => line.Start)
            .ToArray();

        if (ordered.SequenceEqual(document.Lines))
            return;

        document.Lines.Clear();

        foreach (var line in ordered)
            document.Lines.Add(line);
    }
}
