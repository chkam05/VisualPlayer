using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Components
{
    public class ExtendedScrollBar : ScrollBar, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty DraggingThumbBackgroundProperty = DependencyProperty.Register(
            nameof(DraggingThumbBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedScrollBar),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));

        public static readonly DependencyProperty HoveredThumbBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredThumbBackground),
            typeof(SolidColorBrush),
            typeof(ExtendedScrollBar),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  GETTERS & SETTERS

        public SolidColorBrush DraggingThumbBackground
        {
            get => (SolidColorBrush)GetValue(DraggingThumbBackgroundProperty);
            set
            {
                SetValue(DraggingThumbBackgroundProperty, value);
                OnPropertyChanged(nameof(DraggingThumbBackground));
            }
        }

        public SolidColorBrush HoveredThumbBackground
        {
            get => (SolidColorBrush)GetValue(HoveredThumbBackgroundProperty);
            set
            {
                SetValue(HoveredThumbBackgroundProperty, value);
                OnPropertyChanged(nameof(HoveredThumbBackground));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedScrollBar static class constructor. </summary>
        static ExtendedScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedScrollBar),
                new FrameworkPropertyMetadata(typeof(ExtendedScrollBar)));
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
