using chkam05.VisualPlayer.Components.EventArgs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Components
{
    public partial class VolumeSlider : UserControl
    {

        //  EVENTS

        public event EventHandler<SliderDragEventArgs> OnHandlerDrag;


        //  VARIABLES

        private bool _dragging = false;
        private bool _initialized = false;
        private double _previousPosition = 0;


        #region GETTERS & SETTERS

        public Brush HandlerBrush
        {
            get => Slider.Foreground;
            set => Slider.Foreground = value;
        }

        public bool IsDragging
        {
            get => _dragging;
        }

        public bool Mute
        {
            get => Slider.Value <= 0;
            set
            {
                if (value)
                {
                    if (Slider.Value > 0)
                        _previousPosition = Slider.Value;
                    Slider.Value = Slider.Minimum;
                }
                else
                {
                    Slider.Value = Math.Min(_previousPosition, Slider.Maximum);
                }
            }
        }

        public double Value
        {
            get => Slider.Value;
            set => Slider.Value = Math.Max(Slider.Minimum, Math.Min(value, Slider.Maximum));
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VolumeSlider class constructor. </summary>
        public VolumeSlider()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERFACE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mute/unmute button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            //  Calculate handler position.
            double yValueRatio = Slider.Value / (Slider.Maximum - Slider.Minimum);
            double thumbPosition = yValueRatio * Slider.ActualHeight;

            //  Set 0 value for mute or restore previous value.
            Mute = !Mute;
            UpdateMuteButtonIcon();

            //  Invoke external action.
            OnHandlerDrag?.Invoke(this,
                new SliderDragEventArgs(SliderDragAction.ON_DRAG, Slider.Value));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after grabbing slider handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Drag started event arguments. </param>
        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //  Enable drag functionality.
            _dragging = true;

            //  Calculate handler position.
            double yValueRatio = Slider.Value / (Slider.Maximum - Slider.Minimum);
            double thumbPosition = yValueRatio * Slider.ActualHeight;

            //  Update previous value and update interface.
            _previousPosition = Slider.Value;
            UpdateMuteButtonIcon();

            //  Invoke external action.
            OnHandlerDrag?.Invoke(this,
                new SliderDragEventArgs(SliderDragAction.ON_GRAB, Slider.Value));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after releasing slider handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Drag completed event arguments. </param>
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (_dragging)
            {
                //  Disable drag functionality.
                _dragging = false;

                //  Calculate handler position.
                double yValueRatio = Slider.Value / (Slider.Maximum - Slider.Minimum);
                double thumbPosition = yValueRatio * Slider.ActualHeight;

                //  Update previous value and update interface.
                _previousPosition = Slider.Value;
                UpdateMuteButtonIcon();

                //  Invoke external action.
                OnHandlerDrag?.Invoke(this,
                    new SliderDragEventArgs(SliderDragAction.ON_RELEASE, Slider.Value));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after moving slider handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed property change event argumetns. </param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //  Update previous value and update interface.
            if (_dragging)
            {
                //  Calculate handler position.
                double yValueRatio = Slider.Value / (Slider.Maximum - Slider.Minimum);
                double thumbPosition = yValueRatio * Slider.ActualHeight;

                //  Update previous value and update interface.
                _previousPosition = Slider.Value;
                UpdateMuteButtonIcon();

                //  Invoke external action.
                OnHandlerDrag?.Invoke(this,
                    new SliderDragEventArgs(SliderDragAction.ON_DRAG, Slider.Value));
            }
        }

        #endregion INTERFACE INTERACTION METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change mute button icon according to slider position. </summary>
        private void UpdateMuteButtonIcon()
        {
            //  Calculate handler position level.
            var sliderRange = Slider.Maximum - Slider.Minimum;
            var oneOfThree = sliderRange / 3;

            //  Set propertly icon.
            if (Slider.Value <= Slider.Minimum)
                MuteButtonPackIcon.Kind = PackIconKind.VolumeMute;

            else if (Slider.Value <= Slider.Minimum + oneOfThree)
                MuteButtonPackIcon.Kind = PackIconKind.VolumeLow;

            else if (Slider.Value <= Slider.Minimum + (2 * oneOfThree))
                MuteButtonPackIcon.Kind = PackIconKind.VolumeMedium;

            else if (Slider.Value <= Slider.Maximum)
                MuteButtonPackIcon.Kind = PackIconKind.VolumeHigh;

            else
                MuteButtonPackIcon.Kind = PackIconKind.VolumeMedium;
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  Update initialization flag.
            _initialized = true;

            //  Setup values.
            _previousPosition = Slider.Value;
        }

        #endregion USER CONTROL METHODS

    }
}
