using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;

namespace VisualPlayer.Controls
{
    public class CustomUpDown : CustomTextBox
    {

        //  DELEGATES

        public delegate void CustomUpDownValueChangedEventHandler(object sender, CustomUpDownValueChangedEventArgs e);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            nameof(ButtonStyle),
            typeof(Style),
            typeof(CustomUpDown),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(CustomUpDown),
            new PropertyMetadata(100d));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(CustomUpDown),
            new PropertyMetadata(0d));

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            nameof(Step),
            typeof(double),
            typeof(CustomUpDown),
            new PropertyMetadata(1d));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(CustomUpDown),
            new PropertyMetadata(0d, ValuePropertyChangedCallback));


        //  EVENTS

        public event CustomUpDownValueChangedEventHandler ValueChanged;


        //  VARIABLES

        private bool _ignoreValidation = false;
        private string _textBackup = string.Empty;


        //  GETTERS & SETTERS

        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, Math.Max(Minimum, value));
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, Math.Min(Maximum, value));
        }

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, Math.Min(Maximum, Math.Max(Minimum, value)));
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomUpDown class constructor. </summary>
        public CustomUpDown() : base()
        {
            string strValue = Value.ToString();

            _textBackup = strValue;
            Text = strValue;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomUpDown class constructor. </summary>
        static CustomUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomUpDown),
                new FrameworkPropertyMetadata(typeof(CustomUpDown)));
        }

        #endregion CLASS METHODS

        #region COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking down button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OnDownButtonClick(object sender, RoutedEventArgs e)
        {
            if ((Value - Step) >= Minimum)
                Value = Value - Step;
            else
                Value = Minimum;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking up button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OnUpButtonClick(object sender, RoutedEventArgs e)
        {
            if ((Value + Step) <= Maximum)
                Value = Value + Step;
            else
                Value = Maximum;
        }

        #endregion COMPONENTS INTERACTION METHODS

        #region CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after text change. </summary>
        /// <param name="e"> Text Changed Event Arguments. </param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!_ignoreValidation)
            {
                int carretPosition = SelectionStart;
                bool validationResult = Validate(Text, out double newValue);

                DoInValidationIgnore(() =>
                {
                    if (validationResult)
                    {
                        _textBackup = Text;
                        Value = newValue;
                    }
                    else
                    {
                        Text = _textBackup;
                        Value = newValue;
                        SelectionStart = Math.Max(0, Math.Min(carretPosition - 1, _textBackup.Length));
                    }
                });
            }

            InvokeTextModifiedEventHandler(_isFocused);

            base.OnTextChanged(e);
        }

        #endregion CONTROL METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke value changed event. </summary>
        /// <param name="uiModified"> Is modified from user interface. </param>
        /// <param name="value"> External value. </param>
        private void InvokeValueChanged(bool uiModified = false, double? value = null)
        {
            ValueChanged?.Invoke(this, new CustomUpDownValueChangedEventArgs(value ?? Value, uiModified));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing colors collection property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ValuePropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is CustomUpDown instance)
            {
                if (instance._ignoreValidation)
                {
                    instance.InvokeValueChanged(instance._isFocused, (double)e.NewValue);
                    return;
                }

                string strValue = ((double)e.NewValue).ToString();

                instance.DoInValidationIgnore(() =>
                {
                    instance._textBackup = strValue;
                    instance.Text = strValue;
                    instance.InvokeValueChanged(instance._isFocused, (double)e.NewValue);
                });
            }
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            var downButton = GetButton("_downButton");
            var upButton = GetButton("_upButton");

            SetButtonClickEvent(downButton, OnDownButtonClick);
            SetButtonClickEvent(upButton, OnUpButtonClick);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Button from ContentTemplate. </summary>
        /// <param name="buttonName"> Button name. </param>
        /// <returns> Button or null. </returns>
        private Button GetButton(string buttonName)
        {
            return this.Template.FindName(buttonName, this) as Button;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set button click event handler. </summary>
        /// <param name="button"> Button. </param>
        /// <param name="handler"> Handler. </param>
        private void SetButtonClickEvent(Button button, RoutedEventHandler handler)
        {
            if (button != null && handler != null)
                button.Click += handler;
        }

        #endregion TEMPLATE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke method in validation ignore mode. </summary>
        /// <param name="f"> Action. </param>
        protected void DoInValidationIgnore(Action f)
        {
            _ignoreValidation = true;
            f?.Invoke();
            _ignoreValidation = false;
        }

        #endregion UTILITY METHODS

        #region VALIDATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Validate text. </summary>
        /// <param name="inputString"> Input string. </param>
        /// <param name="value"> Output value. </param>
        /// <returns> True - validation successfull; False - otherwise. </returns>
        private bool Validate(string inputString, out double value)
        {
            bool result = false;
            value = Value;

            if (string.IsNullOrEmpty(inputString))
            {
                result = true;
                value = Minimum;
            }
            else if (inputString == "-" || inputString == "," || inputString == ".")
            {
                result = true;
            }
            else if (double.TryParse(inputString.Replace(',', '.'), out value))
            {
                result = true;
            }
            else if (double.TryParse(inputString.Replace('.', ','), out value))
            {
                result = true;
            }

            if (result)
            {
                if (value < Minimum)
                {
                    value = Minimum;
                    _textBackup = Minimum.ToString();
                    result = false;
                }
                else if (value > Maximum)
                {
                    value = Maximum;
                    _textBackup = Maximum.ToString();
                    result = false;
                }
            }

            return result;
        }

        #endregion VALIDATION METHODS

    }
}
