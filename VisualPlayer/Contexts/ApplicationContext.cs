using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using VisualPlayer.Attributes;
using VisualPlayer.Core;
using VisualPlayer.Models.Player;
using VisualPlayer.Utils;
using VisualPlayer.Utils.Platform;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Contexts;

public class ApplicationContext : ViewModelBase
{
    private static ApplicationContext? _instance;
    private static object _instanceLock = new object();

    private ConfigurationContext _config = null!;
    private KeyboardConfig _keyboardConfig = null!;
    private MacOSMediaCommandService? _macOSMediaCommandService;
    private readonly IPlatformSettings? _platformSettings;

    public EqualizerConfigurationContext EqualizerConfig { get; private set; }
    public ControlsState ControlsState { get; private set; }
    public InfoBarContext InfoBarContext { get; private set; }
    public LibraryCore LibraryCore { get; private set; }
    public LyricsCore LyricsCore { get; private set; }
    public PlayListCore PlayListCore { get; private set; }
    public PlayListLibraryCore PlayListLibraryCore { get; private set; }
    public PlayerCore PlayerCore { get; private set; }
    public PlayerDataContext PlayerDataContext => PlayerCore.DataContext;
    public PlayerMiniDataContext PlayerMiniDataContext { get; private set; }
    public TranslationContext Translations { get; private set; }
    public VisualizationCore VisualizationCore { get; private set; }
    public VisualizationConfigurationContext VisualizationConfig { get; private set; }
    

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public ConfigurationContext Config
    {
        get => _config;
        private set => UpdateProperty(ref _config, value);
    }

    #endregion PROPERTIES

    #region  CONSTRUCTORS

    private ApplicationContext()
    {
        Config = ConfigurationContext.Load();
        ControlsState = new ControlsState(Config.Appearance);
        Translations = new TranslationContext(Config.General);
        EqualizerConfig = EqualizerConfigurationContext.Load(Translations);
        VisualizationConfig = VisualizationConfigurationContext.Load();
        _platformSettings = GetPlatformSettings(_config);
        
        PlayListCore = new PlayListCore(Translations);
        LibraryCore = new LibraryCore(Config.Library, PlayListCore.CoverCache);
        PlayListLibraryCore = new PlayListLibraryCore();
        PlayListCore.PlayListViewModel.PropertyChanged += PlayListViewModel_PropertyChanged;

        if (Config.Player.RememberLastPlayList)
            PlayListCore.Load(ConfigurationContext.NowPlayingFilePath);

        PlayerCore = new PlayerCore(Config.Player, PlayListCore, EqualizerConfig);
        PlayerCore.CurrentTrackChanged += PlayerCore_CurrentTrackChanged;
        SetupSystemMediaCommands();
        InfoBarContext = new InfoBarContext(
            PlayerDataContext,
            PlayListCore.PlayListViewModel.CurrentTrack,
            Translations);
        PlayerMiniDataContext = new PlayerMiniDataContext(PlayerDataContext, InfoBarContext);
        LyricsCore = new LyricsCore(Config.Lyrics);
        LyricsCore.SetCurrentTrack(PlayerCore.CurrentTrack);
        VisualizationCore = new VisualizationCore(
            VisualizationConfig,
            PlayerCore.AudioAnalysis,
            () => PlayerCore.IsPlaying);

        _ = LibraryCore.RefreshAsync();
    }

    public static ApplicationContext Instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                    _instance = new ApplicationContext();
                
                return _instance;
            }
        }
    }

    private IPlatformSettings? GetPlatformSettings(ConfigurationContext config)
    {
        var platformSettings = Application.Current?.PlatformSettings;

        if (platformSettings is not null)
        {
            config.Appearance.UpdateSystemTheme(
                SystemUtils.GetSystemTheme(platformSettings.GetColorValues()));

            platformSettings.ColorValuesChanged += PlatformSettings_ColorValuesChanged;
        }

        return platformSettings;
    }

    public void Dispose()
    {
        if (Config.Player.RememberLastPlayList)
            PlayListCore.Save(ConfigurationContext.NowPlayingFilePath);
        else if (File.Exists(ConfigurationContext.NowPlayingFilePath))
            File.Delete(ConfigurationContext.NowPlayingFilePath);

        Config.Save();
        ControlsState.Dispose();
        EqualizerConfig.Save();
        VisualizationConfig.Save();
        Config.PropertyChanged -= Config_PropertyChanged;
        _keyboardConfig.PropertyChanged -= KeyboardConfig_PropertyChanged;
        PlayerDataContext.PropertyChanged -= SystemMediaPlayerDataContext_PropertyChanged;
        _macOSMediaCommandService?.Dispose();
        InfoBarContext.Dispose();
        EqualizerConfig.Dispose();
        LyricsCore.Dispose();
        VisualizationCore.Dispose();
        PlayerCore.Dispose();
        PlayListCore.Dispose();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void SetupSystemMediaCommands()
    {
        _keyboardConfig = Config.Keyboard;
        _keyboardConfig.PropertyChanged += KeyboardConfig_PropertyChanged;
        Config.PropertyChanged += Config_PropertyChanged;

        if (!OperatingSystem.IsMacOS())
            return;

        _macOSMediaCommandService = new MacOSMediaCommandService(
            TogglePlayPauseFromSystem,
            PlayerCore.Next,
            PlayerCore.Previous);
        _macOSMediaCommandService.SetEnabled(_keyboardConfig.UseSystemMediaCommands);
        PlayerDataContext.PropertyChanged += SystemMediaPlayerDataContext_PropertyChanged;
        UpdateSystemNowPlayingInfo();
    }

    private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ConfigurationContext.Keyboard))
            return;

        _keyboardConfig.PropertyChanged -= KeyboardConfig_PropertyChanged;
        _keyboardConfig = Config.Keyboard;
        _keyboardConfig.PropertyChanged += KeyboardConfig_PropertyChanged;
        _macOSMediaCommandService?.SetEnabled(_keyboardConfig.UseSystemMediaCommands);
        UpdateSystemNowPlayingInfo();
    }

    private void KeyboardConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(KeyboardConfig.UseSystemMediaCommands))
        {
            _macOSMediaCommandService?.SetEnabled(_keyboardConfig.UseSystemMediaCommands);
            UpdateSystemNowPlayingInfo();
        }
    }

    private void TogglePlayPauseFromSystem()
    {
        if (PlayerCore.CurrentTrack is null
            && PlayListCore.PlayListViewModel.SelectedTrack is { } selectedTrack)
        {
            PlayListCore.PlayListViewModel.Play(selectedTrack);
            return;
        }

        PlayerCore.TogglePlayPause();
    }

    private void SystemMediaPlayerDataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(PlayerDataContext.IsPlaying)
            or nameof(PlayerDataContext.Duration))
        {
            UpdateSystemNowPlayingInfo();
        }
    }

    private void UpdateSystemNowPlayingInfo()
    {
        var track = PlayerCore.CurrentTrack;

        _macOSMediaCommandService?.UpdateNowPlaying(
            track?.DisplayTitle,
            track?.DisplayAlbum,
            track?.Artist,
            PlayerCore.Duration,
            PlayerCore.Position,
            PlayerCore.IsPlaying);
    }

    private void PlatformSettings_ColorValuesChanged(object? sender, PlatformColorValues e)
    {
        Config.Appearance.UpdateSystemTheme(SystemUtils.GetSystemTheme(e));
    }

    private void PlayerCore_CurrentTrackChanged(object? sender, System.EventArgs e)
    {
        LyricsCore.SetCurrentTrack(PlayerCore.CurrentTrack);
        UpdateSystemNowPlayingInfo();
    }

    private void PlayListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayListViewModel.CurrentTrack))
            InfoBarContext.Track = PlayListCore.PlayListViewModel.CurrentTrack;
    }

    #endregion ACTIONS

    #region FILES MANAGEMENT

    public void AddTracks(IEnumerable<Track> tracks, bool? startPlayback = null)
    {
        var requestedTracks = tracks.ToArray();
        PlayListCore.AddStoredTracks(requestedTracks);
        PlayFirstRequestedFile(requestedTracks.Select(track => track.FilePath), startPlayback);
    }

    public void AddFiles(IEnumerable<string> filePaths)
    {
        var paths = filePaths.ToArray();
        PlayListCore.AddFiles(paths);
        PlayFirstRequestedFile(paths);
    }

    public async Task AddFilesAsync(IEnumerable<string> filePaths)
    {
        var paths = filePaths.ToArray();
        await PlayListCore.AddFilesAsync(paths);
        PlayFirstRequestedFile(paths);
    }

    public void InitializeStartupFiles(IEnumerable<string> filePaths)
    {
        var paths = filePaths.ToArray();
        if (paths.Length > 0)
        {
            AddFiles(paths);
            return;
        }

        if (Config.Player.PlayOnStartup && Config.Player.RememberLastPlayList
            && PlayListCore.RestoredTrack is { } track)
        {
            PlayerCore.Play(track);
        }
    }

    private void PlayFirstRequestedFile(IEnumerable<string> filePaths, bool? startPlayback = null)
    {
        if (!(startPlayback ?? Config.Player.PlayAfterAddingFiles))
            return;

        var comparison = OperatingSystem.IsLinux()
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;
        var track = filePaths
            .Select(AudioFileUtils.NormalizePath)
            .Where(path => path is not null && AudioFileUtils.IsSupported(path))
            .Select(path => PlayListCore.PlayList.Tracks.FirstOrDefault(
                item => string.Equals(item.FilePath, path, comparison)))
            .FirstOrDefault(item => item is not null);

        if (track is not null)
            PlayerCore.Play(track);
    }

    #endregion FILES MANAGEMENT
}
