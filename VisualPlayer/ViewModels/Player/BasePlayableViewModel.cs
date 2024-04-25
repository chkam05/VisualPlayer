using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.ViewModels.Player
{
    public class BasePlayableViewModel : BaseViewModel
    {

        //  VARIABLES

        private string _filePath;


        //  GETTERS & SETTERS

        public string FilePath
        {
            get => _filePath;
            set
            {
                UpdateProperty(ref _filePath, value);
                NotifyPropertyChanged(nameof(FileName));
            }
        }

        public string FileName
        {
            get => Path.GetFileNameWithoutExtension(_filePath);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BasePlayableViewModel class constructor. </summary>
        /// <param name="filePath"> File path. </param>
        public BasePlayableViewModel(string filePath)
        {
            FilePath = filePath;
        }

        #endregion CLASS METHODS

    }
}
