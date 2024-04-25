using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.ViewModels.Player
{
    public class SongViewModel : BasePlayableViewModel
    {

        //  VARIABLES

        private string _title;
        private string _album;
        private string _artist;


        //  GETTERS & SETTERS

        public string Title
        {
            get => !string.IsNullOrEmpty(_title) ? _title : FileName;
            set => UpdateProperty(ref _title, value);
        }

        public string Album
        {
            get => !string.IsNullOrEmpty(_album) ? _album : GetParentCatalogName();
            set => UpdateProperty(ref _album, value);
        }

        public string Artist
        {
            get => _artist;
            set => UpdateProperty(ref _artist, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SongViewModel class constructor. </summary>
        /// <param name="filePath"> File path. </param>
        public SongViewModel(string filePath) : base(filePath)
        {
            //
        }

        #endregion CLASS METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get parent catalog name. </summary>
        /// <returns> Parent catalog name. </returns>
        private string GetParentCatalogName()
        {
            try
            {
                DirectoryInfo parentDir = Directory.GetParent(FilePath);

                return parentDir != null
                    ? parentDir.Name
                    : Path.GetPathRoot(FilePath);
            }
            catch
            {
                return null;
            }
        }

        #endregion UTILITY METHODS

    }
}
