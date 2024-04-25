using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using VisualPlayer.Data.ColorModels;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;
using System.Windows.Input;
using VisualPlayer.Controls.Events;

namespace VisualPlayer.Controls
{
    public class CustomTextBox : TextBox
    {

        //  DELEGATES

        public delegate void CustomTextChangedEventHandler(object sender, CustomTextChangedEventArgs e);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundMouseOverProperty = DependencyProperty.Register(
            nameof(BackgroundMouseOver),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorMouseOver)));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
            nameof(BackgroundSelected),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorShade)));

        public static readonly DependencyProperty BorderBrushMouseOverProperty = DependencyProperty.Register(
            nameof(BorderBrushMouseOver),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
            nameof(BorderBrushSelected),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorSelected)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomTextBox),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ForegroundMouseOverProperty = DependencyProperty.Register(
            nameof(ForegroundMouseOver),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColorForeground)));

        public static readonly DependencyProperty ForegroundSelectedProperty = DependencyProperty.Register(
            nameof(ForegroundSelected),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultThemeColorForeground)));

        public static readonly DependencyProperty SelectedTextBrushProperty = DependencyProperty.Register(
            nameof(SelectedTextBrush),
            typeof(Brush),
            typeof(CustomTextBox),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));


        //  EVENTS

        public event CustomTextChangedEventHandler TextModified;


        //  VARIABLES

        protected bool _isFocused = false;


        //  GETTERS & SETTERS

        public Brush BackgroundMouseOver
        {
            get => (Brush)GetValue(BackgroundMouseOverProperty);
            set => SetValue(BackgroundMouseOverProperty, value);
        }

        public Brush BackgroundSelected
        {
            get => (Brush)GetValue(BackgroundSelectedProperty);
            set => SetValue(BackgroundSelectedProperty, value);
        }

        public Brush BorderBrushMouseOver
        {
            get => (Brush)GetValue(BorderBrushMouseOverProperty);
            set => SetValue(BorderBrushMouseOverProperty, value);
        }

        public Brush BorderBrushSelected
        {
            get => (Brush)GetValue(BorderBrushSelectedProperty);
            set => SetValue(BorderBrushSelectedProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush ForegroundMouseOver
        {
            get => (Brush)GetValue(ForegroundMouseOverProperty);
            set => SetValue(ForegroundMouseOverProperty, value);
        }

        public Brush ForegroundSelected
        {
            get => (Brush)GetValue(ForegroundSelectedProperty);
            set => SetValue(ForegroundSelectedProperty, value);
        }

        public Brush SelectedTextBrush
        {
            get => (Brush)GetValue(SelectedTextBrushProperty);
            set => SetValue(SelectedTextBrushProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomTextBox class constructor. </summary>
        public CustomTextBox()
        {
            //  Initialize events.
            Loaded += OnLoaded;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomTextBox class constructor. </summary>
        static CustomTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTextBox),
                new FrameworkPropertyMetadata(typeof(CustomTextBox)));
        }

        #endregion CLASS METHODS

        #region CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading component. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when component got focused by user. </summary>
        /// <param name="e"> Routed Event Arguments. </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _isFocused = true;
            base.OnGotFocus(e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when component lost focus from user. </summary>
        /// <param name="e"> Routed Event Arguments. </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            _isFocused = false;
            base.OnLostFocus(e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after text change. </summary>
        /// <param name="e"> Text Changed Event Arguments. </param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            InvokeTextModifiedEventHandler(_isFocused);

            base.OnTextChanged(e);
        }

        #endregion CONTROL METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke text modified event handler. </summary>
        /// <param name="uiModified"> Is modified from user interface. </param>
        /// <param name="text"> External text. </param>
        protected void InvokeTextModifiedEventHandler(bool uiModified = false, string text = null)
        {
            string modifiedText = !string.IsNullOrEmpty(text) ? text : Text;

            TextModified?.Invoke(this, new CustomTextChangedEventArgs(modifiedText, uiModified));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
