﻿using chkam05.InternalMessages.Pages;
using chkam05.Visualisations;
using chkam05.Visualisations.Base;
using chkam05.VisualPlayer.Base;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Components.EventArgs;
using chkam05.VisualPlayer.Data;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lists;
using chkam05.VisualPlayer.Data.States;
using chkam05.VisualPlayer.Pages;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Files;
using chkam05.VisualPlayer.Utilities.Files.EventArgs;
using chkam05.VisualPlayer.Utilities.Pages;
using CSCore.SoundOut;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace chkam05.VisualPlayer.Windows
{
    public partial class MainWindow : Window
    {

        //  CONST

        private readonly string PLAYLIST_KEEP_FILE_NAME = "playlist.vpl";
        private const int PLAYLIST_MIN_WIDTH = 320;
        private const string TRACK_TIME_FORMAT = @"hh\:mm\:ss";
        private const int VOLUME_SLIDER_MAX_HEIGHT = 224;


        //  VARIABLES

        private IPagesManager _pagesManager;

        private bool _initialized = false;

        private bool _enabledInterfaceUpdater = false;
        private Task _interfaceUpdater;
        private int _interfaceUpdaterBreak = 100;
        private CancellationTokenSource _interfaceUpdaterTokenSource;

        private bool _keppPlayListOnTop = false;
        private bool _playListDragging = false;
        private Point _playListDragPoint;
        private bool _playListSplitterGrabbed = false;
        private double _playListWidth = 320;
        private SideBarState _sideBarState = SideBarState.COLLAPSED;

        private Color _themeColor;

        private OnScreenDisplay _osd;
        private Canvas _visualisationCanvas;
        private IVisualisation _visualisation;


        #region GETTERS & SETTERS

        private OnScreenDisplay OSD
        {
            get
            {
                if (_osd != null)
                    return _osd;

                var homePage = (HomePage)_pagesManager.GetPageByType(typeof(HomePage));
                _osd = homePage.OSD;
                return _osd;
            }
        }

        private bool InterfaceUpdaterCancelled
        {
            get => _interfaceUpdaterTokenSource == null 
                || _interfaceUpdaterTokenSource.IsCancellationRequested;
        }

        private bool InterfaceUpdaterWorking
        {
            get => _interfaceUpdater != null && _interfaceUpdater.Status == TaskStatus.Running;
        }

        private Canvas VisualisationCanvas
        {
            get
            {
                if (_visualisationCanvas != null)
                    return _visualisationCanvas;

                var homePage = (HomePage)_pagesManager.GetPageByType(typeof(HomePage));
                _visualisationCanvas = homePage.Canvas;
                return _visualisationCanvas;
            }
        }

        private bool VolumeControlVisible
        {
            get => VolumeSliderBorder.Visibility == Visibility.Visible;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainWindow class constructor. </summary>
        public MainWindow()
        {
            InitializeComponent();

            //  Setup pages manager.
            _pagesManager = new SinglePagesManager(ContentFrame);
            _pagesManager.KeepPreviousPages = true;

            //  Setup player core.
            PlayerCore.Instance.PlayList.SetVisualContainer(PlayListView);
        }

        #endregion CLASS METHODS

        #region ANIMATIONS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create animation to change sidebar width and state. </summary>
        /// <param name="playListWidth"> Playlist grid destination width. </param>
        /// <returns> Animation builder with created animation. </returns>
        private AnimationBuilder CreateSidebarWidthChangeAnimation(double playListWidth)
        {
            //  Create animation builder.
            AnimationBuilder animationBuilder = new AnimationBuilder();

            //  Create animation frames.
            var containerAnimationFrames = new DoubleKeyFrameCollection()
            {
                AnimationBuilder.CreateEasingDoubleKeyFrame(0.0, PlayListGrid.ActualWidth),
                AnimationBuilder.CreateEasingDoubleKeyFrame(0.1, playListWidth)
            };

            //  Add playlist width animation to storyboard.
            animationBuilder.AddDoubleAnimationUsingKeyFrames(
                containerAnimationFrames,
                PlayListGrid,
                new PropertyPath("Width"));

            //  Return updated animation builder.
            return animationBuilder;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create animation to change volume slider height and visibility. </summary>
        /// <param name="volumeSliderHeight"> Volue slider container desination height. </param>
        /// <returns> Animation builder with created animation. </returns>
        private AnimationBuilder CreateVolumeSliderHeightChangeAnimation(double volumeSliderHeight)
        {
            //  Create animation builder.
            AnimationBuilder animationBuilder = new AnimationBuilder();

            //  Create animation frames.
            var containerAnimationFrames = new DoubleKeyFrameCollection()
            {
                AnimationBuilder.CreateEasingDoubleKeyFrame(0.0, VolumeSliderBorder.ActualHeight),
                AnimationBuilder.CreateEasingDoubleKeyFrame(0.1, volumeSliderHeight)
            };

            //  Add volume slider height chanange animation to storyboard.
            animationBuilder.AddDoubleAnimationUsingKeyFrames(
                containerAnimationFrames,
                VolumeSliderBorder,
                new PropertyPath("Height"));

            //  Return updated animation builder.
            return animationBuilder;
        }

        #endregion ANIMATION MANAGEMENT METHODS

        #region EXCEPTION HANDLER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup exception handler. </summary>
        private void SetupExceptionHandler()
        {
            var exceptionsHandler = ExceptionsHandler.Instance;
            exceptionsHandler.OnExceptionHandle += OnExceptionHandle;
            exceptionsHandler.InitializeInterface(InternalMessages);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when exception handler process exception. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Handle exception event arguments. </param>
        private void OnExceptionHandle(object sender, Base.EventArgs.HandleExceptionEventArgs e)
        {
            //
        }

        #endregion EXCEPTION HANDLER METHODS

        #region FILES MANAGEMENT METHODS

        //  FILES

        //  --------------------------------------------------------------------------------
        /// <summary> Load files into nowplaying playlist. </summary>
        /// <param name="filesPaths"> Array of files paths to load. </param>
        private void LoadFiles(string[] filesPaths)
        {
            if (filesPaths != null && filesPaths.Any())
            {
                var asyncFilesLoader = new AsyncFileLoader<SongFile>();
                var imProgress = (InternalMessageProgress)InternalMessages.ShowProgress(
                    "Loading files...", "Please wait...", PackIconKind.FolderOpen, true);

                imProgress.ProgressMax = 100;
                imProgress.ProgressValue = 0;

                //  Setup on loaded file method.
                asyncFilesLoader.OnFileLoaded += new EventHandler<FilesLoaderOnLoadEventArgs>((s, e) =>
                {
                    imProgress.Message = $"Loading file {e.FileName}";
                    imProgress.ProgressValue = e.Progress;
                });

                //  Setup on loading files complete method.
                asyncFilesLoader.OnWorkComplete += new EventHandler<FilesLoaderOnCompleteEventArgs>((s, e) =>
                {
                    imProgress.Close();
                });

                //  Start loading files.
                bool started = asyncFilesLoader.LoadFilesAsync(PlayerCore.Instance.PlayList, filesPaths);

                //  Close message if laoding not starts.
                if (!started)
                    imProgress.Close();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load files using open file dialog. </summary>
        private void LoadFilesByOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*aac;*ac3;*.mp3;*.ogg;*.wav;*wma|Raw Audio Files|*pcm;*raw;*sam|All Files|*.*";
            openFileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Open audio files.";

            var result = openFileDialog.ShowDialog();

            if (result ?? false)
                LoadFiles(openFileDialog.FileNames);
        }

        //  PLAYLIST - KEEP PLAYLIST ON RESTART

        //  --------------------------------------------------------------------------------
        /// <summary> Load last played playlist if keep playlist on restart option is enabled. </summary>
        private void LoadPlayListAtStartup()
        {
            var config = ConfigManager.Instance.Config;

            if (config.PlayerKeepPlayListOnRestart)
            {
                var app = (App)Application.Current;
                var playerCore = PlayerCore.Instance;
                string playListFilePath = Path.Combine(app.ConfigurationPath, PLAYLIST_KEEP_FILE_NAME);

                if (File.Exists(playListFilePath))
                {
                    var serializedData = XElement.Load(playListFilePath);
                    playerCore.PlayList.DeserializeFromXML(serializedData);
                }

                if (playerCore.PlayList.SelectedItem != null)
                {
                    var selectedSong = playerCore.SelectFromPlayList(
                        playerCore.PlayList.SelectedItemIndex, false);

                    OnPlayFile(playerCore, selectedSong);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save current playlist to kepp in memory on restart. </summary>
        private void SavePlayListAtShutdown()
        {
            var app = (App)Application.Current;
            var config = ConfigManager.Instance.Config;
            string playListFilePath = Path.Combine(app.ConfigurationPath, PLAYLIST_KEEP_FILE_NAME);

            if (config.PlayerKeepPlayListOnRestart)
            {
                var playerCore = PlayerCore.Instance;

                playerCore.PlayList.SerializeToXML().Save(playListFilePath);
            }
            else
            {
                if (File.Exists(playListFilePath))
                    File.Delete(playListFilePath);
            }
        }

        //  PLAYLIST

        //  --------------------------------------------------------------------------------
        /// <summary> Load playlist as current using open file dialog. </summary>
        private void LoadPlayListByOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Playlist File|*vpl|XML File|*xml|All Files|*.*";
            openFileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Open playlist.";

            var result = openFileDialog.ShowDialog();

            if (result ?? false)
            {
                var serializedData = XElement.Load(openFileDialog.FileName);
                var playlist = PlayerCore.Instance.PlayList;
                playlist.DeserializeFromXML(serializedData);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save playlist to file using save file dialog. </summary>
        /// <typeparam name="T"> Playlist file type. </typeparam>
        /// <param name="playlist"> Playlist to save. </param>
        private void SavePlayListBySaveFileDialog<T>(IPlayList<T> playlist) where T : BaseFile
        {
            if (typeof(ISerializable).IsAssignableFrom(playlist.GetType()))
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".vpl";
                saveFileDialog.Filter = "Playlist File|*vpl|XML File|*xml|All Files|*.*";
                saveFileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
                saveFileDialog.Title = "Save playlist";

                var result = saveFileDialog.ShowDialog();
            }
        }

        #endregion FILES MANAGEMENT METHODS

        #region COMPONENT MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change sidebar state. </summary>
        /// <param name="state"> New sidebar state. </param>
        private void ChangeSideBarState(SideBarState state)
        {
            //  Get current parameters.
            double playListWidth = PlayListGrid.ActualWidth;
            Visibility playListVisibility = PlayListGrid.Visibility;

            //  Setup new and startup parameters.
            switch (state)
            {
                case SideBarState.HIDDEN:
                case SideBarState.COLLAPSED:
                    playListWidth = 0;
                    playListVisibility = Visibility.Collapsed;
                    break;

                case SideBarState.EXPANDED:
                case SideBarState.PLAYLIST:
                    playListWidth = Math.Max(PLAYLIST_MIN_WIDTH, Math.Min(_playListWidth, this.ActualWidth * 0.75));
                    playListVisibility = Visibility.Visible;

                    PlayListGrid.Width = 0;
                    PlayListGrid.Visibility = Visibility.Visible;
                    break;
            }

            //  Create final method to do after animation.
            EventHandler onAnimationComplete = (sender, e) =>
            {
                //  Update sidebar state.
                _sideBarState = state;

                //  Set final parameters.
                PlayListGrid.Visibility = playListVisibility;
                PlayListGrid.Width = playListWidth;
                CloseMainMenuListViewItem.Visibility = playListVisibility;
            };

            if (_initialized)
            {
                //  Create animation builder.
                var animationBuilder = CreateSidebarWidthChangeAnimation(playListWidth);
                animationBuilder.OnAnimationComplete += onAnimationComplete;

                //  Run animation.
                animationBuilder.Run();
            }
            else
            {
                //  Invoke final method if animation was not launched.
                onAnimationComplete?.Invoke(this, null);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show or hide volume slider control. </summary>
        /// <param name="visible"> True - show volume slider control; False - hide. </param>
        private void ShowVolumeControl(bool visible)
        {
            //  Get current parameters.
            double volumeSliderHeight = VolumeSliderBorder.ActualHeight;
            Visibility volumeSliderVisibility = VolumeSliderBorder.Visibility;

            //  Setup new and startup parameters.
            if (visible)
            {
                volumeSliderHeight = VOLUME_SLIDER_MAX_HEIGHT;
                volumeSliderVisibility = Visibility.Visible;

                VolumeSliderBorder.Height = 0;
                VolumeSliderBorder.Visibility = Visibility.Visible;
            }
            else
            {
                volumeSliderHeight = 0;
                volumeSliderVisibility = Visibility.Collapsed;
            }

            //  Create final method to do after animation.
            EventHandler onAnimationComplete = (sender, e) =>
            {
                //  Set final parameters.
                VolumeSliderBorder.Visibility = volumeSliderVisibility;
                VolumeSliderBorder.Height = volumeSliderHeight;
                VolumeControlButton.Pressed = VolumeControlVisible;
            };

            if (_initialized)
            {
                //  Create animation builder.
                var animationBuilder = CreateVolumeSliderHeightChangeAnimation(volumeSliderHeight);
                animationBuilder.OnAnimationComplete += onAnimationComplete;

                //  Run animation.
                animationBuilder.Run();
            }
            else
            {
                //  Invoke final method if animation was not launched.
                onAnimationComplete?.Invoke(this, null);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set new playlist width. </summary>
        /// <param name="width"> New width of playlist. </param>
        private void SetPlayListWidth(double width)
        {
            //  Clamp playlist width to available width.
            var diffWidth = Math.Max(PLAYLIST_MIN_WIDTH, Math.Min(width, this.ActualWidth * 0.75));

            //  Set playlist new width.
            if (diffWidth != width || PlayListGrid.ActualWidth != width)
            {
                _playListWidth = diffWidth;
                PlayListGrid.Width = diffWidth;
            }
        }

        #endregion COMPONENT MANAGEMENT METHODS

        #region CONFIGURATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load components and window configuration. </summary>
        private void LoadConfiguration()
        {
            try
            {
                var config = ConfigManager.Instance.Config;
                var playerCore = PlayerCore.Instance;

                this.Left = config.WinPosition.X;
                this.Top = config.WinPosition.Y;
                this.Height = config.WinSize.Height;
                this.Width = config.WinSize.Width;

                ChangeSideBarState(config.SideBarState);
                SetPlayListWidth(config.PlayListWidth);

                RepeatControlButton.Stage = config.PlayerRepeat;
                playerCore.Repeat = EnumTool<RepeatMode>.GetByIndex(config.PlayerRepeat);

                ShuffleControlButton.Pressed = config.PlayerShuffle;
                playerCore.Shuffle = config.PlayerShuffle;

                //  Update appearance.
                if (config.UseSystemColor)
                    UpdateSystemThemeConfiguration(config.VisualisationColorMode == VisualisationColorMode.APPLICATION);
                else
                    UpdateThemeConfiguration(config.ThemeColor,
                        config.VisualisationColorMode == VisualisationColorMode.APPLICATION);

                //  Setup visualisation.
                SetupVisualisation(config.VisualisationType);

                playerCore.AutoPlayAfterAdd = config.PlayerAutoPlayOnAdd;
                OSD.IsEnabled = config.PlayerOSDEnabled;
                OSD.ShowTimeInMiliseconds = config.PlayerOSDTime;
                UpdateVisualisationColor();

                //config.VisualisationLogoEnabled;
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                    $"Could not load configuration{Environment.NewLine}{exc.Message}",
                    "Visual Player",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save components and window configuration. </summary>
        private void SaveConfiguration()
        {
            var configManager = ConfigManager.Instance;
            var playerCore = PlayerCore.Instance;

            try
            {
                var config = configManager.Config;

                config.PlayerRepeat = EnumTool<RepeatMode>.GetIndex(playerCore.Repeat);
                config.PlayerShuffle = playerCore.Shuffle;

                config.WinPosition = new Point(this.Left, this.Top);
                config.WinSize = new Size(this.ActualWidth, this.ActualHeight);

                config.SideBarState = _sideBarState;
                config.PlayListWidth = _playListWidth;
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                    $"Could not load configuration{Environment.NewLine}{exc.Message}",
                    "Visual Player",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after any configuration change manually. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfigurationUpdate(object sender, Base.EventArgs.ConfigUpdateEventArgs e)
        {
            if (sender.GetType() == typeof(AppConfig))
            {
                var config = (AppConfig) sender;
                var playerCore = PlayerCore.Instance;

                if (e.Key == nameof(config.PlayerAutoPlayOnAdd))
                    PlayerCore.Instance.AutoPlayAfterAdd = config.PlayerAutoPlayOnAdd;

                else if (e.Key == nameof(config.PlayerKeepPlayListOnRestart))
                    return;

                else if (e.Key == nameof(config.PlayerOSDEnabled))
                    OSD.IsEnabled = config.PlayerOSDEnabled;

                else if (e.Key == nameof(config.PlayerOSDTime))
                {
                    var prevInfinite = OSD.IsInfinityVisible;
                    OSD.ShowTimeInMiliseconds = config.PlayerOSDTime;
                    var isInfinite = OSD?.IsInfinityVisible ?? false;

                    if (playerCore.PlaybackState != PlaybackState.Playing && isInfinite)
                        OSD?.Show();
                    else if (prevInfinite != isInfinite)
                        OSD?.Hide();
                }

                else if (e.Key == nameof(config.ThemeColor))
                    UpdateThemeConfiguration(config.ThemeColor,
                        config.VisualisationColorMode == VisualisationColorMode.APPLICATION);

                else if (e.Key == nameof(config.UseSystemColor))
                {
                    if (config.UseSystemColor)
                        UpdateSystemThemeConfiguration(config.VisualisationColorMode == VisualisationColorMode.APPLICATION);
                    else
                        UpdateThemeConfiguration(config.ThemeColor,
                            config.VisualisationColorMode == VisualisationColorMode.APPLICATION);
                }

                else if (e.Key == nameof(config.VisualisationEnabled))
                    _visualisation.Enabled = config.VisualisationEnabled;

                else if (e.Key == nameof(config.VisualisationLogoEnabled))
                    return;

                else if (e.Key == nameof(config.VisualisationType))
                    SetupVisualisation(config.VisualisationType);

                else if (e.Key == nameof(config.VisualisationColor))
                    UpdateVisualisationColor();

                else if (e.Key == nameof(config.VisualisationColorMode))
                    UpdateVisualisationColor();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update interface appearance using system theme color. </summary>
        /// <param name="affectVisualisation"> Change visualisation color too. </param>
        private void UpdateSystemThemeConfiguration(bool affectVisualisation)
        {
            try
            {
                var systemThemeColor = SystemInfo.GetThemeColor();
                UpdateThemeConfiguration(systemThemeColor, affectVisualisation);
            }
            catch
            {
                //  Cannot set system theme color.
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update interface appearance. </summary>
        /// <param name="themeColor"> Theme color. </param>
        /// <param name="affectVisualisation"> Change visualisation color too. </param>
        private void UpdateThemeConfiguration(Color themeColor, bool affectVisualisation)
        {
            _themeColor = themeColor;
            var brush = new SolidColorBrush(themeColor);

            this.TitleBarGrid.Background = brush;
            this.TTrackSlider.HandlerBrush = brush;
            this.TVolumeSlider.HandlerBrush = brush;

            if (affectVisualisation)
                UpdateVisualisationColor();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation color by color from configuration. </summary>
        private void UpdateVisualisationColor()
        {
            var config = ConfigManager.Instance.Config;
            Color visualisationColor = _themeColor;

            if (_visualisation != null)
            {
                switch (config.VisualisationColorMode)
                {
                    case VisualisationColorMode.APPLICATION:
                        visualisationColor = _themeColor;
                        break;

                    case VisualisationColorMode.CUSTOM:
                    case VisualisationColorMode.RAINBOW_2D:
                    case VisualisationColorMode.RAINBOW_3D:
                        visualisationColor = config.VisualisationColor;
                        break;
                }

                if (_visualisation.GetType() == typeof(StripesVisualisation))
                    (_visualisation as StripesVisualisation).FillColor = visualisationColor;
            }
        }

        #endregion CONFIGURATION MANAGEMENT METHODS

        #region CONTROL COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking next track control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void NextTrackControlButton_Click(object sender, RoutedEventArgs e)
        {
            var playerCore = PlayerCore.Instance;
            var selectedSong = playerCore.Next();

            UpdateVisualisationSpectrumProvider(playerCore);
            OnPlayFile(playerCore, selectedSong);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking play pause control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void PlayPauseControlButton_Click(object sender, RoutedEventArgs e)
        {
            var playerCore = PlayerCore.Instance;

            if (playerCore.PlaybackState == PlaybackState.Playing)
                playerCore.Pause();

            else if (playerCore.PlaybackState == PlaybackState.Paused)
                playerCore.Play();

            else if (playerCore.IsFileLoaded)
                playerCore.Play();

            OnPlayFile(playerCore, null);

            if (_visualisation.Enabled && !_visualisation.Initialized)
            {
                UpdateVisualisationSpectrumProvider(playerCore);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking previous track control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void PreviousTrackControlButton_Click(object sender, RoutedEventArgs e)
        {
            var playerCore = PlayerCore.Instance;
            var selectedSong = playerCore.Previous();

            UpdateVisualisationSpectrumProvider(playerCore);
            OnPlayFile(playerCore, selectedSong);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking repeat control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            var controlButton = sender as ControlButton;
            var playerCore = PlayerCore.Instance;
            var repeatMode = RepeatMode.NO;

            switch (controlButton.Stage)
            {
                case 0:
                    repeatMode = RepeatMode.NO;
                    break;

                case 1:
                    repeatMode = RepeatMode.SINGLE;
                    break;

                case 2:
                    repeatMode = RepeatMode.ALL;
                    break;
            }

            playerCore.Repeat = repeatMode;
            SetRepeatControlButton(repeatMode, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking shuffle control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ShuffleControlButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerCore.Instance.Shuffle = (sender as ControlButton).Pressed;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking stop control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void StopControlButton_Click(object sender, RoutedEventArgs e)
        {
            var playerCore = PlayerCore.Instance;

            if (playerCore.PlaybackState != PlaybackState.Stopped)
            {
                playerCore.Stop();
                OnStopPlaying(playerCore);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after manually change position of track slider handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Slider drag event arguments. </param>
        private void TTrackSlider_OnHandlerDrag(object sender, SliderDragEventArgs e)
        {
            if (e.Action == SliderDragAction.ON_RELEASE)
                PlayerCore.Instance.SetProcentageTrackPosition(e.Position);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after manually change position of volume slider handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Slider drag event arguments. </param>
        private void TVolumeSlider_OnHandlerDrag(object sender, SliderDragEventArgs e)
        {
            PlayerCore.Instance.Volume = (int) e.Position;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking colume control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void VolumeControlButton_Click(object sender, RoutedEventArgs e)
        {
            ShowVolumeControl(!VolumeControlVisible);
        }

        #endregion CONTROL COMPONENTES INTERACTION METHODS

        #region INTERFACE UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set repeat button state to state of repeat mode. </summary>
        /// <param name="repeatMode"> Repeat mode state. </param>
        private void SetRepeatControlButton(RepeatMode repeatMode, bool buttonTriggered = false)
        {
            switch (repeatMode)
            {
                case RepeatMode.NO:
                    RepeatControlButton.Icon = PackIconKind.RepeatOff;
                    RepeatControlButton.Title = "Repeat";
                    if (buttonTriggered)
                        RepeatControlButton.Stage = 0;
                    break;

                case RepeatMode.SINGLE:
                    RepeatControlButton.Icon = PackIconKind.RepeatOnce;
                    RepeatControlButton.Title = "Repeat";
                    if (buttonTriggered)
                        RepeatControlButton.Stage = 1;
                    break;

                case RepeatMode.ALL:
                    RepeatControlButton.Icon = PackIconKind.Repeat;
                    RepeatControlButton.Title = "All";
                    if (buttonTriggered)
                        RepeatControlButton.Stage = 2;
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set shuffle button state to state of shuffle mode. </summary>
        /// <param name="shuffleMode"> Shuffle mode state. </param>
        private void SetShuffleControlButton(bool shuffleMode)
        {
            ShuffleControlButton.Pressed = shuffleMode;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update aduio controls to state of player core and playing audio track. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void UpdateAudioControls(PlayerCore playerCore)
        {
            if (playerCore != null)
            {
                switch (playerCore.PlaybackState)
                {
                    case PlaybackState.Playing:
                        PlayPauseControlButton.Icon = PackIconKind.Pause;
                        PlayPauseControlButton.Title = "Pause";
                        break;

                    case PlaybackState.Paused:
                    case PlaybackState.Stopped:
                        PlayPauseControlButton.Icon = PackIconKind.Play;
                        PlayPauseControlButton.Title = "Play";
                        break;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set track slider position to playing audio track playing position. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void UpdateAudioPositionInfo(PlayerCore playerCore)
        {
            if (playerCore != null && !TTrackSlider.IsDragging)
                TTrackSlider.Position = playerCore.ProcentageTrackPosition();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set informations about playing audio track times. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void UpdateAudioTimeInfo(PlayerCore playerCore)
        {
            if (playerCore != null)
            {
                string current = playerCore.TrackPosition.ToString(TRACK_TIME_FORMAT);
                string length = playerCore.TrackLength.ToString(TRACK_TIME_FORMAT);

                TrackTimeTextBlock.Text = current;
                TrackMaxTimeTextBlock.Text = length;

                var osd = OSD;
                if (osd != null)
                {
                    osd.InfoTime = $"{current}/{length}";
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update displayed informations of currently played audio track. </summary>
        /// <param name="song"> Audio track file. </param>
        private void UpdateAudioTrackInformations(SongFile song)
        {
            string album = song != null && !string.IsNullOrEmpty(song.Album) ? song.Title : "Unknown Album";
            string artist = song != null && !string.IsNullOrEmpty(song.Artist) ? song.Title : "Unknown Artist";
            string title = song != null && !string.IsNullOrEmpty(song.Title) ? song.Title : "Unknown Name";

            //  Update control informations.
            InfoAlbumTextBlock.Text = album;
            InfoArtistTextBlock.Text = artist;
            InfoTitleTextBlock.Text = title;

            var osd = OSD;

            //  Update osd informations.
            if (osd != null)
            {
                osd.InfoAlbum = album;
                osd.InfoArtist = artist;
                osd.InfoTitle = title;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update displayed cover of currently played audio track. </summary>
        /// <param name="song"> Audio track file. </param>
        private void UpdateAudioTrackCover(SongFile song)
        {
            var coverExists = song?.Cover != null;

            //  Update control informations.
            CoverAlternateIcon.Visibility = coverExists ? Visibility.Collapsed : Visibility.Visible;
            CoverImage.Source = song?.Cover;

            var osd = OSD;

            //  Update osd informations.
            if (osd != null)
                osd.InfoCover = song?.Cover;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set volume slider position to volume of player core. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void UpdateAudioVolume(PlayerCore playerCore)
        {
            if (playerCore != null && !TVolumeSlider.IsDragging)
                TVolumeSlider.Value = playerCore.Volume;
        }

        #endregion INTERFACE UPDATE METHODS

        #region INTERFACE UPDATER MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and configure interface updater background task. </summary>
        private void SetupInterfaceUpdater()
        {
            if (!InterfaceUpdaterWorking)
            {
                //  Create cancellation token source for task.
                _interfaceUpdaterTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = _interfaceUpdaterTokenSource.Token;

                //  Create interface updater task.
                _interfaceUpdater = new Task(() => InterfaceUpdaterDoWork(cancellationToken));

                //  Start interface updater task.
                _enabledInterfaceUpdater = true;
                _interfaceUpdater.Start();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop interface updater background task. </summary>
        public void StopInterfaceUpdater()
        {
            _enabledInterfaceUpdater = false;

            if (InterfaceUpdaterWorking)
                _interfaceUpdaterTokenSource.Cancel();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Interface updater background task work method. </summary>
        /// <param name="cancellationToken"> Cancellation task token. </param>
        private void InterfaceUpdaterDoWork(CancellationToken cancellationToken)
        {
            var playerCore = PlayerCore.Instance;
            var visualisationOn = false;

            //  Cancel Task on startup if cancellation was raised.
            cancellationToken.ThrowIfCancellationRequested();

            //  Update interface.
            while (_enabledInterfaceUpdater)
            {
                //  Check if task was cancelled.
                if (InterfaceUpdaterCancelled)
                    break;

                if (playerCore == null)
                    break;

                //  Update interface according to player state.
                switch (playerCore.PlaybackState)
                {
                    case PlaybackState.Playing:
                        this.Dispatcher.Invoke(() =>
                        {
                            UpdateAudioPositionInfo(playerCore);
                            UpdateAudioTimeInfo(playerCore);
                        });
                        break;

                    case PlaybackState.Paused:
                    case PlaybackState.Stopped:
                        break;
                }

                //  Update visualisation.
                visualisationOn = _pagesManager.CurrentPage is HomePage;

                if (visualisationOn && VisualisationCanvas != null)
                {
                    VisualisationCanvas.Dispatcher.Invoke(() =>
                    {
                        if (_visualisation != null)
                        {
                            _visualisation.Draw();
                            _visualisation.Logo.RedrawShapes();
                        }
                    });
                }

                //  Auto manage next playlist item after finishing play.
                if (playerCore.IsFinishedPlaying)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        playerCore.Next();
                        UpdateVisualisationSpectrumProvider(playerCore);
                    });

                    var nextSong = playerCore.PlayList.SelectedItem;

                    this.Dispatcher.Invoke(() =>
                    {
                        UpdateAudioControls(playerCore);
                        UpdateAudioPositionInfo(playerCore);
                        UpdateAudioTimeInfo(playerCore);

                        if (nextSong != null)
                        {
                            UpdateAudioTrackInformations(nextSong);
                            UpdateAudioTrackCover(nextSong);
                        }
                    });
                }

                //  Take a break.
                if (!visualisationOn)
                    Thread.Sleep(_interfaceUpdaterBreak);
            }

            //  Finalize task work and cleanup all related memory.
            InterfaceUpdaterDispose();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Finalize interface updater task work and cleanup related memory. </summary>
        private void InterfaceUpdaterDispose()
        {
            //  Release and clear interface updater token source.
            if (_interfaceUpdaterTokenSource != null)
            {
                _interfaceUpdaterTokenSource.Dispose();
                _interfaceUpdaterTokenSource = null;
            }

            //  Relase and clean interface updater task.
            if (_interfaceUpdater != null)
                _interfaceUpdater = null;
        }

        #endregion INTERFACE UPDATER MANAGEMENT METHODS

        #region MAIN MENU INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selection any list view item in main menu list view. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Selection changed event arguments. </param>
        private void MainMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //  Cast sender into list view object.
            var listView = (ListView) sender;

            //  Clear list view selection.
            listView.SelectedIndex = -1;
            listView.SelectedItems.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Open main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void OpenMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Close main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CloseMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ChangeSideBarState(SideBarState.COLLAPSED);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Home main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void HomeMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (_pagesManager.CurrentPage.GetType() != typeof(HomePage))
            {
                LoadHomePage();
                _visualisation.UpdateGraphics();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Open files main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void OpenFilesMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadFilesByOpenFileDialog();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Settings main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void SettingsMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadSettingsPage();
            ChangeSideBarState(SideBarState.COLLAPSED);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting Playlist main menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void PlayListMainMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var newState = _sideBarState != SideBarState.PLAYLIST 
                ? SideBarState.PLAYLIST
                : SideBarState.COLLAPSED;

            ChangeSideBarState(newState);
        }

        #endregion MAIN MENU INTERACTION METHODS

        #region PAGES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load home page. </summary>
        private HomePage LoadHomePage()
        {
            var page = _pagesManager.GetPageByType(typeof(HomePage));

            if (page == null)
            {
                //  Create and configure home page.
                page = new HomePage();

                var homePage = page as HomePage;
                homePage.ControlPanelMargin = ControlPanel.ActualHeight;
                homePage.SideBarMargin = SideBarPanel.ActualWidth;
                homePage.FullBackground = true;

                _pagesManager.LoadPage(page);
            }
            else
            {
                _pagesManager.GoToPage(page);
            }

            page.Margin = new Thickness(0, 0, 0, 0);
            return (HomePage) page;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load settings page. </summary>
        private SettingsPage LoadSettingsPage()
        {
            var page = _pagesManager.GetPageByType(typeof(SettingsPage));

            if (page == null)
            {
                //  Create and configure settings page.
                page = new SettingsPage();

                var settingsPage = page as SettingsPage;
                settingsPage.ControlPanelMargin = ControlPanel.ActualHeight;
                settingsPage.SideBarMargin = SideBarPanel.ActualWidth;
                settingsPage.FullBackground = false;

                _pagesManager.LoadPage(page);
            }
            else
            {
                _pagesManager.GoToPage(page);
            }

            page.Margin = new Thickness(0, 0, 0, 0);
            return (SettingsPage) page;
        }

        #endregion PAGES MANAGEMENT

        #region PLAYBACK METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after starting play any song. </summary>
        /// <param name="playerCore"> Player core. </param>
        /// <param name="selectedSong"> Playing audio track. </param>
        private void OnPlayFile(PlayerCore playerCore, SongFile selectedSong)
        {
            if (selectedSong != null)
            {
                UpdateAudioTrackInformations(selectedSong);
                UpdateAudioTrackCover(selectedSong);
            }
            
            UpdateAudioControls(playerCore);
            UpdateAudioVolume(playerCore);

            if (playerCore.PlaybackState == PlaybackState.Playing)
                OSD?.Show();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after stopping track playing. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void OnStopPlaying(PlayerCore playerCore)
        {
            UpdateAudioControls(playerCore);
            UpdateAudioPositionInfo(playerCore);

            if (playerCore.PlaybackState == PlaybackState.Stopped)
                OSD?.Hide();
        }

        #endregion PLAYBACK METHODS

        #region PLAYLIST INTERACTION METHODS

        //  PLAYLIST

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after double clicking item in playlist. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void PlayListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            var playerCore = PlayerCore.Instance;

            if (listView.SelectedItem != null)
            {
                var selectedSong = playerCore.SelectFromPlayList(listView.SelectedIndex);
                UpdateVisualisationSpectrumProvider(playerCore);
                OnPlayFile(playerCore, selectedSong);
            }
        }

        //  PLAYLIST CONTROL BUTTONS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking keep playlist control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void KeepPlayListControlButton_Click(object sender, RoutedEventArgs e)
        {
            var controlButton = (ControlButton)sender;
            _keppPlayListOnTop = controlButton.Pressed;

            controlButton.Icon = controlButton.Pressed
                ? PackIconKind.VectorArrangeAbove
                : PackIconKind.VectorArrangeBelow;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist sort control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void SortPlayListControlButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (ControlButton)sender;

            button.ContextMenu.DataContext = button.DataContext;
            button.ContextMenu.IsOpen = true;
        }

        //  PLAYLIST CONTEXT MENU

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist play context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void PlayPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var playerCore = PlayerCore.Instance;

            if (PlayListView.SelectedItem != null)
            {
                var selectedSong = playerCore.SelectFromPlayList(PlayListView.SelectedIndex);
                UpdateVisualisationSpectrumProvider(playerCore);
                OnPlayFile(playerCore, selectedSong);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist load files context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void LoadFilesPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadFilesByOpenFileDialog();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist load playlist context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void LoadPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadPlayListByOpenFileDialog();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist save playlist context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void SavePlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SavePlayListBySaveFileDialog(PlayerCore.Instance.PlayList);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist sort by title asc context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void SortByTitleAscendingPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PlayerCore.Instance.PlayList.SortBy(p => p.Title, SortOrder.ASCENDING);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist sort by title desc context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void SortByTitleDescendingPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PlayerCore.Instance.PlayList.SortBy(p => p.Title, SortOrder.DESCENDING);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist remove file context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void RemoveFilePlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PlayListView.SelectedItem != null)
                PlayerCore.Instance.PlayList.RemoveAt(PlayListView.SelectedIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking playlist clear context menu item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ClearPlayListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PlayerCore.Instance.PlayList.Clear();
        }

        #endregion PLAYLIST INTERACTION METHODS

        #region PLAYLIST MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mouse button on playlist for move items inside. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void PlayListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _playListDragging = true;
                _playListDragPoint = e.GetPosition(null);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when mouse is moving over playlist for moving items inside. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void PlayListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_playListDragging)
            {
                Point point = e.GetPosition(null);
                Vector diffrence = _playListDragPoint - point;

                double minHorizontalDragDist = SystemParameters.MinimumHorizontalDragDistance;
                double minVerticalDragDist = SystemParameters.MinimumVerticalDragDistance;

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Math.Abs(diffrence.X) > minHorizontalDragDist || Math.Abs(diffrence.Y) > minVerticalDragDist)
                    {
                        var source = e.OriginalSource;
                        var selectedItem = ComponentUtils.FindVisualParent<ListViewItem>((DependencyObject) e.OriginalSource);

                        if (selectedItem != null)
                            DragDrop.DoDragDrop(selectedItem, selectedItem.DataContext, DragDropEffects.Move);
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after dropping any item to playlist for move items inside. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Drag event arguments. </param>
        private void PlayListView_Drop(object sender, DragEventArgs e)
        {
            if (_playListDragging)
            {
                //  Stop executing method if dropped items contains files.
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    return;

                SongFile source, target = null;

                if (sender is ListView)
                {
                    var listView = sender as ListView;
                    var playList = PlayerCore.Instance.PlayList;
                    var selectedItem = ComponentUtils.FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);

                    source = e.Data.GetData(playList.DataType) as SongFile;

                    if (selectedItem == null)
                    {
                        Point point = e.GetPosition(sender as UIElement);
                        if (point.X < 0 || point.X >= listView.ActualWidth || listView.Items.Count <= 0)
                            return;

                        if (point.Y > 0)
                            target = (SongFile) listView.Items[listView.Items.Count - 1];
                        else if (point.Y <= 0)
                            target = (SongFile) listView.Items[0];
                    }
                    else
                    {
                        target = selectedItem.DataContext as SongFile;
                    }

                    if (target == null)
                        return;

                    var sourceIndex = playList.IndexOf(source);
                    var targetIndex = playList.IndexOf(target);

                    playList.MoveItem(sourceIndex, targetIndex);
                }

                _playListDragging = false;
            }
        }

        #endregion PLAYLIST MANAGEMENT METHODS

        #region PLAYLIST SPLITTER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mouse button on playlist splitter. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void PlayListSplitterGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //  Enable drag functionality.
            _playListSplitterGrabbed = true;
            e.Handled = true;

            //  Lock splitter on mouse cursor.
            UIElement splitter = (UIElement) sender;
            splitter.CaptureMouse();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after releasing mouse button from playlist splitter. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void PlayListSplitterGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //  Disable drag functionality.
            _playListSplitterGrabbed = false;
            e.Handled = true;

            //  Unlock splitter from mouse cursor.
            UIElement splitter = (UIElement) sender;
            splitter.ReleaseMouseCapture();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Metod called when cursor is moving over playlist splitter. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void PlayListSplitterGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_playListSplitterGrabbed)
            {
                //  Calculate new width of playlist.
                double newWidth = e.GetPosition(this).X - (sender as Grid).ActualWidth / 2;

                //  Set playlist width.
                SetPlayListWidth(newWidth);
            }
        }

        #endregion PLAYLIST SPLITTER INTERACTION METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup system listener handling methods. </summary>
        private void SetupSystemListener()
        {
            var config = ConfigManager.Instance.Config;
            var systemListener = SystemListener.Instance;

            systemListener.UserPreferenceChangedHandler += (s, e) =>
            {
                if (config.UseSystemColor)
                    UpdateSystemThemeConfiguration(config.VisualisationColorMode == VisualisationColorMode.APPLICATION);
            };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup initial window position and size. </summary>
        private void SetupWindowPositionSize()
        {
            var screen = WindowUtils.GetScreenWhereIsWindow(this);

            if (screen != null)
                WindowUtils.AdjustWindowToScreen(this, screen);
            else
                WindowUtils.AdjustWindowToPrimaryScreen(this);
        }

        #endregion SETUP METHODS

        #region VISUALISATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        private string GetLogoShapeJson()
        {
            string logoShapeJson = string.Empty;
            byte[] baseLogo = Properties.Resources.BaseLogo;

            try
            {
                using (var ms = new MemoryStream(baseLogo))
                {
                    using (var sr = new StreamReader(ms))
                    {
                        logoShapeJson = sr.ReadToEnd();
                    }    
                }
            }
            catch
            {
                logoShapeJson = string.Empty;
            }

            return logoShapeJson;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup visualisation. </summary>
        /// <param name="visualisationType"> Visualisation type. </param>
        private void SetupVisualisation(VisualisationType visualisationType)
        {
            var homePage = _pagesManager.GetPageByType(typeof(HomePage));
            var canvas = (homePage as HomePage).Canvas;
            var config = ConfigManager.Instance.Config;
            var playerCore = PlayerCore.Instance;
            IVisualisation visualisation = null;

            switch (visualisationType)
            {
                case VisualisationType.StripesVisualisation:
                    visualisation = VisualisationManager.CreateVisualisation(
                        VisualisationType.StripesVisualisation, canvas, playerCore.SpectrumProvider);

                    var stripesVis = (StripesVisualisation) visualisation;
                    stripesVis.FillColor = _themeColor;
                    stripesVis.Margin = new Thickness(48, 0, 0, 88);
                    break;
            }

            string logoShapeJson = GetLogoShapeJson();

            if (visualisation != null)
            {
                visualisation.Enabled = config.VisualisationEnabled;
                visualisation.Logo.SetupShape(logoShapeJson, false);
                _visualisation = visualisation;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation data by updating spectrum provider. </summary>
        /// <param name="playerCore"> Player core. </param>
        private void UpdateVisualisationSpectrumProvider(PlayerCore playerCore)
        {
            _visualisation.SetSpectrumProvider(playerCore.SpectrumProvider);
            _visualisation.UpdateGraphics();
        }

        #endregion VISUALISATION MANAGEMENT METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called when window is closing. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Cancel event arguments. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //  Stop interface updater.
            StopInterfaceUpdater();

            //  Save current playlist if configuration option is enabled.
            SavePlayListAtShutdown();

            //  Save configuration.
            SaveConfiguration();

            //  Dispose PlayerCore.
            PlayerCore.Instance.Dispose();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after dropping any item to window. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Drag event arguments. </param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            //  Check if dropped items contains files.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //  Get dropped files.
                string[] filesPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                //  Load files.
                LoadFiles(filesPaths);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading window. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  Clear control informations data.
            InfoAlbumTextBlock.Text = "";
            InfoArtistTextBlock.Text = "";
            InfoTitleTextBlock.Text = "";

            //  Load main home page.
            LoadHomePage();

            //  Setup exception handler.
            SetupExceptionHandler();

            //  Start interface updater.
            SetupInterfaceUpdater();

            //  Load configuration.
            ConfigManager.Instance.OnConfigUpdate += OnConfigurationUpdate;
            LoadConfiguration();
            SetupWindowPositionSize();

            //  Configure system handlers.
            SetupSystemListener();

            //  Handle exceptions.
            ExceptionsHandler.Instance.HandleStack();

            //  Load previous playlist if option enable in configuration.
            LoadPlayListAtStartup();

            //  Update initialization flag.
            _initialized = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after size of window is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Size changed event arguments. </param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //  Update playlist width if shown.
            if (_sideBarState == SideBarState.PLAYLIST)
                SetPlayListWidth(PlayListGrid.ActualWidth);

            //  Update visualisation if shown.
            if (_pagesManager.CurrentPage?.GetType() == typeof(HomePage) && _visualisation != null && _visualisation.Enabled)
                _visualisation.UpdateGraphics();
        }

        #endregion WINDOW METHODS

    }
}
