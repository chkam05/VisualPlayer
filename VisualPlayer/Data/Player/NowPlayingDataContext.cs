using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualPlayer.Commands;
using VisualPlayer.Data.Base;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Data.Player
{
    public class NowPlayingDataContext : BaseViewModel
    {

        //  COMMANDS

        public ICommand PlayItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand PlayFromBeginningCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand AddSongsCommand { get; set; }
        public ICommand OpenPlayListCommand { get; set; }
        public ICommand SavePlayListCommand { get; set; }
        public ICommand SortCommand { get; set; }


        //  DELEGATES

        public delegate void NowPlayingExceptionThrownEventHandler(object sender, ExceptionThrownEventArgs e);


        //  EVENTS

        public NowPlayingExceptionThrownEventHandler NowPlayingExceptionThrown;


        //  VARIABLES

        private ObservableCollection<NowPlayingItem> _items;
        private NowPlayingItem _selectedItem;


        //  GETTERS & SETTERS

        public ObservableCollection<NowPlayingItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                _items.CollectionChanged += NotifyItemsCollectionChanged;
                NotifyPropertyChanged(nameof(Items));
            }
        }

        public NowPlayingItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                MarkItemSelection(_selectedItem, false);

                if (value == null || !Items.Contains(value))
                {
                    _selectedItem = null;
                    return;
                }

                if (!ValidateFile(value.FilePath, out string errorMessage))
                {
                    InvokeNowPlayingExceptionThrown(errorMessage);
                    return;
                }

                MarkItemSelection(value, true);
                _selectedItem = value;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NowPlayingDataContext class constructor. </summary>
        public NowPlayingDataContext()
        {
            //  Setup data containers.
            Items = new ObservableCollection<NowPlayingItem>();

            //  Setup commands.
            PlayItemCommand = new RelayCommand(OnPlayItemCommandExecuted);
            RemoveItemCommand = new RelayCommand(OnRemoveItemCommandExecuted);
            PlayFromBeginningCommand = new RelayCommand(OnPlayFromBeginningCommandExecuted);
            ClearCommand = new RelayCommand(OnClearCommandExecuted);
            AddSongsCommand = new RelayCommand(OnAddSongsCommandExecuted);
            OpenPlayListCommand = new RelayCommand(OnOpenPlayListCommandExecuted);
            SavePlayListCommand = new RelayCommand(OnSavePlayListCommandExecuted);
            SortCommand = new RelayCommand(OnSavePlayListCommandExecuted);
        }

        #endregion CLASS METHODS

        #region CONTROL COMMANDS METHODS

        //  --------------------------------------------------------------------------------
        private void OnPlayItemCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnRemoveItemCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnPlayFromBeginningCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnClearCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnAddSongsCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnOpenPlayListCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnSavePlayListCommandExecuted(object sender)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        private void OnSortCommandExecuted(object sender)
        {
            //
        }

        #endregion CONTROL COMMANDS METHODS

        #region ITEMS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get async files loader. </summary>
        /// <returns> Background worker as async files loader. </returns>
        public BackgroundWorker GetAsyncFilesLoader()
        {
            var bgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };

            bgWorker.DoWork += (s, e) =>
            {
                var self = (BackgroundWorker)s;
                var filesPaths = e.Argument as IEnumerable<string>;

                if (filesPaths == null || !filesPaths.Any())
                    return;

                int filesCount = filesPaths.Count();
                int fileIndex = 0;

                foreach (var filePath in filesPaths)
                {
                    if (self.CancellationPending || e.Cancel)
                        return;

                    var percentProgress = (int)((double)fileIndex / filesCount * 100);

                    if (!ValidateFile(filePath, out string errorMessage))
                    {
                        self.ReportProgress(percentProgress, new object[] { false, null, errorMessage });
                        continue;
                    }
                    else
                    {

                    }

                    self.ReportProgress(percentProgress, new object[] { true, filePath, null });
                }
            };

            bgWorker.ProgressChanged += (s, e) =>
            {
                var userState = (object[])e.UserState;
                var validationResult = (bool)userState[0];
                var filePath = (string)userState[1];
                var errorMessage = (string)userState[2];

                if (validationResult)
                    Items.Add(new NowPlayingItem(filePath));
                else
                    InvokeNowPlayingExceptionThrown(errorMessage);
            };

            return bgWorker;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load file. </summary>
        /// <param name="filePath"> File path. </param>
        public void LoadFile(string filePath)
        {
            if (!ValidateFile(filePath, out string errorMessage))
            {
                InvokeNowPlayingExceptionThrown(errorMessage);
                return;
            }

            Items.Add(new NowPlayingItem(filePath));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load files. </summary>
        /// <param name="filesPaths"> Files paths. </param>
        public void LoadFiles(IEnumerable<string> filesPaths)
        {
            if (filesPaths == null || !filesPaths.Any())
                return;

            foreach (var filePath in filesPaths)
                LoadFile(filePath);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Move an item to a position in list at a sepcific index. </summary>
        /// <param name="item"> Item to move. </param>
        /// <param name="targetIndex"> Target index. </param>
        public void MoveItem(NowPlayingItem item, int targetIndex)
        {
            int currentIndex = Items.IndexOf(item);

            if (currentIndex != -1 && currentIndex != targetIndex)
            {
                Items.RemoveAt(currentIndex);
                Items.Insert(targetIndex > currentIndex ? Math.Min(targetIndex, Items.Count) : Math.Max(0, targetIndex), item);
            }
        }

        #endregion ITEMS MANAGEMENT METHODS

        #region METADATA SUPPORT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mark item selection. </summary>
        /// <param name="item"> Now playing item. </param>
        /// <param name="isSelected"> Is selected. </param>
        private void MarkItemSelection(NowPlayingItem item, bool isSelected)
        {
            if (item != null)
                item.IsPlaying = isSelected;
        }

        #endregion METADATA SUPPORT METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Notify items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Items));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke now playing exception thrown. </summary>
        /// <param name="errorMessage"> Error message. </param>
        private void InvokeNowPlayingExceptionThrown(string errorMessage)
        {
            var contextName = this.GetType().Name;

            NowPlayingExceptionThrown?.Invoke(this, new ExceptionThrownEventArgs(contextName, errorMessage));
        }

        #endregion UTILITY METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate file. </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="errorMessage"> Error message. </param>
        /// <returns> True - file is valid; False - otherwise. </returns>
        private bool ValidateFile(string filePath, out string errorMessage)
        {
            try
            {
                if (!SystemHelper.IsFile(filePath))
                    throw new FileNotFoundException($"File \"{filePath}\" not found.");

                if (!SystemHelper.IsFileTypeOf(filePath, BaseFilesExtensions.SupportedFiles.Extensions))
                    throw new FileNotFoundException($"File type \"{filePath}\" is not supported.");

                errorMessage = null;
                return true;
            }
            catch (Exception exc)
            {
                errorMessage = exc.Message;
                return false;
            }
        }

        #endregion VALIDATION METHODS

    }
}
