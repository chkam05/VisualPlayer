using chkam05.VisualPlayer.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Core.Events
{
    public class PlayerLoadedFileEventArgs : EventArgs
    {

        //  VARIABLES

        public IFile File { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayerLoadedFileEventArgs class methods. </summary>
        /// <param name="file"> Loaded file. </param>
        public PlayerLoadedFileEventArgs(IFile file)
        {
            File = file;
        }

        #endregion CLASS METHODS

    }
}
