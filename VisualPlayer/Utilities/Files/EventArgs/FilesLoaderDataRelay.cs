using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Files.EventArgs
{
    internal class FilesLoaderDataRelay
    {

        //  VARIABLES

        public string FilePath { get; set; }
        public int FileIndex { get; set; }
        public Exception Exception { get; set; }


        #region GETTERS & SETTERS

        public string ErrorMessage
        {
            get => Exception?.Message;
        }

        public bool IsError
        {
            get => Exception != null;
        }

        #endregion GETTERS & SETTERS

    }
}
