using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Data.Player;
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages.Enums;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.InternalMessages
{
    public partial class IMFilesSelector : IMBase
    {

        //  CONST

        public const int ITEMS_SIZE_MAX = 6;
        public const int ITEMS_SIZE_MIN = 0;


        //  VARIABLES

        private ObservableCollection<FileExtension> _filesExtensions;
        private FileExtension _fileExtension = FileExtension.GetDefaultFileExtension();
        private int _itemsSize = 4;
        private bool _lockSelectedFileName = false;
        private string _selectedFileName;
        private ObservableCollection<FileItem> _selectedItems;

        private string _backupPath = null;
        private string _path = null;
        private string _searchPhrase = null;
        private bool _multiSelect = true;
        private bool _saveMode = false;
        private bool _showHiddenFiles = false;
        private bool _showSystemFiles = false;

        private bool _fileNameFocused = false;
        private bool _pathFocused = false;
        private bool _searchFocused = false;

        private List<string> _forwardHistory;
        private bool _keepForwardHistory;
        private bool _lockForwardHistory;


        //  GETTERS & SETTERS

        public bool CanNavigateForward
        {
            get => _forwardHistory.Any();
        }

        public ObservableCollection<FileExtension> FilesExtensions
        {
            get => _filesExtensions;
            set
            {
                UpdateProperty(ref _filesExtensions, value);
                _filesExtensions.CollectionChanged += FilesExtensionsCollectionChanged;
            }
        }

        public FileExtension FileExtension
        {
            get => _fileExtension;
            set => UpdateProperty(ref _fileExtension, value);
        }

        public int ItemsSize
        {
            get => _itemsSize;
            set => UpdateProperty(ref _itemsSize,
                Math.Max(ITEMS_SIZE_MIN, Math.Min(ITEMS_SIZE_MAX, value)));
        }

        public string SelectedFileName
        {
            get => _selectedFileName;
            set
            {
                UpdateProperty(ref _selectedFileName, value);
            }
        }

        public ObservableCollection<FileItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                UpdateProperty(ref _selectedItems, value);
                _selectedItems.CollectionChanged += SelectedItemsCollectionChanged;
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (_pathFocused)
                    return;

                string path;

                if (string.IsNullOrEmpty(value))
                    path = null;

                else if (SystemHelper.IsPathAvailable(value))
                    path = value;

                else if (SystemHelper.IsPathAvailable(_backupPath))
                    path = _backupPath;

                else
                    path = GetDefaultPath();

                UpdateForwardHistory();

                _backupPath = path;
                UpdateProperty(ref _path, path);
            }
        }

        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                if (_searchFocused)
                    return;

                UpdateProperty(ref _searchPhrase, value);
            }
        }

        public bool MultiSelect
        {
            get => _multiSelect && !_saveMode;
            set => UpdateProperty(ref _multiSelect, value);
        }

        public bool SaveMode
        {
            get => _saveMode;
            set
            {
                UpdateProperty(ref _saveMode, value);
                UpdateProperty(ref _multiSelect, value);
            }
        }

        public bool ShowHiddenFiles
        {
            get => _showHiddenFiles;
            set => UpdateProperty(ref _showHiddenFiles, value);
        }

        public bool ShowSystemFiles
        {
            get => _showSystemFiles;
            set => UpdateProperty(ref _showSystemFiles, value);
        }

        public bool FileNameFocused
        {
            get => _fileNameFocused;
            set => UpdateProperty(ref _fileNameFocused, value);
        }

        public bool PathFocused
        {
            get => _pathFocused;
            set => UpdateProperty(ref _pathFocused, value);
        }

        public bool SearchFocused
        {
            get => _searchFocused;
            set => UpdateProperty(ref _searchFocused, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMFilesSelector class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        /// <param name="saveMode"> Save mode. </param>
        public IMFilesSelector(IIMControl imControl, bool saveMode = false) : base(imControl)
        {
            //  Setup data.
            _forwardHistory = new List<string>();

            FilesExtensions = new ObservableCollection<FileExtension>(new List<FileExtension>()
            {
                BaseFilesExtensions.Default,
                BaseFilesExtensions.SupportedFiles,
            });

            FileExtension = FilesExtensions[1];

            if (string.IsNullOrEmpty(Path))
                Path = GetDefaultPath();

            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message files selector for open files. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="saveMode"> Save mode. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMFilesSelector CreateOpenFilesMessage(IIMControl imControl, string title,
            bool saveMode = false,
            CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var imBox = new IMFilesSelector(imControl, saveMode)
            {
                Title = title,
            };

            if (onClose != null)
                imBox.Closed = onClose;

            return imBox;
        }

        #endregion CLASS METHODS

        #region ADDRESS BAR INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked address bar custom text box got focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddressBarCustomTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            PathFocused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after address bar custom text box lost focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddressBarCustomTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            PathFocused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after preview key down in address bar text box. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void AddressBarCustomTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        #endregion ADDRESS BAR INTERACTION METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ok button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Ok);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CancelCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Cancel);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking options button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OptionsButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender is Button button)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        #endregion BUTTONS INTERACTION METHODS

        #region COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in files tree viewer. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> FilesTreeViewer Item Selected Event Arguments. </param>
        private void FilesTreeViewerItemSelected(object sender, FilesTreeViewerItemSelectedEventArgs e)
        {
            if (e.TreeViewFileItem != null)
                Path = e.TreeViewFileItem.Path;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting items in files viewer. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> FilesViewer Items Selected Event Arguments. </param>
        private void FilesViewerItemsSelected(object sender, FilesViewerItemsSelectedEventArgs e)
        {
            ExecuteInLockedSelectedFileName(() =>
            {
                if (e.FilesItems?.Any() ?? false)
                {
                    var directories = e.FilesItems.Where(f => f.IsDirectory);

                    if (directories.Count() == 1)
                    {
                        Path = directories.First().Path;
                        SelectedItems = new ObservableCollection<FileItem>();
                        return;
                    }

                    var firstItem = e.FilesItems.First();

                    SelectedFileName = e.FilesItems.Count == 1
                        ? firstItem.FileName
                        : string.Join(";", e.FilesItems.Select(f => f.FileName));

                    SelectedItems = new ObservableCollection<FileItem>(e.FilesItems);
                }
                else
                {
                    SelectedFileName = null;
                    SelectedItems = new ObservableCollection<FileItem>();
                }
            });
        }

        #endregion COMPONENTS INTERACTION METHODS

        #region FILE NAME BAR INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked address bar custom text box got focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void FileNameBarCustomTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            FileNameFocused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after address bar custom text box lost focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void FileNameBarCustomTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            FileNameFocused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after preview key down in address bar text box. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void FileNameBarCustomTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        #endregion FILE NAME BAR INTERACTION METHODS

        #region NAVIGATION BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationBackCustomButtonClick(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation forward button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationForwardCustomButtonClick(object sender, RoutedEventArgs e)
        {
            NavigateForward();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation go button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationGoCustomButtonClick(object sender, RoutedEventArgs e)
        {
            Path = _addressBarTextBox.Text;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation search button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchCustomButtonClick(object sender, RoutedEventArgs e)
        {
            SearchPhrase = _searchBarTextBox.Text;
        }

        #endregion NAVIGATION BUTTONS INTERACTION METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate Back. </summary>
        private void NavigateBack()
        {
            var parentPath = System.IO.Path.GetDirectoryName(Path);

            if (!string.IsNullOrEmpty(parentPath))
            {
                _keepForwardHistory = true;
                Path = parentPath;
            }
            else
            {
                _keepForwardHistory = true;
                Path = null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate forward. </summary>
        private void NavigateForward()
        {
            if (_forwardHistory.Any())
            {
                var lastEntry = _forwardHistory.Last();
                _forwardHistory.RemoveAt(_forwardHistory.Count - 1);

                _keepForwardHistory = true;
                _lockForwardHistory = true;
                Path = lastEntry;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update forward history. </summary>
        private void UpdateForwardHistory()
        {
            if (_keepForwardHistory)
            {
                if (!_lockForwardHistory && !string.IsNullOrEmpty(_backupPath))
                    _forwardHistory.Add(_backupPath);
            }
            else
            {
                _forwardHistory.Clear();
            }

            _keepForwardHistory = false;
            _lockForwardHistory = false;
            NotifyPropertyChanged(nameof(CanNavigateForward));
        }

        #endregion NAVIGATION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after files extension collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void FilesExtensionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(FilesExtensions));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selected items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(SelectedItems));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region SEARCH BAR INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked search bar custom text box got focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchBarCustomTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            SearchFocused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked search bar custom text box lost focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchBarCustomTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            SearchFocused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after preview key down in search bar text box. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void SearchBarCustomTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        #endregion SEARCH BAR INTERACTION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Execute method in lock seleced file name mode. </summary>
        /// <param name="action"> Method to execute. </param>
        private void ExecuteInLockedSelectedFileName(Action action)
        {
            if (action == null)
                return;

            _lockSelectedFileName = true;
            action?.Invoke();
            _lockSelectedFileName = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get default path. </summary>
        /// <returns> Default path. </returns>
        private string GetDefaultPath()
        {
            if (SystemHelper.IsUserProfilePath())
                return SystemHelper.GetUserProfilePath();

            else if (SystemHelper.IsHomePath())
                return SystemHelper.GetHomePath();

            else
                return SystemHelper.GetSystemDrivePath();
        }

        #endregion UTILITY METHODS

    }
}
