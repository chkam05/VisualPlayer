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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.Data.Player;
using VisualPlayer.Utilities;

namespace VisualPlayer.Controls
{
    public partial class VolumeControlBar : UserControl
    {

        //  VARIABLES

        private ControlBarDataContext _dataContext;
        private bool _isAnimating = false;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VolumeControlBar class constructor. </summary>
        public VolumeControlBar()
        {
            //  Setup data.
            _dataContext = Player.Instance.ControlBarDataContext;
            _dataContext.ControlCommandExecuted += OnDataContextControlCommandExecuted;
            ConfigurationManager.Instance.UIController.BroadcastAnimationRequest += OnUIControllerBroadcastedAnimationRequest;

            //  Initialize user interface.
            InitializeComponent();

            //  Initialize state before animations.
            ConfigurationManager.Instance.UIController.VolumeBarSlidOut = false;
            _innerGrid.Height = 0;

            //  Set data context.
            DataContext = _dataContext;
        }

        #endregion CLASS METHODS

        #region DATA CONTEXT INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after data context control command executed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Control Command Executed Event Arguments. </param>
        private void OnDataContextControlCommandExecuted(object sender, ControlCommandExecutedEventArgs e)
        {
            if (e.Command == ControlCommand.Volume && !_isAnimating)
            {
                var uiController = ConfigurationManager.Instance.UIController;

                if (uiController.VolumeBarSlidOut)
                {
                    uiController.RequestVolumeBarSlideOut();
                }
                else
                {
                    uiController.RequestVolumeBarSlideIn();
                }
            }
        }

        #endregion DATA CONTEXT INTERACTION METHODS

        #region SLIDE ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide in animation can be started. </summary>
        /// <returns> True - slide in animation can be started; False - otherwise. </returns>
        private bool CanSlideIn()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return !uiController.VolumeBarSlidOut
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide out animation can be started. </summary>
        /// <returns> True - slide out animation can be started; False - otherwise. </returns>
        private bool CanSlideOut()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return uiController.VolumeBarSlidOut
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create slide in Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void SlideIn(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanSlideIn())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            ResourcesHelper.GetResource(this, "VolumeControlBar.InnerGrid.MaxHeight", out double finalHeight);

            double beginHeight = _innerGrid.Height;

            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(200);

            var animation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    new LinearDoubleKeyFrame(beginHeight, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearDoubleKeyFrame(finalHeight, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _innerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.HeightProperty));

            storyboard.Completed += (s, e) =>
            {
                _innerGrid.Height = finalHeight;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.VolumeBarSlidOut = true;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create slide out Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void SlideOut(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanSlideOut())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            double beginHeight = _innerGrid.Height;
            double finalHeight = 0d;

            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(200);

            var animation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    new LinearDoubleKeyFrame(beginHeight, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearDoubleKeyFrame(finalHeight, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _innerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.HeightProperty));

            storyboard.Completed += (s, e) =>
            {
                _innerGrid.Height = finalHeight;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
                uiController.RequestControlBarSlideOut();
            };

            _isAnimating = true;
            uiController.VolumeBarSlidOut = false;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after cursor leave OuterGrid. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void OuterGridMouseLeave(object sender, MouseEventArgs e)
        {
            ConfigurationManager.Instance.UIController.RequestVolumeBarSlideOut();
        }

        #endregion SLIDE ANIMATION METHODS

        #region UI CONTROLLER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after broadcasting animation request by ui controller. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Request Animation Event Arguments. </param>
        private void OnUIControllerBroadcastedAnimationRequest(object sender, RequestAnimationEventArgs e)
        {
            if (e.Target == AnimationTarget.VolumeBarSlide && e.Value is bool value)
            {
                if (value)
                    SlideIn(e.AnimationId);
                else
                    SlideOut(e.AnimationId);
            }
        }

        #endregion UI CONTROLLER INTERACTION METHODS

    }
}
