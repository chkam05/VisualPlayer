using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Files.EventArgs
{
    public class FilesLoaderOnLoadEventArgs
    {

        //  VARIABLES

        public string FilePath { get; private set; }
        public int FileIndex { get; private set; }
        public int Progress { get; private set; }
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }


        #region GETTERS & SETTERS

        public string FileName
        {
            get => !string.IsNullOrEmpty(FilePath)
                ? Path.GetFileNameWithoutExtension(FilePath)
                : FilePath;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesLoaderOnLoadEventArgs class constructor. </summary>
        /// <param name="filePath"> Loaded file path. </param>
        /// <param name="fileIndex"> Index of loaded file. </param>
        /// <param name="progress"> Loading procentage progress. </param>
        /// <param name="isError"> If loading failes. </param>
        /// <param name="errorMessage"> Loading error message. </param>
        public FilesLoaderOnLoadEventArgs(string filePath, int fileIndex, int progress, 
            bool isError = false, string errorMessage = null)
        {
            FilePath = filePath;
            FileIndex = fileIndex;
            Progress = progress;
            IsError = isError;
            ErrorMessage = errorMessage;
        }

        #endregion CLASS METHODS

    }
}
