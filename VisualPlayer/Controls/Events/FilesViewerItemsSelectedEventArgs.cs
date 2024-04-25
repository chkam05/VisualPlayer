using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Controls.Events
{
    public class FilesViewerItemsSelectedEventArgs : EventArgs
    {

        //  VARIABLES

        public List<FileItem> FilesItems { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesViewerItemsSelectedEventArgs class constructor. </summary>
        /// <param name="filesItems"> Files items collection. </param>
        public FilesViewerItemsSelectedEventArgs(IEnumerable<FileItem> filesItems)
        {
            FilesItems = filesItems.ToList();
        }

        #endregion CLASS METHODS

    }
}
