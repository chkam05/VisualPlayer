using chkam05.Tools.ControlsEx.Colors;
using chkam05.Tools.ControlsEx.Static;
using chkam05.Tools.ControlsEx.Utilities;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration.Attributes;
using chkam05.VisualPlayer.Data.Configuration.Events;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Profiles;
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
        private const int CONTRAST_MOUSE_OVER_FACTOR = 25;
        private const int CONTRAST_PRESSED_FACTOR = 50;
        private const double LOGO_ALPHA = 0.87d;
        private const int INACTIVE_FACTOR = 15;
        private const int MOUSE_OVER_FACTOR = 15;
        private const int PRESSED_FACTOR = 10;
        private const int SELECTED_FACTOR = 5;

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

        public const double COLOR_OPACITY_MAX = 1.0;
        public const double COLOR_OPACITY_MIN = 0.0;
        public const double INFOBAR_FONT_HEADER_DIFFRENCE_MAX = 10;
        public const double INFOBAR_FONT_HEADER_DIFFRENCE_MIN = 0;
        public const double INFOBAR_FONT_SPACING_MAX = 8.0;
        public const double INFOBAR_FONT_SPACING_MIN = 2.0;

        public static readonly List<string> AppearanceUpdateProperties = new List<string>()
        {
            nameof(AccentColor),
            nameof(BackgroundOpacity),
            nameof(ColorType),
            nameof(ControlsBackgroundOpacity),
            nameof(ThemeType),
            nameof(ThemeTypeControls),
            nameof(ThemeTypeMenus),
            nameof(LogoAnimated),
            nameof(LogoEnabled),
            nameof(LogoThemeType),
            nameof(LyricsBackgroundOpacity)
        };

        public static readonly List<string> EqualizerUpdateProperties = new List<string>()
        {
            nameof(EqualizerEnabled),
            nameof(EqualizerPresetName),
        };

        public static readonly List<string> LyricsUpdateProperties = new List<string>()
        {
            nameof(LyricsAutoLoad),
            nameof(LyricsMatchingType)
        };

        public static readonly List<string> VisualisationUpdateProperties = new List<string>()
        {
            nameof(LogoAnimationSensitivity),
            nameof(LogoAnimationSpeed),
            nameof(VisualisationAnimationSpeed),
            nameof(VisualisationBorderColor),
            nameof(VisualisationBorderEnabled),
            nameof(VisualisationColor),
            nameof(VisualisationColorOpacity),
            nameof(VisualisationColorType),
            nameof(VisualisationRainbowChangeTime),
            nameof(VisualisationRainbowShift),
            nameof(VisualisationRainbowXShift),
            nameof(VisualisationRainbowYShift),
            nameof(VisualisationScalingStrategy),
            nameof(VisualisationType),
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ConfigUpdateEventArgs> OnConfigUpdate;


        //  VARIABLES

        private static ConfigManager _instance;
        private bool _initialized = false;
        private bool _lockVisualisationConfig = false;
        private Config _configuration;
        private string _screenVersion = "";
        private VisualisationProfilesManager _visualisationProfilesManager;

        #region Appearance Brushes

        //  General

        private Brush _accentColorBrush;
        private Brush _accentForegroundColorBrush;
        private Brush _backgroundColorBrush;
        private Brush _borderColorBrush;
        private Brush _foregroundColorBrush;

        private Brush _contrastedColorBrush;
        private Brush _contrastedForegroundColorBrush;
        private Brush _contrastedMouseOverColorBrush;
        private Brush _contrastedMouseOverForegroundColorBrush;
        private Brush _contrastedPressedColorBrush;
        private Brush _contrastedPressedForegroundColorBrush;

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

        private Brush _trackBarBackgroundColorBrush;

        //  Interface

        private Brush _ifaceBackgroundColorBrush;
        private Brush _ifaceForegroundColorBrush;

        private Brush _ifaceButtonBackgroundColorBrush;
        private Brush _ifaceButtonBorderColorBrush;
        private Brush _ifaceButtonForegroundColorBrush;

        private Brush _ifaceContextMenuBackgroundBrush;
        private Brush _ifaceContextMenuBorderBrush;

        private Brush _ifaceLyricsBackgroundColorBrush;
        private Brush _ifaceLyricsForegroundColorBrush;

        private Brush _ifaceMenuItemBackgroundColorBrush;
        private Brush _ifaceMenuItemBorderColorBrush;
        private Brush _ifaceMenuItemForegroundColorBrush;

        private Brush _ifaceMouseOverBackgroundColorBrush;
        private Brush _ifaceMouseOverBorderColorBrush;
        private Brush _ifaceMouseOverForegroundColorBrush;
        private Brush _ifacePressedBackgroundColorBrush;
        private Brush _ifacePressedBorderColorBrush;
        private Brush _ifacePressedForegroundColorBrush;
        private Brush _ifaceSelectedBackgroundColorBrush;
        private Brush _ifaceSelectedBorderColorBrush;
        private Brush _ifaceSelectedForegroundColorBrush;

        private Brush _ifaceTrackBarBackgroundColorBrush;

        private Brush _pageBackgroundColorBrush;
        private Brush _windowBackgroundColorBrush;

        //  Sub components

        private Brush _subcomponentBackgroundColorBrush;
        private Brush _subcomponentForegroundColorBrush;

        #endregion Appearance Brushes

        #region Information Bar

        private Thickness _informationBarTextMargin;
        private int _informationBarTitleFontSize;

        #endregion Information Bar


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

        public string ScreenVersion
        {
            get => _screenVersion;
            private set
            {
                _screenVersion = value;
                OnPropertyChanged(nameof(ScreenVersion));
            }
        }

        public VisualisationProfilesManager VisualisationProfilesManager
        {
            get => _visualisationProfilesManager;
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
                AppearanceUpdate();
                OnPropertyChanged(nameof(AccentColor));
            }
        }

        public double BackgroundOpacity
        {
            get => _configuration.BackgroundOpacity;
            set
            {
                _configuration.BackgroundOpacity = value;
                AppearanceUpdate();
                OnPropertyChanged(nameof(BackgroundOpacity));
            }
        }

        public AppearanceColorType ColorType
        {
            get => _configuration.ColorType;
            set
            {
                _configuration.ColorType = value;
                AppearanceUpdate();
                OnPropertyChanged(nameof(ColorType));
                OnPropertyChanged(nameof(IsCustomColorType));
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
                AppearanceUpdate();
                OnPropertyChanged(nameof(ControlsBackgroundOpacity));
            }
        }

        public AppearanceThemeType ThemeType
        {
            get => _configuration.ThemeType;
            set
            {
                _configuration.ThemeType = value;
                AppearanceUpdate();
                OnPropertyChanged(nameof(ThemeType));
                OnPropertyChanged(nameof(IsUserThemeType));
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
                AppearanceUpdate();
                OnPropertyChanged(nameof(ThemeTypeControls));
            }
        }

        public AppearanceCustomThemeType ThemeTypeMenus
        {
            get => _configuration.ThemeTypeMenus;
            set
            {
                _configuration.ThemeTypeMenus = value;
                AppearanceUpdate();
                OnPropertyChanged(nameof(ThemeTypeMenus));
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

        //  General

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
        public Brush AccentForegroundColorBrush
        {
            get => _accentForegroundColorBrush;
            set
            {
                _accentForegroundColorBrush = value;
                OnPropertyChanged(nameof(AccentForegroundColorBrush));
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
        public Brush ContrastedColorBrush
        {
            get => _contrastedColorBrush;
            set
            {
                _contrastedColorBrush = value;
                OnPropertyChanged(nameof(ContrastedColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ContrastedForegroundColorBrush
        {
            get => _contrastedForegroundColorBrush;
            set
            {
                _contrastedForegroundColorBrush = value;
                OnPropertyChanged(nameof(ContrastedForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ContrastedMouseOverColorBrush
        {
            get => _contrastedMouseOverColorBrush;
            set
            {
                _contrastedMouseOverColorBrush = value;
                OnPropertyChanged(nameof(ContrastedMouseOverColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ContrastedMouseOverForegroundColorBrush
        {
            get => _contrastedMouseOverForegroundColorBrush;
            set
            {
                _contrastedMouseOverForegroundColorBrush = value;
                OnPropertyChanged(nameof(ContrastedMouseOverForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ContrastedPressedColorBrush
        {
            get => _contrastedPressedColorBrush;
            set
            {
                _contrastedPressedColorBrush = value;
                OnPropertyChanged(nameof(ContrastedPressedColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush ContrastedPressedForegroundColorBrush
        {
            get => _contrastedPressedForegroundColorBrush;
            set
            {
                _contrastedPressedForegroundColorBrush = value;
                OnPropertyChanged(nameof(ContrastedPressedForegroundColorBrush));
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

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush TrackBarBackgroundColorBrush
        {
            get => _trackBarBackgroundColorBrush;
            set
            {
                _trackBarBackgroundColorBrush = value;
                OnPropertyChanged(nameof(TrackBarBackgroundColorBrush));
            }
        }

        //  Interface

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceBackgroundColorBrush
        {
            get => _ifaceBackgroundColorBrush;
            set
            {
                _ifaceBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceForegroundColorBrush
        {
            get => _ifaceForegroundColorBrush;
            set
            {
                _ifaceForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceButtonBackgroundColorBrush
        {
            get => _ifaceButtonBackgroundColorBrush;
            set
            {
                _ifaceButtonBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceButtonBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceButtonBorderColorBrush
        {
            get => _ifaceButtonBorderColorBrush;
            set
            {
                _ifaceButtonBorderColorBrush = value;
                OnPropertyChanged(nameof(IfaceButtonBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceButtonForegroundColorBrush
        {
            get => _ifaceButtonForegroundColorBrush;
            set
            {
                _ifaceButtonForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceButtonForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceContextMenuBackgroundColorBrush
        {
            get => _ifaceContextMenuBackgroundBrush;
            set
            {
                _ifaceContextMenuBackgroundBrush = value;
                OnPropertyChanged(nameof(IfaceContextMenuBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceContextMenuBorderColorBrush
        {
            get => _ifaceContextMenuBorderBrush;
            set
            {
                _ifaceContextMenuBorderBrush = value;
                OnPropertyChanged(nameof(IfaceContextMenuBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceLyricsBackgroundColorBrush
        {
            get => _ifaceLyricsBackgroundColorBrush;
            set
            {
                _ifaceLyricsBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceLyricsBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceLyricsForegroundColorBrush
        {
            get => _ifaceLyricsForegroundColorBrush;
            set
            {
                _ifaceLyricsForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceLyricsForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMenuItemBackgroundColorBrush
        {
            get => _ifaceMenuItemBackgroundColorBrush;
            set
            {
                _ifaceMenuItemBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceMenuItemBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMenuItemBorderColorBrush
        {
            get => _ifaceMenuItemBorderColorBrush;
            set
            {
                _ifaceMenuItemBorderColorBrush = value;
                OnPropertyChanged(nameof(IfaceMenuItemBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMenuItemForegroundColorBrush
        {
            get => _ifaceMenuItemForegroundColorBrush;
            set
            {
                _ifaceMenuItemForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceMenuItemForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMouseOverBackgroundColorBrush
        {
            get => _ifaceMouseOverBackgroundColorBrush;
            set
            {
                _ifaceMouseOverBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceMouseOverBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMouseOverBorderColorBrush
        {
            get => _ifaceMouseOverBorderColorBrush;
            set
            {
                _ifaceMouseOverBorderColorBrush = value;
                OnPropertyChanged(nameof(IfaceMouseOverBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceMouseOverForegroundColorBrush
        {
            get => _ifaceMouseOverForegroundColorBrush;
            set
            {
                _ifaceMouseOverForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceMouseOverForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfacePressedBackgroundColorBrush
        {
            get => _ifacePressedBackgroundColorBrush;
            set
            {
                _ifacePressedBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfacePressedBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfacePressedBorderColorBrush
        {
            get => _ifacePressedBorderColorBrush;
            set
            {
                _ifacePressedBorderColorBrush = value;
                OnPropertyChanged(nameof(IfacePressedBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfacePressedForegroundColorBrush
        {
            get => _ifacePressedForegroundColorBrush;
            set
            {
                _ifacePressedForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfacePressedForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceSelectedBackgroundColorBrush
        {
            get => _ifaceSelectedBackgroundColorBrush;
            set
            {
                _ifaceSelectedBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceSelectedBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceSelectedBorderColorBrush
        {
            get => _ifaceSelectedBorderColorBrush;
            set
            {
                _ifaceSelectedBorderColorBrush = value;
                OnPropertyChanged(nameof(IfaceSelectedBorderColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceSelectedForegroundColorBrush
        {
            get => _ifaceSelectedForegroundColorBrush;
            set
            {
                _ifaceSelectedForegroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceSelectedForegroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush IfaceTrackBarBackgroundColorBrush
        {
            get => _ifaceTrackBarBackgroundColorBrush;
            set
            {
                _ifaceTrackBarBackgroundColorBrush = value;
                OnPropertyChanged(nameof(IfaceTrackBarBackgroundColorBrush));
            }
        }

        //  Pages

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush PageBackgroundColorBrush
        {
            get => _pageBackgroundColorBrush;
            set
            {
                _pageBackgroundColorBrush = value;
                OnPropertyChanged(nameof(PageBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush WindowBackgroundColorBrush
        {
            get => _windowBackgroundColorBrush;
            set
            {
                _windowBackgroundColorBrush = value;
                OnPropertyChanged(nameof(WindowBackgroundColorBrush));
            }
        }

        //  Sub components

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush SubcomponentBackgroundColorBrush
        {
            get => _subcomponentBackgroundColorBrush;
            set
            {
                _subcomponentBackgroundColorBrush = value;
                OnPropertyChanged(nameof(SubcomponentBackgroundColorBrush));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Brush SubcomponentForegroundColorBrush
        {
            get => _subcomponentForegroundColorBrush;
            set
            {
                _subcomponentForegroundColorBrush = value;
                OnPropertyChanged(nameof(SubcomponentForegroundColorBrush));
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

        #region Equalizer

        public bool EqualizerEnabled
        {
            get => _configuration.EqualizerEnabled;
            set
            {
                _configuration.EqualizerEnabled = value;
                OnPropertyChanged(nameof(EqualizerEnabled));
            }
        }

        public string EqualizerPresetName
        {
            get => _configuration.EqualizerPresetName;
            set
            {
                _configuration.EqualizerPresetName = value;
                OnPropertyChanged(nameof(EqualizerPresetName));
            }
        }

        #endregion Equalizer

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

        public string InformationBarFontName
        {
            get => _configuration.InformationBarFont;
            set
            {
                _configuration.InformationBarFont = value;
                OnPropertyChanged(nameof(InformationBarFontName));
                OnPropertyChanged(nameof(InformationBarFont));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public FontContainer InformationBarFont
        {
            get => FontsManager.Instance.Fonts.FirstOrDefault(f => f.ToString() == _configuration.InformationBarFont) 
                ?? FontsManager.Instance.DefaultFont;
            set
            {
                _configuration.InformationBarFont = value.ToString();
                OnPropertyChanged(nameof(InformationBarFontName));
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
                UpdateInformationBarTitleFontSize();
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
                UpdateInformationBarTextMargins();
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public Thickness InformationBarTextMargin
        {
            get => _informationBarTextMargin;
            set
            {
                _informationBarTextMargin = value;
                OnPropertyChanged(nameof(InformationBarTextMargin));
            }
        }

        public int InformationBarTitleTextSizeDiffrence
        {
            get => _configuration.InformationBarTitleTextSizeDiffrence;
            set
            {
                _configuration.InformationBarTitleTextSizeDiffrence = value;
                OnPropertyChanged(nameof(InformationBarTitleTextSizeDiffrence));
                UpdateInformationBarTitleFontSize();
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public int InformationBarTitleFontSize
        {
            get => _informationBarTitleFontSize;
            set
            {
                _informationBarTitleFontSize = value;
                OnPropertyChanged(nameof(InformationBarTitleFontSize));
            }
        }

        public MarqueeTextBlockState InformationBarTitleMarqueeState
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
                AppearanceUpdate();
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

        public string LyricsFontName
        {
            get => _configuration.LyricsFont;
            set
            {
                _configuration.LyricsFont = value;
                OnPropertyChanged(nameof(LyricsFontName));
                OnPropertyChanged(nameof(LyricsFont));
            }
        }

        [ConfigPropertyUpdateAttrib(AllowUpdate = false)]
        public FontContainer LyricsFont
        {
            get => FontsManager.Instance.Fonts.FirstOrDefault(f => f.ToString() == _configuration.LyricsFont)
                ?? FontsManager.Instance.DefaultFont;
            set
            {
                _configuration.LyricsFont = value.ToString();
                OnPropertyChanged(nameof(LyricsFontName));
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

        public MarqueeTextBlockState TitleBarSongInfoMarqueeState
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

        public string VisualisationProfileName
        {
            get => _configuration.VisualisationProfileName;
            set
            {
                _configuration.VisualisationProfileName = value;
                OnPropertyChanged(nameof(VisualisationProfileName));
                VisualisationProfileUpdate();
            }
        }

        public double VisualisationAnimationSpeed
        {
            get => _visualisationProfilesManager.Profile.AnimationSpeed;
            set
            {
                _visualisationProfilesManager.Profile.AnimationSpeed = value;
                OnPropertyChanged(nameof(VisualisationAnimationSpeed));
            }
        }

        public Color VisualisationBorderColor
        {
            get => _visualisationProfilesManager.Profile.BorderColor;
            set
            {
                _visualisationProfilesManager.Profile.BorderColor = value;
                OnPropertyChanged(nameof(VisualisationBorderColor));
            }
        }

        public bool VisualisationBorderEnabled
        {
            get => _visualisationProfilesManager.Profile.BorderEnabled;
            set
            {
                _visualisationProfilesManager.Profile.BorderEnabled = value;
                OnPropertyChanged(nameof(VisualisationBorderEnabled));
            }
        }

        public Color VisualisationColor
        {
            get => _visualisationProfilesManager.Profile.Color;
            set
            {
                _visualisationProfilesManager.Profile.Color = value;
                OnPropertyChanged(nameof(VisualisationColor));
            }
        }

        public double VisualisationColorOpacity
        {
            get => _visualisationProfilesManager.Profile.ColorOpacity;
            set
            {
                _visualisationProfilesManager.Profile.ColorOpacity = value;
                OnPropertyChanged(nameof(VisualisationColorOpacity));
            }
        }

        public VisualisationColorType VisualisationColorType
        {
            get => _visualisationProfilesManager.Profile.ColorType;
            set
            {
                _visualisationProfilesManager.Profile.ColorType = value;
                OnPropertyChanged(nameof(VisualisationColorType));
            }
        }

        public double VisualisationRainbowChangeTime
        {
            get => _visualisationProfilesManager.Profile.RainbowChangeTime;
            set
            {
                _visualisationProfilesManager.Profile.RainbowChangeTime = value;
                OnPropertyChanged(nameof(VisualisationRainbowChangeTime));
            }
        }

        public bool VisualisationRainbowShift
        {
            get => _visualisationProfilesManager.Profile.RainbowShift;
            set
            {
                _visualisationProfilesManager.Profile.RainbowShift = value;
                OnPropertyChanged(nameof(VisualisationRainbowShift));
            }
        }

        public int VisualisationRainbowXShift
        {
            get => _visualisationProfilesManager.Profile.RainbowXShift;
            set
            {
                _visualisationProfilesManager.Profile.RainbowXShift = value;
                OnPropertyChanged(nameof(VisualisationRainbowXShift));
            }
        }

        public int VisualisationRainbowYShift
        {
            get => _visualisationProfilesManager.Profile.RainbowYShift;
            set
            {
                _visualisationProfilesManager.Profile.RainbowYShift = value;
                OnPropertyChanged(nameof(VisualisationRainbowYShift));
            }
        }

        public ScalingStrategy VisualisationScalingStrategy
        {
            get => _visualisationProfilesManager.Profile.ScalingStrategy;
            set
            {
                _visualisationProfilesManager.Profile.ScalingStrategy = value;
                OnPropertyChanged(nameof(VisualisationScalingStrategy));
            }
        }

        public VisualisationType VisualisationType
        {
            get => _visualisationProfilesManager.Profile.Type;
            set
            {
                _visualisationProfilesManager.Profile.Type = value;
                OnPropertyChanged(nameof(VisualisationType));
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
            _visualisationProfilesManager = new VisualisationProfilesManager();

            var appTitle = ApplicationHelper.Instance.GetApplicationTitle();
            var appVersion = ApplicationHelper.Instance.GetApplicationVersion();

            ScreenVersion = $"{appTitle} {appVersion.ToString(3)}{Environment.NewLine}compilation beta.{appVersion.Revision}";
        }

        #endregion CLASS METHODS

        #region APPEARANCE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update appearance confiuration. </summary>
        private void AppearanceUpdate()
        {
            Color accentColor;
            Color backgroundColor;
            Color foregroundColor;
            Color ifaceBackgroundColor;
            Color ifaceForegroundColor;
            Color staticBackgroundColor;
            Color subcomponentBackground;
            Color subcomponentForeground;
            SystemTheme systemTheme = SystemInfo.GetSystemThemeMode();
            bool useContrastColor = false;

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
                    switch (ThemeTypeControls)
                    {
                        case AppearanceCustomThemeType.ACCENT_COLOR:
                            ifaceBackgroundColor = accentColor;
                            ifaceForegroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                            break;

                        case AppearanceCustomThemeType.LIGHT:
                            ifaceBackgroundColor = Colors.White;
                            ifaceForegroundColor = Colors.Black;
                            break;

                        case AppearanceCustomThemeType.DARK:
                        default:
                            ifaceBackgroundColor = Colors.Black;
                            ifaceForegroundColor = Colors.White;
                            break;
                    }

                    switch (ThemeTypeMenus)
                    {
                        case AppearanceCustomThemeType.ACCENT_COLOR:
                            backgroundColor = accentColor;
                            foregroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                            useContrastColor = true;
                            subcomponentBackground = Colors.White;
                            subcomponentForeground = Colors.Black;
                            break;

                        case AppearanceCustomThemeType.LIGHT:
                            backgroundColor = Colors.White;
                            foregroundColor = Colors.Black;
                            subcomponentBackground = Colors.White;
                            subcomponentForeground = Colors.Black;
                            break;

                        case AppearanceCustomThemeType.DARK:
                        default:
                            backgroundColor = Colors.Black;
                            foregroundColor = Colors.White;
                            subcomponentBackground = Colors.Black;
                            subcomponentForeground = Colors.White;
                            break;
                    }
                    break;

                case AppearanceThemeType.SYSTEM:
                    switch (systemTheme)
                    {
                        case SystemTheme.LIGHT:
                            backgroundColor = Colors.White;
                            foregroundColor = Colors.Black;
                            ifaceBackgroundColor = Colors.White;
                            ifaceForegroundColor = Colors.Black;
                            subcomponentBackground = Colors.White;
                            subcomponentForeground = Colors.Black;
                            break;

                        case SystemTheme.DARK:
                        default:
                            backgroundColor = Colors.Black;
                            foregroundColor = Colors.White;
                            ifaceBackgroundColor = Colors.Black;
                            ifaceForegroundColor = Colors.White;
                            subcomponentBackground = Colors.Black;
                            subcomponentForeground = Colors.White;
                            break;
                    }
                    break;

                case AppearanceThemeType.ACCENT_COLOR:
                    backgroundColor = accentColor;
                    foregroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                    ifaceBackgroundColor = accentColor;
                    ifaceForegroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
                    useContrastColor = true;
                    subcomponentBackground = Colors.White;
                    subcomponentForeground = Colors.Black;
                    break;

                case AppearanceThemeType.LIGHT:
                    backgroundColor = Colors.White;
                    foregroundColor = Colors.Black;
                    ifaceBackgroundColor = Colors.White;
                    ifaceForegroundColor = Colors.Black;
                    subcomponentBackground = Colors.White;
                    subcomponentForeground = Colors.Black;
                    break;

                case AppearanceThemeType.DARK:
                default:
                    backgroundColor = Colors.Black;
                    foregroundColor = Colors.White;
                    ifaceBackgroundColor = Colors.Black;
                    ifaceForegroundColor = Colors.White;
                    subcomponentBackground = Colors.Black;
                    subcomponentForeground = Colors.White;
                    break;
            }

            //  Setup theme colors.
            var accentAhslColor = AHSLColor.FromColor(accentColor);
            var accentForegroundColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
            var inactiveColor = ColorsUtilities.UpdateColor(accentAhslColor, saturation: accentAhslColor.S - INACTIVE_FACTOR).ToColor();
            var mouseOverColor = ColorsUtilities.UpdateColor(accentAhslColor, lightness: accentAhslColor.L + MOUSE_OVER_FACTOR).ToColor();
            var pressedColor = ColorsUtilities.UpdateColor(accentAhslColor, lightness: accentAhslColor.L - PRESSED_FACTOR).ToColor();
            var selectedColor = ColorsUtilities.UpdateColor(accentAhslColor, lightness: accentAhslColor.L - SELECTED_FACTOR).ToColor();

            var contrastColor = ColorsUtilities.FoundFontColorContrastingWithBackground(accentColor);
            var contrastAhslColor = AHSLColor.FromColor(contrastColor);

            var contrastMouseOverColor = ColorsUtilities.UpdateColor(
                contrastAhslColor,
                lightness: contrastAhslColor.S > 50
                    ? contrastAhslColor.S - CONTRAST_MOUSE_OVER_FACTOR
                    : contrastAhslColor.L + CONTRAST_MOUSE_OVER_FACTOR,
                saturation: 0).ToColor();

            var contrastPressedColor = ColorsUtilities.UpdateColor(
                contrastAhslColor,
                lightness: contrastAhslColor.S > 50
                    ? contrastAhslColor.S - CONTRAST_PRESSED_FACTOR
                    : contrastAhslColor.L + CONTRAST_PRESSED_FACTOR,
                saturation: 0).ToColor();

            AccentForegroundColorBrush = new SolidColorBrush(accentForegroundColor);
            BackgroundColorBrush = new SolidColorBrush(ColorsUtilities.UpdateColor(backgroundColor, a: BACKGROUND_APLHA));
            BorderColorBrush = new SolidColorBrush(accentColor);
            ForegroundColorBrush = new SolidColorBrush(foregroundColor);

            ContrastedColorBrush = useContrastColor
                ? new SolidColorBrush(contrastColor)
                : new SolidColorBrush(accentColor);
            ContrastedForegroundColorBrush = new SolidColorBrush(contrastColor);

            ContrastedMouseOverColorBrush = useContrastColor
                ? new SolidColorBrush(contrastMouseOverColor)
                : new SolidColorBrush(mouseOverColor);
            ContrastedMouseOverForegroundColorBrush = new SolidColorBrush(contrastMouseOverColor);

            ContrastedPressedColorBrush = useContrastColor
                ? new SolidColorBrush(contrastPressedColor)
                : new SolidColorBrush(pressedColor);
            ContrastedPressedForegroundColorBrush = new SolidColorBrush(contrastPressedColor);

            InactiveBackgroundColorBrush = new SolidColorBrush(inactiveColor);
            InactiveBorderColorBrush = new SolidColorBrush(inactiveColor);
            InactiveForegroundColorBrush = new SolidColorBrush(accentForegroundColor);
            MouseOverBackgroundColorBrush = new SolidColorBrush(mouseOverColor);
            MouseOverBorderColorBrush = new SolidColorBrush(accentColor);
            MouseOverForegroundColorBrush = new SolidColorBrush(accentForegroundColor);
            PressedBackgroundColorBrush = new SolidColorBrush(pressedColor);
            PressedBorderColorBrush = new SolidColorBrush(accentColor);
            PressedForegroundColorBrush = new SolidColorBrush(accentForegroundColor);
            SelectedBackgroundColorBrush = new SolidColorBrush(selectedColor);
            SelectedBorderColorBrush = new SolidColorBrush(accentColor);
            SelectedForegroundColorBrush = new SolidColorBrush(accentForegroundColor);

            staticBackgroundColor = useContrastColor ? foregroundColor : backgroundColor;

            PageBackgroundColorBrush = new SolidColorBrush(ColorsUtilities.UpdateColor(staticBackgroundColor, a: BACKGROUND_APLHA));
            WindowBackgroundColorBrush = new SolidColorBrush(staticBackgroundColor) { Opacity = BackgroundOpacity };

            SubcomponentBackgroundColorBrush = new SolidColorBrush(subcomponentBackground);
            SubcomponentForegroundColorBrush = new SolidColorBrush(subcomponentForeground);

            TrackBarBackgroundColorBrush = useContrastColor
                ? new SolidColorBrush(contrastColor) { Opacity = 0.25 }
                : new SolidColorBrush(foregroundColor) { Opacity = 0.25 };

            //  Setup interface theme colors.
            var ifaceForegroundOpacityUpdated = (BackgroundOpacity + ControlsBackgroundOpacity > 0.40)
                ? ifaceForegroundColor
                : Colors.Black;

            var ifaceLyricsForegroundOpacityUpdated = (BackgroundOpacity + LyricsBackgroundOpacity > 0.40)
                ? ifaceForegroundColor
                : Colors.Black;

            IfaceBackgroundColorBrush = new SolidColorBrush(ifaceBackgroundColor) { Opacity = ControlsBackgroundOpacity };
            IfaceForegroundColorBrush = new SolidColorBrush(ifaceForegroundOpacityUpdated);

            IfaceButtonBackgroundColorBrush = new SolidColorBrush(ifaceBackgroundColor) { Opacity = 0.25 };
            IfaceButtonBorderColorBrush = new SolidColorBrush(ifaceBackgroundColor) { Opacity = 0.25 };
            IfaceButtonForegroundColorBrush = new SolidColorBrush(ifaceForegroundOpacityUpdated);

            IfaceContextMenuBackgroundColorBrush = new SolidColorBrush(ifaceBackgroundColor) { Opacity = 0.85 };
            IfaceContextMenuBorderColorBrush = new SolidColorBrush(ifaceBackgroundColor);

            IfaceLyricsBackgroundColorBrush = new SolidColorBrush(ifaceBackgroundColor) { Opacity = LyricsBackgroundOpacity };
            IfaceLyricsForegroundColorBrush = new SolidColorBrush(ifaceLyricsForegroundOpacityUpdated);

            IfaceMenuItemBackgroundColorBrush = new SolidColorBrush(Colors.Transparent);
            IfaceMenuItemBorderColorBrush = new SolidColorBrush(Colors.Transparent);
            IfaceMenuItemForegroundColorBrush = new SolidColorBrush(ifaceForegroundOpacityUpdated);

            IfaceMouseOverBackgroundColorBrush = new SolidColorBrush(mouseOverColor);
            IfaceMouseOverBorderColorBrush = new SolidColorBrush(mouseOverColor);
            IfaceMouseOverForegroundColorBrush = new SolidColorBrush(accentForegroundColor);

            IfacePressedBackgroundColorBrush = new SolidColorBrush(pressedColor);
            IfacePressedBorderColorBrush = new SolidColorBrush(accentColor);
            IfacePressedForegroundColorBrush = new SolidColorBrush(accentForegroundColor);
            
            IfaceSelectedBackgroundColorBrush = new SolidColorBrush(selectedColor);
            IfaceSelectedBorderColorBrush = new SolidColorBrush(accentColor);
            IfaceSelectedForegroundColorBrush = new SolidColorBrush(accentForegroundColor);

            IfaceTrackBarBackgroundColorBrush = new SolidColorBrush(contrastColor) { Opacity = 0.25 };

            //  Setup logo colors by logo type.
            switch (LogoThemeType)
            {
                case AppearanceLogoTheme.ACCENT_COLOR:
                    LogoBackgroundColorBrush = new SolidColorBrush(accentColor) { Opacity = LOGO_ALPHA };
                    LogoBorderColorBrush = new SolidColorBrush(accentColor);
                    break;

                case AppearanceLogoTheme.LIGHT:
                    LogoBackgroundColorBrush = new SolidColorBrush(Colors.White) { Opacity = LOGO_ALPHA };
                    LogoBorderColorBrush = new SolidColorBrush(Colors.White);
                    break;

                case AppearanceLogoTheme.DARK:
                default:
                    LogoBackgroundColorBrush = new SolidColorBrush(Colors.Black) { Opacity = LOGO_ALPHA };
                    LogoBorderColorBrush = new SolidColorBrush(Colors.Black);
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        public void ForceAppearanceUpdate()
        {
            AppearanceUpdate();
            InvokeConfigUpdate(nameof(ThemeType));
        }

        #endregion APPEARANCE MANAGEMENT METHODS

        #region INFORMATION BAR MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update information bar title font size. </summary>
        public void UpdateInformationBarTitleFontSize()
        {
            InformationBarTitleFontSize = InformationBarFontSize + InformationBarTitleTextSizeDiffrence;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update information bar text margins. </summary>
        public void UpdateInformationBarTextMargins()
        {
            InformationBarTextMargin = new Thickness(0, 0, 0, InformationBarTextSpacing);
        }

        #endregion INFORMATION BAR MANAGEMENT METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration from file. </summary>
        public void LoadConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath;

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
            UpdateInformationBarTitleFontSize();
            UpdateInformationBarTextMargins();
            VisualisationProfilesManager.SelectProfile(VisualisationProfileName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration to file. </summary>
        public void SaveConfiguration()
        {
            var path = FilesManager.Instance.ConfigurationFilePath;
            var serialized = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
            File.WriteAllText(path, serialized);
            VisualisationProfilesManager.SaveCurrentProfile();
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

            InvokeConfigUpdate(propertyName);
        }

        //  --------------------------------------------------------------------------------
        private void InvokeConfigUpdate(string propertyName)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName);
            object value = property?.GetValue(this) ?? null;

            if (OnConfigUpdate != null && !ObjectUtilities.HasAttribute(property, typeof(ConfigPropertyUpdateAttrib)))
                OnConfigUpdate.Invoke(this, new ConfigUpdateEventArgs(propertyName, value, property.GetType()));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup configuration data containers. </summary>
        private void SetupDataContainers()
        {
            if (!(_configuration.UsedColors?.Any() ?? false))
                _configuration.UsedColors = DEFAULT_USED_COLORS;

            if (!(_configuration.VisualisationUsedBorderColors?.Any() ?? false))
                _configuration.VisualisationUsedBorderColors = DEFAULT_USED_COLORS;

            if (!(_configuration.VisualisationUsedFillColors?.Any() ?? false))
                _configuration.VisualisationUsedFillColors = DEFAULT_USED_COLORS;
        }

        #endregion SETUP METHODS

        #region VISUALISATION PROFILES MANAGER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation configuration. </summary>
        private void VisualisationProfileUpdate()
        {
            _lockVisualisationConfig = true;

            OnPropertyChanged(nameof(VisualisationProfileName));
            OnPropertyChanged(nameof(VisualisationAnimationSpeed));
            OnPropertyChanged(nameof(VisualisationBorderColor));
            OnPropertyChanged(nameof(VisualisationBorderEnabled));
            OnPropertyChanged(nameof(VisualisationColor));
            OnPropertyChanged(nameof(VisualisationColorOpacity));
            OnPropertyChanged(nameof(VisualisationColorType));
            OnPropertyChanged(nameof(VisualisationRainbowChangeTime));
            OnPropertyChanged(nameof(VisualisationRainbowShift));
            OnPropertyChanged(nameof(VisualisationRainbowXShift));
            OnPropertyChanged(nameof(VisualisationRainbowYShift));
            OnPropertyChanged(nameof(VisualisationScalingStrategy));
            OnPropertyChanged(nameof(VisualisationType));

            _lockVisualisationConfig = false;
        }

        #endregion VISUALISATION PROFILES MANAGER METHODS

    }
}
