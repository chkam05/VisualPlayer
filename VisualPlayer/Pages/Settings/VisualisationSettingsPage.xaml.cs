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
        private List<Border> _usedColors;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VisualisationSettingsPage class constructor. </summary>
        public VisualisationSettingsPage()
        {
            InitializeComponent();

            //  Make used color borders alias.
            _usedColors = new List<Border>
            {
                UsedColor01Border, UsedColor02Border, UsedColor03Border, UsedColor04Border, UsedColor05Border
            };
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
            CustomColorVisualisationCheckBox.IsChecked = config.VisualisationColorMode == ColorMode.CUSTOM;
            ShowCustomColorSelection(config.VisualisationColorMode == ColorMode.CUSTOM);
            SetSelectedColors(config.UsedVisualisationColors);
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

                else if (checkBox == CustomColorVisualisationCheckBox)
                {
                    var colorMode = checkedValue ? ColorMode.CUSTOM : ColorMode.APPLICATION;
                    ShowCustomColorSelection(colorMode == ColorMode.CUSTOM);

                    //  Update configuration.
                    configManager.Config.VisualisationColorMode = colorMode;
                    configManager.InvokeConfigUpdate<AppConfig>("VisualisationColorMode");
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

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after releasing mouse from theme color changing border component. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ThemeColorBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CustomColorVisualisationCheckBox.IsChecked ?? false)
                return;

            if (e.LeftButton == MouseButtonState.Released)
            {
                var border = (Border)sender;
                var brush = (SolidColorBrush)border.Background;
                var color = brush.Color;

                //  Update visual component.
                AddSelectedColor(color);

                //  Update configuration.
                var configManager = ConfigManager.Instance;
                configManager.Config.VisualisationColor = color;
                configManager.InvokeConfigUpdate<AppConfig>("VisualisationColor");

                SaveSelectedColors(configManager);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking add custom color control button to select custom visualisation color. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void AddCustomColorControlButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion INTERFACE INTERACTION METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show/Hide custom color selection components. </summary>
        /// <param name="showed"> Show/Hide option. </param>
        private void ShowCustomColorSelection(bool showed)
        {
            UsedColorsTextBlock.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
            UsedColorsWrapPanel.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
            PalleteColorsTextBlock.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
            PalleteColorsWrapPanel.Visibility = showed ? Visibility.Visible : Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add new color to used colors in interface. </summary>
        /// <param name="color"> New color to add. </param>
        private void AddSelectedColor(Color color)
        {
            Brush thisBrush = new SolidColorBrush(color);
            Brush prevBrush = thisBrush;

            foreach (var border in _usedColors)
            {
                //  Set color.
                var brush = border.Background;
                border.Background = prevBrush;
                prevBrush = brush;

                try
                {
                    if (prevBrush != null && (prevBrush as SolidColorBrush).Color == (thisBrush as SolidColorBrush).Color)
                        return;
                }
                catch
                {
                    continue;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load selected colors from list. </summary>
        /// <param name="colors"> List of selected colors. </param>
        private void SetSelectedColors(List<Color> colors)
        {
            int counter = 0;

            foreach (var border in _usedColors)
            {
                //  Get color.
                var brush = counter < colors.Count
                    ? new SolidColorBrush(colors[counter])
                    : null;

                //  Set color.
                border.Background = brush;

                counter++;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save selected colors to list. </summary>
        /// <param name="configManager"> Configuration manager instance. </param>
        private void SaveSelectedColors(ConfigManager configManager)
        {
            configManager.Config.UsedVisualisationColors.Clear();

            foreach (var border in _usedColors)
            {
                //  Get brush.
                var brush = border.Background;

                //  Try save color.
                try
                {
                    if (brush != null)
                    {
                        var color = (brush as SolidColorBrush).Color;
                        configManager.Config.UsedVisualisationColors.Add(color);
                    }
                }
                catch
                {
                    //  Continue
                }
            }
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
