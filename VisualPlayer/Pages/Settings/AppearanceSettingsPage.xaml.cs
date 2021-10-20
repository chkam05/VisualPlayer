using chkam05.VisualPlayer.Base;
using chkam05.VisualPlayer.Data.Configuration;
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
    public partial class AppearanceSettingsPage : Page
    {

        //  VARIABLES

        private CheckBox _focusedCheckBox;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AppearanceSettingsPage class constructor. </summary>
        public AppearanceSettingsPage()
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

            SystemColorCheckBox.IsChecked = config.UseSystemColor;
            ShowCustomColorSelection(!config.UseSystemColor);
            ThemeColorsPicker.Color = config.ThemeColor;
            ThemeColorsPicker.UsedColors = config.UsedThemeColors;
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
        /// <summary> Method called when system color checkbox checked is changed. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (_focusedCheckBox == checkBox)
            {
                var checkedValue = checkBox.IsChecked ?? false;
                var configManager = ConfigManager.Instance;

                ShowCustomColorSelection(!checkedValue);

                //  Update configuration.
                configManager.Config.UseSystemColor = checkedValue;
                configManager.InvokeConfigUpdate<AppConfig>("UseSystemColor");
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when user change color in theme color picker. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Selected color. </param>
        private void ThemeColorsPicker_OnColorChange(object sender, Color e)
        {
            var configManager = ConfigManager.Instance;
            var config = configManager.Config;

            config.ThemeColor = ThemeColorsPicker.Color;
            config.UsedThemeColors = ThemeColorsPicker.UsedColors;
            configManager.InvokeConfigUpdate<AppConfig>("ThemeColor");
        }

        #endregion INTERFACE INTERACTION METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show/Hide custom color selection components. </summary>
        /// <param name="showed"> Show/Hide option. </param>
        private void ShowCustomColorSelection(bool showed)
        {
            ThemeColorsPicker.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfiguration();
        }

        #endregion PAGE METHODS

    }
}
