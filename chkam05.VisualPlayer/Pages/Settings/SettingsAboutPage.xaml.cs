using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
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

namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class SettingsAboutPage : Page, IPage, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty AboutCopyrightProperty = DependencyProperty.Register(
            nameof(AboutCopyright),
            typeof(string),
            typeof(SettingsAboutPage),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty AboutTitleProperty = DependencyProperty.Register(
            nameof(AboutTitle),
            typeof(string),
            typeof(SettingsAboutPage),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty AboutVersionProperty = DependencyProperty.Register(
            nameof(AboutVersion),
            typeof(string),
            typeof(SettingsAboutPage),
            new PropertyMetadata(string.Empty));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public string AboutCopyright
        {
            get => (string)GetValue(AboutCopyrightProperty);
            private set
            {
                SetValue(AboutCopyrightProperty, value);
                OnPropertyChanged(nameof(AboutCopyright));
            }
        }

        public string AboutTitle
        {
            get => (string)GetValue(AboutTitleProperty);
            private set
            {
                SetValue(AboutTitleProperty, value);
                OnPropertyChanged(nameof(AboutTitle));
            }
        }

        public string AboutVersion
        {
            get => (string)GetValue(AboutVersionProperty);
            private set
            {
                SetValue(AboutVersionProperty, value);
                OnPropertyChanged(nameof(AboutVersion));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsAppearancePage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsAboutPage(IPagesManager pagesManager)
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;

            AboutCopyright = ApplicationHelper.Instance.GetApplicationCopyright();
            AboutTitle = ApplicationHelper.Instance.GetApplicationTitle();
            AboutVersion = ApplicationHelper.Instance.GetApplicationVersion().ToString();
        }

        #endregion CLASS METHODS

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Back ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.CanGoBack)
                PagesManager.GoBack();
            else
                PagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Close ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PagesManager.HideInterface();
        }

        #endregion CONTROL BUTTONS METHODS

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

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during page unloading. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion PAGE METHODS

    }
}
