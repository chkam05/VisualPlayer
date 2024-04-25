using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Pages.Base;

namespace VisualPlayer.Controls
{
    public partial class ContentViewer : UserControl, IContentViewer
    {

        //  DELEGATES

        public delegate void PageLoadedEventHandler(object sender, PageLoadedEventArgs e);


        //  EVENTS

        public PageLoadedEventHandler PageLoaded;


        //  VARIABLES

        private List<BasePage> _pages;


        //  GETTERS & SETTERS

        public bool CanGoBack
        {
            get => _pages.Any() && CurrentPageIndex > 0;
        }

        public BasePage CurrentPage
        {
            get => _contentFrame.Content as BasePage;
        }

        public int CurrentPageIndex
        {
            get => CurrentPage != null ? _pages.IndexOf(CurrentPage) : -1;
        }

        public int PagesCount
        {
            get => _pages.Count;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ContentViewer class constructor. </summary>
        public ContentViewer()
        {
            //  Setup data containers.
            _pages = new List<BasePage>();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERNAL INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after navigating to page. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Navigation Event Arguments. </param>
        private void ContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            RemoveBackEntry();
        }

        #endregion INTERNAL INTERACTION METHODS

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Go back. </summary>
        /// <param name="stepsBack"> Pages back count. </param>
        public void GoBack(int stepsBack = 1)
        {
            if (CanGoBack)
            {
                var currentPage = CurrentPage;
                var destPageIndex = Math.Max(0, CurrentPageIndex - stepsBack);
                var destPage = _pages[destPageIndex];

                _pages.RemoveRange(destPageIndex + 1, PagesCount - (destPageIndex + 1));
                _contentFrame.Navigate(destPage);

                InvokeOnPageLoaded(new PageLoadedEventArgs(currentPage, destPage, true));
            }
            else
            {
                var currentPage = CurrentPage;

                _contentFrame.Content = null;
                _pages.Clear();
                RemoveBackEntry();

                InvokeOnPageLoaded(new PageLoadedEventArgs(currentPage, null, true));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page has been loaded previously. </summary>
        /// <param name="page"> Page. </param>
        /// <returns> True - page has been loaded previously; False - otherwise. </returns>
        public bool HasPage(BasePage page)
        {
            return _pages.Contains(page);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page with particular type has been loaded previously. </summary>
        /// <param name="pageType"> Page type. </param>
        /// <returns>  True - page with particular type has been loaded previously; False - otherwise. </returns>
        public bool HasPageWithType(Type pageType)
        {
            return _pages.Any(p => pageType.IsAssignableFrom(p.GetType()));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get page index. </summary>
        /// <param name="page"> Page. </param>
        /// <returns> Page index or -1; </returns>
        public int IndexOfPage(BasePage page)
        {
            return _pages.IndexOf(page);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load page. </summary>
        /// <param name="page"> Page. </param>
        public void LoadPage(BasePage page)
        {
            if (page == null)
                return;

            if (page.SingleInstance && HasPageWithType(page.GetType()))
            {
                int pageIndex = IndexOfPage(page);
                GoBack(CurrentPageIndex - pageIndex);
                return;
            }

            var currentPage = CurrentPage;

            _pages.Add(page);
            _contentFrame.Navigate(page);

            InvokeOnPageLoaded(new PageLoadedEventArgs(currentPage, page));
        }

        #endregion PAGES MANAGEMENT METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke page loaded evnet handler. </summary>
        /// <param name="pageLoadedEventArgs"> Page Loaded Event Arguments. </param>
        private void InvokeOnPageLoaded(PageLoadedEventArgs pageLoadedEventArgs)
        {
            if (pageLoadedEventArgs.IsBack && pageLoadedEventArgs.LoadedPage == null)
                ConfigurationManager.Instance.UIController.RequestContentHide();

            PageLoaded?.Invoke(this, pageLoadedEventArgs);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove history from frame navigation service. </summary>
        private void RemoveBackEntry()
        {
            var backEntry = _contentFrame.NavigationService.RemoveBackEntry();

            if (backEntry != null)
                RemoveBackEntry();
        }

        #endregion UTILITY METHODS

    }
}
