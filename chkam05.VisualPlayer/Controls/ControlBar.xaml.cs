using chkam05.Tools.ControlsEx;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Components.Data;
using chkam05.VisualPlayer.Components.Events;
using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Static;
using CSCore.SoundOut;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace chkam05.VisualPlayer.Controls
{
    public partial class ControlBar : UserControl, INotifyPropertyChanged
    {

        //  CONST

        private static readonly double CONTROL_MARGIN_PART = 4;
        private static readonly Thickness CONTROL_MARGIN_VISIBLE = new Thickness(0);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            nameof(AutoHide),
            typeof(bool),
            typeof(ControlBar),
            new PropertyMetadata(true));

        public static readonly DependencyProperty LockVisibilityProperty = DependencyProperty.Register(
            nameof(LockVisibility),
            typeof(bool),
            typeof(ControlBar),
            new PropertyMetadata(false));

        public static readonly DependencyProperty PlayPauseButtonStateProperty = DependencyProperty.Register(
            nameof(PlayPauseButtonState),
            typeof(PlaybackState),
            typeof(ControlBar),
            new PropertyMetadata(PlaybackState.Stopped));
        
        public static readonly DependencyProperty RepeatButtonStateProperty = DependencyProperty.Register(
            nameof(RepeatButtonState),
            typeof(Repeat),
            typeof(ControlBar),
            new PropertyMetadata(Repeat.NORMAL));
        
        public static readonly DependencyProperty ShuffleButtonStateProperty = DependencyProperty.Register(
            nameof(ShuffleButtonState),
            typeof(bool),
            typeof(ControlBar),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TrackLengthProperty = DependencyProperty.Register(
            nameof(TrackLength),
            typeof(double),
            typeof(ControlBar),
            new PropertyMetadata(1.0));

        public static readonly DependencyProperty TrackPositionProperty = DependencyProperty.Register(
            nameof(TrackPosition),
            typeof(double),
            typeof(ControlBar),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty VolumeControlPoppedProperty = DependencyProperty.Register(
            nameof(VolumeControlPopped),
            typeof(bool),
            typeof(ControlBar),
            new PropertyMetadata(false));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<RoutedEventArgs> OnNextClick;
        public event EventHandler<RoutedEventArgs> OnPlayPauseClick;
        public event EventHandler<RoutedEventArgs> OnPreviousClick;
        public event EventHandler<RoutedEventArgs> OnRepeatClick;
        public event EventHandler<RoutedEventArgs> OnStopClick;
        public event EventHandler<RoutedEventArgs> OnShuffleClick;
        public event EventHandler<RoutedEventArgs> OnVolumeClick;
        public event EventHandler<SliderValueChangedEventArgs<double>> OnTrackSliderValueChanged;
        public event EventHandler<ControlBarAnimateEventArgs> OnAnimate;
        public event EventHandler<ControlBarAnimationFinishEventArgs> OnAnimationFinish;


        //  VARIABLES

        private ControlBarState _controlState = ControlBarState.HIDDEN;
        private Thickness _hideMargin = CONTROL_MARGIN_VISIBLE;
        private ControlBarState? _nextControlState = null;
        private bool _trackSliderInUse = false;

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public bool AutoHide
        {
            get => (bool)GetValue(AutoHideProperty);
            set => SetValue(AutoHideProperty, value);
        }

        public double ControlHeight
        {
            get => ControlBorder.ActualHeight;
        }

        public ControlBarState ControlState
        {
            get => _controlState;
        }

        public Thickness HideMargin
        {
            get => _hideMargin;
            set
            {
                _hideMargin = value;
                OnPropertyChanged(nameof(HideMargin));
            }
        }

        public bool IsExpanded
        {
            get => _controlState == ControlBarState.VISIBLE 
                || _nextControlState == ControlBarState.VISIBLE;
        }

        public bool LockVisibility
        {
            get => (bool)GetValue(LockVisibilityProperty);
            set => SetValue(LockVisibilityProperty, value);
        }

        public PlaybackState PlayPauseButtonState
        {
            get => (PlaybackState)GetValue(PlayPauseButtonStateProperty);
            set
            {
                SetValue(PlayPauseButtonStateProperty, value);
                OnPropertyChanged(nameof(PlayPauseButtonState));
            }
        }

        public Repeat RepeatButtonState
        {
            get => (Repeat)GetValue(RepeatButtonStateProperty);
            set
            {
                SetValue(RepeatButtonStateProperty, value);
                OnPropertyChanged(nameof(RepeatButtonState));
            }
        }

        public bool ShuffleButtonState
        {
            get => (bool)GetValue(ShuffleButtonStateProperty);
            set
            {
                SetValue(ShuffleButtonStateProperty, value);
                OnPropertyChanged(nameof(ShuffleButtonState));
            }
        }

        public double TrackLength
        {
            get => (double)GetValue(TrackLengthProperty);
            set
            {
                if (TrackPosition < value)
                    TrackPosition = value;

                SetValue(TrackLengthProperty, Math.Max(1.0, value));
                OnPropertyChanged(nameof(TrackLength));
            }
        }

        public double TrackPosition
        {
            get => (double)GetValue(TrackPositionProperty);
            set
            {
                if (_trackSliderInUse)
                    return;

                SetValue(TrackPositionProperty, Math.Max(0.0, Math.Min(value, TrackLength)));
                OnPropertyChanged(nameof(TrackPosition));
            }
        }

        public bool VolumeControlPopped
        {
            get => (bool)GetValue(VolumeControlPoppedProperty);
            set
            {
                SetValue(VolumeControlPoppedProperty, value);
                OnPropertyChanged(nameof(VolumeControlPopped));

                if (!IsMouseOver && AutoHide)
                    AnimateHideControl();
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlBar class constructor. </summary>
        public ControlBar()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            ControlGrid.Margin = CalculateHiddenMargin();
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Animate showing control interface. </summary>
        private void AnimateShowControl()
        {
            if (!LockVisibility && ControlState == ControlBarState.HIDDEN)
            {
                HideMargin = CONTROL_MARGIN_VISIBLE;
                Storyboard storyboard = Resources["ShowHideStoryboard"] as Storyboard;
                PrepareUpdateState(ControlBarState.VISIBLE);
                OnAnimate?.Invoke(this, new ControlBarAnimateEventArgs(_nextControlState));
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate hiding control interface. </summary>
        private void AnimateHideControl()
        {
            if (!LockVisibility && ControlState != ControlBarState.HIDDEN && !VolumeControlPopped)
            {
                HideMargin = CalculateHiddenMargin();
                Storyboard showingSb = Resources["ShowHideStoryboard"] as Storyboard;
                PrepareUpdateState(ControlBarState.HIDDEN);
                OnAnimate?.Invoke(this, new ControlBarAnimateEventArgs(_nextControlState));
                showingSb?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after completing Storyboard animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            UpdateState();
            OnAnimationFinish?.Invoke(this, new ControlBarAnimationFinishEventArgs(ControlState));
        }

        #endregion ANIMATION METHODS

        #region CONTROL BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Repeat ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            OnRepeatClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Shuffle ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            OnShuffleClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Previous ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            OnPreviousClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking PlayPause ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            OnPlayPauseClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Next ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            OnNextClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Stop ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            OnStopClick?.Invoke(this, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Volume ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            OnVolumeClick?.Invoke(this, e);
        }

        #endregion CONTROL BUTTONS METHODS

        #region CONTROL STATE MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Prepare control state for update. </summary>
        /// <param name="controlState"> New menu state. </param>
        public void PrepareUpdateState(ControlBarState controlState = ControlBarState.VISIBLE)
        {
            _nextControlState = controlState;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update control state to previous prepared state. </summary>
        public void UpdateState()
        {
            if (_nextControlState.HasValue)
                _controlState = _nextControlState.Value;

            _nextControlState = null;
        }

        #endregion CONTROL STATE MANAGEMENT

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show interface programmatically. </summary>
        public void ShowInterface()
        {
            AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide interface programmatically. </summary>
        /// <param name="ifNotMouseOver"> Only if cursor is not over control. </param>
        public void HideInterface(bool ifNotMouseOver = true)
        {
            if (ifNotMouseOver && IsMouseOver)
                return;

            AnimateHideControl();
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region MOVEMENT INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after entering cursor over ControlBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after leaving cursor from ControlBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoHide)
                AnimateHideControl();
        }

        #endregion MOVEMENT INTERACTION METHODS

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

        #region TRACK SLIDER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on track ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void ControlSlider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            _trackSliderInUse = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing click from track ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void ControlSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (_trackSliderInUse)
            {
                var slider = (SliderEx)sender;
                var args = new SliderValueChangedEventArgs<double>(slider.Value, true);
                OnTrackSliderValueChanged?.Invoke(sender, args);
            }

            _trackSliderInUse = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing value of track ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void TrackControlSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_trackSliderInUse)
            {
                var args = new SliderValueChangedEventArgs<double>(e.NewValue, false);
                OnTrackSliderValueChanged?.Invoke(sender, args);
            }
        }

        #endregion TRACK SLIDER CONTROL METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading user control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateShowControl();
        }

        #endregion USER CONTROL METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate margin for hide component. </summary>
        /// <returns> Margin of hidden component. </returns>
        private Thickness CalculateHiddenMargin()
        {
            double marginBottom = CONTROL_MARGIN_PART + ControlHeight;
            return new Thickness(0, 0, 0, -marginBottom);
        }

        #endregion UTILITY METHODS

    }
}
