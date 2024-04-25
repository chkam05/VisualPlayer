using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Controls.Events
{
    public class FilesTreeViewerItemSelectedEventArgs : EventArgs
    {

        //  VARIABLES

        public TreeViewFileItem TreeViewFileItem { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesTreeViewerItemSelectedEventArgs class constructor. </summary>
        /// <param name="treeViewFileItem"> Tree view file item. </param>
        public FilesTreeViewerItemSelectedEventArgs(TreeViewFileItem treeViewFileItem)
        {
            TreeViewFileItem = treeViewFileItem;
        }

        #endregion CLASS METHODS

    }
}
