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
    public class SideBarMenuItem : ListViewItem, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground), 
            typeof(SolidColorBrush), 
            typeof(SideBarMenuItem), 
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(SideBarMenuItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty SelectedActiveBackgroundProperty = DependencyProperty.Register(
            nameof(SelectedActiveBackground),
            typeof(SolidColorBrush),
            typeof(SideBarMenuItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 120, 215))));

        public static readonly DependencyProperty SelectedActiveBorderProperty = DependencyProperty.Register(
            nameof(SelectedActiveBorder),
            typeof(SolidColorBrush),
            typeof(SideBarMenuItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty SelectedInactiveBackgroundProperty = DependencyProperty.Register(
            nameof(SelectedInactiveBackground),
            typeof(SolidColorBrush),
            typeof(SideBarMenuItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255))));

        public static readonly DependencyProperty SelectedInactiveBorderProperty = DependencyProperty.Register(
            nameof(SelectedInactiveBorder),
            typeof(SolidColorBrush),
            typeof(SideBarMenuItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind), 
            typeof(PackIconKind), 
            typeof(SideBarMenuItem), 
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

        public SolidColorBrush SelectedActiveBackground
        {
            get => (SolidColorBrush)GetValue(SelectedActiveBackgroundProperty);
            set
            {
                SetValue(SelectedActiveBackgroundProperty, value);
                OnPropertyChanged(nameof(SelectedActiveBackground));
            }
        }

        public SolidColorBrush SelectedActiveBorder
        {
            get => (SolidColorBrush)GetValue(SelectedActiveBorderProperty);
            set
            {
                SetValue(SelectedActiveBorderProperty, value);
                OnPropertyChanged(nameof(SelectedActiveBorder));
            }
        }

        public SolidColorBrush SelectedInactiveBackground
        {
            get => (SolidColorBrush)GetValue(SelectedInactiveBackgroundProperty);
            set
            {
                SetValue(SelectedInactiveBackgroundProperty, value);
                OnPropertyChanged(nameof(SelectedInactiveBackground));
            }
        }

        public SolidColorBrush SelectedInactiveBorder
        {
            get => (SolidColorBrush)GetValue(SelectedInactiveBorderProperty);
            set
            {
                SetValue(SelectedInactiveBorderProperty, value);
                OnPropertyChanged(nameof(SelectedInactiveBorder));
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
        /// <summary> SideBarMenuItem static class constructor. </summary>
        static SideBarMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SideBarMenuItem),
                new FrameworkPropertyMetadata(typeof(SideBarMenuItem)));
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
