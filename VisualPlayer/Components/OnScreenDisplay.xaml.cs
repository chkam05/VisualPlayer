using chkam05.VisualPlayer.Utilities;
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

namespace chkam05.VisualPlayer.Components
{
    public partial class OnScreenDisplay : UserControl, IDisposable
    {

        //  VARIABLES

        private bool _enabled = true;
        private bool _initialized = false;
        private double _showTime = 5000;
        private bool _visibilityTimerCancelled = false;
        private BackgroundWorker _visibilityTimer;


        #region GETTERS & SETTERS

        public ImageSource InfoCover
        {
            get => CoverImage.Source;
            set => SetCoverImage(value);
        }

        public string InfoAlbum
        {
            get => InfoAlbumTextBlock.Text;
            set => InfoAlbumTextBlock.Text = value;
        }

        public string InfoArtist
        {
            get => InfoArtistTextBlock.Text;
            set => InfoArtistTextBlock.Text = value;
        }

        public string InfoTime
        {
            get => InfoTimeTextBlock.Text;
            set => InfoTimeTextBlock.Text = value;
        }

        public string InfoTitle
        {
            get => InfoTitleTextBlock.Text;
            set => InfoTitleTextBlock.Text = value;
        }

        public new bool IsEnabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                if (IsVisible)
                    Hide();
            }
        }

        public bool IsAnimating
        {
            get => _visibilityTimer != null && _visibilityTimer.IsBusy;
        }

        public bool IsInfinityVisible
        {
            get => _showTime <= 0;
        }

        public new bool IsVisible
        {
            get => Visibility == Visibility.Visible;
        }

        public double ShowTimeInMiliseconds
        {
            get => _showTime;
            set => _showTime = Math.Max(0.0, value);
        }

        public double ShowTimeInSeconds
        {
            get => _showTime / 1000;
            set => _showTime = Math.Max(0.0, value * 1000);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsPage class constructor. </summary>
        public OnScreenDisplay()
        {
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when component is removing. </summary>
        public void Dispose()
        {
            //  Stop visibility timer if is working.
            StopVisibilityTimer();
        }

        #endregion CLASS METHODS

        #region ANIMATION MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        private AnimationBuilder CreateOpacityChangeAnimation(double destOpacity)
        {
            //  Create animation builder.
            AnimationBuilder animationBuilder = new AnimationBuilder();

            //  Create animation frames.
            var containerAnimationFrames = new DoubleKeyFrameCollection()
            {
                AnimationBuilder.CreateEasingDoubleKeyFrame(0.0, Opacity),
                AnimationBuilder.CreateEasingDoubleKeyFrame(1.0, destOpacity)
            };

            //  Add playlist width animation to storyboard.
            animationBuilder.AddDoubleAnimationUsingKeyFrames(
                containerAnimationFrames,
                this,
                new PropertyPath("Opacity"));

            //  Return updated animation builder.
            return animationBuilder;
        }

        #endregion ANIMATION MANAGEMENT METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Hide on screen display. </summary>
        public void Hide()
        {
            if (IsAnimating)
                StopVisibilityTimer();

            if (IsVisible)
                ChangeVisibility(false);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show on screen display. </summary>
        public void Show()
        {
            if (_enabled)
            {
                if (_visibilityTimer != null && _visibilityTimer.IsBusy)
                    _visibilityTimer.CancelAsync();

                ChangeVisibility(true);

                //  Start background visibility timer to hide component after show time.
                if (!IsInfinityVisible)
                    StartVisibilityTimer();
            }
        }

        #endregion INTERACTION METHODS

        #region MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Change visibility of component. </summary>
        /// <param name="visibility"> True - visible; False - hidden. </param>
        private void ChangeVisibility(bool visibility)
        {
            if (IsVisible != visibility)
            {
                //  Configure parameters.
                double destOpacity = visibility ? 1.0 : 0.0;

                //  Create final method to do after animation.
                EventHandler onAnimationComplete = (sender, e) =>
                {
                    //  Set final parameters.
                    if (!visibility)
                        this.Visibility = Visibility.Collapsed;
                    this.Opacity = destOpacity;
                };

                if (visibility)
                {
                    this.Opacity = 0.0;
                    this.Visibility = Visibility.Visible;
                }

                if (_initialized)
                {
                    //  Create animation builder.
                    var animationBuilder = CreateOpacityChangeAnimation(destOpacity);
                    animationBuilder.OnAnimationComplete += onAnimationComplete;

                    //  Run animation.
                    animationBuilder.Run();
                }
                else
                {
                    //  Invoke final method if animation was not launched.
                    onAnimationComplete?.Invoke(this, null);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set cover image and manage alternate icon. </summary>
        /// <param name="coverImage"> Cover image source. </param>
        private void SetCoverImage(ImageSource coverImage)
        {
            CoverImage.Source = coverImage;
            CoverAlternateIcon.Visibility = CoverImage == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        #endregion MANAGEMENT METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  Set initialization flag.
            _initialized = true;
        }

        #endregion USER CONTROL METHODS

        #region VISIBILITY TIME MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup and start background visibility timer. </summary>
        private void StartVisibilityTimer()
        {
            if (_visibilityTimer == null || !_visibilityTimer.IsBusy)
            {
                _visibilityTimer = new BackgroundWorker();
                _visibilityTimer.DoWork += VisibilityTimerDoWork;
                _visibilityTimer.RunWorkerCompleted += VisibilityTimerCompleteWork;
                _visibilityTimer.WorkerSupportsCancellation = true;

                _visibilityTimerCancelled = false;

                //  Start background visibility timer.
                _visibilityTimer.RunWorkerAsync();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop background visibility timer. </summary>
        private void StopVisibilityTimer()
        {
            if (_visibilityTimer != null && _visibilityTimer.IsBusy)
            {
                _visibilityTimer.CancelAsync();
                _visibilityTimerCancelled = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method that hides an component when show time expired. </summary>
        private void VisibilityTimerCompleteWork(object sender, RunWorkerCompletedEventArgs e)
        {
            //  Exit method if timer work is cancelled.
            if (_visibilityTimer.CancellationPending || _visibilityTimerCancelled)
                return;

            //  Hide component after elapsed time.
            Hide();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method that waiting for show time to expire. </summary>
        private void VisibilityTimerDoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            TimeSpan diffrenceTime = DateTime.Now - startTime;

            while (diffrenceTime.TotalMilliseconds < _showTime)
            {
                //  Exit method if timer work is cancelled.
                if (_visibilityTimer.CancellationPending || _visibilityTimerCancelled)
                    return;

                diffrenceTime = DateTime.Now - startTime;
                Thread.Sleep(100);
            }
        }

        #endregion VISIBILITY TIME MANAGEMENT METHODS

    }
}
