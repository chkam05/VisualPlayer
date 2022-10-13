using chkam05.Tools.ControlsEx;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


using MenuItem = chkam05.VisualPlayer.Controls.Data.MenuItem;


namespace chkam05.VisualPlayer.Controls
{
    public partial class SideBarMenu : UserControl, INotifyPropertyChanged
    {

        //  CONST

        public static readonly double CONTROL_MARGIN_PART = 4;
        public static readonly Thickness CONTROL_MARGIN_VISIBLE = new Thickness(0);
        public static readonly double MENU_WIDTH_MIN = 54;
        public static readonly double MENU_WIDTH_MAX = 256;
        public static readonly double PLAYLIST_WIDTH_MIN = 320;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            nameof(AutoHide),
            typeof(bool),
            typeof(SideBarMenu),
            new PropertyMetadata(true));

        public static readonly DependencyProperty AutoHidePlayListProperty = DependencyProperty.Register(
            nameof(AutoHidePlayList),
            typeof(bool),
            typeof(SideBarMenu),
            new PropertyMetadata(true));

        public static readonly DependencyProperty LockPlayListVisibilityProperty = DependencyProperty.Register(
            nameof(LockPlayListVisibility),
            typeof(bool),
            typeof(SideBarMenu),
            new PropertyMetadata(false));

        public static readonly DependencyProperty LockVisibilityProperty = DependencyProperty.Register(
            nameof(LockVisibility),
            typeof(bool),
            typeof(SideBarMenu),
            new PropertyMetadata(false));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnMenuItemSelected;
        public event EventHandler<SideBarAnimateEventArgs> OnAnimate;
        public event EventHandler<SideBarAnimationFinishEventArgs> OnAnimationFinish;


        //  VARIABLES

        private ObservableCollection<MenuItem> _mainMenuItems;
        private ObservableCollection<MenuItem> _moreMenuItems;

        private Thickness _hideMargin = CONTROL_MARGIN_VISIBLE;
        private SideBarMenuState _menuState = SideBarMenuState.HIDDEN;
        private SideBarMenuState? _nextMenuState = null;
        private double _menuWidth = MENU_WIDTH_MAX;
        private double _playListWidth = 0;
        private double _playListWidthCache = PLAYLIST_WIDTH_MIN;

        public Configuration Configuration { get; private set; }
        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public ObservableCollection<MenuItem> MenuItems
        {
            get => _mainMenuItems;
            private set
            {
                _mainMenuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }

        public ObservableCollection<MenuItem> MoreMenuItems
        {
            get => _moreMenuItems;
            private set
            {
                _moreMenuItems = value;
                OnPropertyChanged(nameof(MoreMenuItems));
            }
        }

        public bool AutoHide
        {
            get => (bool)GetValue(AutoHideProperty);
            set => SetValue(AutoHideProperty, value);
        }

        public bool AutoHidePlayList
        {
            get => (bool)GetValue(AutoHidePlayListProperty);
            set => SetValue(AutoHidePlayListProperty, value);
        }

        public Thickness HideMargin
        {
            get => _hideMargin;
            set
            {
                _hideMargin = value;
                OnPropertyChanged(nameof(HideMargin));
            }
        }

        public bool IsCollapsed
        {
            get => _menuState == SideBarMenuState.VISIBLE;
        }

        public bool IsDeploing
        {
            get => _menuState != SideBarMenuState.HIDDEN
                && _nextMenuState == SideBarMenuState.HIDDEN;
        }

        public bool IsDeployed
        {
            get => _menuState != SideBarMenuState.HIDDEN
                || _nextMenuState != SideBarMenuState.HIDDEN;
        }

        public bool IsExtended
        {
            get => _menuState == SideBarMenuState.EXTENDED;
        }

        public bool IsPlayList
        {
            get => _menuState == SideBarMenuState.PLAYLIST
                || _nextMenuState == SideBarMenuState.PLAYLIST;
        }

        public bool LockPlayListVisibility
        {
            get => (bool)GetValue(LockPlayListVisibilityProperty);
            set => SetValue(LockPlayListVisibilityProperty, value);
        }
        
        public bool LockVisibility
        {
            get => (bool)GetValue(LockVisibilityProperty);
            set => SetValue(LockVisibilityProperty, value);
        }

        public SideBarMenuState MenuState
        {
            get => _menuState;
        }

        public double MenuWidth
        {
            get => _menuWidth;
            set
            {
                _menuWidth = value;
                OnPropertyChanged(nameof(MenuWidth));
            }
        }

        public double PlayListWidth
        {
            get => _playListWidth;
            set
            {
                _playListWidth = value;
                OnPropertyChanged(nameof(PlayListWidth));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SideBarMenu class constructor. </summary>
        public SideBarMenu()
        {
            //  Setup modules.
            Configuration = Configuration.Instance;
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            ControlGrid.Margin = CalculateHiddenMargin();
            MenuGrid.Width = MENU_WIDTH_MIN;
            PlayListGrid.Width = 0;
            SetupMenu(MenuItemType.MAIN_MENU);
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Animate showing control interface. </summary>
        public void AnimateShowControl()
        {
            if (!LockVisibility && MenuState == SideBarMenuState.HIDDEN)
            {
                HideMargin = CONTROL_MARGIN_VISIBLE;
                Storyboard storyboard = Resources["ShowHideStoryboard"] as Storyboard;
                PrepareUpdateState(SideBarMenuState.VISIBLE);
                OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate hiding control interface. </summary>
        public void AnimateHideControl()
        {
            if (MenuState != SideBarMenuState.HIDDEN)
            {
                switch (MenuState)
                {
                    case SideBarMenuState.EXTENDED:
                        AnimateCollapseMenu();
                        break;

                    case SideBarMenuState.PLAYLIST:
                        if (!LockPlayListVisibility)
                            AnimateClosePlayList();
                        break;
                }

                if (!LockVisibility && !LockPlayListVisibility)
                {
                    HideMargin = CalculateHiddenMargin();
                    Storyboard showingSb = Resources["ShowHideStoryboard"] as Storyboard;
                    PrepareUpdateState(SideBarMenuState.HIDDEN);
                    OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
                    showingSb?.Begin();
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate collapse menu and close playlist. </summary>
        private void AnimateCollapseExpandedControls()
        {
            if (MenuState != SideBarMenuState.HIDDEN)
            {
                switch (MenuState)
                {
                    case SideBarMenuState.EXTENDED:
                        AnimateCollapseMenu();
                        break;

                    case SideBarMenuState.PLAYLIST:
                        if (!LockPlayListVisibility && AutoHidePlayList)
                            AnimateClosePlayList();
                        break;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate expand menu. </summary>
        private void AnimateExpandMenu()
        {
            MenuWidth = MENU_WIDTH_MAX;
            Storyboard storyboard = Resources["ExpandMenuStoryboard"] as Storyboard;
            PrepareUpdateState(SideBarMenuState.EXTENDED);
            OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate collapse menu. </summary>
        private void AnimateCollapseMenu()
        {
            MenuWidth = MENU_WIDTH_MIN;
            Storyboard storyboard = Resources["ExpandMenuStoryboard"] as Storyboard;
            PrepareUpdateState(SideBarMenuState.VISIBLE);
            OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate open playlist. </summary>
        private void AnimateOpenPlayList()
        {
            if (MenuState == SideBarMenuState.EXTENDED)
                AnimateCollapseMenu();

            PlayListWidth = _playListWidthCache;
            Storyboard storyboard = Resources["OpenClosePlayListStoryboard"] as Storyboard;
            PrepareUpdateState(SideBarMenuState.PLAYLIST);
            OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate close playlist. </summary>
        private void AnimateClosePlayList()
        {
            PlayListWidth = 0;
            Storyboard storyboard = Resources["OpenClosePlayListStoryboard"] as Storyboard;
            PrepareUpdateState(SideBarMenuState.VISIBLE);
            OnAnimate?.Invoke(this, new SideBarAnimateEventArgs(_nextMenuState));
            storyboard?.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after completing ExpandingStoryboard animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            UpdateState();
            OnAnimationFinish?.Invoke(this, new SideBarAnimationFinishEventArgs(MenuState));
        }

        #endregion ANIMATION METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show interface programmatically. </summary>
        public void ShowInterface()
        {
            AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide interface programmatically. </summary>
        /// <param name="ifNotMouseOver"> Only if cursor is not over control. </param>
        public void HideInterface(bool ifNotMouseOver = true)
        {
            if (ifNotMouseOver && IsMouseOver)
                return;

            AnimateHideControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Close expanded controls such as Menu and PlayList. </summary>
        /// <param name="ifNotMouseOver"> Only if cursor is not over control. </param>
        public void HideExpandedControls(bool ifNotMouseOver = true)
        {
            if (ifNotMouseOver && IsMouseOver)
                return;

            AnimateCollapseExpandedControls();
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region MENU MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup menu items. </summary>
        /// <param name="menuType"> Menu type. </param>
        public void SetupMenu(MenuItemType menuType)
        {
            MenuItems = new ObservableCollection<MenuItem>(MenuBuilder.BuildMenu(menuType));
            MoreMenuItems = new ObservableCollection<MenuItem>(MenuBuilder.BuildMoreMenu(menuType));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting any item in SideBarMenuListView. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void SideBarMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListViewEx)sender;
            var selectedItem = listView.SelectedItem;

            if (selectedItem != null)
            {
                var menuItem = (MenuItem)selectedItem;

                switch (menuItem.Type)
                {
                    case MenuItemType.MAIN_MENU:
                        switch (menuItem.SubType)
                        {
                            case MenuItemSubType.OPEN_CLOSE:
                                OnMainMenuOpenCloseItemSelected();
                                break;

                            case MenuItemSubType.NOW_PLAYING:
                                OnMainMenuNowPlayingItemSelected();
                                break;

                            case MenuItemSubType.LYRICS:
                                break;
                        }
                        break;

                    case MenuItemType.SETTINGS_MENU:
                        switch (menuItem.SubType)
                        {
                            case MenuItemSubType.OPEN_CLOSE:
                                OnMainMenuOpenCloseItemSelected();
                                break;
                        }
                        break;
                }

                OnMenuItemSelected?.Invoke(menuItem, new EventArgs());

                listView.SelectedIndex = -1;
                listView.SelectedItem = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting MainMenu OpenClose MenuItem. </summary>
        private void OnMainMenuOpenCloseItemSelected()
        {
            switch (MenuState)
            {
                case SideBarMenuState.PLAYLIST:
                    AnimateClosePlayList();
                    AnimateExpandMenu();
                    break;

                case SideBarMenuState.VISIBLE:
                    AnimateExpandMenu();
                    break;

                case SideBarMenuState.EXTENDED:
                    AnimateCollapseMenu();
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting MainMenu NowPlaying MenuItem. </summary>
        private void OnMainMenuNowPlayingItemSelected()
        {
            switch (MenuState)
            {
                case SideBarMenuState.EXTENDED:
                    if (!LockPlayListVisibility)
                    {
                        AnimateCollapseMenu();
                        AnimateOpenPlayList();
                    }
                    break;

                case SideBarMenuState.VISIBLE:
                    if (!LockPlayListVisibility)
                        AnimateOpenPlayList();
                    break;

                case SideBarMenuState.PLAYLIST:
                    if (!LockPlayListVisibility)
                        AnimateClosePlayList();
                    break;
            }
        }

        #endregion MENU MANAGEMENT METHODS

        #region MOVEMENT INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after entering cursor over SideBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after leaving cursor from SideBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                AnimateHideControl();
            else
                AnimateCollapseExpandedControls();
        }

        #endregion MOVEMENT INTERACTION METHODS

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

        #region MENU STATE MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Prepare menu state for update. </summary>
        /// <param name="menuState"> New menu state. </param>
        private void PrepareUpdateState(SideBarMenuState menuState = SideBarMenuState.VISIBLE)
        {
            _nextMenuState = menuState;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update manu state to previous prepared state. </summary>
        private void UpdateState()
        {
            if (_nextMenuState.HasValue)
                _menuState = _nextMenuState.Value;

            _nextMenuState = null;
        }

        #endregion MENU STATE MANAGEMENT

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show nowplaying playlist interface programmatically. </summary>
        public void ShowPlayList()
        {
            if (MenuState != SideBarMenuState.PLAYLIST)
            {
                if (MenuState == SideBarMenuState.HIDDEN)
                    AnimateShowControl();

                AnimateOpenPlayList();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide nowplaying playlist interface programmatically. </summary>
        public void HidePlayList()
        {
            if (MenuState == SideBarMenuState.PLAYLIST)
                AnimateClosePlayList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton in PlayList. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void sideBarPlayList_OnBackClick(object sender, RoutedEventArgs e)
        {
            AnimateClosePlayList();
        }

        #endregion PLAYLIST METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading user control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateShowControl();
        }

        #endregion USER CONTROL METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate margin for hide component. </summary>
        /// <returns> Margin of hidden component. </returns>
        private Thickness CalculateHiddenMargin()
        {
            double marginLeft = CONTROL_MARGIN_PART + MENU_WIDTH_MIN;
            return new Thickness(-marginLeft, 0, 0, 0);
        }

        #endregion UTILITY METHODS

    }
}
