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

namespace chkam05.VisualPlayer.Components
{
    public partial class BorderedUpDown : UserControl
    {

        //  EVENTS

        public event EventHandler<int> OnUserValueChange;


        //  VARIABLES

        private bool _focused = false;
        private int _maxValue = int.MaxValue;
        private int _minValue = 0;
        private int _value = 0;


        #region GETTERS & SETTERS

        public new Brush Background
        {
            get => ContentBorder.Background;
            set => ContentBorder.Background = value;
        }

        public new Brush BorderBrush
        {
            get => ContentBorder.BorderBrush;
            set => ContentBorder.BorderBrush = value;
        }

        public new double BorderThickness
        {
            get => ContentBorder.BorderThickness.Left;
            set => ContentBorder.BorderThickness = new Thickness(value);
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Max(_minValue, Math.Min(value, _maxValue));
                InputTextBox.Text = _value.ToString();
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BorderedUpDown class constructor. </summary>
        public BorderedUpDown()
        {
            InitializeComponent();
            InputTextBox.Text = _value.ToString();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when up button was clicked. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            _value = Math.Max(0, Math.Min(_value + 1, _maxValue));
            InputTextBox.Text = _value.ToString();
            OnUserValueChange?.Invoke(this, _value);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when down button was clicked. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            _value = Math.Max(0, Math.Min(_value - 1, _maxValue));
            InputTextBox.Text = _value.ToString();
            OnUserValueChange?.Invoke(this, _value);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when textbox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _focused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when textbox is unselected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _focused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when text is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Text change event arguments. </param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_focused)
            {
                var textBox = sender as TextBox;
                var length = textBox.Text.Length;
                int carret = textBox.SelectionStart;

                if (string.IsNullOrEmpty(textBox.Text))
                    return;

                bool parsed = int.TryParse(textBox.Text, out int numeric);

                if (!parsed)
                {
                    int diffTextLength = Math.Max(0, length - _value.ToString().Length);
                    carret = Math.Max(0, Math.Min(carret - diffTextLength, textBox.Text.Length));
                }
                else
                {
                    _value = Math.Max(_minValue, Math.Min(numeric, _maxValue));
                    carret = _value.ToString().Length;
                }

                textBox.Text = _value.ToString();
                textBox.SelectionStart = carret;
                OnUserValueChange?.Invoke(this, _value);
            }
        }

        #endregion INTERACTION METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _value = Math.Max(_minValue, Math.Min(_value, _maxValue));
            InputTextBox.Text = _value.ToString();
        }

        #endregion USER CONTROL METHODS

    }
}
