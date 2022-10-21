using chkam05.Tools.ControlsEx.Events;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class SettingsInfoBarPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<FontContainer> _fonts;
        private ObservableCollection<FontStyle> _fontStyles;
        private ObservableCollection<FontStretch> _fontStretches;
        private ObservableCollection<FontWeight> _fontWeights;
        private ObservableCollection<MarqueeState> _titleMarqueeStates;
        private ObservableCollection<InformationBarAutoHide> _infoBarAutoHides;

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public ObservableCollection<FontContainer> Fonts
        {
            get => _fonts;
            private set
            {
                _fonts = value;
                OnPropertyChanged(nameof(Fonts));
            }
        }

        public ObservableCollection<FontStyle> FontStyles
        {
            get => _fontStyles;
            private set
            {
                _fontStyles = value;
                OnPropertyChanged(nameof(FontStyles));
            }
        }

        public ObservableCollection<FontStretch> FontStretches
        {
            get => _fontStretches;
            private set
            {
                _fontStretches = value;
                OnPropertyChanged(nameof(FontStretches));
            }
        }

        public ObservableCollection<FontWeight> FontWeights
        {
            get => _fontWeights;
            private set
            {
                _fontWeights = value;
                OnPropertyChanged(nameof(FontWeights));
            }
        }

        public ObservableCollection<MarqueeState> TitleMarqueeStates
        {
            get => _titleMarqueeStates;
            set
            {
                _titleMarqueeStates = value;
                OnPropertyChanged(nameof(TitleMarqueeStates));
            }
        }

        public ObservableCollection<InformationBarAutoHide> InfoBarAutoHides
        {
            get => _infoBarAutoHides;
            set
            {
                _infoBarAutoHides = value;
                OnPropertyChanged(nameof(InfoBarAutoHides));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsInfoBarPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsInfoBarPage(IPagesManager pagesManager)
        {
            //  Setup data containers.
            SetupDataContainers();

            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;
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

        #region FONT MODIFICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing font size in UpDownTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Text Modified Event Arguments. </param>
        private void InformationBarFontSizeChanged(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
                ConfigManager.InformationBarFontSize = int.Parse(e.NewText);
        }

        #endregion FONT MODIFICATION METHODS

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
            //  Save configuration.
            ConfigManager.SaveConfiguration();
        }

        #endregion PAGE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            Fonts = FontsManager.Instance.Fonts;
            FontStyles = new ObservableCollection<FontStyle>(FontsManager.GetStyles());
            FontStretches = new ObservableCollection<FontStretch>(FontsManager.GetStretches());
            FontWeights = new ObservableCollection<FontWeight>(FontsManager.GetWeights());

            TitleMarqueeStates = new ObservableCollection<MarqueeState>(
                EnumUtilities.ListOf<MarqueeState>());

            InfoBarAutoHides = new ObservableCollection<InformationBarAutoHide>(
                EnumUtilities.ListOf<InformationBarAutoHide>());
        }

        #endregion SETUP METHODS

    }
}
