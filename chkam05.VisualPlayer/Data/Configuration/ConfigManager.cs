using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class ConfigManager : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static ConfigManager _instance;
        private bool _initialized = false;
        private Config _configuration;
        private VisualisationProfile _visualisationProfile;

        private Brush AccentColorBrush;
        private Brush BackgroundColorBrush;
        private Brush BorderColorBrush;
        private Brush ForegroundColorBrush;
        private Brush ImmersiveForegroundColorBrush;

        private Brush InactiveBackgroundColorBrush;
        private Brush InactiveBorderColorBrush;
        private Brush InactiveForegroundColorBrush;

        private Brush MouseOverBackgroundColorBrush;
        private Brush MouseOverBorderColorBrush;
        private Brush MouseOverForegroundColorBrush;

        private Brush PressedBackgroundColorBrush;
        private Brush PressedBorderColorBrush;
        private Brush PressedForegroundColorBrush;

        private Brush SelectedBackgroundColorBrush;
        private Brush SelectedBorderColorBrush;
        private Brush SelectedForegroundColorBrush;


        //  GETTERS & SETTERS

        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigManager();
                    _instance._initialized = true;
                }

                return _instance;
            }
        }

        public PropertyInfo[] Properties
        {
            get => ObjectUtilities.GetObjectProperties(this.GetType());
        }

        #region Advanced

        public bool ShowVersionOnScreen
        {
            get => _configuration.ShowVersionOnScreen;
            set
            {
                _configuration.ShowVersionOnScreen = value;
                OnPropertyChanged(nameof(ShowVersionOnScreen));
            }
        }

        #endregion Advanced

        #region Appearance

        public Color AccentColor
        {
            get => _configuration.AccentColor;
            set
            {
                _configuration.AccentColor = value;
                OnPropertyChanged(nameof(AccentColor));
            }
        }

        public double BackgroundOpacity
        {
            get => _configuration.BackgroundOpacity;
            set
            {
                _configuration.BackgroundOpacity = value;
                OnPropertyChanged(nameof(BackgroundOpacity));
            }
        }

        public double ControlsBackgroundOpacity
        {
            get => _configuration.ControlsBackgroundOpacity;
            set
            {
                _configuration.ControlsBackgroundOpacity = value;
                OnPropertyChanged(nameof(ControlsBackgroundOpacity));
            }
        }

        public AppearanceThemeType ThemeType
        {
            get => _configuration.ThemeType;
            set
            {
                _configuration.ThemeType = value;
                OnPropertyChanged(nameof(ThemeType));
            }
        }

        public AppearanceCustomThemeType ThemeTypeControls
        {
            get => _configuration.ThemeTypeControls;
            set
            {
                _configuration.ThemeTypeControls = value;
                OnPropertyChanged(nameof(ThemeTypeControls));
            }
        }

        public AppearanceCustomThemeType ThemeTypeMenus
        {
            get => _configuration.ThemeTypeMenus;
            set
            {
                _configuration.ThemeTypeMenus = value;
                OnPropertyChanged(nameof(ThemeTypeMenus));
            }
        }

        public List<ColorInfo> UsedColors
        {
            get => _configuration.UsedColors;
            set
            {
                _configuration.UsedColors = value;
                OnPropertyChanged(nameof(UsedColors));
            }
        }

        #endregion Appearance

        #region Appearance Brushes



        #endregion Appearance Brushes

        #region Control Bar

        public bool ControlBarAutoHide
        {
            get => _configuration.ControlBarAutoHide;
            set
            {
                _configuration.ControlBarAutoHide = value;
                OnPropertyChanged(nameof(ControlBarAutoHide));
            }
        }

        #endregion Control Bar

        #region Information Bar

        public bool InformationBarAdvancedTime
        {
            get => _configuration.InformationBarAdvancedTime;
            set
            {
                _configuration.InformationBarAdvancedTime = value;
                OnPropertyChanged(nameof(InformationBarAdvancedTime));
            }
        }

        public InformationBarAutoHide InformationBarAutoHide
        {
            get => _configuration.InformationBarAutoHide;
            set
            {
                _configuration.InformationBarAutoHide = value;
                OnPropertyChanged(nameof(InformationBarAutoHide));
            }
        }

        public FontContainer InformationBarFont
        {
            get => _configuration.InformationBarFont;
            set
            {
                _configuration.InformationBarFont = value;
                OnPropertyChanged(nameof(InformationBarFont));
            }
        }

        public int InformationBarFontSize
        {
            get => _configuration.InformationBarFontSize;
            set
            {
                _configuration.InformationBarFontSize = value;
                OnPropertyChanged(nameof(InformationBarFontSize));
            }
        }

        public FontStyle InformationBarFontStyle
        {
            get => _configuration.InformationBarFontStyle;
            set
            {
                _configuration.InformationBarFontStyle = value;
                OnPropertyChanged(nameof(InformationBarFontStyle));
            }
        }

        public FontStretch InformationBarFontStretch
        {
            get => _configuration.InformationBarFontStretch;
            set
            {
                _configuration.InformationBarFontStretch = value;
                OnPropertyChanged(nameof(InformationBarFontStretch));
            }
        }

        public FontWeight InformationBarFontWeight
        {
            get => _configuration.InformationBarFontWeight;
            set
            {
                _configuration.InformationBarFontWeight = value;
                OnPropertyChanged(nameof(InformationBarFontWeight));
            }
        }

        public bool InformationBarInfoAlbumVisible
        {
            get => _configuration.InformationBarInfoAlbumVisible;
            set
            {
                _configuration.InformationBarInfoAlbumVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoAlbumVisible));
            }
        }

        public bool InformationBarInfoArtistVisible
        {
            get => _configuration.InformationBarInfoArtistVisible;
            set
            {
                _configuration.InformationBarInfoArtistVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoArtistVisible));
            }
        }

        public bool InformationBarInfoCoverVisible
        {
            get => _configuration.InformationBarInfoCoverVisible;
            set
            {
                _configuration.InformationBarInfoCoverVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoCoverVisible));
            }
        }

        public bool InformationBarInfoTimeVisible
        {
            get => _configuration.InformationBarInfoTimeVisible;
            set
            {
                _configuration.InformationBarInfoTimeVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoTimeVisible));
            }
        }

        public bool InformationBarInfoTitleVisible
        {
            get => _configuration.InformationBarInfoTitleVisible;
            set
            {
                _configuration.InformationBarInfoTitleVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoTitleVisible));
            }
        }

        public double InformationBarTextSpacing
        {
            get => _configuration.InformationBarTextSpacing;
            set
            {
                _configuration.InformationBarTextSpacing = value;
                OnPropertyChanged(nameof(InformationBarTextSpacing));
            }
        }

        public int InformationBarTitleTextSizeDiffrence
        {
            get => _configuration.InformationBarTitleTextSizeDiffrence;
            set
            {
                _configuration.InformationBarTitleTextSizeDiffrence = value;
                OnPropertyChanged(nameof(InformationBarTitleTextSizeDiffrence));
            }
        }

        public MarqueeState InformationBarTitleMarqueeState
        {
            get => _configuration.InformationBarTitleMarqueeState;
            set
            {
                _configuration.InformationBarTitleMarqueeState = value;
                OnPropertyChanged(nameof(InformationBarTitleMarqueeState));
            }
        }

        #endregion Information Bar

        #region Logo

        public bool LogoAnimated
        {
            get => _configuration.LogoAnimated;
            set
            {
                _configuration.LogoAnimated = value;
                OnPropertyChanged(nameof(LogoAnimated));
            }
        }

        public double LogoAnimationSensitivity
        {
            get => _configuration.LogoAnimationSensitivity;
            set
            {
                _configuration.LogoAnimationSensitivity = value;
                OnPropertyChanged(nameof(LogoAnimationSensitivity));
            }
        }

        public double LogoAnimationSpeed
        {
            get => _configuration.LogoAnimationSpeed;
            set
            {
                _configuration.LogoAnimationSpeed = value;
                OnPropertyChanged(nameof(LogoAnimationSpeed));
            }
        }

        public bool LogoEnabled
        {
            get => _configuration.LogoEnabled;
            set
            {
                _configuration.LogoEnabled = value;
                OnPropertyChanged(nameof(LogoEnabled));
            }
        }

        public AppearanceLogoTheme LogoThemeType
        {
            get => _configuration.LogoThemeType;
            set
            {
                _configuration.LogoThemeType = value;
                OnPropertyChanged(nameof(LogoThemeType));
            }
        }

        #endregion Logo

        #region Lyrics

        public bool LyricsAutoLoad
        {
            get => _configuration.LyricsAutoLoad;
            set
            {
                _configuration.LyricsAutoLoad = value;
                OnPropertyChanged(nameof(LyricsAutoLoad));
            }
        }

        public double LyricsBackgroundOpacity
        {
            get => _configuration.LyricsBackgroundOpacity;
            set
            {
                _configuration.LyricsBackgroundOpacity = value;
                OnPropertyChanged(nameof(LyricsBackgroundOpacity));
            }
        }

        public FontContainer LyricsFont
        {
            get => _configuration.LyricsFont;
            set
            {
                _configuration.LyricsFont = value;
                OnPropertyChanged(nameof(LyricsFont));
            }
        }

        public int LyricsFontSize
        {
            get => _configuration.LyricsFontSize;
            set
            {
                _configuration.LyricsFontSize = value;
                OnPropertyChanged(nameof(LyricsFontSize));
            }
        }

        public FontStyle LyricsFontStyle
        {
            get => _configuration.LyricsFontStyle;
            set
            {
                _configuration.LyricsFontStyle = value;
                OnPropertyChanged(nameof(LyricsFontStyle));
            }
        }

        public FontStretch LyricsFontStretch
        {
            get => _configuration.LyricsFontStretch;
            set
            {
                _configuration.LyricsFontStretch = value;
                OnPropertyChanged(nameof(LyricsFontStretch));
            }
        }

        public FontWeight LyricsFontWeight
        {
            get => _configuration.LyricsFontWeight;
            set
            {
                _configuration.LyricsFontWeight = value;
                OnPropertyChanged(nameof(LyricsFontWeight));
            }
        }

        public LyricsMatchingType LyricsMatchingType
        {
            get => _configuration.LyricsMatchingType;
            set
            {
                _configuration.LyricsMatchingType = value;
                OnPropertyChanged(nameof(LyricsMatchingType));
            }
        }

        #endregion Lyrics

        #region Menu Bar

        public bool MenuBarAutoHide
        {
            get => _configuration.MenuBarAutoHide;
            set
            {
                _configuration.MenuBarAutoHide = value;
                OnPropertyChanged(nameof(MenuBarAutoHide));
            }
        }

        public bool PlayListAutoHide
        {
            get => _configuration.PlayListAutoHide;
            set
            {
                _configuration.PlayListAutoHide = value;
                OnPropertyChanged(nameof(PlayListAutoHide));
            }
        }

        #endregion Menu Bar

        #region Player

        public bool RestoreLastPlaylistOnStartup
        {
            get => _configuration.RestoreLastPlaylistOnStartup;
            set
            {
                _configuration.RestoreLastPlaylistOnStartup = value;
                OnPropertyChanged(nameof(RestoreLastPlaylistOnStartup));
            }
        }

        #endregion Player

        #region Title Bar

        public bool TitleBarSongInfo
        {
            get => _configuration.TitleBarSongInfo;
            set
            {
                _configuration.TitleBarSongInfo = value;
                OnPropertyChanged(nameof(TitleBarSongInfo));
            }
        }

        public MarqueeState TitleBarSongInfoMarqueeState
        {
            get => _configuration.TitleBarSongInfoMarqueeState;
            set
            {
                _configuration.TitleBarSongInfoMarqueeState = value;
                OnPropertyChanged(nameof(TitleBarSongInfoMarqueeState));
            }
        }

        #endregion Title Bar

        #region Visualisation

        public VisualisationProfile VisualisationProfile
        {
            get => _visualisationProfile;
            set
            {
                _visualisationProfile = value;
                OnPropertyChanged(nameof(VisualisationProfile));
            }
        }

        public string VisualisationProfileName
        {
            get => _configuration.VisualisationProfileName;
            set
            {
                _configuration.VisualisationProfileName = value;
                OnPropertyChanged(nameof(VisualisationProfileName));
            }
        }

        public List<ColorInfo> VisualisationUsedBorderColors
        {
            get => _configuration.VisualisationUsedBorderColors;
            set
            {
                _configuration.VisualisationUsedBorderColors = value;
                OnPropertyChanged(nameof(VisualisationUsedBorderColors));
            }
        }

        public List<ColorInfo> VisualisationUsedFillColors
        {
            get => _configuration.VisualisationUsedFillColors;
            set
            {
                _configuration.VisualisationUsedFillColors = value;
                OnPropertyChanged(nameof(VisualisationUsedFillColors));
            }
        }

        #endregion Visualisation

        #region Window

        public Point WindowPosition
        {
            get => _configuration.WindowPosition;
            set
            {
                _configuration.WindowPosition = value;
                OnPropertyChanged(nameof(WindowPosition));
            }
        }

        public Size WindowSize
        {
            get => _configuration.WindowSize;
            set
            {
                _configuration.WindowSize = value;
                OnPropertyChanged(nameof(WindowSize));
            }
        }

        #endregion Window


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConfigManager private class constructor for configuration. </summary>
        private ConfigManager()
        {
            _configuration = new Config();
            _visualisationProfile = VisualisationProfile.DefaultProfile;
        }

        #endregion CLASS METHODS

        #region APPEARANCE MANAGEMENT METHODS

        #endregion APPEARANCE MANAGEMENT METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration from file. </summary>
        public void LoadConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath + "2";

            if (File.Exists(path))
            {
                try
                {
                    var serialized = File.ReadAllText(path);
                    var configuration = JsonConvert.DeserializeObject<Config>(serialized);

                    if (configuration != null)
                        _configuration = configuration;
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration to file. </summary>
        public void SaveConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath + "2";
            var serialized = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
            File.WriteAllText(path, serialized);
        }

        #endregion LOAD & SAVE METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
