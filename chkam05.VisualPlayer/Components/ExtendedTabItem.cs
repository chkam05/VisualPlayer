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
    public class ExtendedTabItem : TabItem, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(ExtendedTabItem),
            new PropertyMetadata(PackIconKind.None));

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedTabItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedTabItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register(
            nameof(SelectedBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedTabItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public static readonly DependencyProperty SelectedBorderProperty = DependencyProperty.Register(
            nameof(SelectedBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedTabItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


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
        /// <summary> ExtendedTabItem static class constructor. </summary>
        static ExtendedTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedTabItem),
                new FrameworkPropertyMetadata(typeof(ExtendedTabItem)));
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
