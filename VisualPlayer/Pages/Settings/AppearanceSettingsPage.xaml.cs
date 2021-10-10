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
        private List<Border> _usedColors;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AppearanceSettingsPage class constructor. </summary>
        public AppearanceSettingsPage()
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

            SystemColorCheckBox.IsChecked = config.UseSystemColor;
            ShowCustomColorSelection(!config.UseSystemColor);
            SetSelectedColors(config.UsedThemeColors);
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

                if (checkBox == SystemColorCheckBox)
                {
                    ShowCustomColorSelection(!checkedValue);

                    //  Update configuration.
                    configManager.Config.UseSystemColor = checkedValue;
                    configManager.InvokeConfigUpdate<AppConfig>("UseSystemColor");
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after releasing mouse from theme color changing border component. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ThemeColorBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SystemColorCheckBox.IsChecked ?? false)
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
                configManager.Config.ThemeColor = color;
                configManager.InvokeConfigUpdate<AppConfig>("ThemeColor");

                SaveSelectedColors(configManager);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking add custom color control button to select custom theme color. </summary>
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
            configManager.Config.UsedThemeColors.Clear();

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
                        configManager.Config.UsedThemeColors.Add(color);
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
            LoadConfiguration();
        }

        #endregion PAGE METHODS

    }
}
