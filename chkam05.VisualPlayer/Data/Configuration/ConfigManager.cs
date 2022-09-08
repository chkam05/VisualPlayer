﻿using chkam05.Tools.ControlsEx.Colors;
using chkam05.Tools.ControlsEx.Utilities;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration.Attributes;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

using AHSLColor = chkam05.Tools.ControlsEx.Colors.AHSLColor;


namespace chkam05.VisualPlayer.Data.Configuration
{
    public class ConfigManager : INotifyPropertyChanged
    {

        //  CONST

        private const byte BACKGROUND_APLHA = 192;
        private const int INACTIVE_FACTOR = 15;
        private const int MOUSE_OVER_FACTOR = 10;
        private const int PRESSED_FACTOR = 10;
        private const int SELECTED_FACTOR = 10;

        private static List<ColorInfo> DEFAULT_USED_COLORS
        {
            get => new List<ColorInfo>()
            {
                new ColorInfo(ColorsPaletteItems.Blue),
                new ColorInfo(ColorsPaletteItems.Greeny),
                new ColorInfo(ColorsPaletteItems.GrayBrown),
                new ColorInfo(ColorsPaletteItems.Red),
                new ColorInfo(ColorsPaletteItems.LightPink)
            };
        }


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static ConfigManager _instance;
        private bool _initialized = false;
        private Config _configuration;
        private VisualisationProfile _visualisationProfile;

        private Brush _accentColorBrush;
        private Brush _backgroundColorBrush;
        private Brush _borderColorBrush;
        private Brush _foregroundColorBrush;
        private Brush _immersiveForegroundColorBrush;

        private Brush _inactiveBackgroundColorBrush;
        private Brush _inactiveBorderColorBrush;
        private Brush _inactiveForegroundColorBrush;

        private Brush _logoBackgroundColorBrush;
        private Brush _logoBorderColorBrush;

        private Brush _mouseOverBackgroundColorBrush;
        private Brush _mouseOverBorderColorBrush;
        private Brush _mouseOverForegroundColorBrush;

        private Brush _pressedBackgroundColorBrush;
        private Brush _pressedBorderColorBrush;
        private Brush _pressedForegroundColorBrush;

        private Brush _selectedBackgroundColorBrush;
        private Brush _selectedBorderColorBrush;
        private Brush _selectedForegroundColorBrush;


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
                AppearanceUpdate();
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

        public AppearanceColorType ColorType
        {
            get => _configuration.ColorType;
            set
            {
                _configuration.ColorType = value;
                OnPropertyChanged(nameof(ColorType));
                OnPropertyChanged(nameof(IsCustomColorType));
                AppearanceUpdate();
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public bool IsCustomColorType
        {
            get => ColorType == AppearanceColorType.CUSTOM;
            set
            {
                throw new NotImplementedException();
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
                OnPropertyChanged(nameof(IsUserThemeType));
                AppearanceUpdate();
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public bool IsUserThemeType
        {
            get => ThemeType == AppearanceThemeType.USER;
            set
            {
                throw new NotImplementedException();
            }
        }

        public AppearanceCustomThemeType ThemeTypeControls
        {
            get => _configuration.ThemeTypeControls;
            set
            {
                _configuration.ThemeTypeControls = value;
                OnPropertyChanged(nameof(ThemeTypeControls));
                AppearanceUpdate();
            }
        }

        public AppearanceCustomThemeType ThemeTypeMenus
        {
            get => _configuration.ThemeTypeMenus;
            set
            {
                _configuration.ThemeTypeMenus = value;
                OnPropertyChanged(nameof(ThemeTypeMenus));
                AppearanceUpdate();
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
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

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush AccentColorBrush
        {
            get => _accentColorBrush;
            set
            {
                _accentColorBrush = value;
                OnPropertyChanged(nameof(AccentColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush BackgroundColorBrush
        {
            get => _backgroundColorBrush;
            set
            {
                _backgroundColorBrush = value;
                OnPropertyChanged(nameof(BackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush BorderColorBrush
        {
            get => _borderColorBrush;
            set
            {
                _borderColorBrush = value;
                OnPropertyChanged(nameof(BorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ForegroundColorBrush
        {
            get => _foregroundColorBrush;
            set
            {
                _foregroundColorBrush = value;
                OnPropertyChanged(nameof(ForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ImmersiveForegroundColorBrush
        {
            get => _immersiveForegroundColorBrush;
            set
            {
                _immersiveForegroundColorBrush = value;
                OnPropertyChanged(nameof(ImmersiveForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush InactiveBackgroundColorBrush
        {
            get => _inactiveBackgroundColorBrush;
            set
            {
                _inactiveBackgroundColorBrush = value;
                OnPropertyChanged(nameof(InactiveBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush InactiveBorderColorBrush
        {
            get => _inactiveBorderColorBrush;
            set
            {
                _inactiveBorderColorBrush = value;
                OnPropertyChanged(nameof(InactiveBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush InactiveForegroundColorBrush
        {
            get => _inactiveForegroundColorBrush;
            set
            {
                _inactiveForegroundColorBrush = value;
                OnPropertyChanged(nameof(InactiveForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush MouseOverBackgroundColorBrush
        {
            get => _mouseOverBackgroundColorBrush;
            set
            {
                _mouseOverBackgroundColorBrush = value;
                OnPropertyChanged(nameof(MouseOverBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush MouseOverBorderColorBrush
        {
            get => _mouseOverBorderColorBrush;
            set
            {
                _mouseOverBorderColorBrush = value;
                OnPropertyChanged(nameof(MouseOverBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush MouseOverForegroundColorBrush
        {
            get => _mouseOverForegroundColorBrush;
            set
            {
                _mouseOverForegroundColorBrush = value;
                OnPropertyChanged(nameof(MouseOverForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush PressedBackgroundColorBrush
        {
            get => _pressedBackgroundColorBrush;
            set
            {
                _pressedBackgroundColorBrush = value;
                OnPropertyChanged(nameof(PressedBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush PressedBorderColorBrush
        {
            get => _pressedBorderColorBrush;
            set
            {
                _pressedBorderColorBrush = value;
                OnPropertyChanged(nameof(PressedBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush PressedForegroundColorBrush
        {
            get => _pressedForegroundColorBrush;
            set
            {
                _pressedForegroundColorBrush = value;
                OnPropertyChanged(nameof(PressedForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush SelectedBackgroundColorBrush
        {
            get => _selectedBackgroundColorBrush;
            set
            {
                _selectedBackgroundColorBrush = value;
                OnPropertyChanged(nameof(SelectedBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush SelectedBorderColorBrush
        {
            get => _selectedBorderColorBrush;
            set
            {
                _selectedBorderColorBrush = value;
                OnPropertyChanged(nameof(SelectedBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush SelectedForegroundColorBrush
        {
            get => _selectedForegroundColorBrush;
            set
            {
                _selectedForegroundColorBrush = value;
                OnPropertyChanged(nameof(SelectedForegroundColorBrush));
            }
        }

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

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush LogoBackgroundColorBrush
        {
            get => _logoBackgroundColorBrush;
            set
            {
                _logoBackgroundColorBrush = value;
                OnPropertyChanged(nameof(LogoBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush LogoBorderColorBrush
        {
            get => _logoBorderColorBrush;
            set
            {
                _logoBorderColorBrush = value;
                OnPropertyChanged(nameof(LogoBorderColorBrush));
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
                AppearanceUpdate();
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

                if (value)
                    PlayListAutoHide = true;
            }
        }

        public bool PlayListAutoHide
        {
            get => _configuration.PlayListAutoHide;
            set
            {
                _configuration.PlayListAutoHide = value;
                OnPropertyChanged(nameof(PlayListAutoHide));

                if (!value)
                    MenuBarAutoHide = false;
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

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public List<ColorInfo> VisualisationUsedBorderColors
        {
            get => _configuration.VisualisationUsedBorderColors;
            set
            {
                _configuration.VisualisationUsedBorderColors = value;
                OnPropertyChanged(nameof(VisualisationUsedBorderColors));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
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

        //  --------------------------------------------------------------------------------
        private void AppearanceUpdate()
        {
            Color accentColor;
            Color foregroundColor;
            Color themeColor;
            SystemTheme systemTheme = SystemInfo.GetSystemThemeMode();

            //  Setup accent color by color.
            switch (ColorType)
            {
                case AppearanceColorType.CUSTOM:
                    accentColor = AccentColor;
                    break;

                case AppearanceColorType.SYSTEM:
                default:
                    accentColor = SystemInfo.GetThemeColor();
                    break;
            }

            //  Setup accent colors.
            AccentColorBrush = new SolidColorBrush(accentColor);

            //  Setup theme colors by theme type.
            switch (ThemeType)
            {
                case AppearanceThemeType.USER:
                    /*switch (ThemeTypeControls)
                    {
                        case AppearanceCustomThemeType.ACCENT_COLOR:
                            break;

                        case AppearanceCustomThemeType.LIGHT:
                            break;

                        case AppearanceCustomThemeType.DARK:
                        default:
                            break;
                    }*/

                    switch (ThemeTypeMenus)
                    {
                        case AppearanceCustomThemeType.ACCENT_COLOR:
                            foregroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                            themeColor = accentColor;
                            break;

                        case AppearanceCustomThemeType.LIGHT:
                            foregroundColor = Colors.Black;
                            themeColor = Colors.White;
                            break;

                        case AppearanceCustomThemeType.DARK:
                        default:
                            foregroundColor = Colors.White;
                            themeColor = Colors.Black;
                            break;
                    }
                    break;

                case AppearanceThemeType.SYSTEM:
                    switch (systemTheme)
                    {
                        case SystemTheme.LIGHT:
                            foregroundColor = Colors.Black;
                            themeColor = Colors.White;
                            break;

                        case SystemTheme.DARK:
                        default:
                            foregroundColor = Colors.White;
                            themeColor = Colors.Black;
                            break;
                    }
                    break;

                case AppearanceThemeType.ACCENT_COLOR:
                    foregroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                    themeColor = accentColor;
                    break;

                case AppearanceThemeType.LIGHT:
                    foregroundColor = Colors.Black;
                    themeColor = Colors.White;
                    break;

                case AppearanceThemeType.DARK:
                default:
                    foregroundColor = Colors.White;
                    themeColor = Colors.Black;
                    break;
            }

            //  Setup theme colors.
            var accentAhlsColor = AHSLColor.FromColor(accentColor);
            var immersiveColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
            var inactiveColor = ColorsUtilities.UpdateColor(accentAhlsColor, saturation: accentAhlsColor.S - INACTIVE_FACTOR).ToColor();
            var mouseOverColor = ColorsUtilities.UpdateColor(accentAhlsColor, lightness: accentAhlsColor.L + MOUSE_OVER_FACTOR).ToColor();
            var pressedColor = ColorsUtilities.UpdateColor(accentAhlsColor, lightness: accentAhlsColor.L - PRESSED_FACTOR).ToColor();
            var selectedColor = ColorsUtilities.UpdateColor(accentAhlsColor, lightness: accentAhlsColor.L - SELECTED_FACTOR).ToColor();

            BackgroundColorBrush = new SolidColorBrush(ColorsUtilities.UpdateColor(themeColor, a: BACKGROUND_APLHA));
            BorderColorBrush = new SolidColorBrush(accentColor);
            ForegroundColorBrush = new SolidColorBrush(foregroundColor);
            ImmersiveForegroundColorBrush = new SolidColorBrush(immersiveColor);

            InactiveBackgroundColorBrush = new SolidColorBrush(inactiveColor);
            InactiveBorderColorBrush = new SolidColorBrush(inactiveColor);
            InactiveForegroundColorBrush = new SolidColorBrush(immersiveColor);
            MouseOverBackgroundColorBrush = new SolidColorBrush(mouseOverColor);
            MouseOverBorderColorBrush = new SolidColorBrush(accentColor);
            MouseOverForegroundColorBrush = new SolidColorBrush(immersiveColor);
            PressedBackgroundColorBrush = new SolidColorBrush(pressedColor);
            PressedBorderColorBrush = new SolidColorBrush(accentColor);
            PressedForegroundColorBrush = new SolidColorBrush(immersiveColor);
            SelectedBackgroundColorBrush = new SolidColorBrush(selectedColor);
            SelectedBorderColorBrush = new SolidColorBrush(accentColor);
            SelectedForegroundColorBrush = new SolidColorBrush(immersiveColor);

            //  Setup logo colors by logo type.
            switch (LogoThemeType)
            {
                case AppearanceLogoTheme.ACCENT_COLOR:
                    break;

                case AppearanceLogoTheme.LIGHT:
                    break;

                case AppearanceLogoTheme.DARK:
                default:
                    break;
            }
        }

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
                catch (Exception exc)
                {
                    //
                }
            }

            SetupDataContainers();
            UpdateConfigurationProperties();
            AppearanceUpdate();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration to file. </summary>
        public void SaveConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath + "2";
            var serialized = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
            File.WriteAllText(path, serialized);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update configuration properties after loading configuration from file. </summary>
        private void UpdateConfigurationProperties()
        {
            var thisType = this.GetType();
            var properties = Properties.Where(p => p.CanWrite);

            foreach (var propInfo in properties)
            {
                var property = thisType.GetProperty(propInfo.Name);

                if (ObjectUtilities.HasAttribute(property, typeof(ConfigPropertyUpdateAttrib)))
                {
                    var attribs = ObjectUtilities.GetAttribute<ConfigPropertyUpdateAttrib>(property);
                    if (attribs != null && attribs.Any(a => a.AllowUpdate == false))
                        continue;
                }

                OnPropertyChanged(property.Name);
            }
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
        /// <summary> Setup configuration data containers. </summary>
        private void SetupDataContainers()
        {
            if (!_configuration.UsedColors.Any())
                _configuration.UsedColors = DEFAULT_USED_COLORS;

            if (!_configuration.VisualisationUsedBorderColors.Any())
                _configuration.VisualisationUsedBorderColors = DEFAULT_USED_COLORS;

            if (!_configuration.VisualisationUsedFillColors.Any())
                _configuration.VisualisationUsedFillColors = DEFAULT_USED_COLORS;
        }

        #endregion SETUP METHODS

    }
}
