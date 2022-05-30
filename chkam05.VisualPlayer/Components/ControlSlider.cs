using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Components
{
    public class ControlSlider : Slider, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty DraggingThumbBackgroundProperty = DependencyProperty.Register(
            nameof(DraggingThumbBackground),
            typeof(SolidColorBrush),
            typeof(ControlSlider),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));

        public static readonly DependencyProperty HoveredThumbBackgroundProperty = DependencyProperty.Register(
            nameof(HoveredThumbBackground),
            typeof(SolidColorBrush),
            typeof(ControlSlider),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 255, 255, 255))));

        public static readonly DependencyProperty ThumbSizeProperty = DependencyProperty.Register(
            nameof(ThumbSize),
            typeof(double),
            typeof(ControlSlider),
            new PropertyMetadata(32.0));


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

        public double ThumbSize
        {
            get => (double)GetValue(ThumbSizeProperty);
            set
            {
                SetValue(ThumbSizeProperty, value);
                OnPropertyChanged(nameof(ThumbSize));
            }
        }

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlScrollBar static class constructor. </summary>
        static ControlSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ControlSlider),
                new FrameworkPropertyMetadata(typeof(ControlSlider)));
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
