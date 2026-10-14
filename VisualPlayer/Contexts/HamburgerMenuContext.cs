using System;
using System.Collections.ObjectModel;
using Lucide.Avalonia;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public class HamburgerMenuContext : IDisposable
{
    private static TranslationContext T => ApplicationContext.Instance.Translations;

    #region EVENTS

    public event EventHandler? NowPlayingRequested;
    public event EventHandler? LibraryRequested;
    public event EventHandler? LyricsRequested;
    public event EventHandler? PlaylistsRequested;
    public event EventHandler? EqualizerRequested;
    public event EventHandler? VisualizationsRequested;
    public event EventHandler? SettingsRequested;

    #endregion EVENTS

    public HamburgerMenuItemViewModel MainMenuItem { get; }
    public HamburgerMenuItemViewModel NowPlayingItem { get; }
    public ObservableCollection<HamburgerMenuItemViewModel> TopItems { get; }
    public ObservableCollection<HamburgerMenuItemViewModel> BottomItems { get; }

    private readonly HamburgerMenuItemViewModel _libraryItem;
    private readonly HamburgerMenuItemViewModel _playlistsItem;
    private readonly HamburgerMenuItemViewModel _equalizerItem;
    private readonly HamburgerMenuItemViewModel _lyricsItem;
    private readonly HamburgerMenuItemViewModel _visualizationsItem;
    private readonly HamburgerMenuItemViewModel _settingsItem;

    #region CONSTRUCTORS

    public HamburgerMenuContext()
    {
        MainMenuItem = CreateMenuItem(LucideIconKind.Grip, T["MENU_MAIN"], null);
        NowPlayingItem = CreateMenuItem(LucideIconKind.ListVideo, T["MENU_NOW_PLAYING"], RequestNowPlaying);
        _libraryItem = CreateMenuItem(LucideIconKind.Library, T["MENU_LIBRARY"], RequestLibrary);
        _playlistsItem = CreateMenuItem(LucideIconKind.ListMusic, T["MENU_PLAYLISTS"], RequestPlaylists);
        _equalizerItem = CreateMenuItem(LucideIconKind.SlidersVertical, T["MENU_EQUALIZER"], RequestEqualizer);
        _lyricsItem = CreateMenuItem(LucideIconKind.Type, T["MENU_LYRICS"], RequestLyrics);
        _visualizationsItem = CreateMenuItem(LucideIconKind.AudioLines, T["MENU_VISUALIZATIONS"], RequestVisualizations);
        _settingsItem = CreateMenuItem(LucideIconKind.Settings, T["MENU_SETTINGS"], RequestSettings);

        TopItems = new ObservableCollection<HamburgerMenuItemViewModel>
        {
            MainMenuItem,
            _libraryItem,
            _playlistsItem,
            HamburgerMenuItemViewModel.CreateSeparator(),
            _equalizerItem,
            _lyricsItem,
            _visualizationsItem,
            _settingsItem
        };

        BottomItems = new ObservableCollection<HamburgerMenuItemViewModel>
        {
            NowPlayingItem
        };

        T.LanguageChanged += Translations_LanguageChanged;
    }

    #endregion CONSTRUCTORS

    public void Dispose()
    {
        T.LanguageChanged -= Translations_LanguageChanged;
    }

    #region ACTIONS

    private void Translations_LanguageChanged(object? sender, EventArgs e)
    {
        MainMenuItem.Title = T["MENU_MAIN"];
        NowPlayingItem.Title = T["MENU_NOW_PLAYING"];
        _libraryItem.Title = T["MENU_LIBRARY"];
        _playlistsItem.Title = T["MENU_PLAYLISTS"];
        _equalizerItem.Title = T["MENU_EQUALIZER"];
        _lyricsItem.Title = T["MENU_LYRICS"];
        _visualizationsItem.Title = T["MENU_VISUALIZATIONS"];
        _settingsItem.Title = T["MENU_SETTINGS"];
    }

    private void RequestNowPlaying()
    {
        NowPlayingRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestLibrary()
    {
        LibraryRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestLyrics()
    {
        LyricsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestPlaylists()
    {
        PlaylistsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestEqualizer()
    {
        EqualizerRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestVisualizations()
    {
        VisualizationsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void RequestSettings()
    {
        SettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    #endregion ACTIONS

    #region UTILITIES

    private static HamburgerMenuItemViewModel CreateMenuItem(LucideIconKind icon, string title, Action? action)
    {
        return new HamburgerMenuItemViewModel(icon, title, action);
    }

    #endregion UTILITIES
}
