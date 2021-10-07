using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Files.EventArgs
{
    public class FilesLoaderOnCompleteEventArgs
    {

        //  VARIABLES

        public List<string> ErrorFilesPaths { get; private set; }
        public List<string> LoadedFilesPaths { get; private set; }


        #region GETTERS & SETTERS

        public int ErrorFilesCount
        {
            get => ErrorFilesPaths?.Count ?? 0;
        }

        public int LoadedFilesCount
        {
            get => LoadedFilesPaths?.Count ?? 0;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesLoaderOnLoadEventArgs class constructor. </summary>
        /// <param name="errorFilesPaths"> List of not loaded files. </param>
        /// <param name="loadedFilesPaths"> List of loaded files. </param>
        public FilesLoaderOnCompleteEventArgs(List<string> errorFilesPaths, List<string> loadedFilesPaths)
        {
            ErrorFilesPaths = errorFilesPaths;
            LoadedFilesPaths = loadedFilesPaths;
        }

        #endregion CLASS METHODS

    }
}
