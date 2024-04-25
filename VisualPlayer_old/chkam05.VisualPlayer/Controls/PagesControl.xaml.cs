using chkam05.VisualPlayer.Data.Configuration;
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

        public event EventHandler OnMinimizeButtonClick;
        public event EventHandler OnMaximizeButtonClick;
        public event EventHandler OnCloseButtonClick;

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

        public static readonly DependencyProperty ShowAdditionalControlsProperty = DependencyProperty.Register(
            nameof(ShowAdditionalControls),
            typeof(bool),
            typeof(PagesControl),
            new PropertyMetadata(true));


        //  VARIABLES

        private List<IPage> _loadedPages;

        public ConfigManager ConfigManager { get; private set; }


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

        public bool ShowAdditionalControls
        {
            get => (bool)GetValue(ShowAdditionalControlsProperty);
            set => SetValue(ShowAdditionalControlsProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PagesControl class constructor. </summary>
        public PagesControl()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

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

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoBack)
                GoBack();
            else
                HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Close ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Minimize Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            OnMinimizeButtonClick?.Invoke(this, new EventArgs());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Maximize Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void MaximizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            OnMaximizeButtonClick?.Invoke(this, new EventArgs());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing Close Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            OnCloseButtonClick?.Invoke(this, new EventArgs());
        }

        #endregion CONTROL BUTTONS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over title grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        #endregion INTERACTION METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show pages control user interface. </summary>
        public void ShowInterface()
        {
            if (Visibility != Visibility.Visible)
            {
                Visibility = Visibility.Visible;
                OnShow?.Invoke(this, new EventArgs());

                OnPropertyChanged(nameof(IsVisible));
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

            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(IsVisible));
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

                OnPropertyChanged(nameof(CanGoBack));
                OnPropertyChanged(nameof(LoadedPage));
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

                OnPropertyChanged(nameof(CanGoBack));
                OnPropertyChanged(nameof(LoadedPage));
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
