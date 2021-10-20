using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

namespace chkam05.VisualPlayer.Components
{
    public partial class SettingsColorSelector : UserControl, INotifyPropertyChanged
    {

        //  EVENTS

        public event EventHandler<Color> OnColorChange;
        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private bool _colorPickerFoucused = false;

        private Color _colorPickerColor = Colors.Black;
        private Color _selectedColor;
        private List<Border> _usedColors;

        private byte _alpha = 255;
        private bool _enableColorsPallete = true;
        private bool _enableCustomColors = true;
        private bool _enableUsedColors = true;
        private bool _instantColorSelection = false;
        

        #region GETTERS & SETTERS

        public byte Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                _selectedColor = Color.FromArgb(_alpha, _selectedColor.R, _selectedColor.G, _selectedColor.B);
            }
        }

        public Color Color
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;

                UpdateColorPicker(value);
                UpdateUpDowns(value, _alpha);
            }
        }

        public Color ColorPickerColor
        {
            get => _colorPickerColor;
            set
            {
                _colorPickerColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(MethodInfo.GetCurrentMethod().Name));

                UpdateColorPicker(value);
                UpdateUpDowns(value, _alpha);

                if (_instantColorSelection)
                {
                    _selectedColor = value;
                    AddColorToUsedColors(value);
                    OnColorChange?.Invoke(this, value);
                }
            }
        }

        public bool EnableColorsPallete
        {
            get => _enableColorsPallete;
            set
            {
                _enableColorsPallete = value;
                SetupVisualComponents();
            }
        }

        public bool EnableCustomColors
        {
            get => _enableCustomColors;
            set
            {
                _enableCustomColors = value;
                SetupVisualComponents();
            }
        }

        public bool EnableUsedColors
        {
            get => _enableUsedColors;
            set
            {
                _enableUsedColors = value;
                SetupVisualComponents();
            }
        }

        public bool InstantColorSelection
        {
            get => _instantColorSelection;
            set
            {
                _instantColorSelection = value;
                SetupVisualComponents();
            }
        }

        public List<Color> UsedColors
        {
            get => GetUsedColors();
            set => SetUsedColors(value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsColorSelector class constructor. </summary>
        public SettingsColorSelector()
        {
            InitializeComponent();

            _usedColors = new List<Border>()
            {
                UsedColor01Border,
                UsedColor02Border,
                UsedColor03Border,
                UsedColor04Border,
                UsedColor05Border
            };
        }

        #endregion CLASS METHODS

        #region COLORS PALLETE INTERACTIONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking add custom color control button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void AddCustomColorControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enableCustomColors)
            {
                PalleteColorsStackPanel.Visibility = Visibility.Collapsed;
                ColorPickerStackPanel.Visibility = Visibility.Visible;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after releasing mouse from theme color changing border component. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ColorBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                var border = (Border)sender;
                var brush = (SolidColorBrush)border.Background;
                var color = brush.Color;

                _selectedColor = color;

                //  Update visual component.
                AddColorToUsedColors(color);

                //  Invoke external method.
                OnColorChange?.Invoke(this, _selectedColor);
            }
        }

        #endregion COLORS PALLETE INTERACTIONS METHODS

        #region COLOR PICKER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mouse button on color picker. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ColorPicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _colorPickerFoucused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after moving cursor over color picker. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void ColorPicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (_colorPickerFoucused && e.LeftButton == MouseButtonState.Released)
                ColorPickerColor = ColorPicker.Color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after releasing mouse button on color picker. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ColorPicker_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_colorPickerFoucused)
                ColorPickerColor = ColorPicker.Color;

            _colorPickerFoucused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after changing value of red color picker NumericUpDown. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Value. </param>
        private void RedColorPickerUpDown_OnUserValueChange(object sender, int e)
        {
            var numUpDown = (BorderedNumericUpDown) sender;

            var color = Color.FromRgb((byte)numUpDown.Value, _colorPickerColor.G, _colorPickerColor.B);
            ColorPickerColor = color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after changing value of green color picker NumericUpDown. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Value. </param>
        private void GreenColorPickerUpDown_OnUserValueChange(object sender, int e)
        {
            var numUpDown = (BorderedNumericUpDown)sender;

            var color = Color.FromRgb(_colorPickerColor.R, (byte)numUpDown.Value, _colorPickerColor.B);
            ColorPickerColor = color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after changing value of blue color picker NumericUpDown. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Value. </param>
        private void BlueColorPickerUpDown_OnUserValueChange(object sender, int e)
        {
            var numUpDown = (BorderedNumericUpDown)sender;

            var color = Color.FromRgb(_colorPickerColor.R, _colorPickerColor.G, (byte)numUpDown.Value);
            ColorPickerColor = color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after changing value of alpha color picker NumericUpDown. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Value. </param>
        private void AlphaColorPickerUpDown_OnUserValueChange(object sender, int e)
        {
            var numUpDown = (BorderedNumericUpDown)sender;

            _alpha = (byte)numUpDown.Value;

            if (_instantColorSelection)
            {
                _selectedColor = Color.FromArgb(_alpha, ColorPickerColor.R, ColorPickerColor.G, ColorPickerColor.B);
                AddColorToUsedColors(_selectedColor);
                OnColorChange?.Invoke(this, _selectedColor);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking select color picker button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void ColorPickerSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedColor = Color.FromArgb(_alpha, ColorPickerColor.R, ColorPickerColor.G, ColorPickerColor.B);
            AddColorToUsedColors(_selectedColor);
            OnColorChange?.Invoke(this, _selectedColor);

            if (_enableColorsPallete)
            {
                PalleteColorsStackPanel.Visibility = Visibility.Visible;
                ColorPickerStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking cancel color picker button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void CancelColorPickerSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_enableColorsPallete)
            {
                PalleteColorsStackPanel.Visibility = Visibility.Visible;
                ColorPickerStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion COLOR PICKER INTERACTION METHIDS

        #region COLOR PICKER MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change color picker selected color. </summary>
        /// <param name="color"> Color to set on color picker. </param>
        private void UpdateColorPicker(Color color)
        {
            ColorPicker.Color = color;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change up down color components values. </summary>
        /// <param name="color"> Color to set on up down values. </param>
        /// <param name="alpha"> Alpha. </param>
        private void UpdateUpDowns(Color color, byte alpha)
        {
            RedColorPickerUpDown.Value = color.R;
            GreenColorPickerUpDown.Value = color.G;
            BlueColorPickerUpDown.Value = color.B;
            AlphaColorPickerUpDown.Value = alpha;
        }

        #endregion COLOR PICKER MANAGEMENT METHODS

        #region COLORS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add new color to used colors in interface. </summary>
        /// <param name="color"> New color to add. </param>
        private void AddColorToUsedColors(Color color)
        {
            Brush thisBrush = new SolidColorBrush(color);
            Brush prevBrush = thisBrush;

            foreach (var border in _usedColors)
            {
                //  Set color.
                var brush = border.Background;
                border.Background = prevBrush;

                SetUsedColorBackground(border, prevBrush != null ? (prevBrush as SolidColorBrush).Color : Colors.Black);
                
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
        /// <summary> Get list of used colors from interface. </summary>
        /// <returns> List of used colors. </returns>
        private List<Color> GetUsedColors()
        {
            var result = new List<Color>();

            foreach (var border in _usedColors)
            {
                var brush = border.Background;

                if (brush != null)
                {
                    var color = (brush as SolidColorBrush).Color;
                    result.Add(color);
                }
            }

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load used colors into interface. </summary>
        /// <param name="colors"> List of used colors. </param>
        private void SetUsedColors(List<Color> colors)
        {
            var minCount = Math.Min(colors.Count, _usedColors.Count);

            for (int index = 0; index < minCount; index++)
            {
                var brush = new SolidColorBrush(colors[index]);
                _usedColors[index].Background = brush;
                SetUsedColorBackground(_usedColors[index], colors[index]);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set color pallete item border when color has transparency. </summary>
        /// <param name="border"> Color pallete border. </param>
        /// <param name="color"> Color with transparency. </param>
        private void SetUsedColorBackground(Border border, Color color)
        {
            bool showBorder = color.A < 128;

            border.BorderBrush = showBorder ? new SolidColorBrush(Colors.Black) : null;
            border.BorderThickness = showBorder ? new Thickness(1) : new Thickness(0);
        }

        #endregion COLORS MANAGEMENT METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup components visibility according to configuration. </summary>
        private void SetupVisualComponents()
        {
            UsedColorsStackPanel.Visibility = _enableUsedColors ? Visibility.Visible : Visibility.Collapsed;
            PalleteColorsStackPanel.Visibility = _enableColorsPallete ? Visibility.Visible : Visibility.Collapsed;
            AddCustomColorControlButton.Visibility = _enableCustomColors ? Visibility.Visible : Visibility.Collapsed;
            ColorPickerStackPanel.Visibility = _enableColorsPallete ? Visibility.Collapsed : Visibility.Visible;

            CancelColorPickerSelectionButton.Visibility = (_enableColorsPallete && !_instantColorSelection)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetupVisualComponents();
        }

        #endregion USER CONTROL METHODS

    }
}
