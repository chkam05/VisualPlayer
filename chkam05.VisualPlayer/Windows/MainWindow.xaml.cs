﻿using chkam05.Tools.ControlsEx.InternalMessages;
using chkam05.VisualPlayer.Controls;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Core;
using chkam05.VisualPlayer.Core.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Configuration.Events;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Pages;
using chkam05.VisualPlayer.Pages.Events;
using chkam05.VisualPlayer.Pages.Settings;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Utilities.Serial;
using chkam05.VisualPlayer.Visualisations;
using chkam05.VisualPlayer.Visualisations.Data;
using CSCore.SoundOut;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using MenuItem = chkam05.VisualPlayer.Controls.Data.MenuItem;


namespace chkam05.VisualPlayer.Windows
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        //  CONST

        private const string COMMAND_HOME = "/visual_player_command_page_home";
        private const string COMMAND_LYRICS_MANAGER = "/visual_player_command_page_lyrics";
        private const string COMMAND_SETTINGS = "/visual_player_command_page_settings";


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private Dictionary<UserControl, bool> _controlsUpdating;
        private bool _anyControlUpdated;
        private List<string> _arguments;
        private bool _initialized = false;
        private SystemListener _systemListener;
        private DispatcherHandler _vsDispatcherHandler;

        private double _posTop, _posLeft;
        private double _startW, _startH;
        private double _startX, _startY;

        public ConfigManager ConfigManager { get; private set; }
        public DispatcherHandler DispatcherHandler { get; private set; }
        public FilesManager FilesManager { get; private set; }
        public LyricsManager LyricsManager { get; private set; }
        public Player Player { get; private set; }
        public SerialController SerialController { get; private set; }
        public VisualisationsManager VisualisationsManager { get; private set; }


        //  GETTERS & SETTERS

        private bool AnyControlUpdating
        {
            get => _controlsUpdating.Any(d => d.Value == true);
        }

        public InternalMessagesManager MessagesManager { get; private set; }

        public IPagesManager PagesManager
        {
            get => pagesControl;
        }

        public Size Size
        {
            get => new Size(ActualWidth, ActualHeight);
        }

        public Size VisualisationRenderSize
        {
            get
            {
                try
                {
                    return new Size(
                        visualisationGrid.ActualWidth,
                        visualisationGrid.ActualHeight);
                }
                catch (Exception)
                {
                    return new Size(0, 0);
                }
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainWindow class constructor. </summary>
        /// <param name="args"> List of arguments. </param>
        public MainWindow(List<string> args = null)
        {
            var appTitle = ApplicationHelper.Instance.GetApplicationTitle();
            var appVersion = ApplicationHelper.Instance.GetApplicationVersion();

            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            ConfigManager.OnConfigUpdate += UpdateConfig;
            DispatcherHandler = new DispatcherHandler(this.Dispatcher);
            FilesManager = FilesManager.Instance;
            LyricsManager = LyricsManager.Instance;
            Player = Player.Instnace;
            Player.OnContiniousUpdate += InterfaceContiniousUpdate;
            Player.OnLoadedFile += OnLoadedFile;
            Player.OnStateUpdate += InterfaceStateUpdate;
            Player.EqualizerManager.ApplyConfiguration(ConfigManager);
            Player.Volume = ConfigManager.Volume;
            SerialController = SerialController.Instnace;
            _systemListener = SystemListener.Instance;
            _systemListener.UserPreferenceChangedHandler += UserPreferenceChanged;
            VisualisationsManager = VisualisationsManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            _arguments = args;
            _controlsUpdating = new Dictionary<UserControl, bool>()
            {
                { sideBarMenu, false },
                { controlBar, false },
                { informationBar, false }
            };
            _vsDispatcherHandler = new DispatcherHandler(visualisationRenderImage.Dispatcher);

            LyricsManager.Controller = lyricsControl;
            LyricsManager.UpdateConfiguration(ConfigManager);
            MessagesManager = new InternalMessagesManager(messagesControl, ConfigManager);
            VisualisationsManager.Create(ConfigManager.VisualisationType, null, VisualisationRenderSize);
            VisualisationsManager.UpdateConfiguration(ConfigManager);
        }

        #endregion CLASS METHODS

        #region CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating any configuration property in ConfigManager. </summary>
        /// <param name="sender"> ConfigManager instance. </param>
        /// <param name="e"> Config Update Event Arguments. </param>
        private void UpdateConfig(object sender, ConfigUpdateEventArgs e)
        {
            if (ConfigManager.AppearanceUpdateProperties.Contains(e.PropertyName))
            {
                if (!logoControl.LockVisibility)
                    logoControl.Visibility = ConfigManager.LogoEnabled ? Visibility.Visible : Visibility.Collapsed;

                logoControl.SetShapesBackground(ConfigManager.LogoBackgroundColorBrush);
                logoControl.SetShapesBorderBrush(ConfigManager.LogoBorderColorBrush);
                logoControl.ScaleToObject(Size);

                MessagesManager.UpdateConfiguration();
            }

            if (ConfigManager.LyricsUpdateProperties.Contains(e.PropertyName))
                LyricsManager.UpdateConfiguration(sender, e);

            if (ConfigManager.VisualisationUpdateProperties.Contains(e.PropertyName))
            {
                if (e.PropertyName == nameof(ConfigManager.VisualisationType))
                {
                    var type = (VisualisationType)e.PropertyValue;
                    var spectrumProvider = Player?.SpectrumProvider;

                    VisualisationsManager.Create(type, spectrumProvider, VisualisationRenderSize);

                    if (VisualisationsManager.Created)
                    {
                        var visualisation = VisualisationsManager.Visualisation;
                        visualisation.Margin = GetVisualisationMargin();

                        VisualisationsManager.UpdateConfiguration(sender as ConfigManager);
                        VisualisationsManager.UpdateDrawingAreaSize(VisualisationRenderSize);
                    }
                }

                VisualisationsManager.UpdateConfiguration(sender, e);
            }
        }

        #endregion CONFIGURATION METHODS

        #region CONTROL BAR METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar repeat button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnRepeatClick(object sender, RoutedEventArgs e)
        {
            Player.SwitchRepeatMode();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar shuffle button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnShuffleClick(object sender, RoutedEventArgs e)
        {
            Player.Shuffle = !Player.Shuffle;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar previous button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnPreviousClick(object sender, RoutedEventArgs e)
        {
            Action_LoadPreviousTrack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar play/pause button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnPlayPauseClick(object sender, RoutedEventArgs e)
        {
            Action_PlayPause();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar next button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnNextClick(object sender, RoutedEventArgs e)
        {
            Action_LoadNextTrack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar stop button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnStopClick(object sender, RoutedEventArgs e)
        {
            Player.Stop();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking control bar mute button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ControlBar_OnVolumeClick(object sender, RoutedEventArgs e)
        {
            if (volumeControl.IsExpanded)
            {
                volumeControl.HideInterface(false);
                controlBar.VolumeControlPopped = false;
            }
            else
            {
                volumeControl.ShowInterface();
                controlBar.VolumeControlPopped = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing value of ControlBar track ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void controlBar_OnTrackSliderValueChanged(object sender, SliderValueChangedEventArgs<double> e)
        {
            if (Player.LoadedItem != null && e.Final)
            {
                Player.SetTrackPosition(e.Value);
                informationBar.CurrentTime = Player.TrackPosition;

                if (PagesManager.LoadedPage is LyricsPage)
                    (PagesManager.LoadedPage as LyricsPage).InterfaceStateUpdate(Player, new EventArgs());
            }
        }

        #endregion CONTROL BAR METHODS

        #region CONTROLS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after starting InformationBar show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> InformationBar Animate Event Arguments. </param>
        private void informationBar_OnAnimate(object sender, Controls.Events.InformationBarAnimateEventArgs e)
        {
            InformationBar control = (InformationBar)sender;
            _controlsUpdating[control] = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after finish InformationBar show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> InformationBar Animation Finish Event Arguments. </param>
        private void informationBar_OnAnimationFinish(object sender, Controls.Events.InformationBarAnimationFinishEventArgs e)
        {
            InformationBar control = (InformationBar)sender;
            _controlsUpdating[control] = false;
            _anyControlUpdated = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing InformationBar close button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void informationBar_OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing InformationBar maximize button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void informationBar_OnMaximizeButtonClick(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing InformationBar minimize button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void informationBar_OnMinimizeButtonClick(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after starting ControlBar show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> ControlBar Animate Event Arguments. </param>
        private void controlBar_OnAnimate(object sender, Controls.Events.ControlBarAnimateEventArgs e)
        {
            ControlBar control = (ControlBar)sender;
            _controlsUpdating[control] = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after finish ControlBar show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> ControlBar Animation Finish Event Arguments. </param>
        private void controlBar_OnAnimationFinish(object sender, Controls.Events.ControlBarAnimationFinishEventArgs e)
        {
            ControlBar control = (ControlBar)sender;
            _controlsUpdating[control] = false;
            _anyControlUpdated = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after starting SideBarMenu show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> SideBar Animate Event Arguments. </param>
        private void sideBarMenu_OnAnimate(object sender, Controls.Events.SideBarAnimateEventArgs e)
        {
            SideBarMenu control = (SideBarMenu)sender;
            _controlsUpdating[control] = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after finish SideBarMenu show/hide animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> SideBar Animation Finish Event Arguments. </param>
        private void sideBarMenu_OnAnimationFinish(object sender, Controls.Events.SideBarAnimationFinishEventArgs e)
        {
            SideBarMenu control = (SideBarMenu)sender;
            _controlsUpdating[control] = false;
            _anyControlUpdated = true;
        }

        #endregion CONTROLS INTERACTION METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load files from arguments. </summary>
        /// <param name="arguments"> List of files paths from arguments. </param>
        /// <returns> True - files loaded; False - files not loaded. </returns>
        public bool OpenFilesFromArgs(List<string> arguments)
        {
            if (arguments != null && arguments.Any())
            {
                var filesTypes = FilesManager.GetFileTypesByGroup(FileGroup.MUSIC);

                List<string> validFilesPaths = arguments.Where(f => File.Exists(f) 
                    && filesTypes.Any(t => FilesManager.MatchFileType(f, t))).ToList();

                if (validFilesPaths != null && validFilesPaths.Any())
                {
                    var progressMessage = MessagesManager.CreateProgressMessage("Loading files", "");
                    FilesManager.LoadPlayableFilesAsync(validFilesPaths, FileGroup.MUSIC, Player.PlayList, DispatcherHandler, progressMessage);
                }
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Open files using Open Files Dialog Box. </summary>
        public void OpenFilesByDialog()
        {
            var currentLock = controlBar.LockVisibility;
            controlBar.LockVisibility = false;

            var filesPaths = FilesManager.DialogOpenFiles(FileGroup.MUSIC, "Select music files to load.");

            if (filesPaths != null && filesPaths.Any())
            {
                var progressMessage = MessagesManager.CreateProgressMessage("Loading files", "");
                FilesManager.LoadPlayableFilesAsync(filesPaths, FileGroup.MUSIC, Player.PlayList, DispatcherHandler, progressMessage);
            }

            sideBarMenu.LockVisibility = currentLock;

            if (controlBar.AutoHide)
                controlBar.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load last playlist. </summary>
        private void LoadPlayListOnStartup()
        {
            var filePath = FilesManager.PlayListCache;

            if (ConfigManager.RestoreLastPlaylistOnStartup && File.Exists(filePath))
                Player.PlayList.LoadFromFile(filePath);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save last playlist. </summary>
        private void SavePlayListOnClose()
        {
            var filePath = FilesManager.PlayListCache;

            if (ConfigManager.RestoreLastPlaylistOnStartup)
                Player.PlayList.SaveToFile(filePath);

            else if (File.Exists(filePath))
                File.Delete(filePath);
        }

        #endregion FILES MANAGEMENT METHODS

        #region MAIN MENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting any MenuItem in SideBarMenuListView. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void sideBarMenu_OnMenuItemSelected(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                switch (menuItem.Type)
                {
                    case MenuItemType.MAIN_MENU:
                        switch (menuItem.SubType)
                        {
                            case MenuItemSubType.HOME:
                                MainMenu_LoadHomePage();
                                break;

                            case MenuItemSubType.OPEN_FILES:
                                OpenFilesByDialog();
                                break;

                            case MenuItemSubType.EQUALIZER:
                                MainMenu_LoadEqualizerPage();
                                break;

                            case MenuItemSubType.SETTINGS:
                                MainMenu_LoadSettingsPage();
                                break;

                            case MenuItemSubType.LYRICS:
                                MainMenu_LoadLyricsPage();
                                break;
                        }
                        break;

                    case MenuItemType.SETTINGS_MENU:
                        switch (menuItem.SubType)
                        {
                            case MenuItemSubType.ABOUT:
                                PagesManager.LoadPage(new SettingsAboutPage(PagesManager));
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

                            case MenuItemSubType.EXTERNAL_DEVICES:
                                PagesManager.LoadPage(new SettingsExternalDevicesPage(PagesManager));
                                break;
                        }
                        break;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load Home Page. </summary>
        private void MainMenu_LoadHomePage()
        {
            if (PagesManager.LoadedPage != null)
                PagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load Equalizer Page. </summary>
        private void MainMenu_LoadEqualizerPage()
        {
            PagesManager.LoadPage(new EqualizerPage(PagesManager));
            PagesManager.ShowInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load Lyrics Page. </summary>
        private void MainMenu_LoadLyricsPage()
        {
            PagesManager.LoadPage(new LyricsPage(PagesManager));
            PagesManager.ShowInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load Settings Page. </summary>
        private void MainMenu_LoadSettingsPage()
        {
            PagesManager.LoadPage(new SettingsHomePage(PagesManager));
            PagesManager.ShowInterface();
        }

        #endregion MAIN MENU METHODS

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

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after hiding PagesControl interface. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void pagesControl_OnHide(object sender, EventArgs e)
        {
            informationBar.LockVisibility = false;
            controlBar.LockVisibility = false;
            logoControl.LockVisibility = false;
            lyricsControl.LockVisibility = false;
            sideBarMenu.LockVisibility = false;
            sideBarMenu.LockPlayListVisibility = false;

            if (informationBar.CanShow)
                informationBar.ShowInterface();

            if (ConfigManager.LogoEnabled)
                logoControl.ShowInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after showing PagesControl interface. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void pagesControl_OnShow(object sender, EventArgs e)
        {
            if (!controlBar.IsExpanded)
                controlBar.ShowInterface();

            if (informationBar.IsExpanded)
                informationBar.HideInterface(false);

            if (logoControl.IsShowed)
                logoControl.HideInterface();

            if (lyricsControl.IsShowed)
                lyricsControl.HideInterface();

            if (!sideBarMenu.IsDeploing)
                sideBarMenu.ShowInterface();

            if (sideBarMenu.MenuState == SideBarMenuState.PLAYLIST)
                sideBarMenu.HidePlayList();

            controlBar.LockVisibility = true;
            informationBar.LockVisibility = true;
            logoControl.LockVisibility = true;
            lyricsControl.LockVisibility = true;
            sideBarMenu.LockVisibility = true;
            sideBarMenu.LockPlayListVisibility = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after navigating back in PagesControl. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Page Loaded Event Arguments. </param>
        private void pagesControl_OnPageBack(object sender, PageLoadedEventArgs e)
        {
            SetupSpecialMenu(e?.Page);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading page in PagesControl. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Page Loaded Event Arguments. </param>
        private void pagesControl_OnPageLoaded(object sender, PageLoadedEventArgs e)
        {
            SetupSpecialMenu(e?.Page);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup special menu located in page. </summary>
        /// <param name="page"> Page with special menu configuration. </param>
        private void SetupSpecialMenu(IPage page)
        {
            if (page != null)
            {
                if (page.SpecialMenu.HasValue)
                    sideBarMenu.SetupMenu(page.SpecialMenu.Value);

                return;
            }

            sideBarMenu.SetupMenu(MenuItemType.MAIN_MENU);
        }

        #endregion PAGES MANAGEMENT METHODS

        #region PLAYER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for load next track. </summary>
        private void Action_LoadNextTrack()
        {
            Player.Next();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for load previous track. </summary>
        private void Action_LoadPreviousTrack()
        {
            Player.Previous();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for start or pause playing. </summary>
        private void Action_PlayPause()
        {
            switch (Player.PlaybackState)
            {
                case PlaybackState.Playing:
                    {
                        Player.Pause();
                        break;
                    }

                case PlaybackState.Paused:
                case PlaybackState.Stopped:
                    {
                        if (Player.LoadedItem == null)
                            Player.Play(Player.PlayList.Select(0));

                        else
                            Player.Play();

                        break;
                    }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for updating interface with player data. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event arguments. </param>
        private void InterfaceContiniousUpdate(object sender, EventArgs e)
        {
            DispatcherHandler.TryInvoke(() =>
            {
                controlBar.TrackPosition = Player.TrackPosition.TotalSeconds;
                informationBar.CurrentTime = Player.TrackPosition;
            });

            //  Update Lyrics Page.
            if (PagesManager.LoadedPage is LyricsPage)
                (PagesManager.LoadedPage as LyricsPage).InterfaceStateUpdate(sender, e);

            //  Update Lyrics.
            LyricsManager.GetNextLyrics(Player.LoadedItem, Player.TrackPosition, this.Dispatcher);

            //  Render Visualisation.
            DrawVisualisation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method for updating interface with heavy player data. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event arguments. </param>
        private void InterfaceStateUpdate(object sender, EventArgs e)
        {
            DispatcherHandler.TryInvoke(() =>
            {
                if (Player.PlaybackState == PlaybackState.Playing && informationBar.CanShow)
                    informationBar.ShowInterface(true);

                controlBar.PlayPauseButtonState = Player.PlaybackState;
                controlBar.TrackLength = Player.TrackLength.TotalSeconds;
                controlBar.TrackPosition = Player.TrackPosition.TotalSeconds;

                //TitleInformationBar.Text = Player.LoadedItem?.Title;
                informationBar.TitleInfo = Player.LoadedItem?.Title;
                informationBar.AlbumInfo = Player.LoadedItem?.Album;
                informationBar.ArtistInfo = Player.LoadedItem?.Artist;
                informationBar.CoverImage = Player.LoadedItem?.Cover;

                informationBar.FullTime = Player.TrackLength;
                informationBar.CurrentTime = Player.TrackPosition;

                //  Visualisation update.
                if (Player.PlaybackState == PlaybackState.Playing && VisualisationsManager.Created)
                {
                    var visualisation = VisualisationsManager.Visualisation;
                    visualisation.Margin = GetVisualisationMargin();

                    VisualisationsManager.UpdateSpectrumProvider(Player.SpectrumProvider);
                    VisualisationsManager.UpdateDrawingAreaSize(VisualisationRenderSize);
                }
                else if (Player.PlaybackState != PlaybackState.Playing)
                {
                    logoControl.ScaleToObject(Size);
                }
            });

            //  Update Lyrics Page.
            if (PagesManager.LoadedPage is LyricsPage)
                (PagesManager.LoadedPage as LyricsPage).InterfaceStateUpdate(sender, e);

            //  Clear visualisation.
            DrawVisualisation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading file in Player core. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Player Loaded File Event arguments. </param>
        private void OnLoadedFile(object sender, PlayerLoadedFileEventArgs e)
        {
            if (!LyricsManager.LyricsChanged)
            {
                DispatcherHandler.TryInvoke(() =>
                {
                    bool loaded = false;
                    LyricsManager.LoadedFile = e.File;

                    //  Auto load lyrics.
                    if (LyricsManager.AutoLoad)
                        loaded = LyricsManager.LoadFromLibrary();

                    if (!loaded)
                        LyricsManager.CreateEmpty();
                });
            }
        }

        #endregion PLAYER METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup application jump list. </summary>
        private void SetupJumpList()
        {
            var appLocation = ApplicationHelper.Instance.GetApplicationLocation();
            var creator = new JumpListCreator();

            creator.AddJumpListItem("Home", "Show home screen", appLocation, COMMAND_HOME, 0);
            creator.AddJumpListItem("Lyrics", "Show lyrics manager", appLocation, COMMAND_LYRICS_MANAGER, 2);
            creator.AddJumpListItem("Settings", "Show settings", appLocation, COMMAND_SETTINGS, 1);
            creator.SetupJumpList(Application.Current as App);
        }

        #endregion SETUP METHODS

        #region SYSTEM HANDLERS

        //  --------------------------------------------------------------------------------
        /// <summary> Process command line arguments. </summary>
        /// <param name="args"> Interfaced list of arguments. </param>
        /// <returns> True - method work completed successfully; False - otherwise. </returns>
        public bool ProcessCommandLineArgs(List<string> args)
        {
            if (args == null || args.Count == 0)
                return true;

            //  Bring window to front.
            WindowState = WindowState.Normal;
            Activate();

            //  First index contains location of application executable file.
            if (args.Count > 1)
            {
                //  Try process command.
                if (!ProcessCommand(args[1], args))
                {
                    //  Try to load files.
                    OpenFilesFromArgs(args.GetRange(1, args.Count - 2));
                }
            }

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Process single command line argument. </summary>
        /// <param name="command"> Command to process. </param>
        /// <param name="args"> Additional arguments. </param>
        /// <returns> True - command processed successfully; False - otherwise. </returns>
        public bool ProcessCommand(string command, IList<string> args)
        {
            switch (command)
            {
                case COMMAND_HOME:
                    MainMenu_LoadHomePage();
                    return true;

                case COMMAND_LYRICS_MANAGER:
                    MainMenu_LoadLyricsPage();
                    return true;

                case COMMAND_SETTINGS:
                    MainMenu_LoadSettingsPage();
                    return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing system user preferences. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void UserPreferenceChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                if (ConfigManager.ColorType == AppearanceColorType.SYSTEM 
                    || ConfigManager.ThemeType == Data.Configuration.AppearanceThemeType.SYSTEM)
                {
                    ConfigManager.ForceAppearanceUpdate();
                }
            }
        }

        #endregion SYSTEM HANDLERS

        #region TASKBAR THUMB BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method ivoked after clicking taskbar next thumb button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Next_ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            Action_LoadNextTrack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method ivoked after clicking taskbar play pause thumb button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void PlayPause_ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            Action_PlayPause();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method ivoked after clicking taskbar previous thumb button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Previous_ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            Action_LoadPreviousTrack();
        }

        #endregion TASKBAR THUMB BUTTONS METHODS

        #region VISUALISATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for rendering and updating visualisation. </summary>
        private void DrawVisualisation()
        {
            if (VisualisationsManager.Initialized)
            {
                var visualisation = VisualisationsManager.Visualisation;

                if (AnyControlUpdating || _anyControlUpdated)
                {
                    visualisation.Margin = GetVisualisationMargin();
                    visualisation.UpdateGraphics();

                    _anyControlUpdated = false;
                }

                visualisation.PreCalculate();

                var beatLevel = ConfigManager.LogoEnabled && ConfigManager.LogoAnimated
                    ? visualisation.GetBeatLevel() : 0.0;

                var drawer = Player.PlaybackState == PlaybackState.Playing
                    ? visualisation.Draw()
                    : visualisation.StopDrawing();

                var imageSource = drawer?.RenderImage(true);

                _vsDispatcherHandler?.TryInvoke(() =>
                {
                    if (imageSource != null)
                        visualisationRenderImage.Source = imageSource;

                    if (logoControl.IsShowed && Player.PlaybackState == PlaybackState.Playing)
                        logoControl.ScaleToObject(Size, beatLevel);
                });
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get visualisation area limitation. </summary>
        /// <returns> Visualisation margin. </returns>
        private Thickness GetVisualisationMargin()
        {
            var leftMargin = sideBarMenu.IsDeployed
                ? Math.Min(sideBarMenu.ActualWidth - 28, SideBarMenu.MENU_WIDTH_MIN + SideBarMenu.CONTROL_MARGIN_PART + 4)
                : 4;

            var topMargin = informationBar.IsExpanded ? informationBar.ActualHeight - 28 : 4;
            var bottomMargin = controlBar.IsExpanded ? controlBar.ActualHeight - 26 : 6;

            return new Thickness(leftMargin, topMargin, 4, bottomMargin);
        }

        #endregion VISUALISATION MANAGEMENT METHODS

        #region VOLUME CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Mute VolumeControl ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void volumeControl_OnVolumeMuteClick(object sender, RoutedEventArgs e)
        {
            int currentVolume = Player.Volume;

            if (currentVolume == 0)
            {
                Player.Volume = ConfigManager.Volume;
                volumeControl.Volume = ConfigManager.Volume;
            }
            else
            {
                Player.Volume = 0;
                volumeControl.Volume = 0;

                ConfigManager.Volume = currentVolume;
                ConfigManager.SaveConfiguration();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing value of VolumeControl Volume. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void volumeControl_OnSliderValueChanged(object sender, SliderValueChangedEventArgs<double> e)
        {
            Player.Volume = (int)e.Value;

            ConfigManager.Volume = Player.Volume;
            ConfigManager.SaveConfiguration();
        }

        //  --------------------------------------------------------------------------------
        private void volumeControl_MouseLeave(object sender, MouseEventArgs e)
        {
            controlBar.VolumeControlPopped = false;
        }

        #endregion VOLUME CONTROL METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Close Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseButtonEx_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Maximize Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MaximizeButtonEx_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Minimize Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MinimizeButtonEx_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button on TitleBar. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TitleBarBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during window closing. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Cancel Event Arguments. </param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfigManager.PlayListWidth = sideBarMenu.PlayListExpandedWidth;
            ConfigManager.WindowPosition = new Point(this.Left, this.Top);
            ConfigManager.WindowSize = new Size(this.ActualWidth, this.ActualHeight);
            ConfigManager.SaveConfiguration();

            //  Save current playlist.
            SavePlayListOnClose();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after dropping any item/object inside window. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Drop Event Arguments. </param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            //  Check if dropped items contains files.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //  Get dropped files.
                string[] filesPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                //  Load files into playlist.
                if (filesPaths != null && filesPaths.Any())
                {
                    var progressMessage = MessagesManager.CreateProgressMessage("Loading files", "");
                    FilesManager.LoadPlayableFilesAsync(filesPaths.ToList(), FileGroup.MUSIC, Player.PlayList, DispatcherHandler, progressMessage);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading window. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  Load window position and size.
            this.Left = ConfigManager.WindowPosition.X;
            this.Top = ConfigManager.WindowPosition.Y;
            this.Width = ConfigManager.WindowSize.Width;
            this.Height = ConfigManager.WindowSize.Height;

            //  Correct window position on screen.
            var screen = WindowsUtilities.GetScreenWhereIsWindow(this);

            if (screen != null)
                WindowsUtilities.AdjustWindowToScreen(this, screen);
            else
                WindowsUtilities.AdjustWindowToPrimaryScreen(this);

            //  Create logo.
            logoControl.Visibility = ConfigManager.LogoEnabled ? Visibility.Visible : Visibility.Collapsed;
            logoControl.CreateLogoFromResource(Properties.Resources.BaseLogo);
            logoControl.SetShapesBackground(ConfigManager.LogoBackgroundColorBrush);
            logoControl.SetShapesBorderBrush(ConfigManager.LogoBorderColorBrush);
            logoControl.ScaleToObject(Size);

            //  Other configuration.
            volumeControl.Volume = Player.Volume;

            //  Setup jump list.
            SetupJumpList();

            //  Load files or previous playlist.
            if (!OpenFilesFromArgs(_arguments))
                LoadPlayListOnStartup();

            //  Finish initialization.
            _arguments = null;
            _initialized = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during window size changing. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Size Changed Event Arguments. </param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (VisualisationsManager.Initialized)
                VisualisationsManager.UpdateDrawingAreaSize(VisualisationRenderSize);

            if (logoControl.IsVisible)
                logoControl.ScaleToObject(Size);

            sideBarMenu.OnParentSizeChanged();
        }

        #endregion WINDOW METHODS

        #region WINDOW RESIZE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _posTop = Top;
            _posLeft = Left;
            _startX = e.GetPosition(this).X;
            _startY = e.GetPosition(this).Y;
            _startW = Width;
            _startH = Height;

            Border border = sender as Border;
            switch (border.Name)
            {
                case "ResizeBorderTopLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeBorderTopRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "ResizeBorderBottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "ResizeBorderBottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeBorderTop":
                    Cursor = Cursors.SizeNS;
                    break;
                case "ResizeBorderLeft":
                    Cursor = Cursors.SizeWE;
                    break;
                case "ResizeBorderRight":
                    Cursor = Cursors.SizeWE;
                    break;
                case "ResizeBorderBottom":
                    Cursor = Cursors.SizeNS;
                    break;
            }

            border.CaptureMouse();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing left mouse button when cursor is over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            border.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Border border = sender as Border;
                double x = e.GetPosition(this).X;
                double y = e.GetPosition(this).Y;
                double fX = System.Windows.Forms.Cursor.Position.X;
                double fY = System.Windows.Forms.Cursor.Position.Y;
                double w = Width;
                double h = Height;

                switch (border.Name)
                {
                    case "ResizeBorderTopLeft":
                        w = _startW - (fX - _posLeft);
                        h = _startH - (fY - _posTop);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (w > MinWidth)
                            Left = fX;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case "ResizeBorderTopRight":
                        w = _startW + (x - _startX);
                        h = _startH - (fY - _posTop);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case "ResizeBorderBottomLeft":
                        w = _startW - (fX - _posLeft);
                        h = _startH + (y - _startY);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (w > MinWidth)
                            Left = fX;
                        break;

                    case "ResizeBorderBottomRight":
                        w = _startW + (x - _startX);
                        h = _startH + (y - _startY);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;
                        break;

                    case "ResizeBorderTop":
                        h = _startH - (fY - _posTop);

                        if (h < MinHeight)
                            h = MinHeight;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case "ResizeBorderLeft":
                        w = _startW - (fX - _posLeft);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (w > MinWidth)
                            Left = fX;
                        break;

                    case "ResizeBorderRight":
                        w = _startW + (x - _startX);

                        if (w < MinWidth)
                            w = MinWidth;
                        break;

                    case "ResizeBorderBottom":
                        h = _startH + (y - _startY);

                        if (h < MinHeight)
                            h = MinHeight;
                        break;
                }

                if (w >= MinWidth)
                    Width = w;

                if (h >= MinHeight)
                    Height = h;
            }
        }

        #endregion WINDOW RESIZE METHODS

    }
}
