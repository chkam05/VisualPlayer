using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Config.Events;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Visualisations;
using chkam05.VisualPlayer.Visualisations.Data;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;


namespace chkam05.VisualPlayer.Data.Config
{
    public class Configuration : INotifyPropertyChanged
    {

        //  CONST

        private static readonly Color DEFAULT_ACCENT_COLOR = Color.FromArgb(255, 0, 120, 215);
        private static readonly Color DEFAULT_THEME_COLOR = Color.FromArgb(192, 0, 0, 0);
        public static readonly double COLOR_OPACITY_MAX = 1.0;
        public static readonly double COLOR_OPACITY_MIN = 0.0;


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ConfigurationUpdateEventArgs> OnAppearanceConfigUpdate;
        public event EventHandler<ConfigurationUpdateEventArgs> OnLyricsConfigUpdate;
        public event EventHandler<ConfigurationUpdateEventArgs> OnVisualisationConfigUpdate;


        //  VARIABLES

        private bool _initialized = false;
        private static Configuration _instance;


        #region Advanced

        private bool _showAdvancedTime = true;
        private bool _showVersionOnScreen = true;

        #endregion Advanced

        #region Appearance Colors Section

        private SolidColorBrush _accentColorBrush = new SolidColorBrush(DEFAULT_ACCENT_COLOR);
        private SolidColorBrush _accentHoveredColorBrush = new SolidColorBrush(Color.FromArgb(244, 0, 120, 215));
        private SolidColorBrush _accentSelectedColorBrush = new SolidColorBrush(DEFAULT_ACCENT_COLOR);
        private SolidColorBrush _buttonColorBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
        private SolidColorBrush _buttonBorderColorBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0));
        private SolidColorBrush _draggingColorBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
        private SolidColorBrush _foregroundColorBrush = new SolidColorBrush(Colors.White);
        private SolidColorBrush _hoveredColorBrush = new SolidColorBrush(Color.FromArgb(192, 255, 255, 255));
        private SolidColorBrush _pressedColorBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        private SolidColorBrush _selectedInactiveColorBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        private SolidColorBrush _themeColorBrush = new SolidColorBrush(DEFAULT_THEME_COLOR);
        private SolidColorBrush _themeControlColorBrush = new SolidColorBrush(DEFAULT_THEME_COLOR);
        private SolidColorBrush _themeDarkColorBrush = new SolidColorBrush(Color.FromArgb(192, 0, 0, 0));
        private SolidColorBrush _themeSolidColorBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        private ObservableCollection<ColorInfo> _usedColors;

        #endregion Appearance Colors Section

        #region Appearance

        private Color _accentColor = DEFAULT_ACCENT_COLOR;
        private AppearanceColorType _appearanceColorType = AppearanceColorType.SYSTEM;
        private AppearanceThemeType _appearanceThemeType = AppearanceThemeType.DARK;
        private SolidColorBrush _backgroundColorBrush = new SolidColorBrush(Colors.Transparent);
        private double _backgroundOpacity = 0.25;
        private double _controlBackgroundOpacity = 0.50;
        private bool _titleBarInfo = true;
        private MarqueeState _titleBarInfoMarqueeState = MarqueeState.TOO_LONG_TEXT;
        private Color _themeColor = DEFAULT_THEME_COLOR;

        #endregion Appearance

        #region Components

        private bool _autoHideControlBar = false;
        private InformationBarAutoHide _autoHideInformationBar = InformationBarAutoHide.STAY_3S;
        private bool _autoHideSideBar = false;
        private bool _autoHidePlayList = false;

        #endregion Components

        #region Data

        private bool _restoreLastPlaylist = true;

        #endregion Data

        #region Informations Bar

        private FontConfigStyle _informationBarFontConfigStyle = FontConfigStyle.Standard;
        private FontContainer _informationBarFontContainer = FontsManager.Instance.DefaultFont;
        private int _informationBarFontSize = 16;
        private FontStyle _informationBarFontStyle = FontStyles.Normal;
        private FontStretch _informationBarFontStretch = FontStretches.Normal;
        private FontWeight _informationBarFontWeight = FontWeights.Normal;
        private int _informationBarFontHeaderDiffrence = 8;
        private int _informationBarFontHeaderSize = 24;
        private double _informationBarFontSpacing = 2.0;
        private Thickness _informationBarFontThickness = new Thickness(0, 0, 0, 2.0);
        private bool _informationBarInfoAlbumVisible = true;
        private bool _informationBarInfoArtistVisible = true;
        private bool _informationBarInfoCoverVisible = true;
        private bool _informationBarInfoTimeVisible = true;
        private bool _informationBarInfoTitleVisible = true;
        private MarqueeState _informationBarTitleMarqueeState = MarqueeState.TOO_LONG_TEXT;

        #endregion Informations Bar

        #region Logo

        private bool _logoAnimated = true;
        private SolidColorBrush _logoBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(DEFAULT_ACCENT_COLOR, a: 192));
        private SolidColorBrush _logoBorderColor = new SolidColorBrush(ColorUtilities.UpdateColor(DEFAULT_ACCENT_COLOR, a: 224));
        private AppearanceLogoColorType _logoColorType = AppearanceLogoColorType.APPLICATION;
        private bool _logoEnabled = true;

        #endregion Logo

        #region Lyrics

        private bool _autoLoadLyrics = false;
        private SolidColorBrush _lyricsBackgroundColor = new SolidColorBrush(DEFAULT_THEME_COLOR);
        private double _lyricsBackgroundOpacity = 0.5;
        private LyricsMatchType _lyricsMatchType = LyricsMatchType.FILE_NAME;
        private FontContainer _lyricsFontContainer = FontsManager.Instance.DefaultFont;
        private int _lyricsFontSize = 40;
        private FontStyle _lyricsFontStyle = FontStyles.Normal;
        private FontStretch _lyricsFontStretch = FontStretches.Normal;
        private FontWeight _lyricsFontWeight = FontWeights.SemiBold;

        #endregion Lyrics

        #region Visualisation

        private double _visualisationBeatFallSpeed = 0.02;
        private double _visualisationBeatSensitivity = 0.5;
        private Color _visualisationBorderColor = DEFAULT_ACCENT_COLOR;
        private bool _visualisationBorderEnabled = false;
        private Color _visualisationColor = DEFAULT_ACCENT_COLOR;
        private double _visualisationColorOpacity = 0.8;
        private VisualisationColorType _visualisationColorType = VisualisationColorType.SYSTEM;
        private double _visualisationRainbowChangeTime = 0.0;
        private int _visualisationRainbowXShift = 0;
        private int _visualisationRainbowYShift = 1000;
        private bool _visualisationRainbowShift = false;
        private ScalingStrategy _visualisationScalingStrategy = ScalingStrategy.SQRT;
        private double _visualisationStripesFallSpeed = 8.0;
        private VisualisationType _visualisationType = VisualisationType.PeaksVisualisation;
        private ObservableCollection<ColorInfo> _visualisationUsedColors;
        private ObservableCollection<ColorInfo> _visualisationUsedColorsBorder;

        #endregion Visualisation

        #region Window

        public Point WindowPosition { get; set; } = new Point(0, 0);
        public Size WindowSize { get; set; } = new Size(800, 600);

        #endregion Window


        //  GETTERS & SETTERS

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadConfiguration();
                    SetupDataContainers();
                    _instance._initialized = true;
                }

                return _instance;
            }
        }

        [JsonIgnore]
        public PropertyInfo[] Properties
        {
            get => ObjectUtilities.GetObjectProperties(this.GetType());
        }


        #region Advanced

        public bool ShowAdvencedTime
        {
            get => _showAdvancedTime;
            set
            {
                _showAdvancedTime = value;
                OnPropertyChanged(nameof(ShowAdvencedTime));
            }
        }

        public bool ShowInfoOnScreen
        {
            get => _showVersionOnScreen;
            set
            {
                _showVersionOnScreen = value;
                OnPropertyChanged(nameof(ShowInfoOnScreen));
            }
        }

        #endregion Advanced

        #region Appearance

        public AppearanceColorType AppearanceColorType
        {
            get => _appearanceColorType;
            set
            {
                _appearanceColorType = value;
                OnPropertyChanged(nameof(AppearanceColorType));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(AppearanceColorType), _appearanceColorType, typeof(AppearanceColorType)));

                if (_initialized)
                {
                    UpdateAccentColor();

                    if (LogoColorType == AppearanceLogoColorType.APPLICATION)
                        UpdateLogoColors();
                }
            }
        }

        public AppearanceThemeType AppearanceThemeType
        {
            get => _appearanceThemeType;
            set
            {
                _appearanceThemeType = value;
                OnPropertyChanged(nameof(AppearanceThemeType));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(AppearanceThemeType), _appearanceThemeType, typeof(AppearanceThemeType)));

                UpdateThemeColor();
                UpdateLyricsColor();
            }
        }

        [JsonIgnore]
        public SolidColorBrush BackgroundColorBrush
        {
            get => _backgroundColorBrush;
            set
            {
                _backgroundColorBrush = value;
                OnPropertyChanged(nameof(BackgroundColorBrush));
            }
        }

        public double BackgroundOpacity
        {
            get => _backgroundOpacity;
            set
            {
                _backgroundOpacity = Math.Max(Math.Min(value, COLOR_OPACITY_MAX), COLOR_OPACITY_MIN);
                OnPropertyChanged(nameof(BackgroundOpacity));

                byte alpha = (byte)Math.Max(_backgroundOpacity * 256 - 1, 0);
                BackgroundColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.Black, a: alpha));
            }
        }

        public double ControlBackgroundOpacity
        {
            get => _controlBackgroundOpacity;
            set
            {
                _controlBackgroundOpacity = Math.Max(Math.Min(value, COLOR_OPACITY_MAX), COLOR_OPACITY_MIN);
                OnPropertyChanged(nameof(ControlBackgroundOpacity));

                byte alpha = (byte)Math.Max(_controlBackgroundOpacity * 256 - 1, 0);
                ThemeControlColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: alpha));
            }
        }

        public bool TitleBarInfo
        {
            get => _titleBarInfo;
            set
            {
                _titleBarInfo = value;
                OnPropertyChanged(nameof(TitleBarInfo));

                if (_titleBarInfo == false)
                    TitleBarInfoMarqueeState = MarqueeState.DISABLED;
            }
        }

        public MarqueeState TitleBarInfoMarqueeState
        {
            get => _titleBarInfoMarqueeState;
            set
            {
                _titleBarInfoMarqueeState = value;
                OnPropertyChanged(nameof(TitleBarInfoMarqueeState));
            }
        }

        #endregion Appearance

        #region Appearance Colors Section

        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                OnPropertyChanged(nameof(AccentColor));

                AccentColorBrush = new SolidColorBrush(_accentColor);
                AccentHoveredColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_accentColor, a: 244));
                AccentSelectedColorBrush = new SolidColorBrush(_accentColor);

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(AccentColor), _accentColor, typeof(Color)));
            }
        }

        [JsonIgnore]
        public Color ThemeColor
        {
            get => _themeColor;
            private set
            {
                var invertedColor = ColorUtilities.InverseColor(value);
                byte controlAlpha = (byte)Math.Max(ControlBackgroundOpacity * 256 - 1, 0);

                _themeColor = value;
                OnPropertyChanged(nameof(ThemeColor));

                ButtonColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: 64));
                ButtonBorderColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: 96));
                DraggingColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(invertedColor, a: 128));
                ForegroundColorBrush = new SolidColorBrush(invertedColor);
                HoveredColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(invertedColor, a: 224));
                PressedColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(invertedColor, a: 128));
                SelectedInactiveColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(invertedColor, a: 128));

                ThemeColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: 192));
                ThemeControlColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: controlAlpha));
                ThemeDarkColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: 224));
                ThemeSolidColorBrush = new SolidColorBrush(ColorUtilities.UpdateColor(_themeColor, a: 225));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(ThemeColor), _themeColor, typeof(Color)));
            }
        }

        [JsonIgnore]
        public SolidColorBrush AccentColorBrush
        {
            get => _accentColorBrush;
            set
            {
                _accentColorBrush = value;
                OnPropertyChanged(nameof(AccentColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush AccentHoveredColorBrush
        {
            get => _accentHoveredColorBrush;
            set
            {
                _accentHoveredColorBrush = value;
                OnPropertyChanged(nameof(AccentHoveredColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush AccentSelectedColorBrush
        {
            get => _accentSelectedColorBrush;
            set
            {
                _accentSelectedColorBrush = value;
                OnPropertyChanged(nameof(AccentSelectedColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ButtonColorBrush
        {
            get => _buttonColorBrush;
            set
            {
                _buttonColorBrush = value;
                OnPropertyChanged(nameof(ButtonColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ButtonBorderColorBrush
        {
            get => _buttonBorderColorBrush;
            set
            {
                _buttonBorderColorBrush = value;
                OnPropertyChanged(nameof(ButtonBorderColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush DraggingColorBrush
        {
            get => _draggingColorBrush;
            set
            {
                _draggingColorBrush = value;
                OnPropertyChanged(nameof(DraggingColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ForegroundColorBrush
        {
            get => _foregroundColorBrush;
            set
            {
                _foregroundColorBrush = value;
                OnPropertyChanged(nameof(ForegroundColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush HoveredColorBrush
        {
            get => _hoveredColorBrush;
            set
            {
                _hoveredColorBrush = value;
                OnPropertyChanged(nameof(HoveredColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush PressedColorBrush
        {
            get => _pressedColorBrush;
            set
            {
                _pressedColorBrush = value;
                OnPropertyChanged(nameof(PressedColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush SelectedInactiveColorBrush
        {
            get => _selectedInactiveColorBrush;
            set
            {
                _selectedInactiveColorBrush = value;
                OnPropertyChanged(nameof(SelectedInactiveColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ThemeColorBrush
        {
            get => _themeColorBrush;
            set
            {
                _themeColorBrush = value;
                OnPropertyChanged(nameof(ThemeColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ThemeControlColorBrush
        {
            get => _themeControlColorBrush;
            set
            {
                _themeControlColorBrush = value;
                OnPropertyChanged(nameof(ThemeControlColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ThemeDarkColorBrush
        {
            get => _themeDarkColorBrush;
            set
            {
                _themeDarkColorBrush = value;
                OnPropertyChanged(nameof(ThemeDarkColorBrush));
            }
        }

        [JsonIgnore]
        public SolidColorBrush ThemeSolidColorBrush
        {
            get => _themeSolidColorBrush;
            set
            {
                _themeSolidColorBrush = value;
                OnPropertyChanged(nameof(ThemeSolidColorBrush));
            }
        }

        #endregion Appearance Colors Section

        #region Components

        public bool AutoHideControlBar
        {
            get => _autoHideControlBar;
            set
            {
                _autoHideControlBar = value;
                OnPropertyChanged(nameof(AutoHideControlBar));
            }
        }

        public InformationBarAutoHide AutoHideInformationBar
        {
            get => _autoHideInformationBar;
            set
            {
                _autoHideInformationBar = value;
                OnPropertyChanged(nameof(AutoHideInformationBar));
            }
        }

        public bool AutoHideSideBar
        {
            get => _autoHideSideBar;
            set
            {
                _autoHideSideBar = value;
                OnPropertyChanged(nameof(AutoHideSideBar));

                if (value)
                    AutoHidePlayList = true;
            }
        }

        public bool AutoHidePlayList
        {
            get => _autoHidePlayList;
            set
            {
                _autoHidePlayList = value;
                OnPropertyChanged(nameof(AutoHidePlayList));

                if (!value)
                    AutoHideSideBar = false;
            }
        }

        #endregion Components

        #region Data

        public bool RestoreLastPlaylist
        {
            get => _restoreLastPlaylist;
            set
            {
                _restoreLastPlaylist = value;
                OnPropertyChanged(nameof(RestoreLastPlaylist));
            }
        }

        #endregion Data

        #region Informations Bar

        public FontConfigStyle InformationBarFontConfigStyle
        {
            get => _informationBarFontConfigStyle;
            set
            {
                _informationBarFontConfigStyle = value;
                OnPropertyChanged(nameof(InformationBarFontConfigStyle));

                switch (_informationBarFontConfigStyle)
                {
                    case FontConfigStyle.Standard:
                        UpdateInformationBarFont(FontConfig.StandardFontConfig);
                        break;

                    case FontConfigStyle.Electronic:
                        UpdateInformationBarFont(FontConfig.ElectronicFontConfig);
                        break;

                    case FontConfigStyle.ElectronicOld:
                        UpdateInformationBarFont(FontConfig.ElectronicOldFontConfig);
                        break;

                    case FontConfigStyle.Custom:
                        UpdateInformationBarFont(InformationBarUserFontConfig);
                        break;
                }
            }
        }

        public FontContainer InformationBarFontContainer
        {
            get => _informationBarFontContainer;
            set
            {
                _informationBarFontContainer = value;
                OnPropertyChanged(nameof(InformationBarFontContainer));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.FontFamily = value;
            }
        }

        public int InformationBarFontSize
        {
            get => _informationBarFontSize;
            set
            {
                _informationBarFontSize = value;
                OnPropertyChanged(nameof(InformationBarFontSize));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.FontSize = value;
            }
        }

        public FontStyle InformationBarFontStyle
        {
            get => _informationBarFontStyle;
            set
            {
                _informationBarFontStyle = value;
                OnPropertyChanged(nameof(InformationBarFontStyle));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.FontStyle = value;
            }
        }

        public FontStretch InformationBarFontStretch
        {
            get => _informationBarFontStretch;
            set
            {
                _informationBarFontStretch = value;
                OnPropertyChanged(nameof(InformationBarFontStretch));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.FontStretch = value;
            }
        }

        public FontWeight InformationBarFontWeight
        {
            get => _informationBarFontWeight;
            set
            {
                _informationBarFontWeight = value;
                OnPropertyChanged(nameof(InformationBarFontWeight));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.FontWeight = value;
            }
        }

        public int InformationBarFontHeaderDiffrence
        {
            get => _informationBarFontHeaderDiffrence;
            set
            {
                _informationBarFontHeaderDiffrence = value;
                InformationBarFontHeaderSize =_informationBarFontSize + value;
                OnPropertyChanged(nameof(InformationBarFontHeaderDiffrence));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.HeaderDiffrence = value;
            }
        }

        [JsonIgnore]
        public int InformationBarFontHeaderSize
        {
            get => _informationBarFontHeaderSize;
            set
            {
                _informationBarFontHeaderSize = value;
                OnPropertyChanged(nameof(InformationBarFontHeaderSize));
            }
        }

        public double InformationBarFontSpacing
        {
            get => _informationBarFontSpacing;
            set
            {
                _informationBarFontSpacing = value;
                InformationBarFontThickness = new Thickness(0, 0, 0, value);
                OnPropertyChanged(nameof(InformationBarFontSpacing));

                if (InformationBarFontConfigStyle == FontConfigStyle.Custom)
                    InformationBarUserFontConfig.Spacing = value;
            }
        }

        [JsonIgnore]
        public Thickness InformationBarFontThickness
        {
            get => _informationBarFontThickness;
            set
            {
                _informationBarFontThickness = value;
                OnPropertyChanged(nameof(InformationBarFontThickness));
            }
        }

        public bool InformationBarInfoAlbumVisible
        {
            get => _informationBarInfoAlbumVisible;
            set
            {
                _informationBarInfoAlbumVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoAlbumVisible));
            }
        }

        public bool InformationBarInfoArtistVisible
        {
            get => _informationBarInfoArtistVisible;
            set
            {
                _informationBarInfoArtistVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoArtistVisible));
            }
        }

        public bool InformationBarInfoCoverVisible
        {
            get => _informationBarInfoCoverVisible;
            set
            {
                _informationBarInfoCoverVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoCoverVisible));
            }
        }

        public bool InformationBarInfoTimeVisible
        {
            get => _informationBarInfoTimeVisible;
            set
            {
                _informationBarInfoTimeVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoTimeVisible));
            }
        }

        public bool InformationBarInfoTitleVisible
        {
            get => _informationBarInfoTitleVisible;
            set
            {
                _informationBarInfoTitleVisible = value;
                OnPropertyChanged(nameof(InformationBarInfoTitleVisible));

                if (_informationBarInfoTitleVisible == false)
                    InformationBarTitleMarqueeState = MarqueeState.DISABLED;
            }
        }

        public MarqueeState InformationBarTitleMarqueeState
        {
            get => _informationBarTitleMarqueeState;
            set
            {
                _informationBarTitleMarqueeState = value;
                OnPropertyChanged(nameof(InformationBarTitleMarqueeState));
            }
        }

        public FontConfig InformationBarUserFontConfig = (FontConfig)FontConfig.StandardFontConfig.Clone();

        #endregion Informations Bar

        #region Logo

        public bool LogoAnimated
        {
            get => _logoAnimated;
            set
            {
                _logoAnimated = value;
                OnPropertyChanged(nameof(LogoAnimated));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LogoAnimated), _logoAnimated, typeof(bool)));
            }
        }

        [JsonIgnore]
        public SolidColorBrush LogoBackgroundColor
        {
            get => _logoBackgroundColor;
            set
            {
                _logoBackgroundColor = value;
                OnPropertyChanged(nameof(LogoBackgroundColor));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LogoBackgroundColor), _logoBackgroundColor, typeof(SolidColorBrush)));
            }
        }

        [JsonIgnore]
        public SolidColorBrush LogoBorderColor
        {
            get => _logoBorderColor;
            set
            {
                _logoBorderColor = value;
                OnPropertyChanged(nameof(LogoBackgroundColor));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LogoBorderColor), _logoBorderColor, typeof(SolidColorBrush)));
            }
        }

        public AppearanceLogoColorType LogoColorType
        {
            get => _logoColorType;
            set
            {
                _logoColorType = value;
                OnPropertyChanged(nameof(LogoColorType));

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LogoColorType), _logoColorType, typeof(AppearanceLogoColorType)));

                UpdateLogoColors();
            }
        }

        public bool LogoEnabled
        {
            get => _logoEnabled;
            set
            {
                _logoEnabled = value;
                OnPropertyChanged(nameof(LogoEnabled));

                if (!value)
                    LogoAnimated = false;

                OnAppearanceConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LogoEnabled), _logoEnabled, typeof(bool)));
            }
        }

        #endregion Logo

        #region Lyrics

        public bool AutoLoadLyrics
        {
            get => _autoLoadLyrics;
            set
            {
                _autoLoadLyrics = value;
                OnPropertyChanged(nameof(AutoLoadLyrics));

                OnLyricsConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(AutoLoadLyrics), _autoLoadLyrics, typeof(bool)));
            }
        }

        [JsonIgnore]
        public SolidColorBrush LyricsBackgroundColor
        {
            get => _lyricsBackgroundColor;
            set
            {
                _lyricsBackgroundColor = value;
                OnPropertyChanged(nameof(LyricsBackgroundColor));
            }
        }

        public double LyricsBackgroundOpacity
        {
            get => _lyricsBackgroundOpacity;
            set
            {
                _lyricsBackgroundOpacity = Math.Max(Math.Min(value, COLOR_OPACITY_MAX), COLOR_OPACITY_MIN);
                OnPropertyChanged(nameof(LyricsBackgroundOpacity));

                UpdateLyricsColor();
            }
        }

        public FontContainer LyricsFontContainer
        {
            get => _lyricsFontContainer;
            set
            {
                _lyricsFontContainer = value;
                OnPropertyChanged(nameof(LyricsFontContainer));
            }
        }

        public int LyricsFontSize
        {
            get => _lyricsFontSize;
            set
            {
                _lyricsFontSize = value;
                OnPropertyChanged(nameof(LyricsFontSize));
            }
        }

        public FontStyle LyricsFontStyle
        {
            get => _lyricsFontStyle;
            set
            {
                _lyricsFontStyle = value;
                OnPropertyChanged(nameof(LyricsFontStyle));
            }
        }

        public FontStretch LyricsFontStretch
        {
            get => _lyricsFontStretch;
            set
            {
                _lyricsFontStretch = value;
                OnPropertyChanged(nameof(LyricsFontStretch));
            }
        }

        public FontWeight LyricsFontWeight
        {
            get => _lyricsFontWeight;
            set
            {
                _lyricsFontWeight = value;
                OnPropertyChanged(nameof(LyricsFontWeight));
            }
        }

        public LyricsMatchType LyricsMatchType
        {
            get => _lyricsMatchType;
            set
            {
                _lyricsMatchType = value;
                OnPropertyChanged(nameof(LyricsMatchType));

                OnLyricsConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(LyricsMatchType), _lyricsMatchType, typeof(LyricsMatchType)));
            }
        }

        #endregion Lyrics

        #region Visualisation

        public double VisualisationBeatFallSpeed
        {
            get => _visualisationBeatFallSpeed;
            set
            {
                _visualisationBeatFallSpeed = Math.Min(Math.Max(
                    value, BeatLevel.FALL_SPEED_MIN), BeatLevel.FALL_SPEED_MAX);
                OnPropertyChanged(nameof(VisualisationBeatFallSpeed));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationBeatFallSpeed), _visualisationBeatFallSpeed, typeof(double)));
            }
        }

        public double VisualisationBeatSensitivity
        {
            get => _visualisationBeatSensitivity;
            set
            {
                _visualisationBeatSensitivity = Math.Min(Math.Max(
                    value, BeatLevel.SENSITIVITY_MIN), BeatLevel.SENSITIVITY_MAX);
                OnPropertyChanged(nameof(VisualisationBeatSensitivity));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationBeatSensitivity), _visualisationBeatSensitivity, typeof(double)));
            }
        }

        public Color VisualisationBorderColor
        {
            get => _visualisationBorderColor;
            set
            {
                _visualisationBorderColor = value;
                OnPropertyChanged(nameof(VisualisationBorderColor));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationBorderColor), _visualisationBorderColor, typeof(Color)));
            }
        }

        public bool VisualisationBorderEnabled
        {
            get => _visualisationBorderEnabled;
            set
            {
                _visualisationBorderEnabled = value;
                OnPropertyChanged(nameof(VisualisationBorderEnabled));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationBorderEnabled), _visualisationBorderEnabled, typeof(bool)));
            }
        }

        public Color VisualisationColor
        {
            get => _visualisationColor;
            set
            {
                _visualisationColor = value;
                OnPropertyChanged(nameof(VisualisationColor));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationColor), _visualisationColor, typeof(Color)));
            }
        }

        public double VisualisationColorOpacity
        {
            get => _visualisationColorOpacity;
            set
            {
                _visualisationColorOpacity = Math.Min(Math.Max(
                    value, StripesVisualisation.OPACITY_MIN), StripesVisualisation.OPACITY_MAX);
                OnPropertyChanged(nameof(VisualisationColorOpacity));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationColorOpacity), _visualisationColorOpacity, typeof(double)));
            }
        }

        public VisualisationColorType VisualisationColorType
        {
            get => _visualisationColorType;
            set
            {
                _visualisationColorType = value;
                OnPropertyChanged(nameof(VisualisationColorType));

                if (_initialized)
                    UpdateVisualisationColor();

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationColorType), _visualisationColorType, typeof(VisualisationColorType)));
            }
        }

        public double VisualisationRainbowChangeTime
        {
            get => _visualisationRainbowChangeTime;
            set
            {
                _visualisationRainbowChangeTime = Math.Max(0.0, value);
                OnPropertyChanged(nameof(VisualisationRainbowChangeTime));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationRainbowChangeTime), _visualisationRainbowChangeTime, typeof(double)));
            }
        }

        public int VisualisationRainbowXShift
        {
            get => _visualisationRainbowXShift;
            set
            {
                _visualisationRainbowXShift = Math.Min(Math.Max(value, AHSLColor.HUE_MIN), AHSLColor.HUE_MAX);
                OnPropertyChanged(nameof(VisualisationRainbowXShift));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationRainbowXShift), _visualisationRainbowXShift, typeof(int)));
            }
        }

        public int VisualisationRainbowYShift
        {
            get => _visualisationRainbowYShift;
            set
            {
                _visualisationRainbowYShift = Math.Min(Math.Max(value, AHSLColor.HUE_MIN), AHSLColor.HUE_MAX);
                OnPropertyChanged(nameof(VisualisationRainbowYShift));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationRainbowYShift), _visualisationRainbowYShift, typeof(int)));
            }
        }

        public bool VisualisationRainbowShift
        {
            get => _visualisationRainbowShift;
            set
            {
                _visualisationRainbowShift = value;
                OnPropertyChanged(nameof(VisualisationRainbowShift));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationRainbowShift), _visualisationRainbowShift, typeof(bool)));
            }
        }

        public ScalingStrategy VisualisationScalingStrategy
        {
            get => _visualisationScalingStrategy;
            set
            {
                _visualisationScalingStrategy = value;
                OnPropertyChanged(nameof(VisualisationScalingStrategy));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationScalingStrategy), _visualisationScalingStrategy, typeof(ScalingStrategy)));
            }
        }

        public double VisualisationStripesFallSpeed
        {
            get => _visualisationStripesFallSpeed;
            set
            {
                _visualisationStripesFallSpeed = Math.Min(Math.Max(
                    value, StripesVisualisation.FALL_SPEED_MIN), StripesVisualisation.FALL_SPEED_MAX);
                OnPropertyChanged(nameof(VisualisationStripesFallSpeed));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationStripesFallSpeed), _visualisationStripesFallSpeed, typeof(double)));
            }
        }

        public VisualisationType VisualisationType
        {
            get => _visualisationType;
            set
            {
                _visualisationType = value;
                OnPropertyChanged(nameof(VisualisationType));

                OnVisualisationConfigUpdate?.Invoke(this, new ConfigurationUpdateEventArgs(
                    nameof(VisualisationType), _visualisationType, typeof(VisualisationType)));
            }
        }

        #endregion Visualisation

        #region Collections

        public ObservableCollection<ColorInfo> UsedColors
        {
            get => _usedColors;
            set
            {
                _usedColors = value;
                OnPropertyChanged(nameof(UsedColors));
            }
        }

        public ObservableCollection<ColorInfo> VisualisationUsedColors
        {
            get => _visualisationUsedColors;
            set
            {
                _visualisationUsedColors = value;
                OnPropertyChanged(nameof(VisualisationUsedColors));
            }
        }

        public ObservableCollection<ColorInfo> VisualisationUsedColorsBorder
        {
            get => _visualisationUsedColorsBorder;
            set
            {
                _visualisationUsedColorsBorder = value;
                OnPropertyChanged(nameof(VisualisationUsedColorsBorder));
            }
        }

        #endregion Collections


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Configuration private class constructor for configuration. </summary>
        private Configuration()
        {
            VisualisationUsedColors = new ObservableCollection<ColorInfo>();
            VisualisationUsedColorsBorder = new ObservableCollection<ColorInfo>();
            UsedColors = new ObservableCollection<ColorInfo>();
        }

        #endregion CLASS METHODS

        #region COLORS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update Accent Color. </summary>
        public void UpdateAccentColor()
        {
            try
            {
                switch (AppearanceColorType)
                {
                    case AppearanceColorType.CUSTOM:
                        AccentColor = UsedColors.First().Color;
                        break;

                    case AppearanceColorType.SYSTEM:
                    default:
                        AccentColor = SystemInfo.GetThemeColor();
                        break;
                }
            }
            catch (Exception)
            {
                //AccentColor = DEFAULT_ACCENT_COLOR;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update Logo Colors.. </summary>
        public void UpdateLogoColors()
        {
            switch (LogoColorType)
            {
                case AppearanceLogoColorType.APPLICATION:
                    LogoBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(AccentColor, a: 192));
                    LogoBorderColor = new SolidColorBrush(ColorUtilities.UpdateColor(AccentColor, a: 224));
                    break;

                case AppearanceLogoColorType.LIGHT:
                    LogoBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.White, a: 192));
                    LogoBorderColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.White, a: 224));
                    break;

                case AppearanceLogoColorType.DARK:
                default:
                    LogoBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.Black, a: 192));
                    LogoBorderColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.Black, a: 224));
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update Lyrics Color. </summary>
        public void UpdateLyricsColor()
        {
            try
            {
                byte alpha = (byte)Math.Max(LyricsBackgroundOpacity * 256 - 1, 0);

                //  Check theme configuration.
                switch (AppearanceThemeType)
                {
                    case AppearanceThemeType.DARK:
                        LyricsBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.Black, a: alpha));
                        break;

                    case AppearanceThemeType.LIGHT:
                        LyricsBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.White, a: alpha));
                        break;

                    case AppearanceThemeType.SYSTEM:
                    default:
                        var themeMode = SystemInfo.GetSystemThemeMode();

                        switch (themeMode)
                        {
                            case SystemTheme.LIGHT:
                                LyricsBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.White, a: alpha));
                                break;

                            case SystemTheme.DARK:
                            default:
                                LyricsBackgroundColor = new SolidColorBrush(ColorUtilities.UpdateColor(Colors.Black, a: alpha));
                                break;
                        }

                        break;
                }
            }
            catch (Exception)
            {
                //ThemeColor = Colors.Black;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update Theme Color. </summary>
        public void UpdateThemeColor()
        {
            try
            {
                //  Check theme configuration.
                switch (AppearanceThemeType)
                {
                    case AppearanceThemeType.DARK:
                        ThemeColor = Colors.Black;
                        break;

                    case AppearanceThemeType.LIGHT:
                        ThemeColor = Colors.White;
                        break;

                    case AppearanceThemeType.SYSTEM:
                    default:
                        var themeMode = SystemInfo.GetSystemThemeMode();

                        switch (themeMode)
                        {
                            case SystemTheme.LIGHT:
                                ThemeColor = Colors.White;
                                break;

                            case SystemTheme.DARK:
                            default:
                                ThemeColor = Colors.Black;
                                break;
                        }

                        break;
                }
            }
            catch (Exception)
            {
                //ThemeColor = Colors.Black;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update Visualisation Color. </summary>
        public void UpdateVisualisationColor()
        {
            try
            {
                switch (VisualisationColorType)
                {
                    case VisualisationColorType.CUSTOM:
                    case VisualisationColorType.RAINBOW_HORIZONTAL:
                    case VisualisationColorType.RAINBOW_VERTICAL:
                        VisualisationColor = VisualisationUsedColors.First().Color;
                        VisualisationBorderColor = VisualisationUsedColorsBorder.First().Color;
                        break;

                    case VisualisationColorType.SYSTEM:
                    default:
                        var systemColor = SystemInfo.GetThemeColor();
                        VisualisationColor = systemColor;
                        VisualisationBorderColor = systemColor;
                        break;
                }
            }
            catch (Exception)
            {
                //VisualisationColor = DEFAULT_ACCENT_COLOR;
                //VisualisationBorderColor = DEFAULT_ACCENT_COLOR;
            }
        }

        #endregion COLORS MANAGEMENT METHODS

        #region FONTS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update Information Bar Font. </summary>
        /// <param name="config"> Font configuration. </param>
        private void UpdateInformationBarFont(FontConfig config)
        {
            InformationBarFontContainer = config.FontFamily;
            InformationBarFontSize = config.FontSize;
            InformationBarFontStyle = config.FontStyle;
            InformationBarFontStretch = config.FontStretch;
            InformationBarFontWeight = config.FontWeight;
            InformationBarFontHeaderDiffrence = config.HeaderDiffrence;
            InformationBarFontSpacing = config.Spacing;
        }

        #endregion FONTS MANAGEMENT METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get configuration property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Property value or null. </returns>
        public object GetValue(string propertyName)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property != null)
                return property.GetValue(this);

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Try get configuration property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Property value or null. </returns>
        public bool TryGetValue(string propertyName, out object value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);
            value = property != null ? property.GetValue(this) : null;
            return property != null && value != null;
        }

        #endregion GET METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration from file. </summary>
        private static Configuration LoadConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath;

            if (File.Exists(path))
            {
                try
                {
                    var serialized = File.ReadAllText(path);
                    var loadedInstance = JsonConvert.DeserializeObject<Configuration>(serialized);

                    if (loadedInstance != null)
                        return loadedInstance;
                }
                catch (Exception)
                {
                    //
                }
            }

            return new Configuration();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration to file. </summary>
        public void Save()
        {
            var path = FilesManager.Instance.ConfigurationFilePath;
            var serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private static void SetupDataContainers()
        {
            if (!_instance.UsedColors.Any())
                _instance.UsedColors = new ObservableCollection<ColorInfo>(ColorUtilities.DefaultColors);

            if (!_instance.VisualisationUsedColors.Any())
                _instance.VisualisationUsedColors = new ObservableCollection<ColorInfo>(ColorUtilities.DefaultColors);

            if (!_instance.VisualisationUsedColorsBorder.Any())
                _instance.VisualisationUsedColorsBorder = new ObservableCollection<ColorInfo>(ColorUtilities.DefaultColors);
        }

        #endregion SETUP METHODS

    }
}
