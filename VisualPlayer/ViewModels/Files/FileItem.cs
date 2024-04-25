using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisualPlayer.Utilities;

namespace VisualPlayer.ViewModels.Files
{
    public class FileItem : BaseViewModel
    {

        //  VARIABLES

        private PackIconKind _icon = PackIconKind.FileOutline;
        private string _path;


        //  GETTERS & SETTERS

        public string FileName
        {
            get
            {
                var fileName = System.IO.Path.GetFileName(_path);
                return !string.IsNullOrEmpty(fileName) ? fileName : _path;
            }
        }

        public string Extension
        {
            get
            {
                if (SystemHelper.IsDirectory(new FileInfo(_path)))
                    return null;

                var extension = System.IO.Path.GetExtension(_path);
                return !string.IsNullOrEmpty(extension) ? extension.Replace(".", "").ToLower() : string.Empty;
            }
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public string Path
        {
            get => _path;
            set
            {
                UpdateProperty(ref _path, value);
                NotifyPropertyChanged(nameof(FileName));
                NotifyPropertyChanged(nameof(Extension));
                NotifyPropertyChanged(nameof(IsDirectory));
            }
        }

        public bool IsDirectory
        {
            get => SystemHelper.IsDirectory(Path);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FileItem class constructor. </summary>
        /// <param name="path"> File path. </param>
        public FileItem(string path)
        {
            Path = path;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create file item from file info. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> File item or null. </returns>
        public static FileItem CreateFileItem(FileInfo fileInfo)
        {
            if (fileInfo == null)
                return null;

            var fileItem = new FileItem(fileInfo.FullName);

            if (SystemHelper.IsDirectory(fileInfo))
                fileItem.Icon = PackIconKind.FolderOutline;

            return fileItem;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create file item from drive info. </summary>
        /// <param name="driveInfo"> Drive info. </param>
        /// <returns> File item or null. </returns>
        public static FileItem CreateFileItem(DriveInfo driveInfo)
        {
            if (driveInfo == null)
                return null;

            var fileItem = new FileItem(driveInfo.Name);

            switch (driveInfo.DriveType)
            {
                case DriveType.Fixed:
                    fileItem.Icon = PackIconKind.Harddisk;
                    break;

                case DriveType.CDRom:
                    fileItem.Icon = PackIconKind.Disc;
                    break;

                case DriveType.Network:
                    fileItem.Icon = PackIconKind.FolderNetworkOutline;
                    break;

                case DriveType.Removable:
                    fileItem.Icon = PackIconKind.UsbFlashDriveOutline;
                    break;

                case DriveType.Ram:
                    fileItem.Icon = PackIconKind.Memory;
                    break;

                case DriveType.NoRootDirectory:
                    fileItem.Icon = PackIconKind.Connection;
                    break;

                case DriveType.Unknown:
                    fileItem.Icon = PackIconKind.QuestionMark;
                    break;
            }

            return fileItem;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create file item from path. </summary>
        /// <param name="path"> File or directory path. </param>
        /// <returns> File item or null. </returns>
        public static FileItem CreateTreeViewFileItem(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return null;

            return CreateFileItem(new FileInfo(path));
        }

        #endregion CLASS METHODS

    }
}
