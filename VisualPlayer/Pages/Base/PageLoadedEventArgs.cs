using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Pages.Base
{
    public class PageLoadedEventArgs : EventArgs
    {

        //  VARIABLES

        public bool IsBack { get; private set; }
        public BasePage PreviousPage { get; private set; }
        public BasePage LoadedPage { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PageLoadedEventArgs class constructor. </summary>
        /// <param name="previousPage"> Previous page. </param>
        /// <param name="loadedPage"> Loaded page. </param>
        public PageLoadedEventArgs(BasePage previousPage, BasePage loadedPage, bool isBack = false) : base()
        {
            IsBack = isBack;
            PreviousPage = previousPage;
            LoadedPage = loadedPage;
        }

        #endregion CLASS METHODS

    }
}
