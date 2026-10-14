using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Library;
using VisualPlayer.Enums.UI;
using VisualPlayer.Models.Library;
using VisualPlayer.Models.Player;
using VisualPlayer.ViewModels;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Dialogs;
using VisualPlayer.Windows;

namespace VisualPlayer.Views;

public partial class InternalViewLibraryManager : InternalViewBase
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    private readonly Stack<LibraryLocation> _history = [];
    private LibraryItemViewModel? _lastSelectedItem;
    private string? _openedAlbum;
    private string? _openedArtist;
    private string? _openedDirectory;
    private bool _sortAscending = true;
    private LibrarySortMode _sortMode = LibrarySortMode.Title;

    private bool _canNavigateBack;
    private bool _hasSelection;
    private bool _isSelectionMode;
    private bool _isLoadingLayout;
    private double _layoutValue;
    private string _searchPhrase = string.Empty;
    private string _selectionButtonText = T["COMMON_BUTTON_SELECT"];
    private LibrarySection _section = LibrarySection.Tracks;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewLibraryManager, bool> CanNavigateBackProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(CanNavigateBack), view => view.CanNavigateBack);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> HasSelectionProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(HasSelection), view => view.HasSelection);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsSelectionModeProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsSelectionMode), view => view.IsSelectionMode);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsDetailsLayoutProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsDetailsLayout), view => view.IsDetailsLayout);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsIconLayoutProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsIconLayout), view => view.IsIconLayout);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsListLayoutProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsListLayout), view => view.IsListLayout);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsDetailedSortAvailableProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsDetailedSortAvailable), view => view.IsDetailedSortAvailable);
    public static readonly DirectProperty<InternalViewLibraryManager, bool> IsSimpleSortAvailableProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, bool>(nameof(IsSimpleSortAvailable), view => view.IsSimpleSortAvailable);
    public static readonly DirectProperty<InternalViewLibraryManager, string> LayoutTitleProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, string>(nameof(LayoutTitle), view => view.LayoutTitle);
    public static readonly DirectProperty<InternalViewLibraryManager, double> IconItemHeightProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, double>(nameof(IconItemHeight), view => view.IconItemHeight);
    public static readonly DirectProperty<InternalViewLibraryManager, double> IconItemWidthProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, double>(nameof(IconItemWidth), view => view.IconItemWidth);
    public static readonly DirectProperty<InternalViewLibraryManager, double> IconSizeProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, double>(nameof(IconSize), view => view.IconSize);
    public static readonly DirectProperty<InternalViewLibraryManager, double> LayoutValueProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, double>(nameof(LayoutValue), view => view.LayoutValue, (view, value) => view.LayoutValue = value);
    public static readonly DirectProperty<InternalViewLibraryManager, string> SearchPhraseProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, string>(nameof(SearchPhrase), view => view.SearchPhrase, (view, value) => view.SearchPhrase = value);
    public static readonly DirectProperty<InternalViewLibraryManager, string> SelectionButtonTextProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLibraryManager, string>(nameof(SelectionButtonText), view => view.SelectionButtonText);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool CanNavigateBack
    {
        get => _canNavigateBack;
        private set => SetAndRaise(CanNavigateBackProperty, ref _canNavigateBack, value);
    }

    public bool HasSelection
    {
        get => _hasSelection;
        private set => SetAndRaise(HasSelectionProperty, ref _hasSelection, value);
    }

    public double IconItemHeight
    {
        get
        {
            switch (LayoutValue)
            {
                case >= 4:
                    return 210;

                case >= 3:
                    return 170;

                default:
                    return 135;
            }
        }
    }

    public double IconItemWidth
    {
        get
        {
            switch (LayoutValue)
            {
                case >= 4:
                    return 210;

                case >= 3:
                    return 170;

                default:
                    return 135;
            }
        }
    }

    public double IconSize
    {
        get
        {
            switch (LayoutValue)
            {
                case >= 4:
                    return 92;

                case >= 3:
                    return 68;

                default:
                    return 48;
            }
        }
    }

    public bool IsDetailsLayout
    {
        get => LayoutValue < 0.5;
    }

    public bool IsDetailedSortAvailable
    {
        get
        {
            var contentSection = GetCurrentContentSection();
            return contentSection == LibrarySection.Tracks || contentSection == LibrarySection.Directories;
        }
    }

    public bool IsIconLayout
    {
        get => LayoutValue >= 1.5;
    }

    public bool IsListLayout
    {
        get => LayoutValue >= 0.5 && LayoutValue < 1.5;
    }

    public bool IsSelectionMode
    {
        get => _isSelectionMode;
        private set
        {
            SetAndRaise(IsSelectionModeProperty, ref _isSelectionMode, value);
            SelectionButtonText = value ? T["DIALOG_INTERNAL_MESSAGE_BUTTON_CANCEL"] : T["COMMON_BUTTON_SELECT"];

            if (!value)
            {
                foreach (var item in VisibleItems)
                    item.IsSelected = false;
            }

            UpdateSelectionState();
        }
    }

    public bool IsSimpleSortAvailable
    {
        get => !IsDetailedSortAvailable;
    }

    public string LayoutTitle
    {
        get
        {
            switch (LayoutValue)
            {
                case < 0.5:
                    return T["VIEW_LIBRARY_LAYOUT_DETAILS"];

                case < 1.5:
                    return T["VIEW_LYRICS_NAV_LIST"];

                case < 2.5:
                    return T["VIEW_LIBRARY_LAYOUT_SMALL_ICONS"];

                case < 3.5:
                    return T["VIEW_LIBRARY_LAYOUT_MEDIUM_ICONS"];

                default:
                    return T["VIEW_LIBRARY_LAYOUT_LARGE_ICONS"];
            }
        }
    }

    public double LayoutValue
    {
        get => _layoutValue;
        set
        {
            var normalized = Math.Clamp(Math.Round(value), 0, 4);
            var oldDetails = IsDetailsLayout;
            var oldList = IsListLayout;
            var oldIcons = IsIconLayout;
            var oldTitle = LayoutTitle;
            var oldItemHeight = IconItemHeight;
            var oldItemWidth = IconItemWidth;
            var oldIconSize = IconSize;

            if (!SetAndRaise(LayoutValueProperty, ref _layoutValue, normalized))
                return;

            RaisePropertyChanged(IsDetailsLayoutProperty, oldDetails, IsDetailsLayout);
            RaisePropertyChanged(IsListLayoutProperty, oldList, IsListLayout);
            RaisePropertyChanged(IsIconLayoutProperty, oldIcons, IsIconLayout);
            RaisePropertyChanged(LayoutTitleProperty, oldTitle, LayoutTitle);
            RaisePropertyChanged(IconItemHeightProperty, oldItemHeight, IconItemHeight);
            RaisePropertyChanged(IconItemWidthProperty, oldItemWidth, IconItemWidth);
            RaisePropertyChanged(IconSizeProperty, oldIconSize, IconSize);

            if (!_isLoadingLayout)
                SaveLayoutForCurrentContent(normalized);
        }
    }

    public ObservableCollection<MenuItemViewModel> NavigationItems { get; }

    public string SearchPhrase
    {
        get => _searchPhrase;
        set
        {
            SetAndRaise(SearchPhraseProperty, ref _searchPhrase, value ?? string.Empty);
            RefreshVisibleItems();
        }
    }

    public string SelectionButtonText
    {
        get => _selectionButtonText;
        private set => SetAndRaise(SelectionButtonTextProperty, ref _selectionButtonText, value);
    }

    public ObservableCollection<LibraryItemViewModel> VisibleItems { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalViewLibraryManager()
    {
        Title = T["MENU_LIBRARY"];
        NavigationItems =
        [
            new MenuItemViewModel(LucideIconKind.Music, T["VIEW_LIBRARY_SECTION_TRACKS"], () => SetSection(LibrarySection.Tracks)),
            new MenuItemViewModel(LucideIconKind.DiscAlbum, T["VIEW_LIBRARY_SECTION_ALBUMS"], () => SetSection(LibrarySection.Albums)),
            new MenuItemViewModel(LucideIconKind.UsersRound, T["VIEW_LIBRARY_SECTION_ARTISTS"], () => SetSection(LibrarySection.Artists)),
            new MenuItemViewModel(LucideIconKind.Folders, T["VIEW_LIBRARY_SECTION_DIRECTORIES"], () => SetSection(LibrarySection.Directories))
        ];

        InitializeComponent();
        ApplicationContext.Instance.LibraryCore.Refreshed += LibraryCore_Refreshed;
        T.LanguageChanged += Translations_LanguageChanged;
        PART_NavigationList.SelectedIndex = 0;
        LoadLayoutForCurrentContent();
        RefreshVisibleItems();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    protected override void OnClosed()
    {
        ApplicationContext.Instance.LibraryCore.Refreshed -= LibraryCore_Refreshed;
        T.LanguageChanged -= Translations_LanguageChanged;
        base.OnClosed();
    }

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        Title = T["MENU_LIBRARY"];
        NavigationItems[0].Title = T["VIEW_LIBRARY_SECTION_TRACKS"];
        NavigationItems[1].Title = T["VIEW_LIBRARY_SECTION_ALBUMS"];
        NavigationItems[2].Title = T["VIEW_LIBRARY_SECTION_ARTISTS"];
        NavigationItems[3].Title = T["VIEW_LIBRARY_SECTION_DIRECTORIES"];
        SelectionButtonText = IsSelectionMode
            ? T["DIALOG_INTERNAL_MESSAGE_BUTTON_CANCEL"]
            : T["COMMON_BUTTON_SELECT"];
        LoadLayoutForCurrentContent();
        RefreshVisibleItems();
    }

    private void LibraryCore_Refreshed(object? sender, EventArgs e)
    {
        RefreshVisibleItems();
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private static void ItemMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);
    }

    private static void SelectedMoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);
    }

    private static void SortButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);
    }


    private async void AddItemToPlayListMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: LibraryItemViewModel item })
            await AddTracksToPlayListAsync(item.Tracks);
    }

    private async void AddSelectedToPlayListMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (await AddTracksToPlayListAsync(GetSelectedTracks()))
            FinishSelectionAction();
    }

    private void AppendItemMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: LibraryItemViewModel item })
            AppendTracks(item.Tracks);
    }

    private void AppendSelectedMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        AppendTracks(GetSelectedTracks());
        FinishSelectionAction();
    }

    private async void CreatePlayListMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return;

        var input = new InternalMessageInput(T["VIEW_LIBRARY_CREATE_PLAYLIST_TITLE"], T["COMMON_NEW_PLAYLIST"], LucideIconKind.ListMusic);

        if (await window.ShowDialogAsync(input) == InternalDialogResult.Ok && !string.IsNullOrWhiteSpace(input.Value))
        {
            ApplicationContext.Instance.PlayListLibraryCore.Create(input.Value, GetSelectedTracks());
            FinishSelectionAction();
        }
    }

    private void LibraryItem_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (!IsSelectionMode && sender is Control { DataContext: LibraryItemViewModel item })
            OpenItem(item);
    }

    private void OpenSelectedMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        var items = GetSelectedItems();

        if (items.Count == 1 && items[0].Kind != LibraryItemKind.Track)
            OpenItem(items[0]);
        else
            PlayTracks(GetSelectedTracks());

        FinishSelectionAction();
    }

    private void PlayItemMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: LibraryItemViewModel item })
            PlayTracks(item.Tracks);
    }

    private void SortMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { Tag: string mode })
        {
            if (mode.StartsWith("Album", StringComparison.Ordinal))
                _sortMode = LibrarySortMode.Album;
            else if (mode.StartsWith("Artist", StringComparison.Ordinal))
                _sortMode = LibrarySortMode.Artist;
            else if (mode.StartsWith("Type", StringComparison.Ordinal))
                _sortMode = LibrarySortMode.Type;
            else
                _sortMode = LibrarySortMode.Title;

            _sortAscending = mode.EndsWith("Ascending", StringComparison.Ordinal);
            RefreshVisibleItems();
        }
    }

    #endregion BUTTON ACTIONS

    #region NAVIGATION

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_history.TryPop(out var location))
            return;

        _section = location.Section;
        _openedArtist = location.Artist;
        _openedAlbum = location.Album;
        _openedDirectory = location.Directory;
        CanNavigateBack = _history.Count > 0;
        LoadLayoutForCurrentContent();
        RefreshVisibleItems();
    }

    private void NavigationList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PART_NavigationList.SelectedItem is MenuItemViewModel item && item.Action.CanExecute(null))
            item.Action.Execute(null);
    }

    private void PushLocation()
    {
        _history.Push(new LibraryLocation(_section, _openedArtist, _openedAlbum, _openedDirectory));
        CanNavigateBack = true;
    }

    private void SetSection(LibrarySection section)
    {
        _section = section;
        _openedAlbum = null;
        _openedArtist = null;
        _openedDirectory = null;
        _history.Clear();
        CanNavigateBack = false;
        SearchPhrase = string.Empty;
        IsSelectionMode = false;
        LoadLayoutForCurrentContent();
        RefreshVisibleItems();
    }

    #endregion NAVIGATION

    #region ITEMS

    private static IEnumerable<LibraryItemViewModel> CreateAlbumItems(IEnumerable<Track> tracks)
    {
        return tracks
            .GroupBy(track => track.DisplayAlbum ?? T["COMMON_UNKNOWN_ALBUM"], StringComparer.CurrentCultureIgnoreCase)
            .Select(group => new LibraryItemViewModel
            {
                Cover = GetCover(group.FirstOrDefault()),
                Kind = LibraryItemKind.Album,
                Title = group.Key,
                Subtitle = group.Select(track => track.Artist).FirstOrDefault(artist => !string.IsNullOrWhiteSpace(artist)) ?? T["COMMON_UNKNOWN_ARTIST"],
                Tracks = group.ToList()
            });
    }

    private IEnumerable<LibraryItemViewModel> CreateDirectoryItems(string directory)
    {
        IEnumerable<string> directories;

        try
        {
            directories = Directory.EnumerateDirectories(directory);
        }
        catch
        {
            directories = [];
        }

        var directoryItems = directories.Select(path =>
        {
            var tracks = TracksInDirectory(path, true).ToList();

            return new LibraryItemViewModel
            {
                Cover = GetCover(tracks.FirstOrDefault()),
                Kind = LibraryItemKind.Directory,
                Path = path,
                Title = Path.GetFileName(path),
                Subtitle = path,
                Tracks = tracks
            };
        });
        var trackItems = TracksInDirectory(directory, false).Select(CreateTrackItem);
        return directoryItems.Concat(trackItems);
    }

    private IEnumerable<LibraryItemViewModel> CreateItems()
    {
        var tracks = ApplicationContext.Instance.LibraryCore.Tracks.AsEnumerable();

        if (_openedDirectory is not null)
            return CreateDirectoryItems(_openedDirectory);

        if (_openedArtist is not null)
        {
            tracks = tracks.Where(track => string.Equals(
                track.Artist ?? T["COMMON_UNKNOWN_ARTIST"],
                _openedArtist,
                StringComparison.CurrentCultureIgnoreCase));

            if (_openedAlbum is not null)
            {
                tracks = tracks.Where(track => string.Equals(
                    track.DisplayAlbum ?? T["COMMON_UNKNOWN_ALBUM"],
                    _openedAlbum,
                    StringComparison.CurrentCultureIgnoreCase));
            }
            else if (ApplicationContext.Instance.Config.Library.ShowArtistAlbums)
            {
                return CreateAlbumItems(tracks);
            }

            return tracks.Select(CreateTrackItem);
        }

        if (_openedAlbum is not null)
        {
            return tracks
                .Where(track => string.Equals(
                    track.DisplayAlbum ?? T["COMMON_UNKNOWN_ALBUM"],
                    _openedAlbum,
                    StringComparison.CurrentCultureIgnoreCase))
                .Select(CreateTrackItem);
        }

        switch (_section)
        {
            case LibrarySection.Albums:
                return CreateAlbumItems(tracks);

            case LibrarySection.Artists:
                return tracks
                    .GroupBy(track => track.Artist ?? T["COMMON_UNKNOWN_ARTIST"], StringComparer.CurrentCultureIgnoreCase)
                    .Select(group => new LibraryItemViewModel
                    {
                        Cover = GetCover(group.FirstOrDefault()),
                        Kind = LibraryItemKind.Artist,
                        Title = group.Key,
                        Subtitle = T.Format("VIEW_LIBRARY_TRACK_COUNT", group.Count()),
                        Tracks = group.ToList()
                    });

            case LibrarySection.Directories:
                return ApplicationContext.Instance.Config.Library.Folders
                    .Where(folder => Directory.Exists(folder.Path))
                    .Select(folder =>
                    {
                        var folderTracks = TracksInDirectory(folder.Path, folder.IncludeSubdirectories).ToList();

                        return new LibraryItemViewModel
                        {
                            Cover = GetCover(folderTracks.FirstOrDefault()),
                            Kind = LibraryItemKind.Directory,
                            Path = folder.Path,
                            Title = folder.Name,
                            Subtitle = folder.Path,
                            Tracks = folderTracks
                        };
                    });

            default:
                return tracks.Select(CreateTrackItem);
        }
    }

    private static LibraryItemViewModel CreateTrackItem(Track track)
    {
        return new LibraryItemViewModel
        {
            Cover = GetCover(track),
            Kind = LibraryItemKind.Track,
            Title = track.DisplayTitle,
            Subtitle = string.Join(" • ", new[] { track.Artist, track.DisplayAlbum }.Where(value => !string.IsNullOrWhiteSpace(value))),
            Track = track,
            Tracks = [track]
        };
    }

    private static Avalonia.Media.IImage? GetCover(Track? track)
    {
        return ApplicationContext.Instance.LibraryCore.CoverCache.GetImage(track?.CoverId);
    }

    private void OpenItem(LibraryItemViewModel item)
    {
        switch (item.Kind)
        {
            case LibraryItemKind.Track:
                PlayTracks(item.Tracks);
                break;
            case LibraryItemKind.Artist:
                PushLocation();
                _openedArtist = item.Title;
                _openedAlbum = null;
                LoadLayoutForCurrentContent();
                RefreshVisibleItems();
                break;
            case LibraryItemKind.Album:
                PushLocation();
                _openedAlbum = item.Title;
                LoadLayoutForCurrentContent();
                RefreshVisibleItems();
                break;
            case LibraryItemKind.Directory when item.Path is not null:
                PushLocation();
                _openedDirectory = item.Path;
                LoadLayoutForCurrentContent();
                RefreshVisibleItems();
                break;
        }
    }

    private void RefreshVisibleItems()
    {
        foreach (var item in VisibleItems)
            item.PropertyChanged -= Item_PropertyChanged;

        IEnumerable<LibraryItemViewModel> items = CreateItems();
        var query = SearchPhrase.Trim();

        if (query.Length > 0)
        {
            items = items.Where(item => item.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                || item.Subtitle.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        items = SortItems(items);

        VisibleItems.Clear();

        foreach (var item in items)
        {
            item.PropertyChanged += Item_PropertyChanged;
            VisibleItems.Add(item);
        }

        UpdateSelectionState();
    }

    private static IEnumerable<Track> TracksInDirectory(string directory, bool recursive)
    {
        var normalizedDirectory = NormalizeDirectoryPath(directory);

        return ApplicationContext.Instance.LibraryCore.Tracks.Where(track =>
        {
            var trackDirectory = Path.GetDirectoryName(track.FilePath);

            if (trackDirectory is null)
                return false;

            var normalizedTrackDirectory = NormalizeDirectoryPath(trackDirectory);

            if (!recursive)
                return string.Equals(normalizedTrackDirectory, normalizedDirectory, StringComparison.OrdinalIgnoreCase);

            var relative = Path.GetRelativePath(normalizedDirectory, track.FilePath);
            return !relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                && !string.Equals(relative, "..", StringComparison.Ordinal);
        });
    }

    private IEnumerable<LibraryItemViewModel> SortItems(IEnumerable<LibraryItemViewModel> items)
    {
        if (_sortMode == LibrarySortMode.Title)
        {
            return SortByValue(items, item => item.Title);
        }

        switch (_sortMode)
        {
            case LibrarySortMode.Album:
                return SortByValue(items, GetAlbum);

            case LibrarySortMode.Artist:
                return SortByValue(items, GetArtist);

            case LibrarySortMode.Type:
                return SortByValue(items, GetFileType);

            default:
                return SortByValue(items, item => item.Title);
        }
    }

    private IEnumerable<LibraryItemViewModel> SortByValue(
        IEnumerable<LibraryItemViewModel> items,
        Func<LibraryItemViewModel, string> valueSelector)
    {
        if (_sortAscending)
        {
            return items
                .OrderBy(GetTypeSortGroup)
                .ThenBy(valueSelector, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(item => item.Title, StringComparer.CurrentCultureIgnoreCase);
        }

        return items
            .OrderBy(GetTypeSortGroup)
            .ThenByDescending(valueSelector, StringComparer.CurrentCultureIgnoreCase)
            .ThenByDescending(item => item.Title, StringComparer.CurrentCultureIgnoreCase);
    }

    private static string GetAlbum(LibraryItemViewModel item)
    {
        return item.Track?.DisplayAlbum ?? string.Empty;
    }

    private static string GetArtist(LibraryItemViewModel item)
    {
        return item.Track?.Artist ?? string.Empty;
    }

    private static string GetFileType(LibraryItemViewModel item)
    {
        if (item.Track is null)
            return string.Empty;

        return Path.GetExtension(item.Track.FilePath).TrimStart('.');
    }

    private static int GetTypeSortGroup(LibraryItemViewModel item)
    {
        return item.Kind == LibraryItemKind.Directory ? 0 : 1;
    }

    private LibrarySection GetCurrentContentSection()
    {
        if (_openedDirectory is not null)
            return LibrarySection.Directories;

        if (_openedAlbum is not null)
            return LibrarySection.Tracks;

        if (_openedArtist is not null)
        {
            if (ApplicationContext.Instance.Config.Library.ShowArtistAlbums)
                return LibrarySection.Albums;

            return LibrarySection.Tracks;
        }

        return _section;
    }

    private void LoadLayoutForCurrentContent()
    {
        var config = ApplicationContext.Instance.Config.Library;
        double value;

        switch (GetCurrentContentSection())
        {
            case LibrarySection.Albums:
                value = config.AlbumsLayout;
                break;

            case LibrarySection.Artists:
                value = config.ArtistsLayout;
                break;

            case LibrarySection.Directories:
                value = config.DirectoriesLayout;
                break;

            default:
                value = config.TracksLayout;
                break;
        }

        _isLoadingLayout = true;
        LayoutValue = value;
        _isLoadingLayout = false;

        RaisePropertyChanged(IsDetailedSortAvailableProperty, !IsDetailedSortAvailable, IsDetailedSortAvailable);
        RaisePropertyChanged(IsSimpleSortAvailableProperty, !IsSimpleSortAvailable, IsSimpleSortAvailable);

        if (IsSimpleSortAvailable)
            _sortMode = LibrarySortMode.Title;
    }

    private static string NormalizeDirectoryPath(string path)
    {
        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
    }

    private void SaveLayoutForCurrentContent(double value)
    {
        var config = ApplicationContext.Instance.Config.Library;

        switch (GetCurrentContentSection())
        {
            case LibrarySection.Albums:
                config.AlbumsLayout = value;
                break;

            case LibrarySection.Artists:
                config.ArtistsLayout = value;
                break;

            case LibrarySection.Directories:
                config.DirectoriesLayout = value;
                break;

            default:
                config.TracksLayout = value;
                break;
        }
    }

    #endregion ITEMS

    #region SELECTION

    private void FinishSelectionAction()
    {
        IsSelectionMode = false;
        _lastSelectedItem = null;
    }

    private IReadOnlyList<LibraryItemViewModel> GetSelectedItems()
    {
        return VisibleItems.Where(item => item.IsSelected).ToList();
    }

    private IReadOnlyList<Track> GetSelectedTracks()
    {
        return GetSelectedItems()
            .SelectMany(item => item.Tracks)
            .DistinctBy(track => track.FilePath, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LibraryItemViewModel.IsSelected))
            UpdateSelectionState();
    }

    private void LibraryItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsSelectionMode
            || sender is not Control { DataContext: LibraryItemViewModel item }
            || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed
            || e.Source is Visual visual && visual.FindAncestorOfType<Button>(includeSelf: true) is not null)
        {
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && _lastSelectedItem is not null)
        {
            var start = VisibleItems.IndexOf(_lastSelectedItem);
            var end = VisibleItems.IndexOf(item);

            if (start >= 0 && end >= 0)
            {
                for (var index = Math.Min(start, end); index <= Math.Max(start, end); index++)
                    VisibleItems[index].IsSelected = true;
            }
        }
        else
        {
            item.IsSelected = !item.IsSelected;
        }

        _lastSelectedItem = item;
        e.Handled = true;
    }

    private void ToggleSelectionButton_Click(object? sender, RoutedEventArgs e)
    {
        IsSelectionMode = !IsSelectionMode;
    }

    private void UpdateSelectionState()
    {
        HasSelection = IsSelectionMode && VisibleItems.Any(item => item.IsSelected);
    }

    #endregion SELECTION

    #region PLAYLIST ACTIONS

    private static void AppendTracks(IEnumerable<Track> tracks)
    {
        ApplicationContext.Instance.PlayListCore.AddStoredTracks(tracks);
    }

    private async System.Threading.Tasks.Task<bool> AddTracksToPlayListAsync(IEnumerable<Track> tracks)
    {
        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return false;

        var picker = new InternalMessagePlayListPicker(ApplicationContext.Instance.PlayListLibraryCore.PlayLists);

        if (await window.ShowDialogAsync(picker) != InternalDialogResult.Ok || picker.SelectedPlayList is null)
            return false;

        foreach (var track in tracks)
        {
            if (!picker.SelectedPlayList.Tracks.Any(existing => string.Equals(
                existing.FilePath,
                track.FilePath,
                StringComparison.OrdinalIgnoreCase)))
            {
                picker.SelectedPlayList.Tracks.Add(Core.PlayListLibraryCore.CloneTrack(track));
            }
        }

        picker.SelectedPlayList.Save();
        return true;
    }

    private static void PlayTracks(IEnumerable<Track> tracks)
    {
        var context = ApplicationContext.Instance;
        var first = context.PlayListCore.ReplaceWith(tracks).FirstOrDefault();

        if (first is not null)
            context.PlayerCore.Play(first);
    }

    #endregion PLAYLIST ACTIONS
}
