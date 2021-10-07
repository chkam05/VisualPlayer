using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace chkam05.VisualPlayer.Utilities.Pages
{
    public interface IPagesManager
    {

        //  EVENTS

        event EventHandler<NavigationEventArgs> OnPageChange;


        //  VARIABLES

        int CurrentPageIndex { get; }
        Page CurrentPage { get; }
        int PagesCount { get; }
        List<Page> Pages { get; }

        bool KeepPreviousPages { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by index from loaded pages list. </summary>
        /// <param name="pageIndex"> Index of page in loaded pages list. </param>
        /// <returns> Page with particular index or null. </returns>
        Page GetPageByIndex(int pageIndex);

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by name from loaded pages list. </summary>
        /// <param name="pageName"> Name of page from loaded pages list. </param>
        /// <returns> Page with particular name or null. </returns>
        Page GetPageByName(string pageName);

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by type from loaded pages list. </summary>
        /// <param name="pageType"> Type of page from loaded pages list. </param>
        /// <returns> Page with particular type or null. </returns>
        Page GetPageByType(Type pageType);

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page is loaded into loaded pages list. </summary>
        /// <param name="page"> Page to check. </param>
        /// <returns> True - page is loaded; False - otherwise. </returns>
        bool HasPage(Page page);


        //  --------------------------------------------------------------------------------
        /// <summary> Load previous page from loaded pages list if it is available. </summary>
        /// <returns> Previous loaded page. </returns>
        Page Back();

        //  --------------------------------------------------------------------------------
        /// <summary> Clear pages manager history and leave only current opened page. </summary>
        void ClearHistory();

        //  --------------------------------------------------------------------------------
        /// <summary> Load next page from loaded pages list if it is available. </summary>
        /// <returns> Next loaded page. </returns>
        Page Forward();

        //  --------------------------------------------------------------------------------
        /// <summary> Add new page into loaded pages list and set as active. </summary>
        /// <param name="page"> Page to add and load. </param>
        /// <returns> True - page loaded successfully; False - otherwise. </returns>
        bool LoadPage(Page page);

        //  --------------------------------------------------------------------------------
        /// <summary> Load page from loaded pages list. </summary>
        /// <param name="page"> Page to load. </param>
        void GoToPage(Page page);

        //  --------------------------------------------------------------------------------
        /// <summary> Remove page from loaded pages list. </summary>
        /// <param name="page"> Page to remove. </param>
        void UnloadPage(Page page);

    }
}
