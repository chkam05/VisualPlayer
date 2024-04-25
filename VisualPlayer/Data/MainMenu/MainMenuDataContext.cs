using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.ViewModels;
using static VisualPlayer.Data.MainMenu.MainMenuItem;

namespace VisualPlayer.Data.MainMenu
{
    public class MainMenuDataContext : BaseViewModel
    {

        //  CONST

        private const int DEFAULT_TOP_ITEMS_COUNT = 1;
        private const int DEFAULT_BOTTOM_ITEMS_COUNT = 1;


        //  VARIABLES

        private ObservableCollection<MainMenuItem> _topMenuItems;
        private ObservableCollection<MainMenuItem> _bottomMenuItems;


        //  GETTERS & SETTERS

        public ObservableCollection<MainMenuItem> TopMenuItems
        {
            get => _topMenuItems;
            set
            {
                _topMenuItems = value;
                _topMenuItems.CollectionChanged += NotifyTopMenuItemsCollectionChanged;
                NotifyPropertyChanged(nameof(TopMenuItems));
            }
        }

        public ObservableCollection<MainMenuItem> BottomMenuItems
        {
            get => _bottomMenuItems;
            set
            {
                _bottomMenuItems = value;
                _bottomMenuItems.CollectionChanged += NotifyTopMenuItemsCollectionChanged;
                NotifyPropertyChanged(nameof(BottomMenuItems));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainMenuDataContext class constructor. </summary>
        public MainMenuDataContext()
        {
            TopMenuItems = new ObservableCollection<MainMenuItem>();
            BottomMenuItems = new ObservableCollection<MainMenuItem>();

            SetupTopDefaultMenuItems();
            SetupBottomDefaultMenuItems();
        }

        #endregion CLASS METHODS

        #region MENU ITEMS MANAGEMETN METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clear menu items. </summary>
        /// <param name="menuPosition"> Menu position. </param>
        public void ClearItems(MainMenuPosition menuPosition)
        {
            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    BottomMenuItems.Clear();
                    SetupBottomDefaultMenuItems();
                    break;

                case MainMenuPosition.Top:
                    TopMenuItems.Clear();
                    SetupTopDefaultMenuItems();
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add menu item. </summary>
        /// <param name="item"> Menu item. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void AddItem(MainMenuItem item, MainMenuPosition menuPosition)
        {
            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    BottomMenuItems.Insert(0, item);
                    break;

                case MainMenuPosition.Top:
                    TopMenuItems.Add(item);
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add menu items. </summary>
        /// <param name="items"> Menu items to add. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void AddItems(IEnumerable<MainMenuItem> items, MainMenuPosition menuPosition)
        {
            foreach (MainMenuItem item in items)
            {
                switch (menuPosition)
                {
                    case MainMenuPosition.Bottom:
                        BottomMenuItems.Insert(0, item);
                        break;

                    case MainMenuPosition.Top:
                        TopMenuItems.Add(item);
                        break;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create menu item. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <param name="icon"> Menu item icon. </param>
        /// <param name="clickEvent"> Menu item click event. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void CreateItem(string title, PackIconKind icon,
            MainMenuItemClickEventHandler clickEvent, MainMenuPosition menuPosition)
        {
            AddItem(new MainMenuItem(title, icon, clickEvent), menuPosition);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create menu item. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <param name="icon"> Menu item icon. </param>
        /// <param name="description"> Menu item description. </param>
        /// <param name="clickEvent"> Menu item click event. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void CreateItem(string title, PackIconKind icon, string description,
            MainMenuItemClickEventHandler clickEvent, MainMenuPosition menuPosition)
        {
            AddItem(new MainMenuItem(title, icon, description, clickEvent), menuPosition);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get menu item. </summary>
        /// <param name="itemIndex"> Menu item index. </param>
        /// <returns> Menu item or null. </returns>
        /// <param name="menuPosition"> Menu position. </param>
        public MainMenuItem GetItem(int itemIndex, MainMenuPosition menuPosition)
        {
            if (itemIndex < 0)
                return null;

            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    if (itemIndex < BottomMenuItems.Count)
                        return BottomMenuItems[itemIndex];
                    break;

                case MainMenuPosition.Top:
                default:
                    if (itemIndex < TopMenuItems.Count)
                        return TopMenuItems[itemIndex];
                    break;
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get menu item by title. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <returns> Menu item or null. </returns>
        /// <param name="menuPosition"> Menu position. </param>
        public MainMenuItem GetItemByTitle(string title, MainMenuPosition menuPosition)
        {
            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    return BottomMenuItems.FirstOrDefault(i => i.Title.ToLower() == title.ToLower());

                case MainMenuPosition.Top:
                default:
                    return TopMenuItems.FirstOrDefault(i => i.Title.ToLower() == title.ToLower());
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove menu item. </summary>
        /// <param name="item"> Menu item. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void RemoveItem(MainMenuItem item, MainMenuPosition menuPosition)
        {
            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    var bottomItemIndex = BottomMenuItems.IndexOf(item);

                    if (bottomItemIndex >= 0 && bottomItemIndex < BottomMenuItems.Count - DEFAULT_BOTTOM_ITEMS_COUNT)
                        BottomMenuItems.Remove(item);
                    break;

                case MainMenuPosition.Top:
                    var topItemIndex = TopMenuItems.IndexOf(item);

                    if (topItemIndex >= DEFAULT_TOP_ITEMS_COUNT && topItemIndex < TopMenuItems.Count)
                        TopMenuItems.Remove(item);
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove menu item at index. </summary>
        /// <param name="item"> Menu item index. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void RemoveItem(int itemIndex, MainMenuPosition menuPosition)
        {
            switch (menuPosition)
            {
                case MainMenuPosition.Bottom:
                    if (itemIndex >= 0 && itemIndex < BottomMenuItems.Count - DEFAULT_BOTTOM_ITEMS_COUNT)
                        BottomMenuItems.RemoveAt(itemIndex);
                    break;

                case MainMenuPosition.Top:
                    if (itemIndex >= DEFAULT_TOP_ITEMS_COUNT && itemIndex < TopMenuItems.Count)
                        TopMenuItems.RemoveAt(itemIndex);
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove menu items. </summary>
        /// <param name="items"> Menu items to remove. </param>
        /// <param name="menuPosition"> Menu position. </param>
        public void RemoveItems(IEnumerable<MainMenuItem> items, MainMenuPosition menuPosition)
        {
            foreach (MainMenuItem item in items)
                RemoveItem(item, menuPosition);
        }

        #endregion MENU ITEMS MANAGEMENT METHODS

        #region MENU ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on MainMenu item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Main Menu Item Click Event Arguments. </param>
        private void MainMenuItemClick(object sender, MainMenuItemClickEventArgs e)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (uiController.MainMenuExpanded)
                uiController.RequestMenuBarShrink();
            else
                uiController.RequestMenuBarExpand();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on NowPlaying item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Main Menu Item Click Event Arguments. </param>
        private void NowPlayingItemClick(object sender, MainMenuItemClickEventArgs e)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (uiController.NowPlayingExpanded)
                uiController.RequestNowPlayingSlideOut();
            else
                uiController.RequestNowPlayingSlideIn();
        }

        #endregion MENU ITEMS INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Notify top menu items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyTopMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TopMenuItems));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Notify bottom menu items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyBottomMenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TopMenuItems));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup top default menu items. </summary>
        private void SetupTopDefaultMenuItems()
        {
            CreateItem(
                "Main Menu",
                PackIconKind.Menu,
                "Expand and collapse menu to view labels.",
                MainMenuItemClick,
                MainMenuPosition.Top);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup bottom default menu items. </summary>
        private void SetupBottomDefaultMenuItems()
        {
            CreateItem(
                "Now Playing",
                PackIconKind.PlaylistNote,
                "Now playing play list.",
                NowPlayingItemClick,
                MainMenuPosition.Bottom);
        }

        #endregion SETUP METHODS

    }
}
