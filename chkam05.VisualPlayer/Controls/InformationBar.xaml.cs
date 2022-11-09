using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public partial class InformationBar : UserControl, INotifyPropertyChanged
    {

        //  CONST

        private static readonly double CONTROL_MARGIN_PART = 4;
        private static readonly Thickness CONTROL_MARGIN_VISIBLE = new Thickness(0);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            nameof(AutoHide),
            typeof(InformationBarAutoHide),
            typeof(InformationBar),
            new PropertyMetadata(InformationBarAutoHide.STAY_5S));

        public static readonly DependencyProperty AlbumInfoProperty = DependencyProperty.Register(
            nameof(AlbumInfo),
            typeof(string),
            typeof(InformationBar),
            new PropertyMetadata("Unknown album"));

        public static readonly DependencyProperty ArtistInfoProperty = DependencyProperty.Register(
            nameof(ArtistInfo),
            typeof(string),
            typeof(InformationBar),
            new PropertyMetadata("Unknown artist"));

        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register(
            nameof(CurrentTime),
            typeof(TimeSpan),
            typeof(InformationBar),
            new PropertyMetadata(new TimeSpan(0)));

        public static readonly DependencyProperty FullTimeProperty = DependencyProperty.Register(
            nameof(FullTime),
            typeof(TimeSpan),
            typeof(InformationBar),
            new PropertyMetadata(new TimeSpan(0)));

        public static readonly DependencyProperty HoverAreaSizeProperty = DependencyProperty.Register(
            nameof(HoverAreaSize),
            typeof(double),
            typeof(InformationBar),
            new PropertyMetadata(32.0));

        public static readonly DependencyProperty LockVisibilityProperty = DependencyProperty.Register(
            nameof(LockVisibility),
            typeof(bool),
            typeof(InformationBar),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TitleInfoProperty = DependencyProperty.Register(
            nameof(TitleInfo),
            typeof(string),
            typeof(InformationBar),
            new PropertyMetadata("No name"));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<InformationBarAnimateEventArgs> OnAnimate;
        public event EventHandler<InformationBarAnimationFinishEventArgs> OnAnimationFinish;


        //  VARIABLES

        private DispatcherTimer _autoHideTimer;
        private DateTime _autoHideTime;
        private Thickness _hideMargin = CONTROL_MARGIN_VISIBLE;
        private bool _titleScroll = false;

        private InformationBarState _controlState = InformationBarState.HIDDEN;
        private InformationBarState? _nextControlState = null;

        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public InformationBarAutoHide AutoHide
        {
            get => (InformationBarAutoHide)GetValue(AutoHideProperty);
            set
            {
                SetValue(AutoHideProperty, value);
                OnPropertyChanged(nameof(AutoHide));
            }
        }

        public InformationBarState ControlState
        {
            get => _controlState;
        }

        public bool CanShow
        {
            get => !LockVisibility &&
                (_autoHideTimer.IsEnabled || IsMouseOver || AutoHide != InformationBarAutoHide.AUTOHIDE);
        }

        public ImageSource CoverImage
        {
            get => coverImage.Cover;
            set => coverImage.Cover = value;
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

        public double HoverAreaSize
        {
            get => (double)GetValue(HoverAreaSizeProperty);
            set
            {
                SetValue(HoverAreaSizeProperty, value);
                OnPropertyChanged(nameof(HoverAreaSize));
            }
        }

        public double InformationsHeight
        {
            get => InformationBorder.ActualHeight;
        }

        public bool IsExpanded
        {
            get => _controlState == InformationBarState.VISIBLE
                || _nextControlState == InformationBarState.VISIBLE;
        }

        public bool LockVisibility
        {
            get => (bool)GetValue(LockVisibilityProperty);
            set => SetValue(LockVisibilityProperty, value);
        }


        public string AlbumInfo
        {
            get => (string)GetValue(AlbumInfoProperty);
            set
            {
                SetValue(AlbumInfoProperty, value);
                OnPropertyChanged(nameof(AlbumInfo));
            }
        }

        public string ArtistInfo
        {
            get => (string)GetValue(ArtistInfoProperty);
            set
            {
                SetValue(ArtistInfoProperty, value);
                OnPropertyChanged(nameof(ArtistInfo));
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                var value = GetValue(CurrentTimeProperty);

                if (value == DependencyProperty.UnsetValue)
                    return new TimeSpan(0);

                return (TimeSpan)value;
            }
            set
            {
                SetValue(CurrentTimeProperty, value);
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        public TimeSpan FullTime
        {
            get
            {
                var value = GetValue(FullTimeProperty);

                if (value == DependencyProperty.UnsetValue)
                    return new TimeSpan(0);

                return (TimeSpan)value;
            }
            set
            {
                SetValue(FullTimeProperty, value);
                OnPropertyChanged(nameof(FullTime));
            }
        }

        public string TitleInfo
        {
            get => (string)GetValue(TitleInfoProperty);
            set
            {
                SetValue(TitleInfoProperty, value);
                OnPropertyChanged(nameof(TitleInfo));

                //TitleMarqueeTextBlock.Scroll = TitleMarqueeTextBlock.IsTooLong;
            }
        }

        public bool TitleScroll
        {
            get => _titleScroll;
            private set
            {
                _titleScroll = value;
                OnPropertyChanged(nameof(TitleScroll));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InformationBar class constructor. </summary>
        public InformationBar()
        {
            //  Setup modules.
            ConfigManager = ConfigManager.Instance;

            _autoHideTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1),
                IsEnabled = false
            };
            _autoHideTimer.Tick += AutoHideTimer_Tick;

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
            if (!LockVisibility && ControlState == InformationBarState.HIDDEN)
            {
                HideMargin = CONTROL_MARGIN_VISIBLE;
                Storyboard storyboard = Resources["ShowHideStoryboard"] as Storyboard;
                PrepareUpdateState(InformationBarState.VISIBLE);
                OnAnimate?.Invoke(this, new InformationBarAnimateEventArgs(_nextControlState));
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Animate hiding control interface. </summary>
        private void AnimateHideControl()
        {
            if (!LockVisibility && ControlState != InformationBarState.HIDDEN)
            {
                HideMargin = CalculateHiddenMargin();
                Storyboard showingSb = Resources["ShowHideStoryboard"] as Storyboard;
                PrepareUpdateState(InformationBarState.HIDDEN);
                OnAnimate?.Invoke(this, new InformationBarAnimateEventArgs(_nextControlState));
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
            OnAnimationFinish?.Invoke(this, new InformationBarAnimationFinishEventArgs(ControlState));
        }

        #endregion ANIMATION METHODS

        #region AUTOHIDE CONTROL

        //  --------------------------------------------------------------------------------
        private void SetupAutoHideTimer()
        {
            _autoHideTime = DateTime.UtcNow;

            switch (AutoHide)
            {
                case InformationBarAutoHide.STAY_3S:
                case InformationBarAutoHide.STAY_5S:
                case InformationBarAutoHide.STAY_10S:
                    _autoHideTimer.Start();
                    break;

                case InformationBarAutoHide.AUTOHIDE:
                case InformationBarAutoHide.INFINITE:
                    _autoHideTimer.IsEnabled = false;
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan diffrence = DateTime.UtcNow - _autoHideTime;
            double seconds = diffrence.TotalSeconds;

            if (_autoHideTimer.IsEnabled)
            {
                switch (AutoHide)
                {
                    case InformationBarAutoHide.STAY_3S:
                        if (seconds > 3)
                        {
                            HideInterface();
                            _autoHideTimer.IsEnabled = false;
                        }
                        break;

                    case InformationBarAutoHide.STAY_5S:
                        if (seconds > 5)
                        {
                            HideInterface();
                            _autoHideTimer.IsEnabled = false;
                        }
                        break;

                    case InformationBarAutoHide.STAY_10S:
                        if (seconds > 10)
                        {
                            HideInterface();
                            _autoHideTimer.IsEnabled = false;
                        }
                        break;

                    case InformationBarAutoHide.AUTOHIDE:
                        _autoHideTimer.IsEnabled = false;
                        HideInterface();
                        break;

                    case InformationBarAutoHide.INFINITE:
                        _autoHideTimer.IsEnabled = false;
                        break;
                }
            }
        }

        #endregion AUTOHIDE CONTROL

        #region CONTROL STATE MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Prepare control state for update. </summary>
        /// <param name="controlState"> New menu state. </param>
        public void PrepareUpdateState(InformationBarState controlState = InformationBarState.VISIBLE)
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
        /// <param name="showWithTimer"> Show with timer. </param>
        public void ShowInterface(bool showWithTimer = false)
        {
            if (showWithTimer)
                SetupAutoHideTimer();

            if (!LockVisibility)
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
            SetupAutoHideTimer();
            AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after leaving cursor from ControlBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoHide != InformationBarAutoHide.INFINITE && !_autoHideTimer.IsEnabled)
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

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading user control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateShowControl();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing user control size. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Size Changed Event Arguments. </param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //TitleMarqueeTextBlock.Scroll = TitleMarqueeTextBlock.IsTooLong;
        }

        #endregion USER CONTROL METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate margin for hide component. </summary>
        /// <returns> Margin of hidden component. </returns>
        private Thickness CalculateHiddenMargin()
        {
            double marginTop = CONTROL_MARGIN_PART + InformationsHeight;
            return new Thickness(0, -marginTop, 0, 0);
        }

        #endregion UTILITY METHODS

    }
}
