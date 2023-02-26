using chkam05.Tools.ControlsEx.Colors;
using chkam05.Tools.ControlsEx.Events;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Data;
using chkam05.VisualPlayer.Visualisations.Profiles;
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
    public partial class SettingsVisualisationPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private bool _lockVisualisationUpdate = true;
        private ObservableCollection<string> _visualisationProfiles;
        private string _visualisationProfileName;
        private string _visualisationProfileEditable;
        private ObservableCollection<VisualisationType> _visualisationTypes;
        private ObservableCollection<ScalingStrategy> _visualisationScalingTypes;
        private ObservableCollection<VisualisationColorType> _visualisationColorTypes;
        private ObservableCollection<ColorPaletteItem> _visualisationUsedBorderColors;
        private ObservableCollection<ColorPaletteItem> _visualisationUsedFillColors;

        public ConfigManager ConfigManager { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => MenuItemType.SETTINGS_MENU;
        }

        public ObservableCollection<string> VisualisationProfiles
        {
            get => _visualisationProfiles;
            set
            {
                _visualisationProfiles = value;
                OnPropertyChanged(nameof(VisualisationProfiles));
            }
        }

        public string VisualisationProfileName
        {
            get => _visualisationProfileName;
            set
            {
                _visualisationProfileName = value;
                OnPropertyChanged(nameof(VisualisationProfileName));

                if (!_lockVisualisationUpdate)
                {
                    ConfigManager.VisualisationProfilesManager.SelectProfile(value);
                    string profileName = ConfigManager.VisualisationProfilesManager.Profile.Name;
                    ConfigManager.VisualisationProfileName = profileName;
                    VisualisationProfileNameEditable = profileName;
                }
            }
        }
        
        public string VisualisationProfileNameEditable
        {
            get => _visualisationProfileEditable;
            set
            {
                _visualisationProfileEditable = value;
                OnPropertyChanged(nameof(VisualisationProfileNameEditable));
            }
        }

        public ObservableCollection<VisualisationType> VisualisationTypes
        {
            get => _visualisationTypes;
            private set
            {
                _visualisationTypes = value;
                OnPropertyChanged(nameof(VisualisationTypes));
            }
        }

        public ObservableCollection<ScalingStrategy> VisualisationScalingTypes
        {
            get => _visualisationScalingTypes;
            private set
            {
                _visualisationScalingTypes = value;
                OnPropertyChanged(nameof(VisualisationScalingTypes));
            }
        }

        public ObservableCollection<VisualisationColorType> VisualisationColorTypes
        {
            get => _visualisationColorTypes;
            private set
            {
                _visualisationColorTypes = value;
                OnPropertyChanged(nameof(VisualisationColorTypes));
            }
        }

        public ObservableCollection<ColorPaletteItem> VisualisationUsedBorderColors
        {
            get => _visualisationUsedBorderColors;
            set
            {
                _visualisationUsedBorderColors = value;
                OnPropertyChanged(nameof(VisualisationUsedBorderColors));
            }
        }

        public ObservableCollection<ColorPaletteItem> VisualisationUsedFillColors
        {
            get => _visualisationUsedFillColors;
            set
            {
                _visualisationUsedFillColors = value;
                OnPropertyChanged(nameof(VisualisationUsedFillColors));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsVisualisationPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsVisualisationPage(IPagesManager pagesManager)
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Setup data containers.
            SetupDataContainers();

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            PagesManager = pagesManager;
        }

        #endregion CLASS METHODS

        #region COLOR PALETTES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing color in visualisation fill color ColorsPalette. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Colors Palette Selection Changed Event Arguments. </param>
        private void VisualisationColorFillSelectionChanged(object sender, ColorsPaletteSelectionChangedEventArgs e)
        {
            if (e.SelectedColorItem != null)
                ConfigManager.VisualisationColor = e.SelectedColorItem.Color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing color in visualisation border color ColorsPalette. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Colors Palette Selection Changed Event Arguments. </param>
        private void VisualisationColorBorderSelectionChanged(object sender, ColorsPaletteSelectionChangedEventArgs e)
        {
            if (e.SelectedColorItem != null)
                ConfigManager.VisualisationBorderColor = e.SelectedColorItem.Color;
        }

        #endregion COLOR PALETTE METHODS

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

        #region VISUALISATION PROFILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Create Profile Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CreateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _lockVisualisationUpdate = true;
            ConfigManager.VisualisationProfilesManager.CreateProfile();
            var profileName = ConfigManager.VisualisationProfilesManager.Profile.Name;
            UpdateVisualisationProfilesList();
            UpdateVisualisationProfileNames(profileName);
            _lockVisualisationUpdate = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Remove Profile Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _lockVisualisationUpdate = true;
            ConfigManager.VisualisationProfilesManager.RemoveCurrentProfile();
            var profileName = ConfigManager.VisualisationProfilesManager.Profile.Name;
            UpdateVisualisationProfilesList();
            UpdateVisualisationProfileNames(profileName);
            _lockVisualisationUpdate = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update list of visualisation profiles. </summary>
        private void UpdateVisualisationProfilesList()
        {
            VisualisationProfiles = new ObservableCollection<string>(
                VisualisationProfilesManager.GetProfilesNamesList());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation selected profile name. </summary>
        /// <param name="profileName"> Selected profile name. </param>
        private void UpdateVisualisationProfileNames(string profileName)
        {
            ConfigManager.VisualisationProfileName = profileName;
            VisualisationProfileName = profileName;
            VisualisationProfileNameEditable = profileName;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing name of visualisation profile. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Text Modified Event Arguments. </param>
        private void VisualisationProfileNameTextModified(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified && !_lockVisualisationUpdate)
            {
                _lockVisualisationUpdate = true;

                if (ConfigManager.VisualisationProfilesManager.RenameCurrentProfile(e.NewText))
                {
                    var profileName = ConfigManager.VisualisationProfilesManager.Profile.Name;

                    UpdateVisualisationProfilesList();
                    UpdateVisualisationProfileNames(profileName);
                }

                _lockVisualisationUpdate = false;
            }
        }

        #endregion VISUALISATION PROFILES MANAGEMENT METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            _lockVisualisationUpdate = true;

            UpdateVisualisationProfilesList();
            VisualisationProfileName = ConfigManager.VisualisationProfileName;
            VisualisationProfileNameEditable = ConfigManager.VisualisationProfileName;

            VisualisationTypes = new ObservableCollection<VisualisationType>(
                EnumUtilities.ListOf<VisualisationType>());

            VisualisationScalingTypes = new ObservableCollection<ScalingStrategy>(
                EnumUtilities.ListOf<ScalingStrategy>());

            VisualisationColorTypes = new ObservableCollection<VisualisationColorType>(
                EnumUtilities.ListOf<VisualisationColorType>());

            VisualisationUsedBorderColors = new ObservableCollection<ColorPaletteItem>(
                ConfigManager.VisualisationUsedBorderColors.Select(i => i.ToColorPaletteItem()));

            VisualisationUsedFillColors = new ObservableCollection<ColorPaletteItem>(
                ConfigManager.VisualisationUsedFillColors.Select(i => i.ToColorPaletteItem()));

            _lockVisualisationUpdate = false;
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
            ConfigManager.VisualisationUsedBorderColors = VisualisationUsedBorderColors.Select(i => new ColorInfo(i)).ToList();
            ConfigManager.VisualisationUsedFillColors = VisualisationUsedFillColors.Select(i => new ColorInfo(i)).ToList();
            ConfigManager.SaveConfiguration();
        }

        #endregion PAGE METHODS

    }
}
