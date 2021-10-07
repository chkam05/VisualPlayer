using chkam05.VisualPlayer.Components.EventArgs;
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
    public partial class ScaledHorizontalSlider : UserControl
    {

        //  EVENTS

        public event EventHandler<SliderDragEventArgs> OnHandlerDrag;


        //  VARIABLES

        private bool _dragging = false;
        private string _maxText = "";
        private string _midText = "";
        private string _minText = "";
        private string _prefix = "";


        #region GETTERS & SETTERS

        public new bool IsEnabled
        {
            get => Slider.IsEnabled;
            set => Slider.IsEnabled = value;
        }

        public Brush HandlerBrush
        {
            get => Slider.Foreground;
            set => Slider.Foreground = value;
        }

        public string MaxText
        {
            get => _maxText;
            set
            {
                _maxText = value;
                UpdateMaxText();
            }
        }

        public string MidText
        {
            get => _midText;
            set
            {
                _midText = value;
                UpdateValueText();
            }
        }

        public string MinText
        {
            get => _minText;
            set
            {
                _minText = value;
                UpdateMinText();
            }
        }

        public double Max
        {
            get => Slider.Maximum;
            set
            {
                if (Slider.Value > value)
                    Slider.Value = Math.Max(Slider.Minimum, value);

                Slider.Maximum = value > Slider.Minimum ? value : Slider.Minimum + 1;
                UpdateMaxText();
            }
        }

        public double Min
        {
            get => Slider.Minimum;
            set
            {
                if (Slider.Value < value)
                    Slider.Value = Math.Min(value, Slider.Maximum);

                Slider.Minimum = value < Slider.Maximum ? value : Slider.Maximum - 1;
                UpdateMinText();
            }
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                UpdateMaxText();
                UpdateMinText();
                UpdateValueText();
            }
        }

        public double Value
        {
            get => Slider.Value;
            set
            {
                Slider.Value = Math.Max(Slider.Minimum, Math.Min(value, Slider.Maximum));
                UpdateValueText();
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ScaledHorizontalSlider class constructor. </summary>
        public ScaledHorizontalSlider()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERFACE INTERACTION METHODS

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

                UpdateValueText();

                //  Invoke external action.
                OnHandlerDrag?.Invoke(this,
                    new SliderDragEventArgs(SliderDragAction.ON_DRAG, Slider.Value));
            }
        }

        #endregion INTERFACE INTERACTION METHODS

        #region INTERFACE UPDATE METHODS

        //  --------------------------------------------------------------------------------
        private void UpdateMaxText()
        {
            MaxTextBlock.Text = string.IsNullOrEmpty(_maxText)
                ? ((int)Slider.Maximum).ToString()
                     + (!string.IsNullOrEmpty(_prefix) ? $" {_prefix}" : "")
                : _maxText;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateMinText()
        {
            MinTextBlock.Text = string.IsNullOrEmpty(_minText)
                ? ((int)Slider.Minimum).ToString()
                     + (!string.IsNullOrEmpty(_prefix) ? $" {_prefix}" : "")
                : _minText;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateValueText()
        {
            if (Value == Max && !string.IsNullOrEmpty(_maxText))
            {
                ValueTextBlock.Text = _maxText;
            }

            else if (Value == Min && !string.IsNullOrEmpty(_minText))
            {
                ValueTextBlock.Text = _minText;
            }

            else
            {
                ValueTextBlock.Text = string.IsNullOrEmpty(_midText)
                ? ((int)Slider.Value).ToString()
                     + (!string.IsNullOrEmpty(_prefix) ? $" {_prefix}" : "")
                : _midText;
            }
        }

        #endregion INTERFACE UPDATE METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMaxText();
            UpdateMinText();
            UpdateValueText();
        }

        #endregion USER CONTROL METHODS

    }
}
