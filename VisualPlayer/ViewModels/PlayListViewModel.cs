using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisualPlayer.Attributes;
using VisualPlayer.Commands;
using VisualPlayer.Contexts;
using VisualPlayer.Core;
using VisualPlayer.Enums.Player;
using VisualPlayer.Models.Player;

namespace VisualPlayer.ViewModels;

public class PlayListViewModel : ViewModelBase
{
    private readonly CoverCache _coverCache;
    private readonly PlayListCore _playListCore;
    private readonly TranslationContext _translations;
    
    private TrackViewModel? _currentTrack;
    private bool _isSelectionMode;
    private string _name;
    private string _searchPhrase = string.Empty;
    private TrackViewModel? _selectedTrack;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public TrackViewModel? CurrentTrack
    {
        get => _currentTrack;
        private set => UpdateProperty(ref _currentTrack, value);
    }

    [NotifyPropertyUpdate]
    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        private set
        {
            UpdateProperty(ref _isSelectionMode, value);

            foreach (var track in Tracks)
            {
                track.IsSelectionMode = value;

                if (!value)
                    track.IsSelected = false;
            }

            NotifyPropertyChanged(nameof(SelectionButtonText));
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => string.IsNullOrWhiteSpace(_name) || _name is "Teraz Odtwarzane" or "Teraz odtwarzane" or "Now Playing"
            ? _translations["MENU_NOW_PLAYING"]
            : _name;
        set
        {
            UpdateProperty(ref _name, value);
            _playListCore.PlayList.Name = value;
        }
    }

    [NotifyPropertyUpdate]
    public TrackViewModel? SelectedTrack
    {
        get => _selectedTrack;
        set
        {
            UpdateProperty(ref _selectedTrack, value);
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    [NotifyPropertyUpdate]
    public string SearchPhrase
    {
        get => _searchPhrase;
        set
        {
            var nextValue = value ?? string.Empty;
            if (string.Equals(_searchPhrase, nextValue, StringComparison.Ordinal))
                return;

            UpdateProperty(ref _searchPhrase, nextValue);
            RefreshVisibleTracks();
        }
    }

    public PlayList Model => _playListCore.PlayList;
    public string SelectionButtonText => _translations[IsSelectionMode
        ? "DIALOG_INTERNAL_MESSAGE_BUTTON_CANCEL"
        : "COMMON_BUTTON_SELECT"];
    public ObservableCollection<TrackViewModel> Tracks { get; }
    public ObservableCollection<TrackViewModel> VisibleTracks { get; }

    #endregion PROPERTIES

    #region COMMANDS

    public RelayCommand DeleteCommand { get; }
    public RelayCommand SelectCommand { get; }
    public RelayCommand SortAlbumAscendingCommand { get; }
    public RelayCommand SortAlbumDescendingCommand { get; }
    public RelayCommand SortArtistAscendingCommand { get; }
    public RelayCommand SortArtistDescendingCommand { get; }
    public RelayCommand SortTitleAscendingCommand { get; }
    public RelayCommand SortTitleDescendingCommand { get; }

    #endregion COMMANDS

    #region CONSTRUCTORS

    public PlayListViewModel(PlayListCore playListCore, CoverCache coverCache, TranslationContext translations)
    {
        _playListCore = playListCore;
        _coverCache = coverCache;
        _translations = translations;
        _name = playListCore.PlayList.Name;

        Tracks = new ObservableCollection<TrackViewModel>(
            playListCore.PlayList.Tracks.Select(track => new TrackViewModel(track, _coverCache)));
        VisibleTracks = new ObservableCollection<TrackViewModel>(Tracks);

        foreach (var track in Tracks)
            track.SelectionChanged += Track_SelectionChanged;

        DeleteCommand = new RelayCommand(DeleteSelected, CanDelete);
        SelectCommand = new RelayCommand(ToggleSelectionMode);
        SortAlbumAscendingCommand = new RelayCommand(() => Sort(TrackSortMode.AlbumAscending));
        SortAlbumDescendingCommand = new RelayCommand(() => Sort(TrackSortMode.AlbumDescending));
        SortArtistAscendingCommand = new RelayCommand(() => Sort(TrackSortMode.ArtistAscending));
        SortArtistDescendingCommand = new RelayCommand(() => Sort(TrackSortMode.ArtistDescending));
        SortTitleAscendingCommand = new RelayCommand(() => Sort(TrackSortMode.TitleAscending));
        SortTitleDescendingCommand = new RelayCommand(() => Sort(TrackSortMode.TitleDescending));
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    internal void RefreshTranslations()
    {
        NotifyPropertyChanged(nameof(Name));
        NotifyPropertyChanged(nameof(SelectionButtonText));
    }

    internal void AddTrack(Track track)
    {
        var trackViewModel = new TrackViewModel(track, _coverCache)
        {
            IsSelectionMode = IsSelectionMode
        };

        trackViewModel.SelectionChanged += Track_SelectionChanged;
        Tracks.Add(trackViewModel);
        RefreshVisibleTracks();
    }

    public void Clear()
    {
        _playListCore.Clear();
    }

    private void DeleteSelected()
    {
        var tracksToDelete = IsSelectionMode
            ? Tracks.Where(x => x.IsSelected).ToList()
            : SelectedTrack is not null
                ? new List<TrackViewModel> { SelectedTrack }
                : new List<TrackViewModel>();

        foreach (var track in tracksToDelete)
            RemoveTrack(track);

        SelectedTrack = null;

        if (IsSelectionMode)
            IsSelectionMode = false;

        DeleteCommand.RaiseCanExecuteChanged();
    }

    internal void FinishClear()
    {
        SelectedTrack = null;

        if (IsSelectionMode)
            IsSelectionMode = false;

        DeleteCommand.RaiseCanExecuteChanged();
    }

    public void MoveTrack(int oldIndex, int newIndex)
    {
        _playListCore.MoveTrack(oldIndex, newIndex);
    }

    public void MoveTrack(TrackViewModel track, int newIndex)
    {
        var oldIndex = Tracks.IndexOf(track);
        if (oldIndex >= 0)
            MoveTrack(oldIndex, newIndex);
    }

    public void MoveTrackDown(TrackViewModel track)
    {
        var index = Tracks.IndexOf(track);
        if (index >= 0 && index < Tracks.Count - 1)
            MoveTrack(index, index + 1);
    }

    public void MoveTrackToBottom(TrackViewModel track)
    {
        MoveTrack(track, Tracks.Count - 1);
    }

    public void MoveTrackToTop(TrackViewModel track)
    {
        MoveTrack(track, 0);
    }

    public void MoveTrackUp(TrackViewModel track)
    {
        var index = Tracks.IndexOf(track);
        if (index > 0)
            MoveTrack(index, index - 1);
    }

    internal void MoveTrackInternal(int oldIndex, int newIndex)
    {
        Tracks.Move(oldIndex, newIndex);
        RefreshVisibleTracks();
    }

    public void Play(TrackViewModel track)
    {
        if (Tracks.Contains(track))
            _playListCore.RequestPlay(track.Model);
    }

    public void RemoveTrack(TrackViewModel track)
    {
        _playListCore.RemoveTrack(track.Model);
    }

    internal void RemoveTrackInternal(TrackViewModel track, bool isCurrentTrack)
    {
        track.SelectionChanged -= Track_SelectionChanged;
        Tracks.Remove(track);
        RefreshVisibleTracks();

        if (!isCurrentTrack)
            RemoveUnusedCover(track);

        if (SelectedTrack == track)
            SelectedTrack = null;

        DeleteCommand.RaiseCanExecuteChanged();
    }

    private void Sort(TrackSortMode mode)
    {
        IEnumerable<TrackViewModel> sorted;

        switch (mode)
        {
            case TrackSortMode.TitleAscending:
                sorted = Tracks.OrderBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            case TrackSortMode.TitleDescending:
                sorted = Tracks.OrderByDescending(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            case TrackSortMode.AlbumAscending:
                sorted = Tracks.OrderBy(x => x.Album, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            case TrackSortMode.AlbumDescending:
                sorted = Tracks.OrderByDescending(x => x.Album, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            case TrackSortMode.ArtistAscending:
                sorted = Tracks.OrderBy(x => x.Artist, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            case TrackSortMode.ArtistDescending:
                sorted = Tracks.OrderByDescending(x => x.Artist, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase);
                break;
            default:
                sorted = Tracks;
                break;
        }

        var sortedTracks = sorted.ToList();
        for (var i = 0; i < sortedTracks.Count; i++)
        {
            var oldIndex = Tracks.IndexOf(sortedTracks[i]);

            if (oldIndex != i)
                _playListCore.MoveTrack(oldIndex, i);
        }
    }

    private void ToggleSelectionMode()
    {
        IsSelectionMode = !IsSelectionMode;
    }

    private void Track_SelectionChanged(object? sender, EventArgs e)
    {
        DeleteCommand.RaiseCanExecuteChanged();
    }

    #endregion ACTIONS

    #region PLAYER STATE

    internal void SetCurrentTrack(Track? currentModel, bool isPlaying)
    {
        var previousTrack = CurrentTrack;
        var nextTrack = currentModel is null
            ? null
            : Tracks.FirstOrDefault(track => ReferenceEquals(track.Model, currentModel));

        if (nextTrack is null && ReferenceEquals(previousTrack?.Model, currentModel))
            nextTrack = previousTrack;

        if (previousTrack is not null)
            previousTrack.IsCurrentPlaying = false;

        CurrentTrack = nextTrack;

        if (nextTrack is not null)
            nextTrack.IsCurrentPlaying = isPlaying;

        if (previousTrack is not null && !Tracks.Contains(previousTrack))
            RemoveUnusedCover(previousTrack);
    }

    internal void SetIsPlaying(bool isPlaying)
    {
        if (CurrentTrack is not null)
            CurrentTrack.IsCurrentPlaying = isPlaying;
    }

    #endregion PLAYER STATE

    #region UTILITIES

    private bool CanDelete()
    {
        return IsSelectionMode
            ? Tracks.Any(x => x.IsSelected)
            : SelectedTrack is not null;
    }

    private void RefreshVisibleTracks()
    {
        IEnumerable<TrackViewModel> matchingTracks = string.IsNullOrWhiteSpace(SearchPhrase)
            ? Tracks
            : Tracks
                .Where(track => track.Title.Contains(SearchPhrase.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

        VisibleTracks.Clear();
        foreach (var track in matchingTracks)
            VisibleTracks.Add(track);
    }

    private void RemoveUnusedCover(TrackViewModel track)
    {
        var coverId = track.Model.CoverId;
        if (coverId is not null && !_playListCore.PlayList.Tracks.Any(item => item.CoverId == coverId))
            _coverCache.Remove(coverId);
    }

    #endregion UTILITIES
}
