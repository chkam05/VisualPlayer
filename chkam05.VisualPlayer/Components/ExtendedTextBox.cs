using chkam05.VisualPlayer.Components.Data;
using chkam05.VisualPlayer.Components.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Components
{
    public class ExtendedTextBox : TextBox, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedTextBox),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedTextBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty FocusedBackgroundProperty = DependencyProperty.Register(
            nameof(FocusedBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedTextBox),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty FocusedBorderProperty = DependencyProperty.Register(
            nameof(FocusedBorder),
            typeof(Brush),
            typeof(ExtendedTextBox),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty ExtendedTextBoxDataTypeProperty = DependencyProperty.Register(
            nameof(ExtendedTextBoxDataType),
            typeof(ExtendedTextBoxDataType),
            typeof(ExtendedTextBox),
            new PropertyMetadata(ExtendedTextBoxDataType.STRING));

        public static readonly DependencyProperty SpecialFormatProperty = DependencyProperty.Register(
            nameof(SpecialFormat),
            typeof(string),
            typeof(ExtendedTextBox),
            new PropertyMetadata(string.Empty));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ExtendedTextBoxContentUpdatedEventArgs> OnContentUpdated;


        //  VARIABLES

        private bool _isEditing = false;
        private bool _isUpdating = false;
        private string _previousText = string.Empty;


        //  GETTERS & SETTERS

        public SolidColorBrush HoveredBackground
        {
            get => (SolidColorBrush)GetValue(HoveredBackgroundProperty);
            set
            {
                SetValue(HoveredBackgroundProperty, value);
                OnPropertyChanged(nameof(HoveredBackground));
            }
        }

        public SolidColorBrush HoveredBorder
        {
            get => (SolidColorBrush)GetValue(HoveredBorderProperty);
            set
            {
                SetValue(HoveredBorderProperty, value);
                OnPropertyChanged(nameof(HoveredBorder));
            }
        }

        public SolidColorBrush FocusedBackground
        {
            get => (SolidColorBrush)GetValue(FocusedBackgroundProperty);
            set
            {
                SetValue(FocusedBackgroundProperty, value);
                OnPropertyChanged(nameof(FocusedBackground));
            }
        }

        public Brush FocusedBorder
        {
            get => (Brush)GetValue(FocusedBorderProperty);
            set
            {
                SetValue(FocusedBorderProperty, value);
                OnPropertyChanged(nameof(FocusedBorder));
            }
        }

        public ExtendedTextBoxDataType ExtendedTextBoxDataType
        {
            get => (ExtendedTextBoxDataType)GetValue(ExtendedTextBoxDataTypeProperty);
            set
            {
                SetValue(ExtendedTextBoxDataTypeProperty, value);
                OnPropertyChanged(nameof(ExtendedTextBoxDataType));

                if (UpdatePreviousValue(_previousText))
                {
                    switch (value)
                    {
                        case ExtendedTextBoxDataType.DOUBLE:
                            _previousText = "0";
                            break;

                        case ExtendedTextBoxDataType.INT:
                            _previousText = "0";
                            break;

                        case ExtendedTextBoxDataType.TIMESPAN:
                            _previousText = GetDefaultFromSpecialFormat();
                            break;

                        case ExtendedTextBoxDataType.STRING:
                        default:
                            break;
                    }
                }

                ValidateValue(Text);
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
        }

        public string SpecialFormat
        {
            get => (string)GetValue(SpecialFormatProperty);
            set
            {
                SetValue(SpecialFormatProperty, value);
                OnPropertyChanged(nameof(SpecialFormat));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedTextBox class constructor. </summary>
        public ExtendedTextBox() : base()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;

            _previousText = Text;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ControlButton static class constructor. </summary>
        static ExtendedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedTextBox),
                new FrameworkPropertyMetadata(typeof(ExtendedTextBox)));
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after getting focus on ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loosing focus on ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing text in ExtendedTextBox. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Text Changed event arguments. </param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isUpdating)
                ValidateValue(Text);

            OnContentUpdated?.Invoke(sender, new ExtendedTextBoxContentUpdatedEventArgs(
                Text, _previousText, !_isEditing));
        }

        #endregion INTERACTION METHODS

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

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert special format to default value. </summary>
        /// <returns> Default value from special format. </returns>
        private string GetDefaultFromSpecialFormat()
        {
            return string.Join("", SpecialFormat.Select(c =>
            {
                if (Char.IsLetter(c))
                    return '0';

                return c;
            }));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update previous value. </summary>
        /// <param name="newValue"> Value to set as previous. </param>
        private bool UpdatePreviousValue(string newValue)
        {
            bool correct = true;

            switch (ExtendedTextBoxDataType)
            {
                case ExtendedTextBoxDataType.DOUBLE:
                    correct = ValidateFloatingPointNumber(newValue, out string _);
                    break;

                case ExtendedTextBoxDataType.INT:
                    correct = ValidateNumber(newValue, out string _);
                    break;

                case ExtendedTextBoxDataType.TIMESPAN:
                    correct = ValidateTimeSpan(newValue, out string _);
                    break;

                case ExtendedTextBoxDataType.STRING:
                default:
                    break;
            }

            if (correct)
                _previousText = newValue;

            return correct;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate entered or setted value. </summary>
        /// <param name="value"> Entered or setted value. </param>
        private void ValidateValue(string value)
        {
            bool correct = true;
            string newValue = value;

            switch (ExtendedTextBoxDataType)
            {
                case ExtendedTextBoxDataType.DOUBLE:
                    correct = ValidateFloatingPointNumber(value, out newValue, IsEditing);
                    break;

                case ExtendedTextBoxDataType.INT:
                    correct = ValidateNumber(value, out newValue, IsEditing);
                    break;

                case ExtendedTextBoxDataType.TIMESPAN:
                    correct = ValidateTimeSpan(value, out newValue, IsEditing);
                    break;

                case ExtendedTextBoxDataType.STRING:
                default:
                    break;
            }

            if (!correct)
            {
                _isUpdating = true;

                if (IsEditing)
                {
                    int length = Text.Length;
                    int carret = SelectionStart;

                    int diffTextLength = Math.Max(0, length - newValue.Length);
                    Text = newValue;
                    SelectionStart = Math.Max(0, Math.Min(carret - diffTextLength, newValue.Length));
                }
                else
                {
                    Text = newValue;
                }

                _isUpdating = false;
            }

            UpdatePreviousValue(newValue);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate entered or setted floating point number value. </summary>
        /// <param name="value"> Entered or setted floating point number value. </param>
        /// <param name="newValue"> Output corrected value. </param>
        /// <param name="inEdit"> Allow to use empty and special characters. </param>
        /// <returns> True - validation successfull; False - otherwise. </returns>
        private bool ValidateFloatingPointNumber(string value, out string newValue, bool inEdit = false)
        {
            if (inEdit)
            {
                newValue = value;

                if (string.IsNullOrEmpty(value))
                    return true;

                if (value == "-" || value == "." || value == "-.")
                    return true;
            }

            bool correct = double.TryParse(value, out double _);
            newValue = correct ? value : _previousText;
            return correct;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate entered or setted number value. </summary>
        /// <param name="value"> Entered or setted number value. </param>
        /// <param name="newValue"> Output corrected value. </param>
        /// <param name="inEdit"> Allow to use empty and special characters. </param>
        /// <returns> True - validation successfull; False - otherwise. </returns>
        private bool ValidateNumber(string value, out string newValue, bool inEdit = false)
        {
            if (inEdit)
            {
                newValue = value;

                if (string.IsNullOrEmpty(value))
                    return true;

                if (value == "-")
                    return true;
            }

            bool correct = int.TryParse(value, out int _);
            newValue = correct ? value : _previousText;
            return correct;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate entered or setted timespan value. </summary>
        /// <param name="value"> Entered or setted timespan value. </param>
        /// <param name="newValue"> Output corrected value. </param>
        /// <param name="inEdit"> Allow to use empty and special characters. </param>
        /// <returns> True - validation successfull; False - otherwise. </returns>
        private bool ValidateTimeSpan(string value, out string newValue, bool inEdit = false)
        {
            bool correct = false;

            if (inEdit)
            {
                newValue = value;

                if (string.IsNullOrEmpty(value))
                    return true;
            }

            if (!string.IsNullOrEmpty(SpecialFormat))
                correct = TimeSpan.TryParseExact(value, SpecialFormat, CultureInfo.InvariantCulture, out TimeSpan _);
            else
                correct = TimeSpan.TryParse(value, out TimeSpan _);

            newValue = correct ? value : _previousText;
            return correct;
        }

        #endregion VALIDATION METHODS

    }
}
