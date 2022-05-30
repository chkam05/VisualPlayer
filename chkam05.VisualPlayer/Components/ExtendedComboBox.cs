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
    public class ExtendedComboBox : ComboBox, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundHighlightedProperty = DependencyProperty.Register(
            nameof(BackgroundHighlighted),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 0, 0, 0))));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register(
            nameof(BackgroundSelected),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 0, 0, 0))));

        public static readonly DependencyProperty BorderBrushHighlightedProperty = DependencyProperty.Register(
            nameof(BorderBrushHighlighted),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));

        public static readonly DependencyProperty BorderBrushSelectedProperty = DependencyProperty.Register(
            nameof(BorderBrushSelected),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBox),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public SolidColorBrush BackgroundHighlighted
        {
            get => (SolidColorBrush)GetValue(BackgroundHighlightedProperty);
            set
            {
                SetValue(BackgroundHighlightedProperty, value);
                OnPropertyChanged(nameof(BackgroundHighlighted));
            }
        }

        public SolidColorBrush BackgroundSelected
        {
            get => (SolidColorBrush)GetValue(BackgroundSelectedProperty);
            set
            {
                SetValue(BackgroundSelectedProperty, value);
                OnPropertyChanged(nameof(BackgroundSelected));
            }
        }

        public SolidColorBrush BorderBrushHighlighted
        {
            get => (SolidColorBrush)GetValue(BorderBrushHighlightedProperty);
            set
            {
                SetValue(BorderBrushHighlightedProperty, value);
                OnPropertyChanged(nameof(BorderBrushHighlighted));
            }
        }

        public SolidColorBrush BorderBrushSelected
        {
            get => (SolidColorBrush)GetValue(BorderBrushSelectedProperty);
            set
            {
                SetValue(BorderBrushSelectedProperty, value);
                OnPropertyChanged(nameof(BorderBrushSelected));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedComboBox static class constructor. </summary>
        static ExtendedComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedComboBox),
                new FrameworkPropertyMetadata(typeof(ExtendedComboBox)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns a new System.Windows.Controls.ComboBoxItem container. </summary>
        /// <returns> A new System.Windows.Controls.ComboBoxItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedComboBoxItem();
        }

        #endregion ITEMS METHODS

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
