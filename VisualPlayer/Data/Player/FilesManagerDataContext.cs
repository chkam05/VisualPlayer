using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Data.Player
{
    public class FilesManagerDataContext : BaseViewModel
    {

        //  CONST

        public const int ITEMS_SIZE_MAX = 6;
        public const int ITEMS_SIZE_MIN = 0;


        //  VARIABLES

        private ObservableCollection<FileExtension> _filesExtensions;
        private FileExtension _fileExtension = FileExtension.GetDefaultFileExtension();
        private int _itemsSize = 4;

        private string _backupPath = null;
        private string _path = null;
        private string _searchPhrase = null;
        private bool _multiSelect = false;
        private bool _showHiddenFiles = false;
        private bool _showSystemFiles = false;

        private bool _applyFilterToFilesFromSubdirectories = false;
        private bool _loadFilesFromSubdirectories = false;

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
            get => _multiSelect;
            set => UpdateProperty(ref _multiSelect, value);
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

        public bool ApplyFilterToFilesFromSubdirectories
        {
            get => _applyFilterToFilesFromSubdirectories;
            set => UpdateProperty(ref _applyFilterToFilesFromSubdirectories, value);
        }

        public bool LoadFilesFromSubdirectories
        {
            get => _loadFilesFromSubdirectories;
            set => UpdateProperty(ref _loadFilesFromSubdirectories, value);
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
        /// <summary> FilesManagerDataContext class constructor. </summary>
        public FilesManagerDataContext(string path = null)
        {
            //  Setup data
            _forwardHistory = new List<string>();

            FilesExtensions = new ObservableCollection<FileExtension>(new List<FileExtension>()
            {
                BaseFilesExtensions.Default,
                BaseFilesExtensions.SupportedFiles,
            });

            FileExtension = FilesExtensions[1];
            Path = !string.IsNullOrEmpty(path) ? path : GetDefaultPath();
        }

        #endregion CLASS METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate Back. </summary>
        public void NavigateBack()
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
        public void NavigateForward()
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

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region UTILITY METHODS

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
