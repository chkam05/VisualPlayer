using chkam05.Tools.ControlsEx.Events;
using chkam05.VisualPlayer.Controls.Static;
using chkam05.VisualPlayer.Core;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Lyrics;
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

namespace chkam05.VisualPlayer.Pages
{
    public partial class EqualizerPage : Page, IPage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private bool _lockEqualizerPresetUpdate = true;
        private ObservableCollection<string> _equalizerPresets;
        private string _equalizerPresetName;
        private string _equalizerPresetNameEditable;

        public ConfigManager ConfigManager { get; private set; }
        public Player Player { get; private set; }
        public IPagesManager PagesManager { get; private set; }


        //  GETTERS & SETTERS

        public ObservableCollection<string> EqualizerPresets
        {
            get => _equalizerPresets;
            set
            {
                _equalizerPresets = value;
                OnPropertyChanged(nameof(EqualizerPresets));
            }
        }

        public string EqualizerPresetName
        {
            get => _equalizerPresetName;
            set
            {
                _equalizerPresetName = value;
                OnPropertyChanged(nameof(EqualizerPresetName));

                if (!_lockEqualizerPresetUpdate)
                {
                    Player.EqualizerManager.SelectPreset(value);
                    string presetName = Player.EqualizerManager.Preset.Name;
                    ConfigManager.EqualizerPresetName = presetName;
                    EqualizerPresetNameEditable = presetName;
                }
            }
        }

        public string EqualizerPresetNameEditable
        {
            get => _equalizerPresetNameEditable;
            set
            {
                _equalizerPresetNameEditable = value;
                OnPropertyChanged(nameof(EqualizerPresetNameEditable));
            }
        }

        public MenuItemType? SpecialMenu
        {
            get => null;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> EqualizerPage class constructor. </summary>
        /// <param name="pagesManager"> Pages manager where page will be presented. </param>
        public EqualizerPage(IPagesManager pagesManager)
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;
            Player = Player.Instnace;

            //  Setup data containers.
            SetupDataContainers();

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

        #endregion CONTROL BUTTONS METHODS

        #region EQUALIZER PRESETS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing Equalizer Enable Switch position. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void EqualizerEnableSwitcherEx_Switched(object sender, RoutedEventArgs e)
        {
            ConfigManager.EqualizerEnabled = Player.EqualizerManager.Enabled;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Create Equalizer Preset Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CreateEqualizerPresetButton_Click(object sender, RoutedEventArgs e)
        {
            _lockEqualizerPresetUpdate = true;
            Player.EqualizerManager.CreatePreset();
            var presetName = Player.EqualizerManager.Preset.Name;
            UpdateEqualizerPresetsList();
            UpdateEqualizerPresetNames(presetName);
            _lockEqualizerPresetUpdate = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Remove Equalizer Preset Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RemoveEqualizerPresetButton_Click(object sender, RoutedEventArgs e)
        {
            _lockEqualizerPresetUpdate = true;
            Player.EqualizerManager.RemoveCurrentPreset();
            var presetName = Player.EqualizerManager.Preset.Name;
            UpdateEqualizerPresetsList();
            UpdateEqualizerPresetNames(presetName);
            _lockEqualizerPresetUpdate = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Reset Equalizer Preset Button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ResetEqualizerPresetButton_Click(object sender, RoutedEventArgs e)
        {
            _lockEqualizerPresetUpdate = true;
            Player.EqualizerManager.ResetCurrentPreset();
            _lockEqualizerPresetUpdate = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update list of equalizer presets. </summary>
        private void UpdateEqualizerPresetsList()
        {
            EqualizerPresets = new ObservableCollection<string>(
                EqualizerManager.GetPresetsNamesList());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update selected equalizer preset name. </summary>
        /// <param name="presetName"> Selected equalizer preset name. </param>
        private void UpdateEqualizerPresetNames(string presetName)
        {
            ConfigManager.EqualizerPresetName = presetName;
            EqualizerPresetName = presetName;
            EqualizerPresetNameEditable = presetName;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing name of equalizer preset. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Text Modified Event Arguments. </param>
        private void EqualizerPresetNameTextModified(object sender, TextModifiedEventArgs e)
        {
            if (e.UserModified && !_lockEqualizerPresetUpdate)
            {
                _lockEqualizerPresetUpdate = true;

                if (Player.EqualizerManager.RenameCurrentPreset(e.NewText))
                {
                    var presetName = Player.EqualizerManager.Preset.Name;

                    UpdateEqualizerPresetsList();
                    UpdateEqualizerPresetNames(presetName);
                }

                _lockEqualizerPresetUpdate = false;
            }
        }

        #endregion EQUALIZER PRESETS MANAGEMENT METHODS

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
        /// <summary> Method invoked after loading page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after closing page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Player.EqualizerManager.SaveCurrentPreset();
            ConfigManager.SaveConfiguration();
        }

        #endregion PAGE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup data containers. </summary>
        private void SetupDataContainers()
        {
            _lockEqualizerPresetUpdate = true;

            UpdateEqualizerPresetsList();
            EqualizerPresetName = ConfigManager.EqualizerPresetName;
            EqualizerPresetNameEditable = ConfigManager.EqualizerPresetName;

            _lockEqualizerPresetUpdate = false;
        }

        #endregion SETUP METHODS

    }
}
