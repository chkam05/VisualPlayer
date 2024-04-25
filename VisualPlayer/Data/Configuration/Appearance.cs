using Newtonsoft.Json;
using System;
using System.Windows.Media;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Data.Enums;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class Appearance : BaseViewModel
    {

        //  CONST

        private const double LUMINANCE_R = 0.299;
        private const double LUMINANCE_G = 0.587;
        private const double LUMINANCE_B = 0.114;

        private const int INACTIVE_FACTOR = 15;
        private const int MOUSE_OVER_FACTOR = 15;
        private const int PRESSED_FACTOR = 10;
        private const int SELECTED_FACTOR = 5;

        public static readonly Color DARK_THEME_COLOR = Color.FromArgb(255, 36, 36, 36);
        public static readonly Color LIGHT_THEME_COLOR = Color.FromArgb(255, 219, 219, 219);


        //  VARIABLES

        private Theme _currentTheme;
        private bool _loaded = false;

        private ThemeColor _accentColor = BaseColors.Blue;
        private double _opacity;
        private double _opacityUI;
        private Theme _theme;
        private bool _useSystemColor;

        private SolidColorBrush _accentColorBrush;
        private SolidColorBrush _accentForegroundBrush;
        private SolidColorBrush _accentMouseOverBrush;
        private SolidColorBrush _accentPressedBrush;
        private SolidColorBrush _accentSelectedBrush;
        private SolidColorBrush _themeBackgroundBrush;
        private SolidColorBrush _themeForegroundBrush;
        private SolidColorBrush _themeShadeBackgroundBrush;
        private SolidColorBrush _themeMouseOverBrush;
        private SolidColorBrush _themePressedBrush;
        private SolidColorBrush _themeSelectedBrush;


        //  GETTERS & SETTERS

        public ThemeColor AccentColor
        {
            get => _accentColor;
            set
            {
                UpdateProperty(ref _accentColor, value);
                UpdateAccentBrushes();
            }
        }

        public double Opacity
        {
            get => _opacity;
            set => UpdateProperty(ref _opacity, Math.Min(1f, Math.Max(0f, value)));
        }

        public double OpacityUI
        {
            get => _opacityUI;
            set => UpdateProperty(ref _opacityUI, Math.Min(1f, Math.Max(0f, value)));
        }

        public Theme Theme
        {
            get => _theme;
            set
            {
                UpdateProperty(ref _theme, value);
                CurrentTheme = value != Theme.System ? value : ThemeManager.GetTheme();
            }
        }

        public bool UseSystemColor
        {
            get => _useSystemColor;
            set
            {
                UpdateProperty(ref _useSystemColor, value);

                if (value)
                {
                    var systemAccentColor = ThemeManager.GetAccentColor();
                    var systemAccentColorCode = ColorsHelper.GetColorCode(systemAccentColor);

                    AccentColor = new ThemeColor(systemAccentColor, systemAccentColorCode);
                }
                else if (_loaded)
                {
                    AccentColor = BaseColors.Blue;
                }
            }
        }


        [JsonIgnore]
        public Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                UpdateProperty(ref _currentTheme, value);
                UpdateThemeBrushes();
            }
        }

        [JsonIgnore]
        public SolidColorBrush AccentColorBrush
        {
            get => _accentColorBrush;
            private set => UpdateProperty(ref _accentColorBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush AccentForegroundBrush
        {
            get => _accentForegroundBrush;
            private set => UpdateProperty(ref _accentForegroundBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush AccentMouseOverBrush
        {
            get => _accentMouseOverBrush;
            private set => UpdateProperty(ref _accentMouseOverBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush AccentPressedBrush
        {
            get => _accentPressedBrush;
            private set => UpdateProperty(ref _accentPressedBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush AccentSelectedBrush
        {
            get => _accentSelectedBrush;
            private set => UpdateProperty(ref _accentSelectedBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemeBackgroundBrush
        {
            get => _themeBackgroundBrush;
            private set => UpdateProperty(ref _themeBackgroundBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemeForegroundBrush
        {
            get => _themeForegroundBrush;
            private set => UpdateProperty(ref _themeForegroundBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemeShadeBackgroundBrush
        {
            get => _themeShadeBackgroundBrush;
            private set => UpdateProperty(ref _themeShadeBackgroundBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemeMouseOverBrush
        {
            get => _themeMouseOverBrush;
            private set => UpdateProperty(ref _themeMouseOverBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemePressedBrush
        {
            get => _themePressedBrush;
            private set => UpdateProperty(ref _themePressedBrush, value);
        }

        [JsonIgnore]
        public SolidColorBrush ThemeSelectedBrush
        {
            get => _themeSelectedBrush;
            private set => UpdateProperty(ref _themeSelectedBrush, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Appearance class constructor. </summary>
        [JsonConstructor]
        public Appearance(
            ThemeColor accentColor = null,
            double? opacity = null,
            double? opacityUI = null,
            Theme? theme = null,
            bool? useSystemColor = null)
        {
            _loaded = false;

            AccentColor = accentColor ?? BaseColors.Blue;
            Opacity = opacity ?? 1f;
            OpacityUI = opacityUI ?? 0.75f;
            Theme = theme ?? Theme.System;
            UseSystemColor = useSystemColor ?? true;

            _loaded = true;
        }

        #endregion CLASS METHODS

        #region UPDATE BRUSHES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update accent brushes. </summary>
        private void UpdateAccentBrushes()
        {
            var accentColor = AccentColor.Color;
            var accentAhslColor = AHSLColor.FromColor(accentColor);
            var accentForegroundColor = GetForegroundContrastedColor(accentColor);

            var accentMouseOverColor = UpdateAHSLColor(accentAhslColor,
                l: accentAhslColor.L + MOUSE_OVER_FACTOR).ToColor();

            var accentPressedColor = UpdateAHSLColor(accentAhslColor,
                l: accentAhslColor.L - PRESSED_FACTOR).ToColor();

            var accentSelectedColor = UpdateAHSLColor(accentAhslColor,
                l: accentAhslColor.L - SELECTED_FACTOR).ToColor();

            AccentColorBrush = new SolidColorBrush(accentColor);
            AccentForegroundBrush = new SolidColorBrush(accentForegroundColor);
            AccentMouseOverBrush = new SolidColorBrush(accentMouseOverColor);
            AccentPressedBrush = new SolidColorBrush(accentPressedColor);
            AccentSelectedBrush = new SolidColorBrush(accentSelectedColor);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update theme brushes. </summary>
        private void UpdateThemeBrushes()
        {
            Color backgroundColor;
            Color foregroundColor;
            Color shadeBackgroundColor;

            switch (CurrentTheme)
            {
                case Theme.Light:
                    backgroundColor = Colors.White;
                    foregroundColor = Colors.Black;
                    shadeBackgroundColor = LIGHT_THEME_COLOR;
                    break;

                case Theme.Dark:
                default:
                    backgroundColor = Colors.Black;
                    foregroundColor = Colors.White;
                    shadeBackgroundColor = DARK_THEME_COLOR;
                    break;
            }

            var backgroundAhslColor = AHSLColor.FromColor(backgroundColor);

            var themeMouseOverColor = UpdateAHSLColor(backgroundAhslColor,
                l: backgroundAhslColor.S > 50
                    ? backgroundAhslColor.S + MOUSE_OVER_FACTOR
                    : backgroundAhslColor.L - MOUSE_OVER_FACTOR,
                s: 0).ToColor();

            var themePressedColor = UpdateAHSLColor(backgroundAhslColor,
                l: backgroundAhslColor.S > 50
                    ? backgroundAhslColor.S - PRESSED_FACTOR
                    : backgroundAhslColor.L + PRESSED_FACTOR,
                s: 0).ToColor();

            var themeSelectedColor = UpdateAHSLColor(backgroundAhslColor,
                l: backgroundAhslColor.S > 50
                    ? backgroundAhslColor.S - SELECTED_FACTOR
                    : backgroundAhslColor.L + SELECTED_FACTOR,
                s: 0).ToColor();

            ThemeBackgroundBrush = new SolidColorBrush(backgroundColor);
            ThemeForegroundBrush = new SolidColorBrush(foregroundColor);
            ThemeShadeBackgroundBrush = new SolidColorBrush(shadeBackgroundColor);
            ThemeMouseOverBrush = new SolidColorBrush(themeMouseOverColor);
            ThemePressedBrush = new SolidColorBrush(themePressedColor);
            ThemeSelectedBrush = new SolidColorBrush(themeSelectedColor);
        }

        #endregion UPDATE BRUSHES METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get foreground contrasted color. </summary>
        /// <param name="accentColor"> Accent color. </param>
        /// <returns> Contrasted foreground color. </returns>
        public static Color GetForegroundContrastedColor(Color accentColor)
        {
            double luminance = (LUMINANCE_R * accentColor.R + LUMINANCE_G * accentColor.G
                + LUMINANCE_B * accentColor.B) / 255;

            if (luminance > 0.5)
                return Colors.Black;
            else
                return Colors.White;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update AHSL color. </summary>
        /// <param name="color"> AHSL color. </param>
        /// <param name="a"> Alpha factor. </param>
        /// <param name="h"> Hue factor. </param>
        /// <param name="s"> Saturation factor. </param>
        /// <param name="l"> Lightness factor. </param>
        /// <returns> Updated AHSL color. </returns>
        private static AHSLColor UpdateAHSLColor(AHSLColor color, byte? a = null, int? h = null, int? s = null, int? l = null)
        {
            return new AHSLColor(
                a.HasValue ? a.Value : color.A,
                h.HasValue ? h.Value : color.H,
                s.HasValue ? s.Value : color.S,
                l.HasValue ? l.Value : color.L);
        }

        #endregion UTILITY METHODS

    }
}
