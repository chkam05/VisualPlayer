using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Lucide.Avalonia;
using VisualPlayer.Components;
using VisualPlayer.Controls;
using VisualPlayer.Contexts;
using VisualPlayer.Enums.Config;
using VisualPlayer.Enums.UI;
using VisualPlayer.Events;
using VisualPlayer.Interfaces;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.ViewModels.Lyrics;
using VisualPlayer.Views;
using VisualPlayer.Views.Dialogs;

namespace VisualPlayer.Windows;

public partial class MainWindow : Window
{
    #region CONST

    private const double OVERLAY_BLUR_RADIUS = 18;

    #endregion CONST

    private readonly ApplicationContext _applicationContext;
    private readonly HamburgerMenuContext _hamburgerMenuContext;
    private readonly DispatcherTimer _positionTimer;
    private bool _allowWindowClose;
    private bool _wasPlaying;


    #region CONSTRUCTIRS

    public MainWindow()
    {
        _applicationContext = ApplicationContext.Instance;
        _hamburgerMenuContext = SetupHamburgerMenu();

        InitializeComponent();

        KeyDown += MainWindow_KeyDown;

        AddHandler(
            InputElement.PointerPressedEvent,
            MainWindow_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);

        AddHandler(ColorPaletteControl.AddColorRequestedEvent, ColorPalette_AddColorRequested);

        SetupTransparency();
        SidePanel.MenuContext = _hamburgerMenuContext;

        SetupPlaybackVisibilityBehavior();

        InternalMessages.CurrentViewChanged += Overlay_CurrentViewChanged;
        InternalViews.CurrentViewChanged += Overlay_CurrentViewChanged;
        UpdateOverlayEffects();

        SizeChanged += MainWindow_SizeChanged;
        SidePanel.ConstrainWidth(ClientSize.Width);

        _positionTimer = SetupPositionTimer();

        DragDrop.SetAllowDrop(this, true);
        DragDrop.AddDragOverHandler(this, FileDragOver);
        DragDrop.AddDropHandler(this, FileDrop);
        Closing += MainWindow_Closing;
    }

    private HamburgerMenuContext SetupHamburgerMenu()
    {
        var hamburgerMenuContext = new HamburgerMenuContext();

        hamburgerMenuContext.LibraryRequested += NavigateLibrary;
        hamburgerMenuContext.PlaylistsRequested += NavigatePlaylists;
        hamburgerMenuContext.EqualizerRequested += NavigateEqualizer;
        hamburgerMenuContext.LyricsRequested += NavigateLyrics;
        hamburgerMenuContext.VisualizationsRequested += NavigateVisualizations;
        hamburgerMenuContext.SettingsRequested += NavigateSettings;

        return hamburgerMenuContext;
    }

    private void SetupTransparency()
    {
        TransparencyLevelHint = OperatingSystem.IsWindows()
            ?
            [
                WindowTransparencyLevel.Mica,
                WindowTransparencyLevel.AcrylicBlur,
                WindowTransparencyLevel.Blur,
                WindowTransparencyLevel.Transparent
            ]
            :
            [
                WindowTransparencyLevel.AcrylicBlur,
                WindowTransparencyLevel.Blur,
                WindowTransparencyLevel.Transparent
            ];
    }

    private DispatcherTimer SetupPositionTimer()
    {
        var positionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        positionTimer.Tick += PositionTimer_Tick;

        Opened += (_, _) => positionTimer.Start();
        Closed += (_, _) => positionTimer.Stop();

        return positionTimer;
    }

    private void SetupPlaybackVisibilityBehavior()
    {
        _applicationContext.PlayerDataContext.PropertyChanged += PlayerDataContext_PropertyChanged;
        _applicationContext.PlayerCore.CurrentTrackChanged += PlayerCore_CurrentTrackChanged;

        _wasPlaying = _applicationContext.PlayerDataContext.IsPlaying;

        Closed += MainWindow_Closed;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void PlayerCore_CurrentTrackChanged(object? sender, EventArgs e)
    {
        var appearance = _applicationContext.Config.Appearance;

        if (_applicationContext.PlayerCore.CurrentTrack is not null
            && appearance.InfoBarDisplayMode != InfoBarDisplayMode.Manual)
        {
            TrackInfoBar.ShowTemporarily();
        }
    }

    private void PlayerDataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(PlayerDataContext.IsPlaying))
            return;

        var playerData = _applicationContext.PlayerDataContext;
        var isPlaying = playerData.IsPlaying;

        if (isPlaying && !_wasPlaying)
        {
            var mode = _applicationContext.Config.Appearance.InfoBarDisplayMode;
            var isStartingFromStop = playerData.Position <= double.Epsilon;

            if (mode == InfoBarDisplayMode.PlaybackResumed
                || mode == InfoBarDisplayMode.PlaybackStarted && isStartingFromStop)
            {
                TrackInfoBar.ShowTemporarily();
            }
        }

        _wasPlaying = isPlaying;
    }

    #endregion ACTIONS

    #region COLOR PALETTE ACTIONS

    private async void ColorPalette_AddColorRequested(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not ColorPaletteControl palette)
            return;

        var colorPicker = palette.SelectedColor is { } selectedColor
            ? new InternalMessageColorPicker(selectedColor)
            : new InternalMessageColorPicker();

        var color = await InternalMessages.ShowColorPickerAsync(colorPicker);

        if (color is not null)
            palette.SetCurrentValue(ColorPaletteControl.SelectedColorProperty, color);

        e.Handled = true;
    }

    #endregion COLOR PALETTE ACTIONS

    #region DRAG & DROP

    private void FileDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private async void FileDrop(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer
            .TryGetFiles()?
            .OfType<IStorageFile>()
            .Select(file => file.TryGetLocalPath())
            .Where(path => path is not null)
            .Select(path => path!)
            .ToArray();

        if (files is null || files.Length == 0)
            return;

        if (InternalViews.CurrentView is InternalViewPlayListManager playListManager)
            await playListManager.AddDroppedFilesAsync(files);
        else
            await ApplicationContext.Instance.AddFilesAsync(files);
    }

    #endregion DRAG & DROP

    #region HAMBURGER MENU ACTIONS

    private void NavigateLibrary(object? sender, EventArgs e)
    {
        InternalViews.Open(new InternalViewLibraryManager());
    }

    private void NavigatePlaylists(object? sender, EventArgs e)
    {
        InternalViews.Open(new InternalViewPlayListManager());
    }

    private void NavigateEqualizer(object? sender, EventArgs e)
    {
        InternalViews.Open(new InternalViewEqualizer());
    }

    private void NavigateLyrics(object? sender, EventArgs e)
    {
        var view = new InternalViewLyrics();
        view.EditorRequested += Lyrics_EditorRequested;
        InternalViews.Open(view);
    }

    private void Lyrics_EditorRequested(LyricsDocument document)
    {
        InternalViews.Open(new InternalViewLyricsEditor(document));
    }

    public System.Threading.Tasks.Task<InternalDialogResult> ShowDialogAsync(InternalMessageDialog dialog)
    {
        return InternalMessages.ShowDialogAsync(dialog);
    }

    public System.Threading.Tasks.Task<InternalDialogResult> ShowDialogAsync(InternalMessageInput input)
    {
        return InternalMessages.ShowDialogAsync(input);
    }

    public System.Threading.Tasks.Task<InternalDialogResult> ShowDialogAsync(InternalMessagePlayListPicker picker)
    {
        return InternalMessages.ShowDialogAsync(picker);
    }

    private void NavigateVisualizations(object? sender, EventArgs e)
    {
        var view = new InternalViewVisualizations();
        view.ProfileOverwriteRequested += Visualizations_ProfileOverwriteRequested;

        InternalViews.Open(view);
    }

    private void NavigateSettings(object? sender, EventArgs e)
    {
        InternalViews.Open(new InternalViewSettings());
    }

    private async void Visualizations_ProfileOverwriteRequested(
        object? sender,
        VisualizationProfileOverwriteRequestedEventArgs e)
    {
        if (sender is not InternalViewVisualizations view)
            return;

        var dialog = new InternalMessageDialog(
            _applicationContext.Translations["WINDOW_MAIN_OVERWRITE_VISUALIZATION_TITLE"],
            _applicationContext.Translations.Format(
                "WINDOW_MAIN_OVERWRITE_VISUALIZATION_MESSAGE",
                e.Profile.Name),
            InternalDialogButtons.OkCancel,
            LucideIconKind.TriangleAlert);

        var result = await InternalMessages.ShowDialogAsync(dialog);

        if (result == InternalDialogResult.Ok)
            view.OverwriteProfile(e.Profile);
    }

    #endregion HAMBURGER MENU ACTIONS

    #region PLAYER CONTROL ACTIONS

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if ((!VolumePanel.IsOpen && !SpeedPanel.IsOpen) || e.Source is not Visual source)
            return;

        if (VolumePanel.IsOpen && VolumePanel.Contains(e.GetPosition(VolumePanel))
            || source.FindAncestorOfType<VolumeControl>(includeSelf: true) is not null
            || SpeedPanel.IsOpen && SpeedPanel.Contains(e.GetPosition(SpeedPanel))
            || source.FindAncestorOfType<SpeedControl>(includeSelf: true) is not null
            || PlayPanel.IsVolumeButtonSource(source)
            || PlayPanel.IsSpeedButtonSource(source))
        {
            return;
        }

        VolumePanel.Close();
        SpeedPanel.Close();
    }

    private void PlayPanel_NextRequested(object? sender, RoutedEventArgs e)
    {
        _applicationContext.PlayerCore.Next();
    }

    private void PlayPanel_PlayPauseRequested(object? sender, RoutedEventArgs e)
    {
        if (_applicationContext.PlayerCore.CurrentTrack is null
            && _applicationContext.PlayListCore.PlayListViewModel.SelectedTrack is { } selectedTrack)
        {
            _applicationContext.PlayListCore.PlayListViewModel.Play(selectedTrack);
            return;
        }

        _applicationContext.PlayerCore.TogglePlayPause();
    }

    private void PlayPanel_PositionSeekRequested(object? sender, RoutedEventArgs e)
    {
        _applicationContext.PlayerCore.Seek(PlayPanel.Position);
    }

    private void PositionTimer_Tick(object? sender, EventArgs e)
    {
        _applicationContext.PlayerCore.RefreshPosition();
        _applicationContext.LyricsCore.UpdatePosition(
            TimeSpan.FromSeconds(_applicationContext.PlayerCore.Position));
    }

    private void PlayPanel_PreviousRequested(object? sender, RoutedEventArgs e)
    {
        _applicationContext.PlayerCore.Previous();
    }

    private void PlayPanel_StopRequested(object? sender, RoutedEventArgs e)
    {
        _applicationContext.PlayerCore.Stop();
    }

    private void PlayPanel_SpeedRequested(object? sender, RoutedEventArgs e)
    {
        VolumePanel.Close();
        SpeedPanel.Toggle();
    }

    private void PlayPanel_VolumeRequested(object? sender, RoutedEventArgs e)
    {
        SpeedPanel.Close();
        VolumePanel.Toggle();
    }

    #endregion PLAYER CONTROL ACTIONS

    #region PLAYLIST CONTROL ACTIONS

    private async void PlayList_AddFileRequested(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = _applicationContext.Translations["WINDOW_MAIN_ADD_AUDIO_FILES_TITLE"],
                AllowMultiple = true,
                FileTypeFilter =
                [
                    new FilePickerFileType(_applicationContext.Translations["FILETYPE_AUDIO_FILES"])
                    {
                        Patterns = AudioFileUtils.FilePickerAudioTypes
                    }
                ]
            });

        if (files.Count == 0)
            return;

        var filePaths = files
            .Select(file => file.TryGetLocalPath())
            .Where(path => path is not null)
            .Select(path => path!)
            .ToArray();

        ApplicationContext.Instance.AddFiles(filePaths);
    }

    private async void PlayList_OpenPlaylistRequested(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = _applicationContext.Translations["WINDOW_MAIN_OPEN_PLAYLIST_TITLE"],
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType(_applicationContext.Translations["FILETYPE_VISUALPLAYER_PLAYLIST"])
                    {
                        Patterns = ["*.vpl"]
                    }
                ]
            });

        var filePath = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(filePath))
            _applicationContext.PlayListCore.Load(filePath);
    }

    private async void PlayList_ClearPlaylistRequested(object? sender, RoutedEventArgs e)
    {
        var viewModel = _applicationContext.PlayListCore.PlayListViewModel;

        if (viewModel.Tracks.Count == 0)
            return;

        var dialog = new InternalMessageDialog(
            _applicationContext.Translations["PLAYLIST_ITEM_CLEAR_PLAYLIST"],
            _applicationContext.Translations["WINDOW_MAIN_CLEAR_PLAYLIST_MESSAGE"],
            InternalDialogButtons.YesNo,
            LucideIconKind.TriangleAlert);

        var result = await InternalMessages.ShowDialogAsync(dialog);

        if (result == InternalDialogResult.Yes)
            viewModel.Clear();
    }

    private async void PlayList_SavePlaylistRequested(object? sender, RoutedEventArgs e)
    {
        var input = new InternalMessageInput(
            _applicationContext.Translations["VIEW_PLAYLISTS_SAVE"],
            _applicationContext.PlayListCore.PlayList.Name,
            LucideIconKind.ListMusic);
        var result = await InternalMessages.ShowDialogAsync(input);

        if (result != InternalDialogResult.Ok || string.IsNullOrWhiteSpace(input.Value))
            return;

        _applicationContext.PlayListLibraryCore.Create(
            input.Value,
            _applicationContext.PlayListCore.PlayList.Tracks);
    }

    private async void PlayList_ExportPlaylistRequested(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = _applicationContext.Translations["PLAYLIST_ITEM_EXPORT_PLAYLIST"],
                SuggestedFileName = $"{_applicationContext.PlayListCore.PlayList.Name}.vpl",
                DefaultExtension = "vpl",
                FileTypeChoices =
                [
                    new FilePickerFileType(_applicationContext.Translations["FILETYPE_VISUALPLAYER_PLAYLIST"])
                    {
                        Patterns = ["*.vpl"]
                    }
                ]
            });

        var filePath = file?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(filePath))
            _applicationContext.PlayListCore.Save(filePath);
    }

    #endregion PLAYLIST CONTROL ACTIONS

    #region OVERLAY EVENTS

    private void Overlay_CurrentViewChanged(object? sender, EventArgs e)
    {
        UpdateOverlayEffects();
    }

    private void UpdateOverlayEffects()
    {
        var isMessageOpen = InternalMessages.IsOpen;
        var isViewOpen = InternalViews.IsOpen;

        ApplicationLayer.Effect = isMessageOpen || isViewOpen
            ? new BlurEffect { Radius = OVERLAY_BLUR_RADIUS }
            : null;

        InternalViews.Effect = isMessageOpen
            ? new BlurEffect { Radius = OVERLAY_BLUR_RADIUS }
            : null;
    }

    #endregion OVERLAY EVENTS

    #region WINDOW EVENTS

    private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (_allowWindowClose || InternalViews.CurrentView is not IInternalCloseGuard guard)
            return;

        e.Cancel = true;

        if (!await guard.CanCloseAsync())
            return;

        _allowWindowClose = true;
        Close();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _hamburgerMenuContext.Dispose();
        _applicationContext.PlayerDataContext.PropertyChanged -= PlayerDataContext_PropertyChanged;
        _applicationContext.PlayerCore.CurrentTrackChanged -= PlayerCore_CurrentTrackChanged;
    }

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled || e.Source is TextBox or KeyBindControl)
            return;

        var keyboard = _applicationContext.Config.Keyboard;

        if (KeyboardShortcutUtils.Matches(e, keyboard.TogglePlayList))
            SidePanel.TogglePlayList();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.ToggleInfoBar))
            ToggleAutoHideInfoBar();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.ToggleMenu))
            ToggleAutoHideMenu();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.TogglePlayer))
            ToggleAutoHidePlayer();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.NextTrack))
            _applicationContext.PlayerCore.Next();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.PlayPause))
            PlayPanel_PlayPauseRequested(this, new RoutedEventArgs());
        else if (KeyboardShortcutUtils.Matches(e, keyboard.PreviousTrack))
            _applicationContext.PlayerCore.Previous();
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenLibrary))
            NavigateLibrary(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenPlaylists))
            NavigatePlaylists(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenEqualizer))
            NavigateEqualizer(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenLyrics))
            NavigateLyrics(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenVisualizations))
            NavigateVisualizations(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.OpenSettings))
            NavigateSettings(this, EventArgs.Empty);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.NextVisualizationProfile))
            SelectAdjacentVisualizationProfile(1);
        else if (KeyboardShortcutUtils.Matches(e, keyboard.PreviousVisualizationProfile))
            SelectAdjacentVisualizationProfile(-1);
        else
            return;

        e.Handled = true;
    }

    private void ToggleAutoHideInfoBar()
    {
        var appearance = _applicationContext.Config.Appearance;
        appearance.AutoHideInfoBarControl = !appearance.AutoHideInfoBarControl;
    }

    private void ToggleAutoHideMenu()
    {
        var appearance = _applicationContext.Config.Appearance;
        appearance.AutoHideHamburgerMenu = !appearance.AutoHideHamburgerMenu;
    }

    private void ToggleAutoHidePlayer()
    {
        var appearance = _applicationContext.Config.Appearance;
        appearance.AutoHidePlayerControl = !appearance.AutoHidePlayerControl;
    }

    private void MainWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SidePanel.ConstrainWidth(e.NewSize.Width);
    }

    private void SelectAdjacentVisualizationProfile(int offset)
    {
        var config = _applicationContext.VisualizationConfig;
        config.EnsureProfiles();

        if (config.Profiles.Count == 0)
            return;

        var currentIndex = config.Profiles.IndexOf(config.SelectedProfile);
        var nextIndex = (currentIndex + offset + config.Profiles.Count) % config.Profiles.Count;
        config.SelectedProfileName = config.Profiles[nextIndex].Name;
    }

    #endregion WINDOW EVENTS
}
