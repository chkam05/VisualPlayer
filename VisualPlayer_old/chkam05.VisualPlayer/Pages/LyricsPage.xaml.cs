using chkam05.Tools.ControlsEx;
using chkam05.Tools.ControlsEx.Data;
using chkam05.Tools.ControlsEx.Events;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Core;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lyrics;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Windows;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace chkam05.VisualPlayer.Pages
{
    public partial class LyricsPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private TimeSpan _checkPoint = new TimeSpan(0);
        private TimeSpan _currentPosition = new TimeSpan(0);
        private List<string> _lyricsFilesListCache;
        private ObservableCollection<string> _lyricsFilesList;
        private bool _lyricsFilesListVisible = false;
        private Thickness _lyricsFilesListMargin;
        private Lyrics _selectedLyrics = null;

        public ConfigManager ConfigManager { get; private set; }
        public LyricsManager LyricsManager { get; private set; }
        public Player Player { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public TimeSpan CheckPoint
        {
            get => _checkPoint;
            set
            {
                _checkPoint = value;
                OnPropertyChanged(nameof(CheckPoint));
            }
        }

        public TimeSpan CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                OnPropertyChanged(nameof(CurrentPosition));
            }
        }

        public ObservableCollection<string> LyricsFilesList
        {
            get => _lyricsFilesList;
            set
            {
                _lyricsFilesList = value;
                OnPropertyChanged(nameof(LyricsFilesList));
            }
        }

        public Thickness LyricsFilesListMargin
        {
            get => _lyricsFilesListMargin;
            set
            {
                _lyricsFilesListMargin = value;
                OnPropertyChanged(nameof(LyricsFilesListMargin));
            }
        }

        public bool LyricsFilesListVisible
        {
            get => _lyricsFilesListVisible;
            set
            {
                _lyricsFilesListVisible = value;
                OnPropertyChanged(nameof(LyricsFilesListMargin));
            }
        }

        public Lyrics SelectedLyrics
        {
            get => _selectedLyrics;
            set
            {
                _selectedLyrics = value;
                OnPropertyChanged(nameof(SelectedLyrics));
            }
        }

        public MenuItemType? SpecialMenu
        {
            get => null;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public LyricsPage(IPagesManager pagesManager)
        {
            //  Setup data containers.
            SetupDataContainers();

            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            LyricsManager = LyricsManager.Instance;
            Player = Player.Instnace;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show VolumeControl interface. </summary>
        private void AnimateShowLyricsFiles()
        {
            if (!LyricsFilesListVisible)
            {
                LyricsFilesListMargin = new Thickness(0);
                Storyboard storyboard = Resources["ShowHideLyricsFilesStoryboard"] as Storyboard;
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide VolumeControl interface. </summary>
        private void AnimateHideLyricsFiles()
        {
            if (LyricsFilesListVisible)
            {
                LyricsFilesListMargin = CalculateLyricsFilesListHiddenMargin();
                Storyboard storyboard = Resources["ShowHideLyricsFilesStoryboard"] as Storyboard;
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after completing ExpandingStoryboard animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            LyricsFilesListVisible = LyricsFilesListMargin.Right >= 0.0;
        }

        #endregion ANIMATION METHODS

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

        #endregion CONTROL BUTTONS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over title grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        #endregion INTERACTION METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create empty lyrics. </summary>
        private void CreateNewLyrics()
        {
            LyricsManager.CreateEmpty();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load lyrics from library or leave empty. </summary>
        private void LoadFromLibrary()
        {
            if (!LyricsManager.LoadFromLibrary())
            {
                var window = (MainWindow)Application.Current.MainWindow;

                window.MessagesManager.CreateAlertMessageBox(
                    "Opening lyrics from local library",
                    "Lyrics have not been found in local library. \nCreated empty space for new lyrics.");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load lyrics from selected file. </summary>
        private void LoadFromFile()
        {
            var filePaths = FilesManager.Instance.DialogOpenFiles(
                FileGroup.LYRICS, "Open lyrics file", false);

            if (filePaths != null && filePaths.Any())
                LyricsManager.LoadFromFile(filePaths[0]);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save lyrics to library. </summary>
        private void SaveToLibrary()
        {
            LyricsManager.SaveToLibrary();
            LoadLibraryLyricsToCache();
            SearchLyricsFile(lyricsSearchExtendedTextBox.Text);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save lyrics to file. </summary>
        private void SaveToFile()
        {
            var filePath = FilesManager.Instance.DialogGetSaveFilePath(
                FileGroup.LYRICS, "Save lyrics to file", "Lyrics");

            if (!string.IsNullOrEmpty(filePath))
                LyricsManager.SaveToFile(filePath);
        }

        #endregion LOAD & SAVE METHODS

        #region LYRICS CONTEXT MENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Add Lyrics LyricsView ExtendedContextMenuItem.</summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Lyrics_AddExtendedContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var lyrics = lyricsListView.SelectedItem as Lyrics;

            if (lyrics == null)
            {
                LyricsManager.Add(new Lyrics(CheckPoint, CheckPoint, string.Empty));
                return;
            }

            int index = LyricsManager.IndexOf(lyrics);
            var startTime = LyricsManager.GetPreviousLyrics(lyrics)?.EndTime ?? new TimeSpan(0);
            var endTime = lyrics.StartTime;

            LyricsManager.Insert(index, new Lyrics(startTime, endTime, string.Empty));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Remove Lyrics LyricsView ExtendedContextMenuItem.</summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Lyrics_RemoveExtendedContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedLyrics = lyricsListView.SelectedItem as Lyrics;

            if (selectedLyrics != null)
                LyricsManager.Remove(selectedLyrics);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Clear LyricsView ExtendedContextMenuItem.</summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Lyrics_ClearExtendedContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            var message = window.MessagesManager.CreateQuestionMessageBox(
                "Cleaning up lyrics",
                "Do You want to remove all created lyrics texts?",
                (s, me) => { if (me.Result == InternalMessageResult.Yes) LyricsManager.Clear(); });
        }

        #endregion LYRICS CONTEXT MENU METHODS

        #region LYRICS ITEMS BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking update start time pack icon button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event arguments. </param>
        private void StartTimeUpdate_PackIconButton_Click(object sender, RoutedEventArgs e)
        {
            var lyrics = (e.Source as ButtonEx)?.DataContext as Lyrics;

            if (lyrics != null && Player.LoadedItem == LyricsManager.LoadedFile)
                lyrics.StartTime = Player.TrackPosition;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking update end time pack icon button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event arguments. </param>
        private void EndTimeUpdate_PackIconButton_Click(object sender, RoutedEventArgs e)
        {
            var lyrics = (e.Source as ButtonEx)?.DataContext as Lyrics;

            if (lyrics != null && Player.LoadedItem == LyricsManager.LoadedFile)
                lyrics.EndTime = Player.TrackPosition;
        }

        #endregion LYRICS ITEMS BUTTONS METHODS

        #region LYRICS TOOLBAR BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking File ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void FileButtonEx_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                ButtonEx button = sender as ButtonEx;
                ContextMenuEx contextMenu = button.ContextMenu as ContextMenuEx;
                contextMenu.PlacementTarget = button;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                contextMenu.IsOpen = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Create Lyrics ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CreateLyricsContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            if (!LyricsManager.LyricsChanged)
            {
                CreateNewLyrics();
                return;
            }

            var window = (MainWindow)Application.Current.MainWindow;
            var message = window.MessagesManager.CreateQuestionMessageBox(
                "Creating new lyrics",
                "Do You want to abandon the previous changes and create new lyrics?",
                (s, me) => { if (me.Result == InternalMessageResult.Yes) CreateNewLyrics(); });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Open Lyrics ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OpenLyricsContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            if (!LyricsManager.LyricsChanged)
            {
                LoadFromFile();
                return;
            }

            var window = (MainWindow)Application.Current.MainWindow;

            window.MessagesManager.CreateQuestionMessageBox(
                "Opening lyrics from file",
                "Do You want to abandon the previous changes and open lyrics from file?",
                (s, me) => { if (me.Result == InternalMessageResult.Yes) LoadFromFile(); });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Save As Lyrics ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SaveAsLyricsContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Open from Library Lyrics ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OpenFromLibLyricsContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            if (!LyricsManager.LyricsChanged)
            {
                LoadFromLibrary();
                return;
            }

            var window = (MainWindow)Application.Current.MainWindow;

            window.MessagesManager.CreateQuestionMessageBox(
                "Opening lyrics from local library",
                "Do You want to abandon the previous changes and open lyrics from library?",
                (s, me) => { if (me.Result == InternalMessageResult.Yes) LoadFromLibrary(); });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Save to Library Lyrics ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SaveToLibLyricsContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            if (!LyricsManager.ExistsInLibrary())
            {
                SaveToLibrary();
                return;
            }

            var window = (MainWindow)Application.Current.MainWindow;

            window.MessagesManager.CreateQuestionMessageBox(
                "Saving lyrics to local library",
                "Lyrics for this file exists in library. Do You want to overwrite them?",
                (s, me) => { if (me.Result == InternalMessageResult.Yes) SaveToLibrary(); });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Sort ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SortButtonEx_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                ButtonEx button = sender as ButtonEx;
                ContextMenuEx contextMenu = button.ContextMenu as ContextMenuEx;
                contextMenu.PlacementTarget = button;
                contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                contextMenu.IsOpen = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Sort Lyrics Texts by Time ContextMenuItem. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SortLyricsTextsByTimeContextMenuItemEx_Click(object sender, RoutedEventArgs e)
        {
            LyricsManager.ArrangeLyricsByTime();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Local Lyrics ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void LocalLyrics_PackIconButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LyricsFilesListVisible)
                AnimateShowLyricsFiles();
            else
                AnimateHideLyricsFiles();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Add Lyrics ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddLyricsButton_Click(object sender, RoutedEventArgs e)
        {
            LyricsManager.Add(new Lyrics(CheckPoint, CheckPoint, string.Empty));
        }

        #endregion LYRICS TOOLBAR BUTTONS METHODS

        #region LYRICS FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after editing lyrics files search ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Extended Text Box Content Updated Event arguments. </param>
        private void LyricsFilesSearch_TextModified(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified && !string.IsNullOrEmpty(e.NewText))
                SearchLyricsFile(e.NewText);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load lyrics from library to cache. </summary>
        private void LoadLibraryLyricsToCache()
        {
            var filter = FilesManager.GetFileTypesByGroup(FileGroup.LYRICS);
            var lyricsStoragePath = FilesManager.Instance.LyricsStoragePath;

            _lyricsFilesListCache = FilesManager.GetFilesList(lyricsStoragePath, filter, false)
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Find text by search phrase. </summary>
        /// <param name="searchPhrase"> Search phrase. </param>
        private void SearchLyricsFile(string searchPhrase)
        {
            LyricsFilesList = new ObservableCollection<string>(
                _lyricsFilesListCache.Where(t => string.IsNullOrEmpty(searchPhrase) || t.Contains(searchPhrase)));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Remove Lyrics File LyricsFilesView ExtendedContextMenuItem.</summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void LyricsFile_RemoveExtendedContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var lyricsFileName = lyricsFilesView.SelectedItem as string;

            if (!string.IsNullOrEmpty(lyricsFileName))
            {
                var filter = FilesManager.GetFileTypesByGroup(FileGroup.PLAYLIST);
                var lyricsStoragePath = FilesManager.Instance.LyricsStoragePath;
                var lyricsFiles = FilesManager.GetFilesList(lyricsStoragePath, filter, false)
                    .Where(f => f.Contains(lyricsFileName));

                if (lyricsFiles != null && lyricsFiles.Count() == 1)
                {
                    var lyricsFilePath = lyricsFiles.First();

                    if (File.Exists(lyricsFilePath))
                    {
                        var window = (MainWindow)Application.Current.MainWindow;
                        var message = window.MessagesManager.CreateQuestionMessageBox(
                            "Removing lyrics",
                            "Do You want to remove selected lyrics from library?",
                            (s, me) => {
                                if (me.Result == InternalMessageResult.Yes)
                                {
                                    File.Delete(lyricsFilePath);
                                    LoadLibraryLyricsToCache();
                                    SearchLyricsFile(lyricsSearchExtendedTextBox.Text);
                                }
                            });
                    }
                }
            }
        }

        #endregion LYRICS FILES MANAGEMENT METHODS

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
        /// <summary> Method invoked after loading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LyricsManager.EditMode = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after closing page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            LyricsManager.EditMode = false;
        }

        #endregion PAGE METHODS

        #region PLAYER CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Rewind 500 ms. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void FastRewindBackButton_Click(object sender, RoutedEventArgs e)
        {
            var position = Player.TrackPosition.TotalMilliseconds;
            var newPosition = TimeSpan.FromMilliseconds(Math.Max(0, position - 100));
            Player.TrackPosition = newPosition;
            CurrentPosition = newPosition;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Rewind 100 ms. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RewindBackButton_Click(object sender, RoutedEventArgs e)
        {
            var position = Player.TrackPosition.TotalMilliseconds;
            var newPosition = TimeSpan.FromMilliseconds(Math.Max(0, position - 10));
            Player.TrackPosition = newPosition;
            CurrentPosition = newPosition;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop playing and back to checkpoint. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void StopBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.PlaybackState == PlaybackState.Playing)
                Player.Pause();

            var maxLength = Player.TrackLength.TotalMilliseconds;
            var newPosition = TimeSpan.FromMilliseconds(Math.Max(Math.Min(CheckPoint.TotalMilliseconds, maxLength), 0));
            Player.TrackPosition = newPosition;

            if (newPosition.TotalMilliseconds != newPosition.TotalMilliseconds)
            {
                CheckPoint = newPosition;
                CurrentPosition = newPosition;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set checkpoint and start playing. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CheckPointPlayButton_Click(object sender, RoutedEventArgs e)
        {
            CheckPoint = Player.TrackPosition;
            Player.Play();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Forward 100 ms. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            var maxLength = Player.TrackLength.TotalMilliseconds;
            var position = Player.TrackPosition.TotalMilliseconds;
            var newPosition = TimeSpan.FromMilliseconds(Math.Min(position + 10, maxLength));
            Player.TrackPosition = newPosition;
            CurrentPosition = newPosition;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Forward 500 ms. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void FastForwardButton_Click(object sender, RoutedEventArgs e)
        {
            var maxLength = Player.TrackLength.TotalMilliseconds;
            var position = Player.TrackPosition.TotalMilliseconds;
            var newPosition = TimeSpan.FromMilliseconds(Math.Min(position + 100, maxLength));
            Player.TrackPosition = newPosition;
            CurrentPosition = newPosition;
        }

        #endregion PLAYER CONTROL BUTTONS METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            LoadLibraryLyricsToCache();
            LyricsFilesList = new ObservableCollection<string>(_lyricsFilesListCache);
        }

        #endregion SETUP METHODS

        #region TEXTBOXES TEXT UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating start time value in ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Extended Text Box Content Updated Event Arguments. </param>
        private void StartTime_ExtendedTextBox_OnContentUpdated(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
                LyricsManager.LyricsChanged = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating end time value in ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Extended Text Box Content Updated Event Arguments. </param>
        private void EndTime_ExtendedTextBox_OnContentUpdated(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
                LyricsManager.LyricsChanged = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after updating lyrics text value in ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Extended Text Box Content Updated Event Arguments. </param>
        private void Text_ExtendedTextBox_OnContentUpdated(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
                LyricsManager.LyricsChanged = true;
        }

        #endregion TEXTBOXED TEXT UPDATE METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for updating interface with heavy player data. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event arguments. </param>
        public void InterfaceStateUpdate(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    CurrentPosition = Player.TrackPosition;
                }
                catch (Exception)
                {
                    //
                }
            });
        }

        #endregion UPDATE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate margin for hide lyrics files list component. </summary>
        /// <returns> Margin of hidden component. </returns>
        private Thickness CalculateLyricsFilesListHiddenMargin()
        {
            double marginRight = -LyricsFilesGrid.ActualWidth;
            return new Thickness(0, 0, marginRight, 0);
        }

        #endregion UTILITY METHODS

    }
}
