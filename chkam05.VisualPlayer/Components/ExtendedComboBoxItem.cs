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
    public class ExtendedComboBoxItem : ComboBoxItem, INotifyPropertyChanged
    {

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register(
            nameof(SelectedBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty SelectedBorderProperty = DependencyProperty.Register(
            nameof(SelectedBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedComboBoxItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 0, 120, 215))));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


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

        public SolidColorBrush SelectedBackground
        {
            get => (SolidColorBrush)GetValue(SelectedBackgroundProperty);
            set
            {
                SetValue(SelectedBackgroundProperty, value);
                OnPropertyChanged(nameof(SelectedBackground));
            }
        }

        public SolidColorBrush SelectedBorder
        {
            get => (SolidColorBrush)GetValue(SelectedBorderProperty);
            set
            {
                SetValue(SelectedBorderProperty, value);
                OnPropertyChanged(nameof(SelectedBorder));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedComboBoxItem static class constructor. </summary>
        static ExtendedComboBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedComboBoxItem),
                new FrameworkPropertyMetadata(typeof(ExtendedComboBoxItem)));
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
