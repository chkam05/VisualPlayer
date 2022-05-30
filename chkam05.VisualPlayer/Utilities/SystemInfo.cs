using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Utilities
{
    public static class SystemInfo
    {

        //  LIBRARIES

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);

        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        [DllImport("uxtheme.dll", EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        //[DllImport("uxtheme.dll", EntryPoint = "#139")]
        //public static extern bool ShouldAppsUseDarkMode();


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get Windows 8/8.1/10 theme color. </summary>
        /// <returns> Theme color. </returns>
        public static Color GetThemeColor()
        {
            var colorSetPref = (uint)GetImmersiveUserColorSetPreference(false, false);
            var colorType = GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground"));
            var colorSetEx = GetImmersiveColorFromColorSetEx(colorSetPref, colorType, false, 0);

            var color = Color.FromArgb(
                (byte)((0xFF000000 & colorSetEx) >> 24),
                (byte)(0x000000FF & colorSetEx),
                (byte)((0x0000FF00 & colorSetEx) >> 8),
                (byte)((0x00FF0000 & colorSetEx) >> 16));

            return color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check is system color mode set to dark. </summary>
        /// <returns> Is system color mode set to dark. </returns>
        public static SystemTheme GetSystemThemeMode()
        {
            return ShouldSystemUseDarkMode()
                ? SystemTheme.DARK
                : SystemTheme.LIGHT;
        }

    }
}
