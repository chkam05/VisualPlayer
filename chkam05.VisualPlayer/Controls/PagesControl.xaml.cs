using chkam05.VisualPlayer.Pages;
using chkam05.VisualPlayer.Pages.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace chkam05.VisualPlayer.Controls
{
    public partial class PagesControl : UserControl, IPagesManager, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<PageLoadedEventArgs> OnPageBack;
        public event EventHandler<PageLoadedEventArgs> OnPageLoaded;
        public event EventHandler OnShow;
        public event EventHandler OnHide;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register(
            nameof(BorderBackground),
            typeof(Brush),
            typeof(PagesControl),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));


        //  VARIABLES

        private List<IPage> _loadedPages;


        //  GETTERS & SETTERS

        public Brush BorderBackground
        {
            get => (Brush)GetValue(BorderBackgroundProperty);
            set
            {
                SetValue(BorderBackgroundProperty, value);
                OnPropertyChanged(nameof(BorderBackground));
            }
        }

        public bool CanGoBack
        {
            get => _loadedPages.Any() && _loadedPages.IndexOf(LoadedPage) > 0;
        }

        public IPage LoadedPage
        {
            get => _loadedPages.LastOrDefault();
        }

        public int PagesCount
        {
            get => _loadedPages.Count;
        }

        public new bool IsVisible
        {
            get => Visibility == Visibility.Visible;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PagesControl class constructor. </summary>
        public PagesControl()
        {
            //  Setup Data Containers.
            _loadedPages = new List<IPage>();

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region CONTENT FRAME METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all loaded pages from history. </summary>
        private void ClearAllContent()
        {
            _loadedPages.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove currently loaded page from content frame. </summary>
        private void ClearCurrentContent()
        {
            ContentFrame.Content = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Metod invoked after loading page in frame. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Navigated Event Arguments. </param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //  Remove previous pages from content frame back entry.
            RemoveBackEntry();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove previous pages from content fram back entry. </summary>
        private void RemoveBackEntry()
        {
            //  Get previous pages from content frame navigation service.
            var backEntry = ContentFrame.NavigationService.RemoveBackEntry();

            //  While previous pages are available - try to remove it.
            while (backEntry != null)
                backEntry = ContentFrame.NavigationService.RemoveBackEntry();
        }

        #endregion CONTENT FRAME METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show pages control user interface. </summary>
        public void ShowInterface()
        {
            if (Visibility != Visibility.Visible)
            {
                Visibility = Visibility.Visible;
                OnShow?.Invoke(this, new EventArgs());
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide pages control user interface. </summary>
        public void HideInterface()
        {
            Visibility = Visibility.Collapsed;
            OnHide?.Invoke(this, new EventArgs());

            //  Clear all pages.
            ClearAllContent();
            ClearCurrentContent();

            //  Invoke external event.
            var args = new PageLoadedEventArgs(null);
            OnPageBack?.Invoke(this, args);
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Go to previous page/pages by certain number of steps. </summary>
        /// <param name="backCount"> Number of steps back. </param>
        public void GoBack(int backCount = 1)
        {
            if (CanGoBack)
            {
                var currPageIndex = _loadedPages.IndexOf(LoadedPage);
                var destPageIndex = Math.Max(0, currPageIndex - backCount);

                //  Get previous page from list to load into content frame.
                var destPage = _loadedPages[destPageIndex];

                //  Remove other pages loaded further.
                _loadedPages.RemoveRange(destPageIndex + 1, PagesCount - (destPageIndex + 1));

                //  Load previous page and update current page index.
                ContentFrame.Navigate(destPage);

                //  Invoke external event.
                var args = new PageLoadedEventArgs(destPage);
                OnPageBack?.Invoke(this, args);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate to newly created page. </summary>
        /// <param name="page"> Page to load. </param>
        public void LoadPage(IPage page)
        {
            var pageToLoad = page as Page;

            if (pageToLoad != null)
            {
                //  Add page to history.
                _loadedPages.Add(page);

                //  Load page.
                ContentFrame.Navigate(pageToLoad);

                //  Invoke external event.
                var args = new PageLoadedEventArgs(page);
                OnPageLoaded?.Invoke(this, args);
            }
        }

        #endregion NAVIGATION METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page is already loaded. </summary>
        /// <param name="page"> Page to check. </param>
        /// <returns> True - pages is already loaded; False - otherwise. </returns>
        public bool HasPage(IPage page)
        {
            return _loadedPages.Contains(page);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if page with specified type is already loaded. </summary>
        /// <param name="pageType"> Page type to check. </param>
        /// <returns> True - page with this type is already loaded; False - otherwise. </returns>
        public bool HasPage(Type pageType)
        {
            return _loadedPages.Any(p => pageType.IsAssignableFrom(((Page)p).GetType()));
        }

        #endregion PAGES MANAGEMENT METHODS

    }
}
