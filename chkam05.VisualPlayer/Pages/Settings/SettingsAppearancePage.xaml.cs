using chkam05.Tools.ControlsEx.Colors;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Data.Configuration;
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

using AppearanceColorType = chkam05.VisualPlayer.Data.Config.AppearanceColorType;


namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class SettingsAppearancePage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<AppearanceColorType> _appearanceColorTypes;
        private ObservableCollection<AppearanceCustomThemeType> _appearanceCustomThemeTypes;
        private ObservableCollection<AppearanceThemeType> _appearanceThemeTypes;
        private ObservableCollection<ColorPaletteItem> _appearanceUsedCustomColors;
        private ObservableCollection<InformationBarAutoHide> _infoBarAutoHides;

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public ObservableCollection<AppearanceColorType> AppearanceColorTypes
        {
            get => _appearanceColorTypes;
            set
            {
                _appearanceColorTypes = value;
                OnPropertyChanged(nameof(AppearanceColorTypes));
            }
        }

        public ObservableCollection<AppearanceThemeType> AppearanceThemeTypes
        {
            get => _appearanceThemeTypes;
            set
            {
                _appearanceThemeTypes = value;
                OnPropertyChanged(nameof(AppearanceThemeTypes));
            }
        }

        public ObservableCollection<AppearanceCustomThemeType> AppearanceCustomThemeTypes
        {
            get => _appearanceCustomThemeTypes;
            set
            {
                _appearanceCustomThemeTypes = value;
                OnPropertyChanged(nameof(AppearanceCustomThemeTypes));
            }
        }

        public ObservableCollection<ColorPaletteItem> AppearanceUsedCustomColors
        {
            get => _appearanceUsedCustomColors;
            set
            {
                _appearanceUsedCustomColors = value;
                OnPropertyChanged(nameof(AppearanceUsedCustomColors));
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
        /// <summary> SettingsAppearancePage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsAppearancePage(IPagesManager pagesManager)
        {
            //  Setup data containers.
            SetupDataContainers();

            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            AppearanceUsedCustomColors = new ObservableCollection<ColorPaletteItem>(
                ConfigManager.UsedColors.Select(i => i.ToColorPaletteItem()));

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

            AppearanceCustomThemeTypes = new ObservableCollection<AppearanceCustomThemeType>(
                EnumUtilities.ListOf<AppearanceCustomThemeType>());

            AppearanceThemeTypes = new ObservableCollection<AppearanceThemeType>(
                EnumUtilities.ListOf<AppearanceThemeType>());

            InfoBarAutoHides = new ObservableCollection<InformationBarAutoHide>(
                EnumUtilities.ListOf<InformationBarAutoHide>());
        }

        #endregion SETUP METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during page unloading. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //  Save configuration.
            ConfigManager.UsedColors = AppearanceUsedCustomColors.Select(i => new ColorInfo(i)).ToList();
            ConfigManager.SaveConfiguration();
        }

        #endregion PAGE METHODS

    }
}
