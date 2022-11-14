using chkam05.Tools.ControlsEx.Events;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Data.Lyrics;
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
    public partial class SettingsLyricsPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<string> _fonts;
        private ObservableCollection<FontStyle> _fontStyles;
        private ObservableCollection<FontStretch> _fontStretches;
        private ObservableCollection<FontWeight> _fontWeights;
        private ObservableCollection<LyricsMatchingType> _lyricsMatchTypes;

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public ObservableCollection<string> Fonts
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

        public ObservableCollection<LyricsMatchingType> LyricsMatchTypes
        {
            get => _lyricsMatchTypes;
            private set
            {
                _lyricsMatchTypes = value;
                OnPropertyChanged(nameof(LyricsMatchTypes));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsLyricsPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsLyricsPage(IPagesManager pagesManager)
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
        private void LyricsFontSizeChanged(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified)
                ConfigManager.LyricsFontSize = int.Parse(e.NewText);
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
            Fonts = new ObservableCollection<string>(FontsManager.Instance.Fonts.Select(f => f.ToString()));
            FontStyles = new ObservableCollection<FontStyle>(FontsManager.GetStyles());
            FontStretches = new ObservableCollection<FontStretch>(FontsManager.GetStretches());
            FontWeights = new ObservableCollection<FontWeight>(FontsManager.GetWeights());

            LyricsMatchTypes = new ObservableCollection<LyricsMatchingType>(
                EnumUtilities.ListOf<LyricsMatchingType>());
        }

        #endregion SETUP METHODS

    }
}
