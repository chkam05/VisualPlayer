using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Data.MainMenu
{
    public class MainMenuItemClickEventArgs : EventArgs
    {

        //  VARIABLES

        public string Title { get; private set; }
        public object[] Args { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainMenuItemClickEventArgs class constructor. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <param name="parameters"> Additional parameters. </param>
        public MainMenuItemClickEventArgs(string title, params object[] parameters)
        {
            Title = title;
            Args = parameters ?? new object[0];
        }

        #endregion CLASS METHODS

    }
}
