using chkam05.VisualPlayer.Controls.Messages.Data;
using chkam05.VisualPlayer.Controls.Messages.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls.Messages
{
    public partial class AwaitMessage : Page, IProgressMessage, INotifyPropertyChanged
    {

        //  CONST

        private const double INDICATOR_INNER_RADIUS = 6;
        private const long TICK = 50000;

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty IndicatorHeightProperty = DependencyProperty.Register(
            nameof(IndicatorHeight),
            typeof(double),
            typeof(AwaitMessage),
            new PropertyMetadata(32.0));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(AwaitMessage),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(AwaitMessage),
            new PropertyMetadata(PackIconKind.ProgressClock));

        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register(
            nameof(ProgressValue),
            typeof(double),
            typeof(AwaitMessage),
            new PropertyMetadata(0.0));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageCloseEventArgs> OnMessageClose;


        //  VARIABLES

        private BackgroundWorker _animationWorker;
        private double _indicatorRadius = 0;
        private Point _indicatorCenterPoint;

        public AwaitIndicatorData AwaitIndicatorData { get; private set; }
        public Configuration Configuration { get; private set; }
        public DispatcherHandler DispatcherHandler { get; private set; }
        public IMessagesManager MessagesManager { get; private set; }
        public MessageResult Result { get; private set; }


        //  GETTERS & SETTERS

        public double IndicatorHeight
        {
            get => (double)GetValue(IndicatorHeightProperty);
            set
            {
                SetValue(IndicatorHeightProperty, value);
                OnPropertyChanged(nameof(IndicatorHeight));
            }
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set
            {
                SetValue(MessageProperty, value);
                OnPropertyChanged(nameof(Message));
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

        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set
            {
                SetValue(ProgressValueProperty, Math.Max(Math.Min(value, 100.0), 0.0));
                OnPropertyChanged(nameof(ProgressValue));//ProgressBarStyle
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AwaitMessage class constructor. </summary>
        /// <param name="messagesManager"> Messages manager where page will be presented. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        public AwaitMessage(IMessagesManager messagesManager, string title, string message,
            EventHandler<MessageCloseEventArgs> messageCloseEvent)
        {
            //  Setup modules.
            Configuration = Configuration.Instance;
            DispatcherHandler = new DispatcherHandler(Dispatcher);

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            AwaitIndicatorData = new AwaitIndicatorData();
            MessagesManager = messagesManager;
            Message = message;
            OnMessageClose = messageCloseEvent;
            Result = MessageResult.NONE;
            Title = title;
        }

        #endregion CLASS METHODS

        #region INDICATOR MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Indicator animation work method. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Do Work Event Arguments. </param>
        private void AnimateIndicator(object sender, DoWorkEventArgs e)
        {
            var bgWorker = sender as BackgroundWorker;

            double startPos = 0.0;
            double endPos = 0.0;
            bool inverse = false;
            bool inversed = false;
            bool isLarge = false;
            long tick = 0;

            while (!bgWorker.CancellationPending)
            {
                if (tick < TICK)
                {
                    tick++;
                    continue;
                }

                //  Update indicator progress.
                if (endPos == startPos)
                {
                    inverse = !inverse;
                    inversed = true;
                }

                if (inverse)
                {
                    startPos = (startPos + 1) % 360;
                    if (!inversed && startPos % 3 == 0)
                        endPos = (endPos + 1) % 360;
                }
                else
                {
                    endPos = (endPos + 1) % 360;
                    if (!inversed && endPos % 3 == 0)
                        startPos = (startPos + 1) % 360;
                }

                inversed = false;
                tick = 0;

                //  Calculate indicator properties.
                Point outerStartPoint = CalculateOuterIndicatorStartPoint(startPos);
                Point outerEndPoint = CalculateOuterIndicatorEndPoint(endPos);
                Point innerStartPoint = CalculateInnerIndicatorStartPoint(endPos);
                Point innerEndPoint = CalculateInnerIndicatorEndPoint(startPos);

                isLarge = CheckIndicatorArcSizeType(endPos, startPos);

                bool secDispResult = DispatcherHandler.TryInvoke(() =>
                {
                    var data = AwaitIndicatorData;

                    if (data.InnerArcLarge != data.OuterArcLarge || data.InnerArcLarge != isLarge)
                    {
                        data.InnerArcLarge = isLarge;
                        data.OuterArcLarge = isLarge;
                    }

                    data.OuterArcEndPoint = outerEndPoint;
                    data.InnerArcStartPoint = innerStartPoint;
                    data.OuterArcStartPoint = outerStartPoint;
                    data.InnerArcEndPoint = innerEndPoint;
                });

                //  Break after error from dispatcher.
                if (!secDispResult)
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator inner arc end point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator inner arc end point. </returns>
        private Point CalculateInnerIndicatorEndPoint(double progress)
        {
            return MathUtilities.FindPointOnCircle(
                _indicatorCenterPoint, _indicatorRadius - INDICATOR_INNER_RADIUS, progress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator inner arc start point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator inner arc start point. </returns>
        private Point CalculateInnerIndicatorStartPoint(double progress)
        {
            return MathUtilities.FindPointOnCircle(
                _indicatorCenterPoint, _indicatorRadius - INDICATOR_INNER_RADIUS, progress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator outer arc end point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator outer arc end point. </returns>
        private Point CalculateOuterIndicatorEndPoint(double progress)
        {
            return MathUtilities.FindPointOnCircle(
                _indicatorCenterPoint, _indicatorRadius, progress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator outer arc start point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator outer arc start point. </returns>
        private Point CalculateOuterIndicatorStartPoint(double progress)
        {
            return MathUtilities.FindPointOnCircle(
                _indicatorCenterPoint, _indicatorRadius, progress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if indicator arcs need to be changed to large. </summary>
        /// <returns> True - large arcs; False - otherwise. </returns>
        private bool CheckIndicatorArcSizeType(double endProgress, double startProgress)
        {
            double correctedIndicatorEnd = endProgress < startProgress
                ? endProgress + 360 : endProgress;

            return correctedIndicatorEnd - startProgress >= 180;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup indicator configuration. </summary>
        private void SetupIndicator()
        {
            _indicatorRadius = IndicatorHeight / 2;
            _indicatorCenterPoint = new Point(_indicatorRadius, _indicatorRadius);

            double _innerRadius = _indicatorRadius - INDICATOR_INNER_RADIUS;
            
            AwaitIndicatorData.OuterArcSize = new Size(_indicatorRadius, _indicatorRadius);
            AwaitIndicatorData.InnerArcSize = new Size(_innerRadius, _innerRadius);

            _animationWorker = new BackgroundWorker();
            _animationWorker.WorkerSupportsCancellation = true;
            _animationWorker.DoWork += AnimateIndicator;
            _animationWorker.RunWorkerAsync();
        }

        #endregion INDICATOR MANAGEMENT METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message. </summary>
        public void CloseMessage()
        {
            if (MessagesManager.CanGoBack)
                MessagesManager.GoBack();
            else
                MessagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get mapped progress value. </summary>
        /// <param name="maxValue"> Max value to get. </param>
        /// <returns> Mapped progress value. </returns>
        public double GetMapProgressValue(double maxValue)
        {
            return (maxValue > 0 && ProgressValue > 0) ? ProgressValue * maxValue / 100 : 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set progress value by mapping value. </summary>
        /// <param name="value"> Value to map. </param>
        /// <param name="maxValue"> Max value to set. </param>
        public void SetMapProgressValue(double value, double maxValue)
        {
            ProgressValue = (maxValue > 0 && value > 0) ? value * 100 / maxValue : 0;
        }

        #endregion INTERACTION METHODS

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

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading message page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetupIndicator();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading message page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_animationWorker != null && _animationWorker.IsBusy)
                _animationWorker.CancelAsync();

            OnMessageClose?.Invoke(this, new MessageCloseEventArgs(Result));
        }

        #endregion PAGE METHODS

    }
}
