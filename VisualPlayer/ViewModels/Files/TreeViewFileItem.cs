using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Utilities;

namespace VisualPlayer.ViewModels.Files
{
    public class TreeViewFileItem : BaseViewModel
    {

        //  DELEGATES

        public delegate void TreeViewFileItemExpandEventHandler(object sender, TreeViewFileItemExpandEventArgs e);


        //  EVENTS

        public TreeViewFileItemExpandEventHandler Expanded;


        //  VARIABLES

        private ObservableCollection<TreeViewFileItem> _childItems;
        private PackIconKind _icon = PackIconKind.FolderOutline;
        private bool _isExpanded = false;
        private string _path;


        //  GETTERS & SETTERS

        public ObservableCollection<TreeViewFileItem> ChildItems
        {
            get => _childItems;
            set
            {
                UpdateProperty(ref _childItems, value);
                _childItems.CollectionChanged += NotifyChildItemsCollectionChanged;
            }
        }

        public string FileName
        {
            get
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(_path);
                 return !string.IsNullOrEmpty(fileName) ? fileName : _path;
            }
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                UpdateProperty(ref _isExpanded, value);
                Expanded?.Invoke(this, new TreeViewFileItemExpandEventArgs(this));
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                UpdateProperty(ref _path, value);
                NotifyPropertyChanged(nameof(FileName));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> TreeViewFileItem class constructor. </summary>
        /// <param name="path"> File path. </param>
        public TreeViewFileItem(string path)
        {
            ChildItems = new ObservableCollection<TreeViewFileItem>();
            Path = path;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create tree view item from file info. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> Tree view item or null. </returns>
        public static TreeViewFileItem CreateTreeViewFileItem(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;

            var treeViewFileItem = new TreeViewFileItem(fileInfo.FullName);

            if (!SystemHelper.IsDirectory(fileInfo))
                treeViewFileItem.Icon = PackIconKind.FileOutline;

            return treeViewFileItem;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create tree view item from drive info. </summary>
        /// <param name="driveInfo"> Drive info. </param>
        /// <returns> Tree view item or null. </returns>
        public static TreeViewFileItem CreateTreeViewFileItem(DriveInfo driveInfo)
        {
            if (driveInfo == null)
                return null;

            var treeViewFileItem = new TreeViewFileItem(driveInfo.Name);

            switch (driveInfo.DriveType)
            {
                case DriveType.Fixed:
                    treeViewFileItem.Icon = PackIconKind.Harddisk;
                    break;

                case DriveType.CDRom:
                    treeViewFileItem.Icon = PackIconKind.Disc;
                    break;

                case DriveType.Network:
                    treeViewFileItem.Icon = PackIconKind.FolderNetworkOutline;
                    break;

                case DriveType.Removable:
                    treeViewFileItem.Icon = PackIconKind.UsbFlashDriveOutline;
                    break;

                case DriveType.Ram:
                    treeViewFileItem.Icon = PackIconKind.Memory;
                    break;

                case DriveType.NoRootDirectory:
                    treeViewFileItem.Icon = PackIconKind.Connection;
                    break;

                case DriveType.Unknown:
                    treeViewFileItem.Icon = PackIconKind.QuestionMark;
                    break;
            }

            return treeViewFileItem;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create tree view item from path. </summary>
        /// <param name="path"> File or directory path. </param>
        /// <returns> Tree view item or null. </returns>
        public static TreeViewFileItem CreateTreeViewFileItem(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return null;

            return CreateTreeViewFileItem(new FileInfo(path));
        }

        #endregion CLASS METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Notify child items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyChildItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(ChildItems));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
