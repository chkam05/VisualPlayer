using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace chkam05.VisualPlayer.Utilities.Pages
{
    public class SinglePagesManager : IPagesManager
    {

        //  EVENTS

        public event EventHandler<NavigationEventArgs> OnPageChange;


        //  VARIABLES

        private Frame _container;
        private int _loadedPageIndex = -1;
        private List<Page> _loadedPages;

        public bool KeepPreviousPages { get; set; } = true;


        #region GETTERS & SETTERS

        public int CurrentPageIndex
        {
            get => _loadedPages.Count > 0 ? Math.Max(0, Math.Min(_loadedPageIndex, PagesCount)) : -1;
        }

        public Page CurrentPage
        {
            get => PagesCount > 0 ? _loadedPages[CurrentPageIndex] : null;
        }

        public int PagesCount
        {
            get => _loadedPages.Count;
        }

        public List<Page> Pages
        {
            get => _loadedPages;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SinglePagesManager class constructor. </summary>
        /// <param name="container"> Content frame where pages will be loaded. </param>
        public SinglePagesManager(Frame container)
        {
            //  Setup data containers.
            _loadedPages = new List<Page>();

            //  Setup content frame.
            _container = container;
            _container.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            _container.Navigated += ContentFrame_Navigated;
        }

        #endregion CLASS METHODS

        #region CONTENT FRAME METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when page is loded in content frame. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Navigation event arguments. </param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //  Invoke external method.
            OnPageChange?.Invoke(CurrentPage, e);

            //  Remove back entry pages from content frame.
            RemoveBackEntry();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove current page from content frame. </summary>
        private void ClearContentFrame()
        {
            //  Remove current page.
            _container.Content = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove previous pages from content frame naviation service. </summary>
        private void RemoveBackEntry()
        {
            //  Get previous pages from content frame navigation service.
            var entry = _container.NavigationService.RemoveBackEntry();

            //  While previous pages are available - try to remove it.
            while (entry != null)
                entry = _container.NavigationService.RemoveBackEntry();
        }

        #endregion CONTENT FRAME METHODS

        #region GET PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by index from loaded pages list. </summary>
        /// <param name="pageIndex"> Index of page in loaded pages list. </param>
        /// <returns> Page with particular index or null. </returns>
        public Page GetPageByIndex(int pageIndex)
        {
            if (_loadedPages.Count <= 0)
                return null;

            if (pageIndex < 0 || pageIndex >= PagesCount)
                return null;

            return _loadedPages[pageIndex];
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by name from loaded pages list. </summary>
        /// <param name="pageName"> Name of page from loaded pages list. </param>
        /// <returns> Page with particular name or null. </returns>
        public Page GetPageByName(string pageName)
        {
            var foundPage = _loadedPages.FirstOrDefault(p => p.Name == pageName);

            if (foundPage != null)
                return foundPage;

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get page by type from loaded pages list. </summary>
        /// <param name="pageType"> Type of page from loaded pages list. </param>
        /// <returns> Page with particular type or null. </returns>
        public Page GetPageByType(Type pageType)
        {
            var foundPage = _loadedPages.FirstOrDefault(p => p.GetType() == pageType);

            if (foundPage != null)
                return foundPage;

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page is loaded into loaded pages list. </summary>
        /// <param name="page"> Page to check. </param>
        /// <returns> True - page is loaded; False - otherwise. </returns>
        public bool HasPage(Page page)
        {
            return _loadedPages.Contains(page);
        }

        #endregion GET PAGE METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load previous page from loaded pages list if it is available. </summary>
        /// <returns> Previous loaded page. </returns>
        public Page Back()
        {
            //  Check if previous page is available.
            if (CurrentPageIndex > 0 && _loadedPages.Count > 0)
            {
                //  Get previous page from list to load into content frame.
                var previousPage = _loadedPages[CurrentPageIndex - 1];

                //  Remove other pages loaded further.
                if (!KeepPreviousPages)
                    _loadedPages.RemoveRange(CurrentPageIndex, Math.Max(0, _loadedPages.Count - CurrentPageIndex));

                //  Load previous page and update current page index.
                _container.Navigate(previousPage);
                _loadedPageIndex = CurrentPageIndex - 1;

                //  Return loaded page.
                return previousPage;
            }

            return CurrentPage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear pages manager history and leave only current opened page. </summary>
        public void ClearHistory()
        {
            //  Check if any page is loaded.
            if (CurrentPageIndex > 0 && _loadedPages.Count > 0)
            {
                //  Get current opened page.
                var currentPage = _loadedPages[CurrentPageIndex];

                //  Remove all pages from pages list and leave current opened page.
                _loadedPages.RemoveAll(p => p != currentPage);

                //  Update current page index.
                _loadedPageIndex = 0;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load next page from loaded pages list if it is available. </summary>
        /// <returns> Next loaded page. </returns>
        public Page Forward()
        {
            //  Check if next page are available.
            if (CurrentPageIndex < _loadedPages.Count - 1)
            {
                //  Get next page from list to load into content frame.
                var nextPage = _loadedPages[CurrentPageIndex + 1];

                //  Load next page and update loaded page index.
                _container.Navigate(nextPage);
                _loadedPageIndex = CurrentPageIndex + 1;

                //  Return loaded page.
                return nextPage;
            }

            return CurrentPage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add new page into loaded pages list and set as active. </summary>
        /// <param name="page"> Page to add and load. </param>
        /// <returns> True - page loaded successfully; False - otherwise. </returns>
        public bool LoadPage(Page page)
        {
            //  Remove other pages from list.
            if (!KeepPreviousPages)
                _loadedPages.Clear();

            //  Prevent to load page with same type as loaded previously.
            if (_loadedPages.Any(p => p.GetType() == page.GetType()))
                return false;

            //  Add page to list, load and update current page index.
            _loadedPages.Add(page);
            _container.Navigate(page);
            _loadedPageIndex = _loadedPages.Count - 1;

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load page from loaded pages list. </summary>
        /// <param name="page"> Page to load. </param>
        public void GoToPage(Page page)
        {
            if (_loadedPages.Contains(page))
            {
                //  Get page index.
                int pageIndex = _loadedPages.IndexOf(page);

                //  Load page and update loaded page index.
                _container.Navigate(page);
                _loadedPageIndex = pageIndex;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove page from loaded pages list. </summary>
        /// <param name="page"> Page to remove. </param>
        public void UnloadPage(Page page)
        {
            if (_loadedPages.Contains(page))
            {
                int pageIndex = _loadedPages.IndexOf(page);

                //  Correct current page index.
                if (CurrentPageIndex > pageIndex)
                    _loadedPageIndex = Math.Max(0, CurrentPageIndex - 1);

                //  Remove page from loaded pages list.
                _loadedPages.Remove(page);

                //  Unload page from content frame.
                if (CurrentPageIndex == pageIndex)
                {
                    if (PagesCount > 1)
                    {
                        //  Load previous or closest available page.
                        var pageToLoad = GetPageByIndex(Math.Max(0, _loadedPageIndex));
                        GoToPage(pageToLoad);
                    }
                    else
                    {
                        //  Unload page from content frame and update page index.
                        ClearContentFrame();
                        _loadedPageIndex = -1;
                    }
                }
            }
        }

        #endregion INTERACTION METHODS

    }
}
