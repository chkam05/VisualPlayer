using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class SettingsAppearancePage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<AppearanceColorType> _appearanceColorTypes;
        private ObservableCollection<AppearanceLogoColorType> _appearanceLogoColorTypes;
        private ObservableCollection<AppearanceThemeType> _appearanceThemeTypes;
        private ObservableCollection<FontConfigStyle> _fontConfigStyles;
        private ObservableCollection<MarqueeState> _titleMarqueeStates;
        private ObservableCollection<ColorInfo> _staticCustomColors;

        public Configuration Configuration { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => null;
        }

        public ObservableCollection<AppearanceColorType> AppearanceColorTypes
        {
            get => _appearanceColorTypes;
            private set
            {
                _appearanceColorTypes = value;
                OnPropertyChanged(nameof(AppearanceColorTypes));
            }
        }

        public ObservableCollection<AppearanceLogoColorType> AppearanceLogoColorTypes
        {
            get => _appearanceLogoColorTypes;
            private set
            {
                _appearanceLogoColorTypes = value;
                OnPropertyChanged(nameof(AppearanceLogoColorTypes));
            }
        }

        public ObservableCollection<AppearanceThemeType> AppearanceThemeTypes
        {
            get => _appearanceThemeTypes;
            private set
            {
                _appearanceThemeTypes = value;
                OnPropertyChanged(nameof(AppearanceThemeTypes));
            }
        }

        public ObservableCollection<FontConfigStyle> FontConfigStyles
        {
            get => _fontConfigStyles;
            set
            {
                _fontConfigStyles = value;
                OnPropertyChanged(nameof(FontConfigStyles));
            }
        }

        public ObservableCollection<ColorInfo> StaticCustomColors
        {
            get => _staticCustomColors;
            private set
            {
                _staticCustomColors = value;
                OnPropertyChanged(nameof(StaticCustomColors));
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


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsAppearancePage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsAppearancePage(IPagesManager pagesManager)
        {
            //  Setup data containers.
            SetupDataContainers();

            //  Setup modules.
            Configuration = Configuration.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;
        }

        #endregion CLASS METHODS

        #region COLOR SELECTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting color in ColorsPalette. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Palette Color Update Event Arguments. </param>
        private void ColorsPaletteControl_OnColorUpdate(object sender, PaletteColorUpdateEventArgs e)
        {
            Configuration.UpdateAccentColor();
        }

        #endregion COLOR SELECTION METHODS

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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            AppearanceColorTypes = new ObservableCollection<AppearanceColorType>(
                EnumUtilities.ListOf<AppearanceColorType>());

            AppearanceLogoColorTypes = new ObservableCollection<AppearanceLogoColorType>(
                EnumUtilities.ListOf<AppearanceLogoColorType>());

            AppearanceThemeTypes = new ObservableCollection<AppearanceThemeType>(
                EnumUtilities.ListOf<AppearanceThemeType>());

            FontConfigStyles = new ObservableCollection<FontConfigStyle>(
                EnumUtilities.ListOf<FontConfigStyle>());

            StaticCustomColors = new ObservableCollection<ColorInfo>(
                ColorUtilities.StaticColors.OrderByDescending(o => o.ColorCode));

            TitleMarqueeStates = new ObservableCollection<MarqueeState>(
                EnumUtilities.ListOf<MarqueeState>());
        }

        #endregion SETUP METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //  Save configuration.
            Configuration.Save();
        }

        #endregion PAGE METHODS

    }
}
