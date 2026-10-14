using System.IO;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Contexts;

public class ConfigurationContext : ConfigurationContextBase
{
    #region  CONST

    private const string BACKGROUNDS_CATALOG_PATH = "Backgrounds";
    private const string FILE_PATH = "settings.json";
    private const string LYRICS_CATALOG_PATH = "Lyrics";
    private const string LYRICS_CACHE_CATALOG_PATH = "Cache";
    private const string NOW_PLAYING_FILE_PATH = "now_playing.vpl";
    private const string PLAYLISTS_CATALOG_PATH = "PlayLists";

    #endregion CONST

    #region  PRIVATE VARIABLES

    private static ConfigurationContext? _instance;
    private static object _instanceLock = new object();

    private AppearanceConfig _appearance = new();
    private GeneralConfig _general = new();
    private KeyboardConfig _keyboard = new();
    private LibraryConfig _library = new();
    private LogoConfig _logo = new();
    private LyricsConfig _lyrics = new();
    private PlayerConfig _player = new();
    private WindowConfig _window = new();

    #endregion PRIVATE VARIABLES

    #region  PROPERTIES

    [NotifyPropertyUpdate]
    public AppearanceConfig Appearance
    {
        get => _appearance;
        set => UpdateProperty(ref _appearance, value);
    }

    [NotifyPropertyUpdate]
    public GeneralConfig General
    {
        get => _general;
        set => UpdateProperty(ref _general, value ?? new GeneralConfig());
    }

    [NotifyPropertyUpdate]
    public KeyboardConfig Keyboard
    {
        get => _keyboard;
        set => UpdateProperty(ref _keyboard, value ?? new KeyboardConfig());
    }

    [NotifyPropertyUpdate]
    public LibraryConfig Library
    {
        get => _library;
        set => UpdateProperty(ref _library, value ?? new LibraryConfig());
    }

    [NotifyPropertyUpdate]
    public LogoConfig Logo
    {
        get => _logo;
        set => UpdateProperty(ref _logo, value ?? new LogoConfig());
    }

    [NotifyPropertyUpdate]
    public LyricsConfig Lyrics
    {
        get => _lyrics;
        set => UpdateProperty(ref _lyrics, value ?? new LyricsConfig());
    }

    [NotifyPropertyUpdate]
    public PlayerConfig Player
    {
        get => _player;
        set => UpdateProperty(ref _player, value);
    }

    [NotifyPropertyUpdate]
    public WindowConfig Window
    {
        get => _window;
        set => UpdateProperty(ref _window, value);
    }


    public static string BackgroundsDirectoryPath
    {
        get => Path.Combine(DirectoryPath, BACKGROUNDS_CATALOG_PATH);
    }

    public static string FilePath
    {
        get => Path.Combine(DirectoryPath, FILE_PATH);
    }

    public static string LyricsDirectoryPath
    {
        get => Path.Combine(DirectoryPath, LYRICS_CATALOG_PATH);
    }

    public static string LyricsCacheDirectoryPath
    {
        get => Path.Combine(LyricsDirectoryPath, LYRICS_CACHE_CATALOG_PATH);
    }

    public static string NowPlayingFilePath
    {
        get => Path.Combine(DirectoryPath, NOW_PLAYING_FILE_PATH);
    }

    public static string PlayListsDirectoryPath
    {
        get => Path.Combine(DirectoryPath, PLAYLISTS_CATALOG_PATH);
    }

    #endregion PROPERTIES

    #region  CONSTRUCTORS

    [JsonConstructor]
    private ConfigurationContext()
    {
        //
    }

    public static ConfigurationContext Instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                    _instance = new ConfigurationContext();
                
                return _instance;
            }
        }
    }

    #endregion CONSTRUCTORS

    #region  LOAD & SAVE

    public static ConfigurationContext Load()
    {
        lock (_instanceLock)
        {
            _instance = LoadFromFile(FilePath, () => new ConfigurationContext());
            return _instance;
        }
    }

    public void Save()
    {
        SaveToFile(FilePath, this);
    }

    #endregion LOAD & SAVE
}
