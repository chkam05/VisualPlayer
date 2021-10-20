using chkam05.VisualPlayer.Pages.Settings;
using chkam05.VisualPlayer.Utilities.Pages;
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

namespace chkam05.VisualPlayer.Pages
{
    public partial class SettingsPage : Page, IContentPage
    {

        //  VARIABLES

        private IPagesManager _pagesManager;

        private bool _fullBackground = true;


        #region GETTERS & SETTERS

        public bool FullBackground
        {
            get => _fullBackground;
            set => SwitchBackground(value);
        }

        public double ControlPanelMargin
        {
            get => this.Margin.Bottom;
            set => SetControlMargin(value);
        }

        public double SideBarMargin
        {
            get => this.Margin.Left;
            set => SetSideBarMargin(value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsPage class constructor. </summary>
        public SettingsPage()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set bottom margin for control component. </summary>
        /// <param name="bottomMargin"> New bottom margin size. </param>
        private void SetControlMargin(double bottomMargin)
        {
            //  Get current margin.
            var currentMargin = this.Margin;

            //  Setup new margin.
            this.Margin = new Thickness(currentMargin.Left, 0, 0, bottomMargin);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set left margin for sidebar component. </summary>
        /// <param name="leftMargin"> New left margin size. </param>
        private void SetSideBarMargin(double leftMargin)
        {
            //  Get current margin.
            var currentMargin = this.Margin;

            //  Setup new margin.
            this.Margin = new Thickness(leftMargin, 0, 0, currentMargin.Bottom);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Switch background mode to full or limited. </summary>
        /// <param name="fullBackground"> True - full background mode; False - limited background mode. </param>
        private void SwitchBackground(bool fullBackground)
        {
            //  Get current brush.
            var currentBrush = _fullBackground ? this.Background : ContainerGrid.Background;

            //  Switch brushes.
            this.Background = fullBackground ? currentBrush : null;
            ContainerGrid.Background = fullBackground ? null : currentBrush;

            //  Set full background variable.
            _fullBackground = fullBackground;
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region MENU INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selection any list view item in menu list view. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Selection changed event arguments. </param>
        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //  Cast sender into list view object.
            var listView = (ListView)sender;

            //  Clear list view selection.
            listView.SelectedIndex = -1;
            listView.SelectedItems.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting appearance menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void AppearanceMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadAppearanceSubPage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting player menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void PlayerMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadPlayerSubPage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting visualisation menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void VisualisationMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadVisualisationSubPage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after selecting informations menu list view item. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InfoMenuListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            LoadInformationsSubPage();
        }

        #endregion MENU INTERACTION METHODS

        #region PAGES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load appearance page. </summary>
        private void LoadAppearanceSubPage()
        {
            var appearanceSettingsPage = new AppearanceSettingsPage();
            _pagesManager.LoadPage(appearanceSettingsPage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load player page. </summary>
        private void LoadPlayerSubPage()
        {
            var playerSettingsPage = new PlayerSettingsPage();
            _pagesManager.LoadPage(playerSettingsPage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load visualisations page. </summary>
        private void LoadVisualisationSubPage()
        {
            var visualisationSettingsPage = new VisualisationSettingsPage();
            _pagesManager.LoadPage(visualisationSettingsPage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and load informations page. </summary>
        private void LoadInformationsSubPage()
        {
            var infoSettingsPage = new InfoSettingsPage();
            _pagesManager.LoadPage(infoSettingsPage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //  Setup pages manager.
            _pagesManager = new SinglePagesManager(SettingsFrame);
            _pagesManager.KeepPreviousPages = false;

            //  Load first page.
            LoadAppearanceSubPage();
        }

        #endregion PAGES METHODS

    }
}
