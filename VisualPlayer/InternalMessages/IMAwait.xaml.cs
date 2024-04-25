using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages.Enums;
using VisualPlayer.InternalMessages.Models;
using VisualPlayer.Utilities;

namespace VisualPlayer.InternalMessages
{
    public partial class IMAwait : IMBase
    {

        //  CONST

        private const double ANIMATION_SPEED = 1d;
        private const double INDICATOR_MARGIN = 8d;
        private const double INDICATOR_SHIFT = 6d;


        //  VARIABLES

        private BackgroundWorker _animationWorker;

        private Point _indicatorCenterPoint;
        private double _indicatorRadius = 0;
        private Size _indicatorSize;
        private double _indicatorStartPos = 0.0;
        private double _indicatorEndPos = 0.0;
        private bool _workInverse = false;
        private bool _workInversed = false;

        private bool _allowCancel = true;
        private TimeSpan _animationSpeed = TimeSpan.FromMilliseconds(ANIMATION_SPEED);
        private PackIconKind _icon = PackIconKind.None;
        private bool _isFinished = false;
        private string _message = string.Empty;


        //  GETTERS & SETTERS

        public bool AllowCancel
        {
            get => _allowCancel;
            set => UpdateProperty(ref _allowCancel, value);
        }

        public TimeSpan AnimationSpeed
        {
            get => _animationSpeed;
            set => UpdateProperty(ref _animationSpeed, value);
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public bool IsFinished
        {
            get => _isFinished;
            private set => UpdateProperty(ref _isFinished, value);
        }

        public string Message
        {
            get => _message;
            set => UpdateProperty(ref _message, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMAwait class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        public IMAwait(IIMControl imControl) : base(imControl)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message await. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Message title icon. </param>
        /// <param name="onClose"> On close method. </param>
        /// <param name="allowCancel"> Allow cancel. </param>
        /// <returns> Internal message box. </returns>
        public static IMAwait CreateMessage(IIMControl imControl, string title, string message,
            PackIconKind icon = PackIconKind.ProgressHelper,
            CloseEventHandler<IIMCloseEventArgs> onClose = null,
            bool allowCancel = true)
        {
            var imBox = new IMAwait(imControl)
            {
                AllowCancel = allowCancel,
                Icon = icon,
                Message = message,
                Title = title,
            };

            if (onClose != null)
                imBox.Closed = onClose;

            return imBox;
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Indicator aniation work method. </summary>
        /// <param name="sender"> Object that invoked the mehtod. </param>
        /// <param name="e"> Do Work Event Arguments. </param>
        private void IndicatorAnimationDoWork(object sender, DoWorkEventArgs e)
        {
            DateTime workTime = DateTime.Now;
            bool working = !_animationWorker.CancellationPending;
            TimeSpan frameTime = AnimationSpeed;

            var arcDataHandler = UpdateIndicatorAnimationFrame();
            _animationWorker.ReportProgress(0, arcDataHandler);

            while (working)
            {
                if (DateTime.Now - workTime <= frameTime)
                    continue;

                if (_animationWorker.CancellationPending)
                    break;

                arcDataHandler = UpdateIndicatorAnimationFrame();
                _animationWorker.ReportProgress(0, arcDataHandler);

                workTime = DateTime.Now;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update indicator animation frame. </summary>
        /// <returns> Arc Data Handler. </returns>
        private ArcDataHandler UpdateIndicatorAnimationFrame()
        {
            if (_indicatorEndPos == _indicatorStartPos)
            {
                _workInverse = !_workInverse;
                _workInversed = true;
            }

            if (_workInverse)
            {
                _indicatorStartPos = (_indicatorStartPos + 1) % 360;
                if (!_workInversed && _indicatorStartPos % 3 == 0)
                    _indicatorEndPos = (_indicatorEndPos + 1) % 360;
            }
            else
            {
                _indicatorEndPos = (_indicatorEndPos + 1) % 360;
                if (!_workInversed && _indicatorEndPos % 3 == 0)
                    _indicatorStartPos = (_indicatorStartPos + 1) % 360;
            }

            _workInversed = false;

            Point startPoint = CalculateOuterIndicatorStartPoint(_indicatorStartPos);
            Point endPoint = CalculateOuterIndicatorEndPoint(_indicatorEndPos);
            bool isLarge = CheckIndicatorArcSizeType(_indicatorEndPos, _indicatorStartPos);

            return new ArcDataHandler()
            {
                StartPoint = startPoint,
                EndPoint = endPoint,
                Size = _indicatorSize,
                IsLargeArc = isLarge
            };
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Indicator animation work method progress changed method. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Progress Changed Event Arguments. </param>
        private void IndicatorAnimationProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ArcDataHandler arcDataHandler)
            {
                _indicatorPathFigure.StartPoint = arcDataHandler.StartPoint;
                _indicatorArcSegment.Point = arcDataHandler.EndPoint;
                _indicatorArcSegment.Size = arcDataHandler.Size;
                _indicatorArcSegment.IsLargeArc = arcDataHandler.IsLargeArc;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Indicator animation work method finished. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Run Worker Completed Event Arguments. </param>
        private void IndicatorAnimationFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            _indicator.Visibility = Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup indicator animation. </summary>
        private void SetupIndicatorAnimation()
        {
            if (_animationWorker != null && _animationWorker.IsBusy)
            {
                StopIndicatorAnimation();
                while (_animationWorker != null && _animationWorker.IsBusy) { }
            }

            _indicatorRadius = (_indicator.ActualHeight / 2) - INDICATOR_MARGIN;
            _indicatorCenterPoint = new Point(_indicatorRadius + INDICATOR_SHIFT, _indicatorRadius + INDICATOR_SHIFT);

            _indicatorSize = new Size(_indicatorRadius, _indicatorRadius);

            _animationWorker = new BackgroundWorker();
            _animationWorker.WorkerReportsProgress = true;
            _animationWorker.WorkerSupportsCancellation = true;

            _animationWorker.DoWork += IndicatorAnimationDoWork;
            _animationWorker.ProgressChanged += IndicatorAnimationProgressChanged;
            _animationWorker.RunWorkerCompleted += IndicatorAnimationFinished;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Start indicator animation. </summary>
        private void StartIndicatorAnimation()
        {
            if (_animationWorker != null && !_animationWorker.IsBusy)
                _animationWorker.RunWorkerAsync();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop indicator animation. </summary>
        private void StopIndicatorAnimation()
        {
            if (_animationWorker != null && _animationWorker.IsBusy)
                _animationWorker.CancelAsync();
        }

        #endregion ANIMATION METHODS

        #region ANIMATION CALCULATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator outer arc end point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator outer arc end point. </returns>
        private Point CalculateOuterIndicatorEndPoint(double progress)
        {
            return MathHelper.FindPointOnCircle(
                _indicatorCenterPoint, _indicatorRadius, progress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate indicator outer arc start point. </summary>
        /// <param name="progress"> Current progress value. </param>
        /// <returns> Indicator outer arc start point. </returns>
        private Point CalculateOuterIndicatorStartPoint(double progress)
        {
            return MathHelper.FindPointOnCircle(
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

        #endregion ANIMATION CALCULATION METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after ok cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Ok);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CancelCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Cancel);
        }

        #endregion BUTTONS INTERACTION METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Finish await. </summary>
        /// <param name="message"> Optional message. </param>
        public void Finish(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message;

            IsFinished = true;

            StopIndicatorAnimation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update progress. </summary>
        /// <param name="message"> Optional message. </param>
        public void UpdateProgress(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message;
        }

        #endregion INTERACTION METHODS

        #region INTERNAL MESSAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetupIndicatorAnimation();
            StartIndicatorAnimation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopIndicatorAnimation();
        }

        #endregion INTERNAL MESSAGE METHODS

    }
}
