using chkam05.VisualPlayer.Pages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Pages
{
    public interface IPagesManager
    {

        //  EVENTS

        event EventHandler<PageLoadedEventArgs> OnPageBack;
        event EventHandler<PageLoadedEventArgs> OnPageLoaded;
        event EventHandler OnShow;
        event EventHandler OnHide;


        //  GETTERS & SETTERS

        bool CanGoBack { get; }
        IPage LoadedPage { get; }
        int PagesCount { get; }
        bool IsVisible { get; }


        //  METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show pages control user interface. </summary>
        void ShowInterface();

        //  --------------------------------------------------------------------------------
        /// <summary> Hide pages control user interface. </summary>
        void HideInterface();

        #endregion INTERFACE MANAGEMENT METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Go to previous page/pages by certain number of steps. </summary>
        /// <param name="backCount"> Number of steps back. </param>
        void GoBack(int backCount = 1);

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate to newly created page. </summary>
        /// <param name="page"> Page to load. </param>
        void LoadPage(IPage page);

        #endregion NAVIGATION METHODS

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page is already loaded. </summary>
        /// <param name="page"> Page to check. </param>
        /// <returns> True - pages is already loaded; False - otherwise. </returns>
        bool HasPage(IPage page);

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page with specified type is already loaded. </summary>
        /// <param name="pageType"> Page type to check. </param>
        /// <returns> True - page with this type is already loaded; False - otherwise. </returns>
        bool HasPage(Type pageType);

        #endregion PAGES MANAGEMENT METHODS

    }
}
