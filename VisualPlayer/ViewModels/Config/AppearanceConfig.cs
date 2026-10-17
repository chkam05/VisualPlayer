using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Avalonia.Media;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Enums.Config;
using VisualPlayer.Models.Config;
using VisualPlayer.Utils;

namespace VisualPlayer.ViewModels.Config;

public class AppearanceConfig : ViewModelBase
{
    #region CONST

    private const int ACCENT_DISABLED_SATURATION_FACTOR = 50;
    private const int ACCENT_MOUSE_OVER_FACTOR = 8;
    private const int ACCENT_PRESSED_FACTOR = 10;
    private const int ACCENT_SELECTED_FACTOR = 5;

    private const double DEFAULT_INTERFACE_OPACITY = 0.85;
    private const double DEFAULT_MINI_PLAYER_OPACITY = 0.90;
    private const double DEFAULT_OVERLAY_OPACITY = 0.85;
    private const double DEFAULT_WINDOW_OPACITY = 0.85;

    public const int INFO_BAR_DISPLAY_DURATION_INCREMENT = 1;
    public const int INFO_BAR_DISPLAY_DURATION_MAXIMUM = 30;
    public const int INFO_BAR_DISPLAY_DURATION_MINIMUM = 5;

    public const double MINI_PLAYER_OPACITY_MAXIMUM = 1.0;
    public const double MINI_PLAYER_OPACITY_MINIMUM = 0.75;
    public const double MINI_PLAYER_OPACITY_SMALL_CHANGE = 0.01;

    public const double OPACITY_MAXIMUM = 1.0;
    public const double OPACITY_MINIMUM = 0.5;
    public const double OPACITY_SMALL_CHANGE = 0.01;

    private const int THEME_DISABLED_FACTOR = 6;
    private const int THEME_MOUSE_OVER_FACTOR = 4;
    private const int THEME_PRESSED_FACTOR = 7;
    private const int THEME_SELECTED_FACTOR = 10;

    private static readonly Color DARK_THEME_BACKGROUND_COLOR = Color.FromRgb(24, 24, 29);
    private static readonly Color DARK_THEME_FOREGROUND_COLOR = Color.FromRgb(245, 245, 247);
    private static readonly Color DARK_THEME_SHADE_BACKGROUND_COLOR = Color.FromRgb(36, 36, 43);

    private static readonly Color LIGHT_THEME_BACKGROUND_COLOR = Color.FromRgb(242, 242, 245);
    private static readonly Color LIGHT_THEME_FOREGROUND_COLOR = Color.FromRgb(28, 28, 30);
    private static readonly Color LIGHT_THEME_SHADE_BACKGROUND_COLOR = Color.FromRgb(226, 226, 231);

    #endregion CONST

    #region PRIVATE VARIABLES

    private AccentColor _accentColor = new("#5B8DEF", "Domyślny");
    private bool _autoHideHamburgerMenu;
    private bool _autoHideInfoBarControl;
    private bool _autoHidePlayerControl;
    private ApplicationTheme _currentTheme = ApplicationTheme.Dark;
    private ColorizationLevel _colorizationLevel = ColorizationLevel.Poorly;
    private int _infoBarDisplayDuration = 5;
    private InfoBarDisplayMode _infoBarDisplayMode = InfoBarDisplayMode.Manual;
    private double _interfaceOpacity = DEFAULT_INTERFACE_OPACITY;
    private double _miniPlayerOpacity = DEFAULT_MINI_PLAYER_OPACITY;
    //private double _overlayOpacity = DEFAULT_OVERLAY_OPACITY;
    private ObservableCollection<AccentColor> _recentAccentColors = new();
    private ApplicationTheme _theme = ApplicationTheme.Dark;
    private ObservableCollection<string> _windowBackgroundImages = new();
    private AccentColor _windowBackgroundColor = new("#18181D", "Tło okna");
    private ImageFillMode _imageFillMode = ImageFillMode.Uniform;
    private WindowBackgroundMode _windowBackgroundMode = WindowBackgroundMode.Transparent;
    private string? _windowBackgroundImagePath;
    private double _windowOpacity = DEFAULT_WINDOW_OPACITY;

    private SolidColorBrush _accentBackgroundBrush = new(Colors.Transparent);
    private SolidColorBrush _accentColorBrush = new(Colors.Transparent);
    private SolidColorBrush _accentDisabledBrush = new(Colors.Transparent);
    private SolidColorBrush _accentForegroundColorBrush = new(Colors.Transparent);
    private SolidColorBrush _accentMouseOverColorBrush = new(Colors.Transparent);
    private SolidColorBrush _accentPressedBrush = new(Colors.Transparent);
    private SolidColorBrush _accentSelectedColorBrush = new(Colors.Transparent);
    private SolidColorBrush _controlBackgroundBrush = new(Colors.Transparent);

    private SolidColorBrush _themeBackgroundBrush = new(Colors.Transparent);
    private SolidColorBrush _themeDisabledBrush = new(Colors.Transparent);
    private SolidColorBrush _themeForegroundBrush = new(Colors.Transparent);
    private SolidColorBrush _themeShadeBackgroundBrush = new(Colors.Transparent);
    private SolidColorBrush _themeMouseOverBrush = new(Colors.Transparent);
    private SolidColorBrush _themePressedBrush = new(Colors.Transparent);
    private SolidColorBrush _themeSelectedBrush = new(Colors.Transparent);

    #endregion PRIVATE VARIABLES

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public AccentColor AccentColor
    {
        get => _accentColor;

        set
        {
            if (value is null || _accentColor.Equals(value))
                return;

            UpdateProperty(ref _accentColor, value);
            UpdateAccentBrushes();
        }
    }

    [NotifyPropertyUpdate]
    public bool AutoHideHamburgerMenu
    {
        get => _autoHideHamburgerMenu;
        set => UpdateProperty(ref _autoHideHamburgerMenu, value);
    }

    [NotifyPropertyUpdate]
    public bool AutoHideInfoBarControl
    {
        get => _autoHideInfoBarControl;
        set => UpdateProperty(ref _autoHideInfoBarControl, value);
    }

    [NotifyPropertyUpdate]
    public bool AutoHidePlayerControl
    {
        get => _autoHidePlayerControl;
        set => UpdateProperty(ref _autoHidePlayerControl, value);
    }

    [NotifyPropertyUpdate]
    public ColorizationLevel ColorizationLevel
    {
        get => _colorizationLevel;

        set
        {
            if (_colorizationLevel == value)
                return;

            UpdateProperty(ref _colorizationLevel, value);
            UpdateControlBackgroundBrush();
        }
    }

    [JsonIgnore]
    public ApplicationTheme CurrentTheme
    {
        get => _currentTheme;

        private set
        {
            if (_currentTheme == value)
                return;

            UpdateProperty(ref _currentTheme, value);
            UpdateThemeBrushes();
        }
    }

    [NotifyPropertyUpdate]
    public ImageFillMode ImageFillMode
    {
        get => _imageFillMode;
        set => UpdateProperty(ref _imageFillMode, value);
    }

    [NotifyPropertyUpdate]
    public int InfoBarDisplayDuration
    {
        get => _infoBarDisplayDuration;
        set => UpdateProperty(ref _infoBarDisplayDuration, Math.Clamp(value, INFO_BAR_DISPLAY_DURATION_MINIMUM, INFO_BAR_DISPLAY_DURATION_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public InfoBarDisplayMode InfoBarDisplayMode
    {
        get => _infoBarDisplayMode;
        set => UpdateProperty(ref _infoBarDisplayMode, value);
    }

    [NotifyPropertyUpdate]
    public double InterfaceOpacity
    {
        get => _interfaceOpacity;
        set => UpdateProperty(ref _interfaceOpacity, Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public double MiniPlayerOpacity
    {
        get => _miniPlayerOpacity;
        set => UpdateProperty(
            ref _miniPlayerOpacity,
            Math.Clamp(value, MINI_PLAYER_OPACITY_MINIMUM, MINI_PLAYER_OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    [JsonIgnore]
    public double OverlayOpacity
    {
        get => DEFAULT_OVERLAY_OPACITY; //_overlayOpacity;
        //set => UpdateProperty(ref _overlayOpacity, Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<AccentColor> RecentAccentColors
    {
        get => _recentAccentColors;
        set => UpdateProperty(ref _recentAccentColors, value ?? new ObservableCollection<AccentColor>());
    }

    [NotifyPropertyUpdate]
    public ApplicationTheme Theme
    {
        get => _theme;

        set
        {
            if (_theme == value)
                return;

            UpdateProperty(ref _theme, value);
            UpdateCurrentTheme();
        }
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public bool IsImageWindowBackground
    {
        get => WindowBackgroundMode == WindowBackgroundMode.Image;
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public bool IsSolidColorWindowBackground
    {
        get => WindowBackgroundMode == WindowBackgroundMode.SolidColor;
    }

    [NotifyPropertyUpdate]
    public AccentColor WindowBackgroundColor
    {
        get => _windowBackgroundColor;

        set
        {
            if (value is null || _windowBackgroundColor.Equals(value))
                return;

            UpdateProperty(ref _windowBackgroundColor, value);
            NotifyPropertyChanged(nameof(WindowBackgroundBrush));
            NotifyPropertyChanged(nameof(WindowTransparencyFallbackBrush));
        }
    }

    [NotifyPropertyUpdate]
    public ObservableCollection<string> WindowBackgroundImages
    {
        get => _windowBackgroundImages;
        set => UpdateProperty(ref _windowBackgroundImages, value ?? new ObservableCollection<string>());
    }

    [NotifyPropertyUpdate]
    public string? WindowBackgroundImagePath
    {
        get => _windowBackgroundImagePath;
        set => UpdateProperty(ref _windowBackgroundImagePath, value);
    }

    [NotifyPropertyUpdate]
    public WindowBackgroundMode WindowBackgroundMode
    {
        get => _windowBackgroundMode;

        set
        {
            if (_windowBackgroundMode == value)
                return;

            UpdateProperty(ref _windowBackgroundMode, value);
            NotifyPropertyChanged(nameof(IsImageWindowBackground));
            NotifyPropertyChanged(nameof(IsSolidColorWindowBackground));
            NotifyPropertyChanged(nameof(WindowBackgroundBrush));
            NotifyPropertyChanged(nameof(WindowTransparencyFallbackBrush));
        }
    }

    [NotifyPropertyUpdate]
    public double WindowOpacity
    {
        get => _windowOpacity;
        set => UpdateProperty(ref _windowOpacity, Math.Clamp(value, OPACITY_MINIMUM, OPACITY_MAXIMUM));
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush WindowBackgroundBrush
    {
        get => WindowBackgroundMode == WindowBackgroundMode.SolidColor
            ? new SolidColorBrush(WindowBackgroundColor.ToColor())
            : ThemeBackgroundBrush;
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush WindowTransparencyFallbackBrush
    {
        get => WindowBackgroundMode == WindowBackgroundMode.SolidColor
            ? new SolidColorBrush(WindowBackgroundColor.ToColor())
            : ThemeBackgroundBrush;
    }


    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentBackgroundBrush
    {
        get => _accentBackgroundBrush;
        private set => UpdateProperty(ref _accentBackgroundBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentColorBrush
    {
        get => _accentColorBrush;
        private set => UpdateProperty(ref _accentColorBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentDisabledBrush
    {
        get => _accentDisabledBrush;
        private set => UpdateProperty(ref _accentDisabledBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentForegroundColorBrush
    {
        get => _accentForegroundColorBrush;
        private set => UpdateProperty(ref _accentForegroundColorBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentMouseOverColorBrush
    {
        get => _accentMouseOverColorBrush;
        private set => UpdateProperty(ref _accentMouseOverColorBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentPressedBrush
    {
        get => _accentPressedBrush;
        private set => UpdateProperty(ref _accentPressedBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush AccentSelectedColorBrush
    {
        get => _accentSelectedColorBrush;
        private set => UpdateProperty(ref _accentSelectedColorBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ControlBackgroundBrush
    {
        get => _controlBackgroundBrush;
        private set => UpdateProperty(ref _controlBackgroundBrush, value);
    }
    

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeBackgroundBrush
    {
        get => _themeBackgroundBrush;
        private set => UpdateProperty(ref _themeBackgroundBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeDisabledBrush
    {
        get => _themeDisabledBrush;
        private set => UpdateProperty(ref _themeDisabledBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeForegroundBrush
    {
        get => _themeForegroundBrush;
        private set => UpdateProperty(ref _themeForegroundBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeShadeBackgroundBrush
    {
        get => _themeShadeBackgroundBrush;
        private set => UpdateProperty(ref _themeShadeBackgroundBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeMouseOverBrush
    {
        get => _themeMouseOverBrush;
        private set => UpdateProperty(ref _themeMouseOverBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemePressedBrush
    {
        get => _themePressedBrush;
        private set => UpdateProperty(ref _themePressedBrush, value);
    }

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public SolidColorBrush ThemeSelectedBrush
    {
        get => _themeSelectedBrush;
        private set => UpdateProperty(ref _themeSelectedBrush, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    [JsonConstructor]
    public AppearanceConfig()
    {
        UpdateCurrentTheme();
        UpdateAccentBrushes();
        UpdateThemeBrushes();
    }

    #endregion CONSTRUCTORS

    #region BRUSHES UPDATE

    private void UpdateAccentBrushes()
    {
        var accentColor = AccentColor.ToColor();

        var accentAHSL = AHSLColor.FromColor(accentColor);

        var disabledColor = accentAHSL
            .AdjustSaturation(-ACCENT_DISABLED_SATURATION_FACTOR)
            .ToColor();

        var foregroundColor = GetContrastedForegroundColor(accentColor);
        var mouseOverColor = accentAHSL.AdjustLightness(ACCENT_MOUSE_OVER_FACTOR).ToColor();
        var pressedColor = accentAHSL.AdjustLightness(-ACCENT_PRESSED_FACTOR).ToColor();
        var selectedColor = accentAHSL.AdjustLightness(-ACCENT_SELECTED_FACTOR).ToColor();

        AccentColorBrush = new SolidColorBrush(accentColor);
        AccentDisabledBrush = new SolidColorBrush(disabledColor);
        AccentForegroundColorBrush = new SolidColorBrush(foregroundColor);
        AccentMouseOverColorBrush = new SolidColorBrush(mouseOverColor);
        AccentPressedBrush = new SolidColorBrush(pressedColor);
        AccentSelectedColorBrush = new SolidColorBrush(selectedColor);

        AccentBackgroundBrush = new SolidColorBrush(accentColor);
        UpdateControlBackgroundBrush();
    }

    private void UpdateThemeBrushes()
    {
        var isDark = CurrentTheme == ApplicationTheme.Dark;

        var backgroundColor = isDark
            ? DARK_THEME_BACKGROUND_COLOR
            : LIGHT_THEME_BACKGROUND_COLOR;

        var foregroundColor = isDark
            ? DARK_THEME_FOREGROUND_COLOR
            : LIGHT_THEME_FOREGROUND_COLOR;

        var shadeBackgroundColor = isDark
            ? DARK_THEME_SHADE_BACKGROUND_COLOR
            : LIGHT_THEME_SHADE_BACKGROUND_COLOR;

        var shadeAHSL = AHSLColor.FromColor(shadeBackgroundColor);

        var disabledColor = shadeAHSL
            .AdjustLightness(isDark ? -THEME_DISABLED_FACTOR : THEME_DISABLED_FACTOR)
            .ToColor();

        var mouseOverColor = shadeAHSL
            .AdjustLightness(isDark ? THEME_MOUSE_OVER_FACTOR : -THEME_MOUSE_OVER_FACTOR)
            .ToColor();

        var pressedColor = shadeAHSL
            .AdjustLightness(isDark ? -THEME_PRESSED_FACTOR : -THEME_PRESSED_FACTOR)
            .ToColor();

        var selectedColor = shadeAHSL
            .AdjustLightness(isDark ? THEME_SELECTED_FACTOR : -THEME_SELECTED_FACTOR)
            .ToColor();

        ThemeBackgroundBrush = new SolidColorBrush(backgroundColor);
        ThemeDisabledBrush = new SolidColorBrush(disabledColor);
        ThemeForegroundBrush = new SolidColorBrush(foregroundColor);
        ThemeShadeBackgroundBrush = new SolidColorBrush(shadeBackgroundColor);
        ThemeMouseOverBrush = new SolidColorBrush(mouseOverColor);
        ThemePressedBrush = new SolidColorBrush(pressedColor);
        ThemeSelectedBrush = new SolidColorBrush(selectedColor);
        UpdateAccentBrushes();
        UpdateControlBackgroundBrush();

        if (WindowBackgroundMode != WindowBackgroundMode.SolidColor)
        {
            NotifyPropertyChanged(nameof(WindowBackgroundBrush));
            NotifyPropertyChanged(nameof(WindowTransparencyFallbackBrush));
        }
    }

    #endregion BRUSHES UPDATE

    private void UpdateControlBackgroundBrush()
    {
        ControlBackgroundBrush = ColorizationLevel == ColorizationLevel.Whole
            ? AccentBackgroundBrush
            : ThemeBackgroundBrush;
    }

    #region THEME

    public void SetSystemTheme(ApplicationTheme theme)
    {
        if (Theme != ApplicationTheme.System)
            return;

        CurrentTheme = theme;
    }

    public void UpdateSystemTheme(ApplicationTheme theme)
    {
        if (Theme != ApplicationTheme.System)
            return;

        CurrentTheme = theme;
    }

    private void UpdateCurrentTheme()
    {
        CurrentTheme = Theme == ApplicationTheme.System
            ? SystemUtils.GetSystemTheme()
            : Theme;
    }

    #endregion THEME

    #region UTILITIES

    private static Color GetContrastedForegroundColor(Color color)
    {
        var luminance = GetRelativeLuminance(color);

        var whiteContrast = 1.05 / (luminance + 0.05);
        var blackContrast = (luminance + 0.05) / 0.05;

        return blackContrast >= whiteContrast
            ? Colors.Black
            : Colors.White;
    }

    private static double GetLinearColorComponent(byte value)
    {
        var component = value / 255.0;

        return component <= 0.04045
            ? component / 12.92
            : Math.Pow((component + 0.055) / 1.055, 2.4);
    }

    private static double GetRelativeLuminance(Color color)
    {
        var r = GetLinearColorComponent(color.R);
        var g = GetLinearColorComponent(color.G);
        var b = GetLinearColorComponent(color.B);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    #endregion UTILITIES
}
