using chkam05.VisualPlayer.Base;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Components.EventArgs;
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
    public partial class PlayerSettingsPage : Page
    {

        //  VARIABLES

        private CheckBox _focusedCheckBox;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayerSettingsPage class constructor. </summary>
        public PlayerSettingsPage()
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

            AutoPlayOnAddCheckBox.Background = themeBrush;
            KeepPlayListAfterRestartCheckBox.Background = themeBrush;
            ShowOSDCheckBox.Background = themeBrush;
            OSDTimeSlider.HandlerBrush = themeBrush;

            //  Settings.
            AutoPlayOnAddCheckBox.IsChecked = config.PlayerAutoPlayOnAdd;
            KeepPlayListAfterRestartCheckBox.IsChecked = config.PlayerKeepPlayListOnRestart;
            ShowOSDCheckBox.IsChecked = config.PlayerOSDEnabled;
            UpdateOSDEnabledConfiguration(config.PlayerOSDEnabled);
            OSDTimeSlider.Value = config.PlayerOSDTime < 0 ? OSDTimeSlider.Max : (config.PlayerOSDTime / 1000);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update OSD configuration components to enable config. </summary>
        /// <param name="enabled"> Enable/disable OSD config components. </param>
        private void UpdateOSDEnabledConfiguration(bool enabled)
        {
            OSDTimeSlider.IsEnabled = enabled;
        }

        #endregion CONFIGURATION METHODS

        #region INTERFACE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when checkbox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CheckBoxes_GotFocus(object sender, RoutedEventArgs e)
        {
            _focusedCheckBox = (CheckBox) sender;
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
            var checkBox = (CheckBox) sender;

            if (_focusedCheckBox != null && _focusedCheckBox == checkBox)
            {
                var checkedValue = checkBox.IsChecked ?? false;
                var configManager = ConfigManager.Instance;

                if (checkBox == AutoPlayOnAddCheckBox)
                {
                    //  Update configuration.
                    configManager.Config.PlayerAutoPlayOnAdd = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("PlayerAutoPlayOnAdd");
                }

                else if (checkBox == KeepPlayListAfterRestartCheckBox)
                {
                    //  Update configuration.
                    configManager.Config.PlayerKeepPlayListOnRestart = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("PlayerKeepPlayListOnRestart");
                }

                else if (checkBox == ShowOSDCheckBox)
                {
                    UpdateOSDEnabledConfiguration(checkedValue);

                    //  Update configuration.
                    configManager.Config.PlayerOSDEnabled = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("PlayerOSDEnabled");
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when osd time slider value is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Slider drag event arguments. </param>
        private void OSDTimeSlider_OnHandlerDrag(object sender, Components.EventArgs.SliderDragEventArgs e)
        {
            var slider = (ScaledHorizontalSlider) sender;

            if (e.Action == SliderDragAction.ON_RELEASE)
            {
                var configManager = ConfigManager.Instance;
                bool isMax = e.Position == slider.Max;

                configManager.Config.PlayerOSDTime = !isMax ? e.Position * 1000 : -1;
                configManager.InvokeConfigUpdate<AppConfig>("PlayerOSDTime");
            }
        }

        #endregion INTERFACE INTERACTION METHODS

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
