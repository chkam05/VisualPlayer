using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VisualPlayer.Events;
using VisualPlayer.Contexts;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Core;

public class PlayListCore : IDisposable
{
    private readonly SemaphoreSlim _addFilesSemaphore = new(1, 1);
    private readonly CoverCache _coverCache;
    private readonly ShuffleCycle _shuffleCycle;
    private readonly TranslationContext _translations;

    private bool _isDisposed;
    private int? _removedTrackPreviousIndex;
    private int _shuffleIndex = -1;

    #region EVENTS

    public event EventHandler<TrackEventArgs>? PlayRequested;

    #endregion EVENTS

    #region PROPERTIES

    public Track? CurrentTrack { get; private set; }
    internal CoverCache CoverCache => _coverCache;
    public int CurrentTrackIndex
    {
        get => CurrentTrack is null ? -1 : PlayList.Tracks.IndexOf(CurrentTrack);
    }
    public PlayList PlayList { get; private set; }
    public PlayListViewModel PlayListViewModel { get; private set; }
    public Track? RestoredTrack { get; private set; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PlayListCore(TranslationContext translations)
    {
        _translations = translations;
        _coverCache = new CoverCache();
        PlayList = new PlayList();
        _shuffleCycle = new ShuffleCycle(PlayList.Tracks.Count);
        PlayListViewModel = new PlayListViewModel(this, _coverCache, translations);
        _translations.LanguageChanged += Translations_LanguageChanged;

        PlayList.Tracks.CollectionChanged += Tracks_CollectionChanged;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        PlayList.Tracks.CollectionChanged -= Tracks_CollectionChanged;
        _translations.LanguageChanged -= Translations_LanguageChanged;
        _addFilesSemaphore.Dispose();
        _isDisposed = true;
    }

    #endregion CONSTRUCTORS

    #region PLAYLIST MANAGEMENT

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        PlayListViewModel.RefreshTranslations();
    }

    public void AddTrack(Track track, byte[]? coverData = null)
    {
        ArgumentNullException.ThrowIfNull(track);

        if (PlayList.Tracks.Contains(track) || ContainsTrack(track.FilePath))
            return;

        if (coverData is not null)
            track.CoverId = _coverCache.Add(coverData);

        PlayList.Tracks.Add(track);
        PlayListViewModel.AddTrack(track);
    }

    public void Clear()
    {
        foreach (var track in PlayListViewModel.Tracks.ToList())
            RemoveTrack(track.Model);

        RestoredTrack = null;
        PlayListViewModel.FinishClear();
    }

    public bool ContainsTrack(string filePath)
    {
        return PlayList.ContainsTrack(filePath);
    }

    public void MoveTrack(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= PlayList.Tracks.Count)
            return;

        if (newIndex < 0 || newIndex >= PlayList.Tracks.Count)
            return;

        if (oldIndex == newIndex)
            return;

        PlayList.Tracks.Move(oldIndex, newIndex);
        PlayListViewModel.MoveTrackInternal(oldIndex, newIndex);
    }

    public void RemoveTrack(Track track)
    {
        var trackViewModel = PlayListViewModel.Tracks
            .FirstOrDefault(item => ReferenceEquals(item.Model, track));

        if (trackViewModel is null)
            return;

        if (ReferenceEquals(RestoredTrack, track))
            RestoredTrack = null;

        PlayList.Tracks.Remove(track);
        PlayListViewModel.RemoveTrackInternal(trackViewModel, ReferenceEquals(CurrentTrack, track));
    }

    #endregion PLAYLIST MANAGEMENT

    #region FILES MANAGEMENT

    public IReadOnlyList<Track> AddFiles(IEnumerable<string> filePaths)
    {
        var addedTracks = new List<Track>();

        foreach (var filePath in filePaths)
        {
            var track = AddFile(filePath);

            if (track is not null)
                addedTracks.Add(track);
        }

        return addedTracks;
    }

    public async Task<IReadOnlyList<Track>> AddFilesAsync(IEnumerable<string> filePaths)
    {
        var addedTracks = new List<Track>();
        await _addFilesSemaphore.WaitAsync();

        try
        {
            foreach (var filePath in filePaths)
            {
                var normalizedPath = AudioFileUtils.NormalizePath(filePath);

                if (normalizedPath is null || !AudioFileUtils.IsSupported(normalizedPath))
                    continue;

                if (ContainsTrack(normalizedPath))
                    continue;

                var track = new Track
                {
                    FilePath = normalizedPath
                };

                byte[]? coverData = null;
                await Task.Run(() => TrackMetadataUtils.TryLoadMetadata(track, out coverData));
                AddTrack(track, coverData);
                addedTracks.Add(track);
            }
        }
        finally
        {
            _addFilesSemaphore.Release();
        }

        return addedTracks;
    }

    public IReadOnlyList<Track> AddStoredTracks(IEnumerable<Track> storedTracks)
    {
        var addedTracks = new List<Track>();

        foreach (var storedTrack in storedTracks)
        {
            var track = AddStoredTrack(storedTrack);
            if (track is not null)
                addedTracks.Add(track);
        }

        return addedTracks;
    }

    public IReadOnlyList<Track> ReplaceWith(IEnumerable<Track> storedTracks)
    {
        Clear();
        return AddStoredTracks(storedTracks);
    }


    private Track? AddFile(string filePath)
    {
        var normalizedPath = AudioFileUtils.NormalizePath(filePath);

        if (normalizedPath is null || !AudioFileUtils.IsSupported(normalizedPath))
            return null;

        if (ContainsTrack(normalizedPath))
            return null;

        var track = new Track
        {
            FilePath = normalizedPath
        };

        TrackMetadataUtils.TryLoadMetadata(track, out var coverData);
        AddTrack(track, coverData);
        return track;
    }

    private Track? AddStoredTrack(Track storedTrack)
    {
        var normalizedPath = AudioFileUtils.NormalizePath(storedTrack.FilePath);

        if (normalizedPath is null || !AudioFileUtils.IsSupported(normalizedPath))
            return null;

        if (ContainsTrack(normalizedPath))
            return null;

        var track = new Track
        {
            FilePath = normalizedPath,
            Album = storedTrack.Album,
            Artist = storedTrack.Artist,
            Duration = storedTrack.Duration,
            Title = storedTrack.Title
        };

        TrackMetadataUtils.TryLoadMetadata(track, out var coverData);
        AddTrack(track, coverData);
        return track;
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            var json = File.ReadAllText(filePath);
            var playListFile = JsonSerializer.Deserialize<PlayListFile>(json);

            if (playListFile is null)
                return;

            PlayList.Name = playListFile.Name;
            PlayListViewModel.Name = playListFile.Name;
            foreach (var storedTrack in playListFile.Tracks)
                AddStoredTrack(storedTrack);

            RestoredTrack = string.IsNullOrWhiteSpace(playListFile.CurrentTrackPath)
                ? null
                : PlayList.Tracks.FirstOrDefault(track => PathsEqual(track.FilePath, playListFile.CurrentTrackPath));
        }
        catch (JsonException)
        {
            RestoredTrack = null;
        }
        catch (IOException)
        {
            RestoredTrack = null;
        }
    }

    public void Save(string filePath)
    {
        var directoryPath = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var playListFile = new PlayListFile
        {
            Name = PlayList.Name,
            CurrentTrackPath = (CurrentTrack ?? RestoredTrack)?.FilePath,
            Tracks = PlayList.Tracks.ToList()
        };

        var json = JsonSerializer.Serialize(playListFile, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }

    private static bool PathsEqual(string firstPath, string secondPath)
    {
        var comparisonType = OperatingSystem.IsLinux()
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        return string.Equals(firstPath, secondPath, comparisonType);
    }

    #endregion FILES MANAGEMENT

    #region PLAYBACK REQUESTS

    internal void RequestPlay(Track track)
    {
        if (PlayList.Tracks.Contains(track))
            PlayRequested?.Invoke(this, new TrackEventArgs(track));
    }

    internal void SetCurrentTrack(Track? track, bool isPlaying)
    {
        CurrentTrack = track;
        _removedTrackPreviousIndex = null;

        if (track is not null)
            _shuffleIndex = PlayList.Tracks.IndexOf(track);

        PlayListViewModel.SetCurrentTrack(track, isPlaying);
    }

    internal void SetIsPlaying(bool isPlaying)
    {
        PlayListViewModel.SetIsPlaying(isPlaying);
    }

    #endregion PLAYBACK REQUESTS

    #region NAVIGATION

    public IReadOnlyList<Track> GetInitialTracks(bool shuffle, bool repeatAll)
    {
        return GetTracksFromIndex(0, 1, shuffle, repeatAll, resetShuffle: shuffle);
    }

    public IReadOnlyList<Track> GetTracksFrom(Track track, bool shuffle, bool repeatAll)
    {
        var index = PlayList.Tracks.IndexOf(track);
        if (index < 0)
            return Array.Empty<Track>();

        return GetTracksFromIndex(index, 1, shuffle, repeatAll, resetShuffle: shuffle);
    }

    private IReadOnlyList<Track> GetTracksFromIndex(
        int startIndex, int direction, bool shuffle, bool repeatAll, bool resetShuffle)
    {
        if (!shuffle)
            return GetSequentialTracks(startIndex, direction, repeatAll);

        if (resetShuffle)
        {
            var firstTrack = startIndex >= 0 && startIndex < PlayList.Tracks.Count
                ? PlayList.Tracks[startIndex]
                : null;
            ResetShuffle(firstTrack);
        }

        return GetShuffleTracks(direction > 0, repeatAll);
    }

    public IReadOnlyList<Track> GetNextTracks(bool shuffle, bool repeatAll)
    {
        if (shuffle)
            return GetShuffleTracks(forward: true, repeatAll);

        var startIndex = CurrentTrackIndex >= 0
            ? CurrentTrackIndex + 1
            : (_removedTrackPreviousIndex ?? -1) + 1;

        return GetSequentialTracks(startIndex, 1, repeatAll);
    }

    public IReadOnlyList<Track> GetPreviousTracks(bool shuffle, bool repeatAll)
    {
        if (shuffle)
            return GetShuffleTracks(forward: false, repeatAll);

        var startIndex = CurrentTrackIndex >= 0
            ? CurrentTrackIndex - 1
            : _removedTrackPreviousIndex ?? -1;

        return GetSequentialTracks(startIndex, -1, repeatAll);
    }

    private IReadOnlyList<Track> GetSequentialTracks(int startIndex, int direction, bool repeatAll)
    {
        var count = PlayList.Tracks.Count;
        if (count == 0)
            return Array.Empty<Track>();

        var tracks = new List<Track>(count);

        for (var attempt = 0; attempt < count; attempt++)
        {
            var candidateIndex = startIndex + direction * attempt;

            if (repeatAll)
                candidateIndex = ((candidateIndex % count) + count) % count;
            else if (candidateIndex < 0 || candidateIndex >= count)
                break;

            tracks.Add(PlayList.Tracks[candidateIndex]);
        }

        return tracks;
    }

    private IReadOnlyList<Track> GetShuffleTracks(bool forward, bool repeatAll)
    {
        var count = PlayList.Tracks.Count;
        if (count == 0)
            return Array.Empty<Track>();

        var tracks = new List<Track>(count);
        var attempted = new HashSet<int>();
        var index = _shuffleIndex;

        while (attempted.Count < count)
        {
            var candidateIndex = forward
                ? _shuffleCycle.GetNextIndex(index)
                : _shuffleCycle.GetPreviousIndex(index);

            if (candidateIndex < 0)
            {
                if (!forward || !repeatAll)
                    break;

                _shuffleCycle.Rebuild(count);
                index = -1;
                continue;
            }

            index = candidateIndex;

            if (attempted.Add(candidateIndex))
                tracks.Add(PlayList.Tracks[candidateIndex]);
        }

        return tracks;
    }

    public void ResetShuffle(Track? firstTrack = null)
    {
        var firstIndex = firstTrack is null ? -1 : PlayList.Tracks.IndexOf(firstTrack);
        _shuffleCycle.Rebuild(PlayList.Tracks.Count, firstIndex >= 0 ? firstIndex : null);
        _shuffleIndex = firstIndex >= 0 ? -1 : CurrentTrackIndex;
    }

    #endregion NAVIGATION

    #region COLLECTION CHANGES

    private void Tracks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _shuffleIndex = _shuffleCycle.Reindex(e, _shuffleIndex, PlayList.Tracks.Count);

        if (e.Action == NotifyCollectionChangedAction.Move &&
            _removedTrackPreviousIndex is int movedPreviousIndex)
        {
            _removedTrackPreviousIndex = ReindexMovedItem(
                movedPreviousIndex,
                e.OldStartingIndex,
                e.NewStartingIndex);
        }

        if (CurrentTrack is null)
            return;

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            if (!PlayList.Tracks.Contains(CurrentTrack))
                _removedTrackPreviousIndex = -1;
            return;
        }

        if (e.Action != NotifyCollectionChangedAction.Remove || e.OldItems is null)
            return;

        for (var i = 0; i < e.OldItems.Count; i++)
        {
            if (ReferenceEquals(e.OldItems[i], CurrentTrack))
            {
                _removedTrackPreviousIndex = e.OldStartingIndex + i - 1;
                return;
            }
        }

        if (_removedTrackPreviousIndex is int previousIndex && e.OldStartingIndex <= previousIndex)
            _removedTrackPreviousIndex = Math.Max(-1, previousIndex - e.OldItems.Count);
    }

    private static int ReindexMovedItem(int index, int oldIndex, int newIndex)
    {
        if (index == oldIndex)
            return newIndex;

        if (oldIndex < newIndex && index > oldIndex && index <= newIndex)
            return index - 1;

        if (oldIndex > newIndex && index >= newIndex && index < oldIndex)
            return index + 1;

        return index;
    }

    #endregion COLLECTION CHANGES
}
