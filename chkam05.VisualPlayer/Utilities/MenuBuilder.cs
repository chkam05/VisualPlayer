using chkam05.VisualPlayer.Controls.Data;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public static class MenuBuilder
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Build menu items list. </summary>
        /// <param name="menuType"> Menu type. </param>
        /// <returns> List of menu items. </returns>
        public static List<MenuItem> BuildMenu(MenuItemType menuType)
        {
            switch (menuType)
            {
                case MenuItemType.MAIN_MENU:
                    return new List<MenuItem>()
                    {
                        new MenuItem(menuType, MenuItemSubType.OPEN_CLOSE, "Main Menu", PackIconKind.HamburgerMenu),
                        new MenuItem(menuType, MenuItemSubType.HOME, "Home", PackIconKind.Home),
                        new MenuItem(menuType, MenuItemSubType.OPEN_FILES, "Open", PackIconKind.FolderOpen),
                        new MenuItem(menuType, MenuItemSubType.SETTINGS, "Settings", PackIconKind.SettingsOff),
                        new MenuItem(menuType, MenuItemSubType.SETTINGS_2, "Settings", PackIconKind.Settings)
                    };

                case MenuItemType.SETTINGS_MENU:
                    return new List<MenuItem>()
                    {
                        new MenuItem(menuType, MenuItemSubType.OPEN_CLOSE, "Settings", PackIconKind.HamburgerMenu),
                        new MenuItem(menuType, MenuItemSubType.APPEARANCE, "Appearance", PackIconKind.Palette),
                        new MenuItem(menuType, MenuItemSubType.GENERAL, "General", PackIconKind.Application),
                        new MenuItem(menuType, MenuItemSubType.LYRICS, "Lyrics", PackIconKind.Subtitles),
                        new MenuItem(menuType, MenuItemSubType.VISUALISATION, "Visualisation", PackIconKind.Waveform),
                        new MenuItem(menuType, MenuItemSubType.ABOUT, "About", PackIconKind.InformationCircleOutline),
                    };

                case MenuItemType.SETTINGS_MENU_2:
                    return new List<MenuItem>()
                    {
                        new MenuItem(menuType, MenuItemSubType.OPEN_CLOSE, "Settings", PackIconKind.HamburgerMenu),
                        new MenuItem(menuType, MenuItemSubType.APPEARANCE, "Appearance", PackIconKind.Palette),
                        new MenuItem(menuType, MenuItemSubType.GENERAL, "General", PackIconKind.Application),
                        new MenuItem(menuType, MenuItemSubType.LYRICS, "Lyrics", PackIconKind.Subtitles),
                        new MenuItem(menuType, MenuItemSubType.VISUALISATION, "Visualisation", PackIconKind.Waveform),
                        new MenuItem(menuType, MenuItemSubType.ABOUT, "About", PackIconKind.InformationCircleOutline),
                    };
            }

            return new List<MenuItem>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Build additional menu items list. </summary>
        /// <param name="menuType"> Menu type. </param>
        /// <returns> List of additioanl menu items. </returns>
        public static List<MenuItem> BuildMoreMenu(MenuItemType menuType)
        {
            switch (menuType)
            {
                case MenuItemType.MAIN_MENU:
                    return new List<MenuItem>()
                    {
                        new MenuItem(MenuItemType.MAIN_MENU, MenuItemSubType.LYRICS, "Lyrics", PackIconKind.Subtitles),
                        new MenuItem(MenuItemType.MAIN_MENU, MenuItemSubType.NOW_PLAYING, "Now Playing", PackIconKind.PlaylistMusic)
                    };

                case MenuItemType.SETTINGS_MENU:
                    break;
            }

            return new List<MenuItem>();
        }

    }
}
