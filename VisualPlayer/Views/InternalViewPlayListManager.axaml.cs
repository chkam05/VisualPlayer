using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.UI;
using VisualPlayer.Interfaces;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;
using VisualPlayer.ViewModels;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Dialogs;
using VisualPlayer.Windows;

namespace VisualPlayer.Views;

public partial class InternalViewPlayListManager : InternalViewBase, IInternalCloseGuard
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private Track? _draggedTrack;
    private bool _hasSelectedTracks;
    private TrackViewModel? _lastSelectedTrack;
    private bool _isSelectionMode;
    private string _playListSearch = string.Empty;
    private string _selectionButtonText = T["COMMON_BUTTON_SELECT"];
    private SavedPlayListViewModel? _selectedPlayList;
    private readonly ObservableCollection<Track> _workingTracks = [];
    private bool _suppressPlayListSelection;
    private bool _sortPlayListsAscending = true;
    private string _trackSearch = string.Empty;
    
    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewPlayListManager, bool> IsSelectionModeProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, bool>(nameof(IsSelectionMode), view => view.IsSelectionMode);
    public static readonly DirectProperty<InternalViewPlayListManager, bool> HasSelectedTracksProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, bool>(nameof(HasSelectedTracks), view => view.HasSelectedTracks);
    public static readonly DirectProperty<InternalViewPlayListManager, string> PlayListSearchProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, string>(nameof(PlayListSearch), view => view.PlayListSearch, (view, value) => view.PlayListSearch = value);
    public static readonly DirectProperty<InternalViewPlayListManager, SavedPlayListViewModel?> SelectedPlayListProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, SavedPlayListViewModel?>(nameof(SelectedPlayList), view => view.SelectedPlayList, (view, value) => view.SelectedPlayList = value);
    public static readonly DirectProperty<InternalViewPlayListManager, string> TrackSearchProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, string>(nameof(TrackSearch), view => view.TrackSearch, (view, value) => view.TrackSearch = value);
    public static readonly DirectProperty<InternalViewPlayListManager, string> SelectionButtonTextProperty
        = AvaloniaProperty.RegisterDirect<InternalViewPlayListManager, string>(nameof(SelectionButtonText), view => view.SelectionButtonText);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool HasSelectedTracks
    {
        get => _hasSelectedTracks;
        private set => SetAndRaise(HasSelectedTracksProperty, ref _hasSelectedTracks, value);
    }

    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        private set
        {
            SetAndRaise(IsSelectionModeProperty, ref _isSelectionMode, value);
            SelectionButtonText = value ? T["DIALOG_INTERNAL_MESSAGE_BUTTON_CANCEL"] : T["COMMON_BUTTON_SELECT"];

            foreach (var track in VisibleTracks)
            {
                track.IsSelectionMode = value;

                if (!value)
                    track.IsSelected = false;
            }

            if (!value && PART_Tracks is not null)
                PART_Tracks.SelectedItems?.Clear();

            HasSelectedTracks = value && PART_Tracks?.SelectedItems?.Count > 0;
        }
    }

    public string PlayListSearch
    {
        get => _playListSearch;
        set
        {
            SetAndRaise(PlayListSearchProperty, ref _playListSearch, value ?? string.Empty);
            RefreshPlayLists();
        }
    }

    public string SelectionButtonText
    {
        get => _selectionButtonText;
        private set => SetAndRaise(SelectionButtonTextProperty, ref _selectionButtonText, value);
    }

    public SavedPlayListViewModel? SelectedPlayList
    {
        get => _selectedPlayList;
        set
        {
            SetAndRaise(SelectedPlayListProperty, ref _selectedPlayList, value);
            IsSelectionMode = false;
            RefreshTracks();
        }
    }

    private ObservableCollection<Track> SelectedTracksSource
        => SelectedPlayList is null
            ? ApplicationContext.Instance.PlayListCore.PlayList.Tracks
            : _workingTracks;

    public string TrackSearch
    {
        get => _trackSearch;
        set
        {
            SetAndRaise(TrackSearchProperty, ref _trackSearch, value ?? string.Empty);
            RefreshTracks();
        }
    }

    public ObservableCollection<SavedPlayListViewModel> VisiblePlayLists { get; } = [];

    public ObservableCollection<TrackViewModel> VisibleTracks { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalViewPlayListManager()
    {
        Title = T["MENU_PLAYLISTS"];
        InitializeComponent();
        T.LanguageChanged += Translations_LanguageChanged;
        RefreshPlayLists();
        LoadPlayList(null);
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    protected override void OnClosed()
    {
        T.LanguageChanged -= Translations_LanguageChanged;
        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_PLAYLISTS"];
        SelectionButtonText = IsSelectionMode
            ? T["DIALOG_INTERNAL_MESSAGE_BUTTON_CANCEL"]
            : T["COMMON_BUTTON_SELECT"];
    }

    private static void AppendTracks(IEnumerable<Track> tracks)
    {
        ApplicationContext.Instance.PlayListCore.AddStoredTracks(tracks);
    }

    private void PlayTracks(IEnumerable<Track> tracks)
    {
        var context = ApplicationContext.Instance;
        var first = context.PlayListCore.ReplaceWith(tracks).FirstOrDefault();
        if (first is not null)
            context.PlayerCore.Play(first);
    }

    private void RefreshPlayLists()
    {
        var query = PlayListSearch.Trim();
        IEnumerable<SavedPlayListViewModel> items = ApplicationContext.Instance.PlayListLibraryCore.PlayLists;
        if (query.Length > 0)
            items = items.Where(item => item.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        items = _sortPlayListsAscending
            ? items.OrderBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
            : items.OrderByDescending(item => item.Name, StringComparer.CurrentCultureIgnoreCase);
        VisiblePlayLists.Clear();
        foreach (var item in items)
            VisiblePlayLists.Add(item);
    }

    private void RefreshTracks()
    {
        var query = TrackSearch.Trim();
        var items = query.Length == 0
            ? SelectedTracksSource
            : SelectedTracksSource.Where(track => track.DisplayTitle.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        VisibleTracks.Clear();
        var currentTrack = ApplicationContext.Instance.PlayListCore.CurrentTrack;
        var coverCache = ApplicationContext.Instance.PlayListCore.CoverCache;

        foreach (var item in items)
        {
            VisibleTracks.Add(new TrackViewModel(item, coverCache)
            {
                IsCurrentPlaying = ReferenceEquals(item, currentTrack),
                IsSelectionMode = IsSelectionMode
            });
        }
    }

    private async Task<SavedPlayListViewModel?> RequestDestinationAsync()
    {
        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return null;

        var picker = new InternalMessagePlayListPicker(
            ApplicationContext.Instance.PlayListLibraryCore.PlayLists);
        var result = await window.ShowDialogAsync(picker);
        return result == InternalDialogResult.Ok
            ? picker.SelectedPlayList
            : null;
    }

    private async Task<string?> RequestNameAsync(string title, string defaultValue = "")
    {
        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return null;
        var input = new InternalMessageInput(title, defaultValue, LucideIconKind.ListMusic);
        var result = await window.ShowDialogAsync(input);
        return result == InternalDialogResult.Ok && !string.IsNullOrWhiteSpace(input.Value) ? input.Value : null;
    }

    private void RefreshAfterChange()
    {
        RefreshTracks();
    }

    private void LoadPlayList(SavedPlayListViewModel? playList)
    {
        _workingTracks.Clear();

        if (playList is not null)
        {
            foreach (var track in playList.Tracks)
                _workingTracks.Add(Core.PlayListLibraryCore.CloneTrack(track));
        }

        SelectedPlayList = playList;
        _suppressPlayListSelection = true;
        PART_PlayLists.SelectedItem = playList;
        _suppressPlayListSelection = false;
    }

    private bool HasUnsavedChanges()
    {
        if (SelectedPlayList is null)
            return false;

        return !TracksEqual(_workingTracks, SelectedPlayList.Tracks);
    }

    private static bool TracksEqual(IReadOnlyList<Track> left, IReadOnlyList<Track> right)
    {
        if (left.Count != right.Count)
            return false;

        for (var index = 0; index < left.Count; index++)
        {
            var first = left[index];
            var second = right[index];

            if (!string.Equals(first.FilePath, second.FilePath, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(first.Title, second.Title, StringComparison.Ordinal)
                || !string.Equals(first.Album, second.Album, StringComparison.Ordinal)
                || !string.Equals(first.Artist, second.Artist, StringComparison.Ordinal)
                || first.Duration != second.Duration)
            {
                return false;
            }
        }

        return true;
    }

    private void SavePlayList()
    {
        if (SelectedPlayList is null)
            return;

        SelectedPlayList.Tracks.Clear();

        foreach (var track in _workingTracks)
            SelectedPlayList.Tracks.Add(Core.PlayListLibraryCore.CloneTrack(track));

        SelectedPlayList.Save();
    }

    private async Task<bool> ConfirmUnsavedChangesAsync()
    {
        if (!HasUnsavedChanges())
            return true;

        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return false;

        var dialog = new InternalMessageDialog(
            T["DIALOG_UNSAVED_CHANGES_TITLE"],
            T.Format("VIEW_PLAYLIST_MANAGER_UNSAVED_MESSAGE", SelectedPlayList!.Name),
            InternalDialogButtons.YesNoCancel,
            LucideIconKind.TriangleAlert,
            false);
        var result = await window.ShowDialogAsync(dialog);

        if (result == InternalDialogResult.Yes)
            SavePlayList();

        return result is InternalDialogResult.Yes or InternalDialogResult.No;
    }

    public async Task AddDroppedFilesAsync(IEnumerable<string> paths)
    {
        if (SelectedPlayList is null)
        {
            await ApplicationContext.Instance.AddFilesAsync(paths);
            RefreshTracks();
            return;
        }

        foreach (var path in paths)
        {
            if (_workingTracks.Any(track => string.Equals(
                track.FilePath,
                path,
                StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var track = new Track { FilePath = path };
            TrackMetadataUtils.TryLoadMetadata(track, out _);
            _workingTracks.Add(track);
        }

        RefreshTracks();
    }

    public Task<bool> CanCloseAsync()
    {
        return ConfirmUnsavedChangesAsync();
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private async void PlayLists_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressPlayListSelection || sender is not ListBox listBox)
            return;

        var requested = listBox.SelectedItem as SavedPlayListViewModel;

        if (ReferenceEquals(requested, SelectedPlayList))
            return;

        if (!await ConfirmUnsavedChangesAsync())
        {
            _suppressPlayListSelection = true;
            listBox.SelectedItem = SelectedPlayList;
            _suppressPlayListSelection = false;
            return;
        }

        LoadPlayList(requested);
    }

    private async void AddFilesButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not { } topLevel)
            return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = T["VIEW_PLAYLIST_MANAGER_ADD_TRACKS_TITLE"],
            AllowMultiple = true,
            FileTypeFilter = [
                new FilePickerFileType(T["FILETYPE_AUDIO_FILES"])
                {
                    Patterns = AudioFileUtils.FilePickerAudioTypes
                }
            ]
        });

        var paths = files
            .Select(file => file.TryGetLocalPath())
            .Where(path => path is not null)
            .Select(path => path!)
            .ToArray();

        await AddDroppedFilesAsync(paths);
    }

    private void AppendSavedPlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: SavedPlayListViewModel item })
            AppendTracks(item.Tracks);
    }

    private void AppendSelectedPlayList_Click(object? sender, RoutedEventArgs e)
    {
        AppendTracks(SelectedTracksSource.ToList());
    }

    private void DeletePlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SavedPlayListViewModel item })
            return;

        ApplicationContext.Instance.PlayListLibraryCore.Delete(item);

        if (ReferenceEquals(SelectedPlayList, item))
            LoadPlayList(null);

        RefreshPlayLists();
    }

    private async void DuplicatePlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SavedPlayListViewModel item })
            return;

        var name = await RequestNameAsync(T["VIEW_PLAYLIST_MANAGER_DUPLICATE_TITLE"], T.Format("VIEW_PLAYLIST_MANAGER_COPY_NAME", item.Name));

        if (name is null)
            return;

        ApplicationContext.Instance.PlayListLibraryCore.Duplicate(item, name);
        RefreshPlayLists();
    }

    private async void ExportSavedPlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: SavedPlayListViewModel item } || TopLevel.GetTopLevel(this) is not { } topLevel)
            return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = T["PLAYLIST_ITEM_EXPORT_PLAYLIST"],
            SuggestedFileName = $"{item.Name}.vpl",
            DefaultExtension = "vpl",
            FileTypeChoices = [
                new FilePickerFileType(T["FILETYPE_VISUALPLAYER_PLAYLIST"])
                {
                    Patterns = ["*.vpl"]
                }
            ]
        });

        var path = file?.TryGetLocalPath();

        if (path is not null)
            File.WriteAllText(path, JsonSerializer.Serialize(
                new PlayListFile
                {
                    Name = item.Name,
                    Tracks = [.. item.Tracks]
                }
                , new JsonSerializerOptions { WriteIndented = true }
            ));
    }

    private async void NowPlayingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (await ConfirmUnsavedChangesAsync())
            LoadPlayList(null);
    }

    private async void NewPlayListButton_Click(object? sender, RoutedEventArgs e)
    {
        var name = await RequestNameAsync(T["COMMON_NEW_PLAYLIST"], T["COMMON_NEW_PLAYLIST"]);

        if (name is null)
            return;

        if (!await ConfirmUnsavedChangesAsync())
            return;

        LoadPlayList(ApplicationContext.Instance.PlayListLibraryCore.Create(name));
        RefreshPlayLists();
    }

    private void SavePlayListButton_Click(object? sender, RoutedEventArgs e)
    {
        SavePlayList();
    }

    private void ClearPlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (SelectedPlayList is null)
            ApplicationContext.Instance.PlayListCore.Clear();
        else
            _workingTracks.Clear();

        IsSelectionMode = false;
        RefreshTracks();
    }

    private static void OpenMenu(object? sender)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);
    }

    private void PlayListMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenMenu(sender);
    }

    private void PlaySavedPlayList_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: SavedPlayListViewModel item })
            PlayTracks(item.Tracks);
    }

    private void PlaySelectedPlayList_Click(object? sender, RoutedEventArgs e)
    {
        PlayTracks(SelectedTracksSource.ToList());
    }

    private void SelectedTracksMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenMenu(sender);
    }

    private void SortPlayListsAscending_Click(object? sender, RoutedEventArgs e)
    {
        _sortPlayListsAscending = true;
        RefreshPlayLists();
    }

    private void SortPlayListsDescending_Click(object? sender, RoutedEventArgs e)
    {
        _sortPlayListsAscending = false;
        RefreshPlayLists();
    }

    private void SortPlayListsButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenMenu(sender);
    }

    private void SortTracksButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenMenu(sender);
    }

    private void TrackMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        OpenMenu(sender);
    }

    private void TrackItem_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (IsSelectionMode || sender is not Control { DataContext: TrackViewModel track })
            return;

        if (e.Source is Visual visual
            && (visual.FindAncestorOfType<Button>(includeSelf: true) is not null
                || visual.FindAncestorOfType<CheckBox>(includeSelf: true) is not null
                || visual.GetSelfAndVisualAncestors()
                    .OfType<Control>()
                    .Any(control => control.Name == "PART_ReorderHandle")))
        {
            return;
        }

        PlayTrack(track.Model);
        e.Handled = true;
    }

    private void TrackItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsSelectionMode
            || sender is not Control { DataContext: TrackViewModel track }
            || !e.GetCurrentPoint(PART_Tracks).Properties.IsLeftButtonPressed
            || e.ClickCount != 1)
        {
            return;
        }

        if (e.Source is Visual visual
            && (visual.FindAncestorOfType<Button>(includeSelf: true) is not null
                || visual.FindAncestorOfType<CheckBox>(includeSelf: true) is not null
                || visual.GetSelfAndVisualAncestors()
                    .OfType<Control>()
                    .Any(control => control.Name == "PART_ReorderHandle")))
        {
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && _lastSelectedTrack is not null)
        {
            var anchorIndex = VisibleTracks.IndexOf(_lastSelectedTrack);
            var currentIndex = VisibleTracks.IndexOf(track);

            if (anchorIndex >= 0 && currentIndex >= 0)
            {
                var start = Math.Min(anchorIndex, currentIndex);
                var end = Math.Max(anchorIndex, currentIndex);

                for (var index = start; index <= end; index++)
                {
                    var item = VisibleTracks[index];

                    if (PART_Tracks.SelectedItems?.Contains(item) != true)
                        PART_Tracks.SelectedItems?.Add(item);
                }
            }
        }
        else if (PART_Tracks.SelectedItems?.Contains(track) == true)
            PART_Tracks.SelectedItems.Remove(track);
        else
            PART_Tracks.SelectedItems?.Add(track);

        _lastSelectedTrack = track;

        e.Handled = true;
    }

    private void ToggleSelectionButton_Click(object? sender, RoutedEventArgs e)
    {
        IsSelectionMode = !IsSelectionMode;
    }

    #endregion BUTTON ACTIONS

    #region DRAG & DROP
    
    private void CompleteTrackReorder(IPointer pointer)
    {
        pointer.Capture(null);
        _draggedTrack = null;
    }
    
    private void ReorderHandle_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: TrackViewModel track }
            || !e.GetCurrentPoint(PART_Tracks).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _draggedTrack = track.Model;
        e.Pointer.Capture(PART_Tracks);
        e.Handled = true;
    }

    private void Tracks_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is not Visual source)
            return;

        var item = source.FindAncestorOfType<ListBoxItem>(includeSelf: true);
        var scrollBar = source.FindAncestorOfType<Avalonia.Controls.Primitives.ScrollBar>(includeSelf: true);

        if (item is null && scrollBar is null)
            PART_Tracks.SelectedItems?.Clear();
    }

    private void Tracks_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _draggedTrack = null;
    }

    private void Tracks_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedTrack is null)
            return;

        var pointerY = e.GetPosition(PART_Tracks).Y;
        var targetItem = PART_Tracks
            .GetVisualDescendants()
            .OfType<ListBoxItem>()
            .FirstOrDefault(item =>
            {
                var position = item.TranslatePoint(default, PART_Tracks);
                return position is { } point
                    && pointerY >= point.Y
                    && pointerY <= point.Y + item.Bounds.Height;
            });

        if (targetItem?.DataContext is not TrackViewModel targetTrackViewModel
            || ReferenceEquals(targetTrackViewModel.Model, _draggedTrack))
            return;

        var targetTrack = targetTrackViewModel.Model;
        var source = SelectedTracksSource;
        var oldIndex = source.IndexOf(_draggedTrack);
        var newIndex = source.IndexOf(targetTrack);

        if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
            return;

        if (SelectedPlayList is null)
            ApplicationContext.Instance.PlayListCore.MoveTrack(oldIndex, newIndex);
        else
            source.Move(oldIndex, newIndex);

        RefreshTracks();
        e.Handled = true;
    }

    private void Tracks_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CompleteTrackReorder(e.Pointer);
    }

    private void Tracks_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        HasSelectedTracks = IsSelectionMode && PART_Tracks.SelectedItems?.Count > 0;
    }

    #endregion DRAG & DROP

    #region SELECTED TRACKS ACTIONS

    private async void AddSelectedTracksTo_Click(object? sender, RoutedEventArgs e)
    {
        var tracks = GetSelectedTracks().ToList();

        if (!await ConfirmUnsavedChangesAsync())
            return;

        var destination = await RequestDestinationAsync();

        if (destination is not null)
            CopyTracksTo(destination, tracks);
    }

    private List<Track> GetSelectedTracks()
    {
        return PART_Tracks.SelectedItems?
            .OfType<TrackViewModel>()
            .Select(track => track.Model)
            .ToList() ?? [];
    }

    private void MoveSelected(bool top)
    {
        var selected = GetSelectedTracks();

        foreach (var item in selected)
            SelectedTracksSource.Remove(item);

        var index = top ? 0 : SelectedTracksSource.Count;

        foreach (var item in selected)
            SelectedTracksSource.Insert(index++, item);

        RefreshAfterChange();
    }

    private void MoveSelectedTracksBottom_Click(object? sender, RoutedEventArgs e)
    {
        MoveSelected(false);
    }

    private void MoveSelectedTracksTop_Click(object? sender, RoutedEventArgs e)
    {
        MoveSelected(true);
    }

    private void RemoveSelectedTracks_Click(object? sender, RoutedEventArgs e)
    {
        foreach (var item in GetSelectedTracks())
            RemoveTrack(item);

        IsSelectionMode = false;
        RefreshTracks();
    }

    #endregion SELECTED TRACKS ACTIONS

    #region TRACK ACTIONS

    private void PlayTrack(Track track)
    {
        var context = ApplicationContext.Instance;

        if (SelectedPlayList is null)
        {
            context.PlayerCore.Play(track);
            return;
        }

        var sourceIndex = SelectedTracksSource.IndexOf(track);
        var addedTracks = context.PlayListCore.ReplaceWith(SelectedTracksSource.ToList());

        if (sourceIndex >= 0 && sourceIndex < addedTracks.Count)
            context.PlayerCore.Play(addedTracks[sourceIndex]);
    }

    private void CopyTracksTo(SavedPlayListViewModel destination, IEnumerable<Track> tracks)
    {
        LoadPlayList(destination);

        foreach (var track in tracks)
        {
            if (!_workingTracks.Any(item => string.Equals(
                item.FilePath,
                track.FilePath,
                StringComparison.OrdinalIgnoreCase)))
            {
                _workingTracks.Add(Core.PlayListLibraryCore.CloneTrack(track));
            }
        }

        SavePlayList();
        RefreshTracks();
    }

    private async void AddTrackTo_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: TrackViewModel track })
            return;

        var sourceTrack = track.Model;

        if (!await ConfirmUnsavedChangesAsync())
            return;

        var destination = await RequestDestinationAsync();

        if (destination is not null)
            CopyTracksTo(destination, [sourceTrack]);
    }

    private void MoveTrack_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: TrackViewModel trackViewModel, Tag: string action })
            return;

        var track = trackViewModel.Model;
        var source = SelectedTracksSource;
        var oldIndex = source.IndexOf(track);
        var newIndex = action switch
        {
            "Top" => 0,
            "Bottom" => source.Count - 1,
            "Up" => Math.Max(0, oldIndex - 1),
            "Down" => Math.Min(source.Count - 1, oldIndex + 1),
            _ => oldIndex
        };

        if (oldIndex >= 0 && oldIndex != newIndex)
            source.Move(oldIndex, newIndex);

        RefreshAfterChange();
    }

    private void RemoveTrack(Track track)
    {
        if (SelectedPlayList is null)
        {
            ApplicationContext.Instance.PlayListCore.RemoveTrack(track);
        }
        else
        {
            _workingTracks.Remove(track);
        }
    }

    private void RemoveTrack_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: TrackViewModel track })
            RemoveTrack(track.Model);

        RefreshTracks();
    }

    #endregion TRACK ACTIONS

    #region SORTING ACTIONS

    private void SortTracks_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { Tag: string mode })
            return;

        Func<Track, string?> selector = mode.StartsWith("Album")
            ? track => track.DisplayAlbum
            : mode.StartsWith("Artist")
                ? track => track.Artist
                : track => track.DisplayTitle;

        var sorted = mode.EndsWith("Descending")
            ? SelectedTracksSource
                .OrderByDescending(selector, StringComparer.CurrentCultureIgnoreCase)
                .ToList()
            : SelectedTracksSource
                .OrderBy(selector, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

        for (var index = 0; index < sorted.Count; index++)
        {
            var oldIndex = SelectedTracksSource.IndexOf(sorted[index]);

            if (oldIndex != index)
                SelectedTracksSource.Move(oldIndex, index);
        }

        RefreshAfterChange();
    }

    #endregion SORTING ACTIONS

    
}
