using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Utilities.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace chkam05.VisualPlayer.Controls
{
    public partial class MarqueeTextBlock : UserControl, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
            nameof(Interval),
            typeof(double),
            typeof(MarqueeTextBlock),
            new PropertyMetadata(100.0));

        public static readonly DependencyProperty MarqueeBehaviourProperty = DependencyProperty.Register(
            nameof(MarqueeBehaviour),
            typeof(MarqueeBehaviour),
            typeof(MarqueeTextBlock),
            new PropertyMetadata(MarqueeBehaviour.LEFT_OUT_TO_RIGHT_OUT));

        public static readonly DependencyProperty MarqueeStateProperty = DependencyProperty.Register(
            nameof(MarqueeState),
            typeof(MarqueeState),
            typeof(MarqueeTextBlock),
            new PropertyMetadata(MarqueeState.ENABLED));

        public static readonly DependencyProperty PauseIntervalProperty = DependencyProperty.Register(
            nameof(PauseInterval),
            typeof(double),
            typeof(MarqueeTextBlock),
            new PropertyMetadata(1000.0));

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            nameof(Step),
            typeof(double),
            typeof(MarqueeTextBlock),
            new PropertyMetadata(2.0));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(MarqueeTextBlock),
            new PropertyMetadata("This is test"));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private double _marqueeDirection = 1;
        private double _marqueeX = 0;
        private DispatcherTimer _timer;


        //  GETTERS & SETTERS

        public double CanvasWidth
        {
            get => ContentCanvas.ActualWidth;
        }

        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set
            {
                SetValue(IntervalProperty, Math.Max(value, 10.0));
                OnPropertyChanged(nameof(Interval));

                _timer.Stop();
                SetupAnimation();
            }
        }

        public bool IsTextTooLong
        {
            get => TextWidth >= CanvasWidth;
        }

        public MarqueeBehaviour MarqueeBehaviour
        {
            get => (MarqueeBehaviour)GetValue(MarqueeBehaviourProperty);
            set
            {
                SetValue(MarqueeBehaviourProperty, value);
                OnPropertyChanged(nameof(MarqueeBehaviour));
            }
        }

        public MarqueeState MarqueeState
        {
            get => (MarqueeState)GetValue(MarqueeStateProperty);
            set
            {
                SetValue(MarqueeStateProperty, value);
                OnPropertyChanged(nameof(MarqueeState));
            }
        }

        public double PauseInterval
        {
            get => (double)GetValue(PauseIntervalProperty);
            set
            {
                SetValue(PauseIntervalProperty, Math.Max(value, 1000.0));
                OnPropertyChanged(nameof(PauseInterval));
            }
        }

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set
            {
                SetValue(StepProperty, Math.Max(value, 1.0));
                OnPropertyChanged(nameof(Step));
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(nameof(Text));
            }
        }

        public double TextPosition
        {
            get => Canvas.GetLeft(ContentText);
        }

        public double TextWidth
        {
            get => ContentText.ActualWidth;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MarqueeTextBlock class constructor. </summary>
        public MarqueeTextBlock()
        {
            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region ANIMATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update marquee animation frame. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void AnimationFrameUpdate(object sender, EventArgs e)
        {
            if (MarqueeState == MarqueeState.DISABLED)
            {
                AnimationSetDefaultFrame();
                return;
            }

            if (MarqueeState == MarqueeState.TOO_LONG_TEXT && !IsTextTooLong)
            {
                AnimationSetDefaultFrame();
                return;
            }

            switch (MarqueeBehaviour)
            {
                case MarqueeBehaviour.RIGHT_OUT_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_IN_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_TO_LEFT:
                    _marqueeDirection = -1;
                    _marqueeX = _marqueeX + (Step * _marqueeDirection);
                    break;

                case MarqueeBehaviour.RIGHT_TO_LEFT_TO_RIGHT:
                case MarqueeBehaviour.LEFT_TO_RIGHT_TO_LEFT:
                    _marqueeX = _marqueeX + (Step * _marqueeDirection);
                    break;

                case MarqueeBehaviour.LEFT_OUT_TO_RIGHT_OUT:
                case MarqueeBehaviour.LEFT_IN_TO_RIGHT_OUT:
                case MarqueeBehaviour.LEFT_TO_RIGHT:
                    _marqueeDirection = 1;
                    _marqueeX = _marqueeX + (Step * _marqueeDirection);
                    break;

                default:
                    _marqueeDirection = -1;
                    break;
            }

            Canvas.SetLeft(ContentText, _marqueeX);

            if (AnimationDetectLastFrame())
            {
                AnimationSetStartupFrame();
                WaitAnimation();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Detect if frame is last to reset animation. </summary>
        /// <returns> True - last frame; False - otherwise. </returns>
        private bool AnimationDetectLastFrame()
        {
            switch (MarqueeBehaviour)
            {
                case MarqueeBehaviour.RIGHT_OUT_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_IN_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_TO_LEFT_OUT:
                    return TextPosition + TextWidth <= 0;

                case MarqueeBehaviour.RIGHT_TO_LEFT:
                    return TextPosition + TextWidth <= CanvasWidth;

                case MarqueeBehaviour.RIGHT_TO_LEFT_TO_RIGHT:
                case MarqueeBehaviour.LEFT_TO_RIGHT_TO_LEFT:
                    if (_marqueeDirection < 0)
                        return IsTextTooLong
                            ? TextPosition + TextWidth <= CanvasWidth
                            : TextPosition <= 0;
                    else
                        return IsTextTooLong
                            ? TextPosition >= 0
                            : TextPosition + TextWidth >= CanvasWidth;

                case MarqueeBehaviour.LEFT_OUT_TO_RIGHT_OUT:
                    return TextPosition > CanvasWidth;

                case MarqueeBehaviour.LEFT_IN_TO_RIGHT_OUT:
                    return TextPosition > CanvasWidth;

                case MarqueeBehaviour.LEFT_TO_RIGHT:
                    return TextPosition + TextWidth >= CanvasWidth;

                default:
                    return TextPosition + TextWidth <= 0;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set default frame where text is placed at 0 position. </summary>
        private void AnimationSetDefaultFrame()
        {
            if (_marqueeX != 0)
            {
                _marqueeX = 0;
                Canvas.SetLeft(ContentText, 0);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set startup frame where text is placed in beginning of marquee. </summary>
        private void AnimationSetStartupFrame()
        {
            switch (MarqueeBehaviour)
            {
                case MarqueeBehaviour.RIGHT_OUT_TO_LEFT_OUT:
                    _marqueeX = CanvasWidth;
                    break;

                case MarqueeBehaviour.RIGHT_IN_TO_LEFT_OUT:
                case MarqueeBehaviour.RIGHT_TO_LEFT:
                    _marqueeX = IsTextTooLong ? 0 : CanvasWidth - TextWidth;
                    break;

                case MarqueeBehaviour.RIGHT_TO_LEFT_OUT:
                case MarqueeBehaviour.LEFT_IN_TO_RIGHT_OUT:
                case MarqueeBehaviour.LEFT_TO_RIGHT:
                    _marqueeX = 0;
                    break;

                case MarqueeBehaviour.RIGHT_TO_LEFT_TO_RIGHT:
                case MarqueeBehaviour.LEFT_TO_RIGHT_TO_LEFT:
                    if (_marqueeDirection < 0)
                    {
                        _marqueeDirection = 1;
                        _marqueeX = IsTextTooLong ? CanvasWidth - TextWidth : 0;
                    }
                    else
                    {
                        _marqueeDirection = -1;
                        _marqueeX = IsTextTooLong ? 0 : CanvasWidth - TextWidth;
                    }
                    break;

                case MarqueeBehaviour.LEFT_OUT_TO_RIGHT_OUT:
                    _marqueeX = -TextWidth;
                    break;

                default:
                    _marqueeX = 0;
                    break;
            }

            Canvas.SetLeft(ContentText, _marqueeX);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Process animation break. </summary>
        public void WaitAnimation()
        {
            var waitTimer = new DispatcherTimer();
            waitTimer.Interval = TimeSpan.FromMilliseconds(PauseInterval);
            waitTimer.Tick += (s, e) =>
            {
                _timer.IsEnabled = true;
                waitTimer.Stop();
            };

            _timer.IsEnabled = false;
            waitTimer.Start();
        }

        #endregion ANIMATION MANAGEMENT METHODS

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

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup timer. </summary>
        private void SetupAnimation()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(Interval);
            _timer.Tick += AnimationFrameUpdate;
            _timer.Start();
        }

        #endregion SETUP METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading user control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetupAnimation();
        }

        #endregion USER CONTROL METHODS

    }
}
