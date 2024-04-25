using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Utilities
{
    public class ThemeManager : IDisposable
    {

        //  CONST

        private const string IMMERSIVE_COLOR_TYPE_NAME = "ImmersiveStartSelectionBackground";
        
        private static readonly UserPreferenceCategory[] USER_PREFERENCE_CHANGED_CATEGORIES = new UserPreferenceCategory[]
        {
            UserPreferenceCategory.Color,
            UserPreferenceCategory.General
        };


        //  DELEGATES

        public delegate void SystemThemeChangedEventHandler(object sender, SystemThemeChangedEventArgs e);


        //  EVENTS

        public SystemThemeChangedEventHandler SystemThemeChanged;


        //  IMPORTS

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);

        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        [DllImport("uxtheme.dll", EntryPoint = "#132")]
        private static extern int ShouldSystemUseDarkMode();

        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        private static extern int ShouldAppsUseDarkMode();


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ThemeManager class constructor. </summary>
        public ThemeManager()
        {
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
        public void Dispose()
        {
            SystemEvents.UserPreferenceChanged -= UserPreferenceChanged;
        }

        #endregion CLASS METHODS

        #region GETTERS

        //  --------------------------------------------------------------------------------
        /// <summary> Get system accent color. </summary>
        /// <returns> System accent color. </returns>
        public static Color GetAccentColor()
        {
            var colorSetPref = (uint)GetImmersiveUserColorSetPreference(false, false);
            var colorType = GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni(IMMERSIVE_COLOR_TYPE_NAME));
            var colorSetEx = GetImmersiveColorFromColorSetEx(colorSetPref, colorType, false, 0);

            var color = Color.FromArgb(
                (byte)((0xFF000000 & colorSetEx) >> 24),
                (byte)(0x000000FF & colorSetEx),
                (byte)((0x0000FF00 & colorSetEx) >> 8),
                (byte)((0x00FF0000 & colorSetEx) >> 16));

            return color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get system theme. </summary>
        /// <param name="useSystemTheme"> Use system theme, instead of apps theme. </param>
        /// <returns> Theme. </returns>
        public static Theme GetTheme(bool useSystemTheme = false)
        {
            if (useSystemTheme)
                return ShouldSystemUseDarkMode() == 0 ? Theme.Light : Theme.Dark;
            else
                return ShouldAppsUseDarkMode() == 0 ? Theme.Light : Theme.Dark;
        }

        #endregion GETTERS

        #region SYSTEM LISTENER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when a user preference has changed. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (USER_PREFERENCE_CHANGED_CATEGORIES.Contains(e.Category))
            {
                var color = GetAccentColor();
                var theme = GetTheme();

                SystemThemeChanged.Invoke(this, new SystemThemeChangedEventArgs(color, theme));
            }
        }

        #endregion SYSTEM LISTENER METHODS

    }
}
