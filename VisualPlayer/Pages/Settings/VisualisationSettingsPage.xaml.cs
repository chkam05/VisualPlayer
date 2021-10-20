using chkam05.Visualisations;
using chkam05.VisualPlayer.Base;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.States;
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
            CustomColorVisualisationCheckBox.IsChecked = config.VisualisationColorMode == VisualisationColorMode.CUSTOM;
            ShowCustomColorSelection(config.VisualisationColorMode == VisualisationColorMode.CUSTOM);
            VisualisationColorsPicker.Color = config.VisualisationColor;
            VisualisationColorsPicker.UsedColors = config.UsedVisualisationColors;
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
        /// <summary> Method called when enable visualisation checkbox checked is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void EnableVisualisationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (_focusedCheckBox == checkBox)
            {
                var checkedValue = checkBox.IsChecked ?? false;
                var configManager = ConfigManager.Instance;

                UpdateVisualisationEnabledConfiguration(checkedValue);

                //  Update configuration.
                configManager.Config.VisualisationEnabled = checkedValue;
                configManager.InvokeConfigUpdate<AppConfig>("VisualisationEnabled");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when custom color visualisation checkbox checked is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CustomColorVisualisationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (_focusedCheckBox == checkBox)
            {
                var checkedValue = checkBox.IsChecked ?? false;
                var configManager = ConfigManager.Instance;
                var colorMode = checkedValue ? VisualisationColorMode.CUSTOM : VisualisationColorMode.APPLICATION;

                ShowCustomColorSelection(colorMode == VisualisationColorMode.CUSTOM);

                //  Update configuration.
                configManager.Config.VisualisationColorMode = colorMode;
                configManager.InvokeConfigUpdate<AppConfig>("VisualisationColorMode");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when show logo checkbox checked is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ShowLogoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (_focusedCheckBox == checkBox)
            {
                var configManager = ConfigManager.Instance;

                //  Update configuration.
                configManager.Config.VisualisationLogoEnabled = checkBox.IsChecked ?? false;
                configManager.InvokeConfigUpdate<AppConfig>("VisualisationLogoEnabled");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when user change color in visualisation color picker. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Selected color. </param>
        private void VisualisationColorsPicker_OnColorChange(object sender, Color e)
        {
            var configManager = ConfigManager.Instance;
            var config = configManager.Config;

            config.VisualisationColor = VisualisationColorsPicker.Color;
            config.UsedVisualisationColors = VisualisationColorsPicker.UsedColors;
            configManager.InvokeConfigUpdate<AppConfig>("VisualisationColor");
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

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show/Hide custom color selection components. </summary>
        /// <param name="showed"> Show/Hide option. </param>
        private void ShowCustomColorSelection(bool showed)
        {
            VisualisationColorsPicker.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion INTERFACE MANAGEMENT METHODS

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
