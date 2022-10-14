using chkam05.Tools.ControlsEx;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Core;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls
{
    public partial class SideBarPlayList : UserControl, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<RoutedEventArgs> OnBackClick;
        public event EventHandler<RoutedEventArgs> OnOptionsClick;


        //  VARIABLES

        private bool _holdOnContextMenuCloseMethodExecution = false;
        private SideBarMenu _parentControl;
        private IPlayable _selectedItem;

        public ConfigManager ConfigManager { get; private set; }
        public Player Player { get; private set; }


        //  GETTERS & SETTERS

        public IPlayable SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SideBarPlayList class constructor. </summary>
        public SideBarPlayList()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            Player = Player.Instnace;

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Option ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            OnOptionsClick?.Invoke(this, e);
        }

        #endregion CONTROL BUTTONS METHODS

        #region GLOBAL CONTEXTMENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after opening ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ExtendedContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if (_holdOnContextMenuCloseMethodExecution)
                return;

            LockVisibility(false);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Option Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void OptionButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                return;

            LockVisibility(true);

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
        /// <summary> Method invoked after clicking on PlayList. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PlayListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                LockVisibility(true);
                return;
            }
        }

        #endregion GLOBAL CONTEXTMENU METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Lock PlayList and Sidebar hiding when ContextMenu is open. </summary>
        /// <param name="isLocked"> Lock hiding state. </param>
        private void LockVisibility(bool isLocked)
        {
            _parentControl.LockPlayListVisibility = isLocked;

            if (!isLocked && _parentControl.AutoHide)
                _parentControl.HideInterface();

            else if (!isLocked && _parentControl.AutoHidePlayList)
                _parentControl.HideExpandedControls();
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region MORE CONTEXTMENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Play MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemPlay_Click(object sender, RoutedEventArgs e)
        {
            var item = Player.PlayList.Select(0);

            if (item != null)
                Player.Play(item);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Open Playlist MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemOpenPlayList_Click(object sender, RoutedEventArgs e)
        {
            _holdOnContextMenuCloseMethodExecution = true;

            LockVisibility(true);

            var filePaths = FilesManager.Instance.DialogOpenFiles(
                FileGroup.PLAYLIST, "Load playlist from file", false);

            if (filePaths != null && filePaths.Any())
                Player.PlayList.LoadFromFile(filePaths[0]);

            LockVisibility(false);

            _holdOnContextMenuCloseMethodExecution = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Save Playlist MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemSavePlayList_Click(object sender, RoutedEventArgs e)
        {
            _holdOnContextMenuCloseMethodExecution = true;

            LockVisibility(true);

            var filePath = FilesManager.Instance.DialogGetSaveFilePath(
                FileGroup.PLAYLIST, "Save playlist to file", "NowPlaying");

            if (!string.IsNullOrEmpty(filePath))
                Player.PlayList.SaveToFile(filePath);

            LockVisibility(false);

            _holdOnContextMenuCloseMethodExecution = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Add item MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemAddItem_Click(object sender, RoutedEventArgs e)
        {
            _holdOnContextMenuCloseMethodExecution = true;

            var mainWindow = (MainWindow) Application.Current.MainWindow;

            mainWindow.OpenFilesByDialog();

            LockVisibility(false);

            _holdOnContextMenuCloseMethodExecution = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Sort by title ascending MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemSortByTitleAsc_Click(object sender, RoutedEventArgs e)
        {
            var playList = Player.PlayList;

            playList.DataContext = new ObservableCollection<IPlayable>(
                playList.DataContext.OrderBy(o => o.Title));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Sort by title descending MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemSortByTitleDesc_Click(object sender, RoutedEventArgs e)
        {
            var playList = Player.PlayList;

            playList.DataContext = new ObservableCollection<IPlayable>(
                playList.DataContext.OrderByDescending(o => o.Title));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Clear Playlist MenuItem in more ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MoreExtendedContextMenuItemClearPlayList_Click(object sender, RoutedEventArgs e)
        {
            Player.PlayList.Clear();
        }

        #endregion MODE CONTEXTMENU METHODS

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

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on PlayList item and grabbing it. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PlayList_ExtendedListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ExtendedListViewItem && e.ClickCount == 1)
            {
                ExtendedListViewItem draggedItem = sender as ExtendedListViewItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after dropping item on another PlayList item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Drag Event Arguments. </param>
        private void PlayList_ExtendedListViewItem_Drop(object sender, DragEventArgs e)
        {
            string[] dataFormats = e.Data.GetFormats();

            if (dataFormats != null && dataFormats.Any())
            {
                IPlayable droppedData = e.Data.GetData(dataFormats[0]) as IPlayable;

                if (droppedData == null)
                    return;

                IPlayable targetData = ((ExtendedListViewItem)(sender)).DataContext as IPlayable;

                int removedIndex = Player.PlayList.IndexOf(droppedData);
                int targetIndex = Player.PlayList.IndexOf(targetData);

                if (removedIndex < targetIndex)
                {
                    Player.PlayList.Insert(targetIndex + 1, droppedData);
                    Player.PlayList.RemoveAt(removedIndex);
                }
                else
                {
                    var incRemovedIndex = removedIndex + 1;

                    if (Player.PlayList.Count + 1 > incRemovedIndex)
                    {
                        Player.PlayList.Insert(targetIndex, droppedData);
                        Player.PlayList.RemoveAt(incRemovedIndex);
                    }
                }
            }
        }

        #endregion PLAYLIST METHODS

        #region PLAYLIST ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on playlist item. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void PlayListItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListViewItem)sender).Content as IPlayable;

            if (item != null)
                Player.Play(item);
        }

        #endregion PLAYLIST ITEMS METHODS

        #region PLAYLIST ITEMS CONTEXTMENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Play MenuItem in PlayListItem ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void PlayListItem_ExtendedContextMenuItemPlay_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && Player.PlayList.Select(SelectedItem))
                Player.Play(SelectedItem);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Remove MenuItem in PlayListItem ContextMenu. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void PlayListItem_ExtendedContextMenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
                Player.PlayList.Remove(SelectedItem);
        }

        #endregion PLAYLIST ITEMS CONTEXTMENU METHODS

        #region USERCONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading UserControl interface. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  Setup initial data.
            _parentControl = ControlsUtilities.FindParent<SideBarMenu>(this.Parent as FrameworkElement);
        }

        #endregion USERCONTROL METHODS

    }
}
