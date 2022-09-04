using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations.Data;
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

namespace chkam05.VisualPlayer.Pages.Settings_old
{
    public partial class SettingsOldVisualisationPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<ColorInfo> _staticCustomColors;
        private ObservableCollection<VisualisationColorType> _visualisationColorTypes;
        private ObservableCollection<ScalingStrategy> _visualisationScalingTypes;
        private ObservableCollection<VisualisationType> _visualisationTypes;

        public Configuration Configuration { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public MenuItemType? SpecialMenu
        {
            get => null;
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

        public ObservableCollection<VisualisationColorType> VisualisationColorTypes
        {
            get => _visualisationColorTypes;
            private set
            {
                _visualisationColorTypes = value;
                OnPropertyChanged(nameof(VisualisationColorTypes));
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

        public ObservableCollection<VisualisationType> VisualisationTypes
        {
            get => _visualisationTypes;
            private set
            {
                _visualisationTypes = value;
                OnPropertyChanged(nameof(VisualisationTypes));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsVisualisationPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public SettingsOldVisualisationPage(IPagesManager pagesManager)
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
            Configuration.UpdateVisualisationColor();
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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            StaticCustomColors = new ObservableCollection<ColorInfo>(
                ColorUtilities.StaticColors.OrderByDescending(o => o.ColorCode));

            VisualisationColorTypes = new ObservableCollection<VisualisationColorType>(
                EnumUtilities.ListOf<VisualisationColorType>());

            VisualisationScalingTypes = new ObservableCollection<ScalingStrategy>()
            {
                ScalingStrategy.LINEAR,
                ScalingStrategy.SQRT
            };

            VisualisationTypes = new ObservableCollection<VisualisationType>(
                EnumUtilities.ListOf<VisualisationType>());
        }

        #endregion SETUP METHODS

    }
}
