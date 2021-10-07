using chkam05.Visualisations;
using chkam05.VisualPlayer.Base;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Utilities;
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

namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class VisualisationSettingsPage : Page
    {

        //  VARIABLES

        private CheckBox _focusedCheckBox;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationSettingsPage class constructor. </summary>
        public VisualisationSettingsPage()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region CONFIGURATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration to components. </summary>
        private void LoadConfiguration()
        {
            var config = ConfigManager.Instance.Config;

            //  Configuration.
            var themeBrush = new SolidColorBrush(config.ThemeColor);

            EnableVisualisationCheckBox.Background = themeBrush;
            ShowLogoCheckBox.Background = themeBrush;

            //  Settings.
            EnableVisualisationCheckBox.IsChecked = config.VisualisationEnabled;
            UpdateVisualisationEnabledConfiguration(config.VisualisationEnabled);
            VisualisationTypeComboBox.SelectedItemIndex = EnumTool<VisualisationType>.GetIndex(config.VisualisationType);
            ShowLogoCheckBox.IsChecked = config.VisualisationLogoEnabled;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Fill visualisations combobox selector with visualisation types. </summary>
        private void SetupVisualisationTypesCheckBox()
        {
            VisualisationTypeComboBox.Items = EnumTool<VisualisationType>.Names;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visualisation configuration components to enable config. </summary>
        /// <param name="enabled"> Enable/disable visualisation config components. </param>
        private void UpdateVisualisationEnabledConfiguration(bool enabled)
        {
            VisualisationTypeComboBox.IsEnabled = enabled;
        }

        #endregion CONFIGURATION METHODS

        #region INTERFACE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when checkbox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CheckBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            _focusedCheckBox = (CheckBox)sender;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when checkbox is unselected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CheckBoxes_LostFocus(object sender, RoutedEventArgs e)
        {
            _focusedCheckBox = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when checkbox checked is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CheckBoxes_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (_focusedCheckBox != null && _focusedCheckBox == checkBox)
            {
                var checkedValue = checkBox.IsChecked ?? false;
                var configManager = ConfigManager.Instance;

                if (checkBox == EnableVisualisationCheckBox)
                {
                    UpdateVisualisationEnabledConfiguration(checkedValue);

                    //  Update configuration.
                    configManager.Config.VisualisationEnabled = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("VisualisationEnabled");
                }

                else if (checkBox == ShowLogoCheckBox)
                {
                    //  Update configuration.
                    configManager.Config.VisualisationLogoEnabled = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("VisualisationLogoEnabled");
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when visualisation typs combobox selected item is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Index of newly selected item. </param>
        private void VisualisationTypeComboBox_OnItemSelect(object sender, int e)
        {
            var configManager = ConfigManager.Instance;
            int typesCount = EnumTool<VisualisationType>.Count;
            int typeIndex = e >= 0 && e < typesCount ? e : 0;

            //  Update configuration.
            configManager.Config.VisualisationType = EnumTool<VisualisationType>.GetByIndex(typeIndex);
            configManager.InvokeConfigUpdate<AppConfig>("VisualisationType");
        }

        #endregion INTERFACE INTERACTION METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetupVisualisationTypesCheckBox();
            LoadConfiguration();
        }

        #endregion PAGE METHODS

    }
}
