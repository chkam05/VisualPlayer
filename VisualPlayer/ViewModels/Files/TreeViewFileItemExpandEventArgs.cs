using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.ViewModels.Files
{
    public class TreeViewFileItemExpandEventArgs : EventArgs
    {

        //  VARIABLES

        public TreeViewFileItem TreeViewFileItem { get; private set; }


        //  GETTERS & SETTERS

        public bool IsExpanded
        {
            get => TreeViewFileItem.IsExpanded;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> TreeViewFileItemExpandEventArgs class constructor. </summary>
        /// <param name="treeViewFileItem"> Tree view file item. </param>
        public TreeViewFileItemExpandEventArgs(TreeViewFileItem treeViewFileItem)
        {
            TreeViewFileItem = treeViewFileItem;
        }

        #endregion CLASS METHODS

    }
}
