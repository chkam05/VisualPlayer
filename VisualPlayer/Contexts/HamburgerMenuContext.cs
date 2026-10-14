using System;
using System.Collections.ObjectModel;
using Lucide.Avalonia;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Contexts;

public class HamburgerMenuContext
{
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

    #region CONSTRUCTORS

    public HamburgerMenuContext()
    {
        MainMenuItem = CreateMenuItem(LucideIconKind.Grip, "Menu Główne", null);
        NowPlayingItem = CreateMenuItem(LucideIconKind.ListVideo, "Teraz Odtwarzane", RequestNowPlaying);

        TopItems = new ObservableCollection<HamburgerMenuItemViewModel>
        {
            MainMenuItem,
            CreateMenuItem(LucideIconKind.Library, "Biblioteka", RequestLibrary),
            CreateMenuItem(LucideIconKind.ListMusic, "Listy Odtwarzania", RequestPlaylists),
            HamburgerMenuItemViewModel.CreateSeparator(),
            CreateMenuItem(LucideIconKind.SlidersVertical, "Equalizer", RequestEqualizer),
            CreateMenuItem(LucideIconKind.Type, "Teksty", RequestLyrics),
            CreateMenuItem(LucideIconKind.AudioLines, "Wizualizacje", RequestVisualizations),
            CreateMenuItem(LucideIconKind.Settings, "Ustawienia", RequestSettings)
        };

        BottomItems = new ObservableCollection<HamburgerMenuItemViewModel>
        {
            NowPlayingItem
        };
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

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
