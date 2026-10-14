using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels.Config;

public class KeyboardConfig : ViewModelBase
{
    #region CONST

    public const string DEFAULT_TOGGLE_INFO_BAR = "Ctrl+I";
    public const string DEFAULT_TOGGLE_MENU = "Ctrl+M";
    public const string DEFAULT_TOGGLE_PLAYER = "Ctrl+P";
    public const string DEFAULT_TOGGLE_PLAYLIST = "Ctrl+L";
    
    public const string DEFAULT_NEXT_TRACK = "MediaNextTrack";
    public const string DEFAULT_PLAY_PAUSE = "MediaPlayPause";
    public const string DEFAULT_PREVIOUS_TRACK = "MediaPreviousTrack";

    public const string DEFAULT_OPEN_EQUALIZER = "Ctrl+Shift+E";
    public const string DEFAULT_OPEN_LIBRARY = "Ctrl+Shift+L";
    public const string DEFAULT_OPEN_LYRICS = "Ctrl+Shift+T";
    public const string DEFAULT_OPEN_PLAYLISTS = "Ctrl+Shift+P";
    public const string DEFAULT_OPEN_SETTINGS = "Ctrl+Shift+C";
    public const string DEFAULT_OPEN_VISUALIZATIONS = "Ctrl+Shift+V";

    public const string DEFAULT_NEXT_VISUALIZATION_PROFILE = "Ctrl+Right";
    public const string DEFAULT_PREVIOUS_VISUALIZATION_PROFILE = "Ctrl+Left";
    public const bool DEFAULT_USE_SYSTEM_MEDIA_COMMANDS = true;
    
    #endregion CONST

    private string? _toggleInfoBar = DEFAULT_TOGGLE_INFO_BAR;
    private string? _toggleMenu = DEFAULT_TOGGLE_MENU;
    private string? _togglePlayer = DEFAULT_TOGGLE_PLAYER;
    private string? _togglePlayList = DEFAULT_TOGGLE_PLAYLIST;
    
    private string? _nextTrack = DEFAULT_NEXT_TRACK;
    private string? _playPause = DEFAULT_PLAY_PAUSE;
    private string? _previousTrack = DEFAULT_PREVIOUS_TRACK;

    private string? _openEqualizer = DEFAULT_OPEN_EQUALIZER;
    private string? _openLibrary = DEFAULT_OPEN_LIBRARY;
    private string? _openLyrics = DEFAULT_OPEN_LYRICS;
    private string? _openPlaylists = DEFAULT_OPEN_PLAYLISTS;
    private string? _openSettings = DEFAULT_OPEN_SETTINGS;
    private string? _openVisualizations = DEFAULT_OPEN_VISUALIZATIONS;

    private string? _nextVisualizationProfile = DEFAULT_NEXT_VISUALIZATION_PROFILE;
    private string? _previousVisualizationProfile = DEFAULT_PREVIOUS_VISUALIZATION_PROFILE;
    private bool _useSystemMediaCommands = DEFAULT_USE_SYSTEM_MEDIA_COMMANDS;
    
    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool UseSystemMediaCommands
    {
        get => _useSystemMediaCommands;
        set => UpdateProperty(ref _useSystemMediaCommands, value);
    }

    [NotifyPropertyUpdate]
    public string? ToggleInfoBar
    {
        get => _toggleInfoBar;
        set => UpdateProperty(ref _toggleInfoBar, value);
    }

    [NotifyPropertyUpdate]
    public string? ToggleMenu
    {
        get => _toggleMenu;
        set => UpdateProperty(ref _toggleMenu, value);
    }

    [NotifyPropertyUpdate]
    public string? TogglePlayer
    {
        get => _togglePlayer;
        set => UpdateProperty(ref _togglePlayer, value);
    }

    [NotifyPropertyUpdate]
    public string? TogglePlayList
    {
        get => _togglePlayList;
        set => UpdateProperty(ref _togglePlayList, value);
    }


    [NotifyPropertyUpdate]
    public string? NextTrack
    {
        get => _nextTrack;
        set => UpdateProperty(ref _nextTrack, value);
    }

    [NotifyPropertyUpdate]
    public string? PlayPause
    {
        get => _playPause;
        set => UpdateProperty(ref _playPause, value);
    }

    [NotifyPropertyUpdate]
    public string? PreviousTrack
    {
        get => _previousTrack;
        set => UpdateProperty(ref _previousTrack, value);
    }


    [NotifyPropertyUpdate]
    public string? OpenEqualizer
    {
        get => _openEqualizer;
        set => UpdateProperty(ref _openEqualizer, value);
    }

    [NotifyPropertyUpdate]
    public string? OpenLibrary
    {
        get => _openLibrary;
        set => UpdateProperty(ref _openLibrary, value);
    }

    [NotifyPropertyUpdate]
    public string? OpenLyrics
    {
        get => _openLyrics;
        set => UpdateProperty(ref _openLyrics, value);
    }

    [NotifyPropertyUpdate]
    public string? OpenPlaylists
    {
        get => _openPlaylists;
        set => UpdateProperty(ref _openPlaylists, value);
    }

    [NotifyPropertyUpdate]
    public string? OpenSettings
    {
        get => _openSettings;
        set => UpdateProperty(ref _openSettings, value);
    }

    [NotifyPropertyUpdate]
    public string? OpenVisualizations
    {
        get => _openVisualizations;
        set => UpdateProperty(ref _openVisualizations, value);
    }


    [NotifyPropertyUpdate]
    public string? NextVisualizationProfile
    {
        get => _nextVisualizationProfile;
        set => UpdateProperty(ref _nextVisualizationProfile, value);
    }

    [NotifyPropertyUpdate]
    public string? PreviousVisualizationProfile
    {
        get => _previousVisualizationProfile;
        set => UpdateProperty(ref _previousVisualizationProfile, value);
    }

    #endregion PROPERTIES
}
