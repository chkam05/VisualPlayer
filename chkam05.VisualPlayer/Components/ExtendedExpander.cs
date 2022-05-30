using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Components
{
    public class ExtendedExpander : Expander, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ArrowBrushProperty = DependencyProperty.Register(
            nameof(ArrowBrush),
            typeof(SolidColorBrush),
            typeof(ExtendedExpander),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))));

        public static readonly DependencyProperty ArrowHighlightedBrushProperty = DependencyProperty.Register(
            nameof(ArrowHighlightedBrush),
            typeof(SolidColorBrush),
            typeof(ExtendedExpander),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));

        public static readonly DependencyProperty ArrowPressedBrushProperty = DependencyProperty.Register(
            nameof(ArrowPressedBrush),
            typeof(SolidColorBrush),
            typeof(ExtendedExpander),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register(
            nameof(HeaderBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedExpander),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty ArrowHeightProperty = DependencyProperty.Register(
            nameof(ArrowHeight),
            typeof(double),
            typeof(ExtendedExpander),
            new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty ArrowWidthProperty = DependencyProperty.Register(
            nameof(ArrowWidth),
            typeof(double),
            typeof(ExtendedExpander),
            new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty HeaderBorderBrushProperty = DependencyProperty.Register(
            nameof(HeaderBorderBrush),
            typeof(SolidColorBrush),
            typeof(ExtendedExpander),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty HeaderBorderThicknessProperty = DependencyProperty.Register(
            nameof(HeaderBorderThickness),
            typeof(Thickness),
            typeof(ExtendedExpander),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register(
            nameof(HeaderPadding),
            typeof(Thickness),
            typeof(ExtendedExpander),
            new PropertyMetadata(new Thickness(0)));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public SolidColorBrush ArrowBrush
        {
            get => (SolidColorBrush)GetValue(ArrowBrushProperty);
            set
            {
                SetValue(ArrowBrushProperty, value);
                OnPropertyChanged(nameof(ArrowBrush));
            }
        }

        public SolidColorBrush ArrowHighlightedBrush
        {
            get => (SolidColorBrush)GetValue(ArrowHighlightedBrushProperty);
            set
            {
                SetValue(ArrowHighlightedBrushProperty, value);
                OnPropertyChanged(nameof(ArrowHighlightedBrush));
            }
        }

        public SolidColorBrush ArrowPressedBrush
        {
            get => (SolidColorBrush)GetValue(ArrowPressedBrushProperty);
            set
            {
                SetValue(ArrowPressedBrushProperty, value);
                OnPropertyChanged(nameof(ArrowPressedBrush));
            }
        }

        public double ArrowHeight
        {
            get => (double)GetValue(ArrowHeightProperty);
            set
            {
                SetValue(ArrowHeightProperty, value);
                OnPropertyChanged(nameof(ArrowHeight));
            }
        }

        public double ArrowWidth
        {
            get => (double)GetValue(ArrowWidthProperty);
            set
            {
                SetValue(ArrowWidthProperty, value);
                OnPropertyChanged(nameof(ArrowWidth));
            }
        }

        public SolidColorBrush HeaderBackground
        {
            get => (SolidColorBrush)GetValue(HeaderBackgroundProperty);
            set
            {
                SetValue(HeaderBackgroundProperty, value);
                OnPropertyChanged(nameof(HeaderBackground));
            }
        }

        public SolidColorBrush HeaderBorderBrush
        {
            get => (SolidColorBrush)GetValue(HeaderBorderBrushProperty);
            set
            {
                SetValue(HeaderBorderBrushProperty, value);
                OnPropertyChanged(nameof(HeaderBorderBrush));
            }
        }

        public Thickness HeaderBorderThickness
        {
            get => (Thickness)GetValue(HeaderBorderThicknessProperty);
            set
            {
                SetValue(HeaderBorderThicknessProperty, value);
                OnPropertyChanged(nameof(HeaderBorderThickness));
            }
        }
        
        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set
            {
                SetValue(HeaderPaddingProperty, value);
                OnPropertyChanged(nameof(HeaderPadding));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedExpander static class constructor. </summary>
        static ExtendedExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedExpander),
                new FrameworkPropertyMetadata(typeof(ExtendedExpander)));
        }

        #endregion CLASS METHODS

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

    }
}
