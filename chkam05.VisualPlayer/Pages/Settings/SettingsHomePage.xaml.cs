using chkam05.Tools.ControlsEx;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using MenuItem = chkam05.VisualPlayer.Controls.Data.MenuItem;


namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class SettingsHomePage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private List<MenuItem> _menuItems;

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public List<MenuItem> MenuItems
        {
            get => _menuItems;
            private set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsHomePage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsHomePage(IPagesManager pagesManager)
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;

            if (SpecialMenu.HasValue)
                MenuItems = MenuBuilder.BuildMenu(SpecialMenu.Value)
                    .Where(i => i.SubType != MenuItemSubType.OPEN_CLOSE)
                    .ToList();
        }

        #endregion CLASS METHODS

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.CanGoBack)
                PagesManager.GoBack();
            else
                PagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Close ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PagesManager.HideInterface();
        }

        #endregion CONTROL BUTTONS METHODS

        #region MENU MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting any item in Settings menu list view. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListViewEx)sender;
            var selectedItem = listView.SelectedItem;

            if (selectedItem != null)
            {
                var menuItem = (MenuItem)selectedItem;

                if (menuItem != null)
                {
                    switch (menuItem.Type)
                    {
                        case MenuItemType.SETTINGS_MENU:
                            switch (menuItem.SubType)
                            {
                                case MenuItemSubType.ABOUT:
                                    //PagesManager.LoadPage(new SettingsOldAboutPage(PagesManager));
                                    break;

                                case MenuItemSubType.APPEARANCE:
                                    PagesManager.LoadPage(new SettingsAppearancePage(PagesManager));
                                    break;

                                case MenuItemSubType.INFOBAR:
                                    PagesManager.LoadPage(new SettingsInfoBarPage(PagesManager));
                                    break;

                                case MenuItemSubType.GENERAL:
                                    PagesManager.LoadPage(new SettingsGeneralPage(PagesManager));
                                    break;

                                case MenuItemSubType.LYRICS:
                                    PagesManager.LoadPage(new SettingsLyricsPage(PagesManager));
                                    break;

                                case MenuItemSubType.VISUALISATION:
                                    PagesManager.LoadPage(new SettingsVisualisationPage(PagesManager));
                                    break;
                            }
                            break;
                    }
                }

                listView.SelectedIndex = -1;
                listView.SelectedItem = null;
            }
        }

        #endregion MENU MANAGEMENT METHODS

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

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during page unloading. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion PAGE METHODS

    }
}
