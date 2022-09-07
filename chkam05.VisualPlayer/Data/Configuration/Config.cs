using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class Config
    {

        //  CONST

        private static readonly Color DEFAULT_ACCENT_COLOR = Color.FromArgb(255, 0, 120, 215);


        //  VARIABLES

        //  Advanced

        public bool ShowVersionOnScreen { get; set; } = true;

        //  Appearance

        public Color AccentColor { get; set; } = DEFAULT_ACCENT_COLOR;
        public double BackgroundOpacity { get; set; } = 0.25d;
        public AppearanceColorType ColorType { get; set; } = AppearanceColorType.SYSTEM;
        public double ControlsBackgroundOpacity { get; set; } = 0.50d;
        public AppearanceThemeType ThemeType { get; set; } = AppearanceThemeType.SYSTEM;
        public AppearanceCustomThemeType ThemeTypeControls { get; set; } = AppearanceCustomThemeType.DARK;
        public AppearanceCustomThemeType ThemeTypeMenus { get; set; } = AppearanceCustomThemeType.DARK;
        public List<ColorInfo> UsedColors { get; set; } = ColorUtilities.DefaultColors;

        //  Control Bar

        public bool ControlBarAutoHide { get; set; } = false;

        //  Information Bar

        public bool InformationBarAdvancedTime { get; set; } = false;
        public InformationBarAutoHide InformationBarAutoHide { get; set; } = InformationBarAutoHide.STAY_3S;
        public FontContainer InformationBarFont { get; set; } = FontsManager.Instance.DefaultFont;
        public int InformationBarFontSize { get; set; } = 16;
        public FontStyle InformationBarFontStyle { get; set; } = FontStyles.Normal;
        public FontStretch InformationBarFontStretch { get; set; } = FontStretches.Normal;
        public FontWeight InformationBarFontWeight { get; set; } = FontWeights.Normal;
        public bool InformationBarInfoAlbumVisible { get; set; } = true;
        public bool InformationBarInfoArtistVisible { get; set; } = true;
        public bool InformationBarInfoCoverVisible { get; set; } = true;
        public bool InformationBarInfoTimeVisible { get; set; } = true;
        public bool InformationBarInfoTitleVisible { get; set; } = true;
        public double InformationBarTextSpacing { get; set; } = 2.0d;
        public int InformationBarTitleTextSizeDiffrence { get; set; } = 8;
        public MarqueeState InformationBarTitleMarqueeState { get; set; } = MarqueeState.TOO_LONG_TEXT;

        //  Logo

        public bool LogoAnimated { get; set; } = true;
        public double LogoAnimationSensitivity { get; set; } = 0.5d;
        public double LogoAnimationSpeed { get; set; } = 0.02d;
        public bool LogoEnabled { get; set; } = true;
        public AppearanceLogoTheme LogoThemeType { get; set; } = AppearanceLogoTheme.ACCENT_COLOR;

        //  Lyrics

        public bool LyricsAutoLoad { get; set; } = false;
        public double LyricsBackgroundOpacity { get; set; } = 0.5d;
        public FontContainer LyricsFont { get; set; } = FontsManager.Instance.DefaultFont;
        public int LyricsFontSize { get; set; } = 40;
        public FontStyle LyricsFontStyle { get; set; } = FontStyles.Normal;
        public FontStretch LyricsFontStretch { get; set; } = FontStretches.Normal;
        public FontWeight LyricsFontWeight { get; set; } = FontWeights.SemiBold;
        public LyricsMatchingType LyricsMatchingType { get; set; } = LyricsMatchingType.FILE_NAME;

        //  Menu Bar

        public bool MenuBarAutoHide { get; set; } = false;
        public bool PlayListAutoHide { get; set; } = false;

        //  Player

        public bool RestoreLastPlaylistOnStartup { get; set; } = true;

        //  Title Bar

        public bool TitleBarSongInfo { get; set; } = true;
        public MarqueeState TitleBarSongInfoMarqueeState { get; set; } = MarqueeState.TOO_LONG_TEXT;

        //  Visualisation

        public string VisualisationProfileName { get; set; } = VisualisationProfile.DEFAULT_PROFILE_NAME;
        public List<ColorInfo> VisualisationUsedBorderColors { get; set; } = ColorUtilities.DefaultColors;
        public List<ColorInfo> VisualisationUsedFillColors { get; set; } = ColorUtilities.DefaultColors;

        //  Window

        public Point WindowPosition { get; set; } = new Point(0, 0);
        public Size WindowSize { get; set; } = new Size(800, 600);


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Config class constructor. </summary>
        public Config()
        {
            //
        }

        #endregion CLASS METHODS

    }
}
