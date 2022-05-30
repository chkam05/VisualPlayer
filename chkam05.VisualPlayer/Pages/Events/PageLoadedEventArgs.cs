using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Pages.Events
{
    public class PageLoadedEventArgs : EventArgs
    {

        //  VARIABLES

        public IPage Page { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PageLoadedEventArgs class constructor. </summary>
        /// <param name="page"> Loaded page. </param>
        public PageLoadedEventArgs(IPage page)
        {
            Page = page;
        }

        #endregion CLASS METHODS

    }
}
