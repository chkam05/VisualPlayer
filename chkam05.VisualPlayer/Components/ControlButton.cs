using MaterialDesignThemes.Wpf;
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
    public class ControlButton : Button, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(ControlButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(ControlButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
            nameof(PressedBackground),
            typeof(SolidColorBrush),
            typeof(ControlButton),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));

        public static readonly DependencyProperty PressedBorderProperty = DependencyProperty.Register(
            nameof(PressedBorder),
            typeof(SolidColorBrush),
            typeof(ControlButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(ControlButton),
            new PropertyMetadata(PackIconKind.None));


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

        public SolidColorBrush PressedBackground
        {
            get => (SolidColorBrush)GetValue(PressedBackgroundProperty);
            set
            {
                SetValue(PressedBackgroundProperty, value);
                OnPropertyChanged(nameof(PressedBackground));
            }
        }

        public SolidColorBrush PressedBorder
        {
            get => (SolidColorBrush)GetValue(PressedBorderProperty);
            set
            {
                SetValue(PressedBorderProperty, value);
                OnPropertyChanged(nameof(PressedBorder));
            }
        }

        public PackIconKind PackIconKind
        {
            get => (PackIconKind)GetValue(PackIconKindProperty);
            set
            {
                SetValue(PackIconKindProperty, value);
                OnPropertyChanged(nameof(PackIconKind));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlButton static class constructor. </summary>
        static ControlButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ControlButton),
                new FrameworkPropertyMetadata(typeof(ControlButton)));
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
