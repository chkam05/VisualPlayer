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
    public partial class BorderedTextBox : UserControl
    {

        //  EVENTS

        public event EventHandler<string> OnUserTextChange;


        //  VARIABLES

        private bool _focused = false;
        private bool _numeric = false;
        private string _previousText = "";


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

        public bool Numeric
        {
            get => _numeric;
            set
            {
                _numeric = value;
            }
        }

        public string Text
        {
            get => InputTextBox.Text;
            set
            {
                InputTextBox.Text = _numeric
                    ? ValidateAndCorrectNumeric(value)
                    : value;
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BorderedTextBox class constructor. </summary>
        public BorderedTextBox()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when textbox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _focused = true;
            _previousText = (sender as TextBox).Text;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when textbox is unselected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _focused = false;
            _previousText = "";
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when text is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Text change event arguments. </param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var length = textBox.Text.Length;
            int carret = textBox.SelectionStart;

            if (_numeric)
            {
                string correctedText = ValidateAndCorrectNumeric(textBox.Text);

                if (textBox.Text != correctedText)
                {
                    int diffTextLength = Math.Max(0, length - correctedText.Length);

                    textBox.Text = correctedText;
                    textBox.SelectionStart = Math.Max(0, Math.Min(carret - diffTextLength, textBox.Text.Length));
                }

                if (_previousText != correctedText)
                    _previousText = correctedText;
            }

            if (_focused)
                OnUserTextChange?.Invoke(this, textBox.Text);
        }

        #endregion INTERACTION METHODS

        #region VALIDATION TEXT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate i correct text to be numeric text. </summary>
        /// <returns> Validated and corrected text. </returns>
        private string ValidateAndCorrectNumeric(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            else if (int.TryParse(text, out int numeric))
                return text;

            else
                return _previousText;
        }

        #endregion VALIDATION TEXT METHODS

    }
}
