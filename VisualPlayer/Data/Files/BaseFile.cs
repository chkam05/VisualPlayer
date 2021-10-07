using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Files
{
    public class BaseFile
    {

        //  VARIABLES

        public string FilePath { get; private set; }


        #region GETTERS & SETTERS

        public bool Exists
        {
            get => File.Exists(FilePath);
        }

        public string FileName
        {
            get => !string.IsNullOrEmpty(FilePath)
                ? Path.GetFileNameWithoutExtension(FilePath)
                : null;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseFile class constructor. </summary>
        /// <param name="filePath"> Path to file. </param>
        /// <param name="loadMetadata"> Load metadata at creation. </param>
        public BaseFile(string filePath, bool loadMetadata = true)
        {
            FilePath = filePath;

            if (loadMetadata)
                FillMetadata();
        }

        #endregion CLASS METHODS

        #region METADATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Fill additional informations about file. </summary>
        public virtual void FillMetadata()
        {
            //
        }

        #endregion METADATA MANAGEMENT METHODS

    }
}
