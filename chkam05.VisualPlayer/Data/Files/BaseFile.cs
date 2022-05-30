using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;


namespace chkam05.VisualPlayer.Data.Files
{
    public class BaseFile : IFile
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private string _filePath { get; set; }


        //  GETTERS & SETTERS

        [XmlIgnore]
        public bool Exists
        {
            get => !string.IsNullOrEmpty(_filePath) && File.Exists(_filePath);
        }

        [XmlElement("Path")]
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        [XmlIgnore]
        public string FileExtension
        {
            get => !string.IsNullOrEmpty(_filePath) ? Path.GetExtension(_filePath) : null;
        }

        [XmlIgnore]
        public string FileName
        {
            get => !string.IsNullOrEmpty(_filePath) ? Path.GetFileNameWithoutExtension(_filePath) : null;
        }

        [XmlIgnore]
        public string FileNameWithExt
        {
            get => !string.IsNullOrEmpty(_filePath) ? Path.GetFileName(_filePath) : null;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseFileModel class constructor. </summary>
        public BaseFile()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> BaseFileModel class constructor. </summary>
        /// <param name="filePath"> Initial file path. </param>
        /// <param name="loadMetadata"> Load file metadata on create. </param>
        public BaseFile(string filePath, bool loadMetadata = true)
        {
            _filePath = filePath;

            if (Exists && loadMetadata)
                LoadMetadata();
        }

        #endregion CLASS METHODS

        #region CHECKSUM METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get file checksum. </summary>
        /// <returns> File checksum. </returns>
        public string GetChecksum()
        {
            if (Exists)
            {
                using (var md5 = MD5.Create())
                {
                    using (var fileStream = File.OpenRead(FilePath))
                    {
                        var checksum = md5.ComputeHash(fileStream);
                        return BitConverter.ToString(checksum).Replace("-", "");
                    }
                }
            }

            return null;
        }

        #endregion CHECKSUM METHODS

        #region METADATA METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load file metadata. </summary>
        public virtual void LoadMetadata()
        {
            //
        }

        #endregion METADATA METHODS

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

        #region UTILITIES

        //  --------------------------------------------------------------------------------
        /// <summary> Get folder name containing file. </summary>
        /// <returns> Folder name containing file. </returns>
        protected string GetContainingFolderName()
        {
            if (Exists)
                return Path.GetDirectoryName(FilePath);

            return null;
        }

        #endregion UTILITIES

    }
}
