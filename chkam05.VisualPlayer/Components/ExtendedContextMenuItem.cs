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
    public class ExtendedContextMenuItem : MenuItem, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static new readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(PackIconKind),
            typeof(ExtendedContextMenuItem),
            new PropertyMetadata(PackIconKind.None));

        public static readonly DependencyProperty HoveredBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedContextMenuItem),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(244, 0, 120, 215))));

        public static readonly DependencyProperty HoveredBorderProperty = DependencyProperty.Register(
            nameof(HoveredBorder),
            typeof(SolidColorBrush),
            typeof(ExtendedContextMenuItem),
            new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public new PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set
            {
                SetValue(IconProperty, value);
                OnPropertyChanged(nameof(Icon));
            }
        }

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


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedContextMenuItem static class constructor. </summary>
        static ExtendedContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedContextMenuItem),
                new FrameworkPropertyMetadata(typeof(ExtendedContextMenuItem)));
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
