using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.Data.Player;
using VisualPlayer.Utilities;

namespace VisualPlayer.Controls
{
    public partial class ControlBar : UserControl, IControlBar
    {

        //  VARIABLES

        private ControlBarDataContext _dataContext;
        private bool _isAnimating = false;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlBar class constructor. </summary>
        public ControlBar()
        {
            //  Setup data.
            _dataContext = Player.Instance.ControlBarDataContext;
            ConfigurationManager.Instance.UIController.BroadcastAnimationRequest += OnUIControllerBroadcastedAnimationRequest;

            //  Initialize user interface.
            InitializeComponent();

            //  Set data context.
            DataContext = _dataContext;
        }

        #endregion CLASS METHODS

        #region COMPONENT REFERENCES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get outer grid. </summary>
        /// <returns> Outer grid. </returns>
        public Grid GetOuterGrid()
        {
            return _outerGrid;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get volume control button. </summary>
        /// <returns> Volume control button. </returns>
        public CustomButton GetVolumeControlButton()
        {
            return _volumeControlButton;
        }

        #endregion COMPONENT REFERENCES METHODS

        #region SLIDE ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide in animation can be started. </summary>
        /// <returns> True - slide in animation can be started; False - otherwise. </returns>
        private bool CanSlideIn()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return !uiController.ControlBarSlidOut
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide out animation can be started. </summary>
        /// <returns> True - slide out animation can be started; False - otherwise. </returns>
        private bool CanSlideOut()
        {
            var uiConfig = ConfigurationManager.Instance.Config.UserInterface;
            var uiController = ConfigurationManager.Instance.UIController;

            return uiConfig.ControlBarAutoHide
                && !uiController.ContentVisible
                && uiController.ControlBarSlidOut
                && !uiController.VolumeBarSlidOut
                && !uiController.ControlBarCursorInRange
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

            Thickness beginThickness = ObjectHelper.CloneThickness(_controlGrid.Margin);
            Thickness finalThickness = ResourcesHelper.GetResource(this, "ControlBar.ControlGrid.Margin", out Thickness margin)
                ? margin : new Thickness(8);
            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(250);

            var animation = new ThicknessAnimationUsingKeyFrames()
            {
                KeyFrames = new ThicknessKeyFrameCollection()
                {
                    new LinearThicknessKeyFrame(beginThickness, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearThicknessKeyFrame(finalThickness, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _controlGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));

            storyboard.Completed += (s, e) =>
            {
                _controlGrid.Margin = finalThickness;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.ControlBarSlidOut = true;
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

            ResourcesHelper.GetResource(this, "ControlBar.ButtonsGrid.Height", out double buttonsHeight);
            ResourcesHelper.GetResource(this, "ControlBar.ControlGrid.Margin", out Thickness margin);
            ResourcesHelper.GetResource(this, "ControlBar.TrackBarGrid.Height", out double trackBarHeight);

            double bottomMargin = -(buttonsHeight + margin.Bottom + trackBarHeight);

            Thickness beginThickness = ObjectHelper.CloneThickness(_controlGrid.Margin);
            Thickness finalThickness = ObjectHelper.ModifyThickness(margin, bottom: bottomMargin);
            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(250);

            var animation = new ThicknessAnimationUsingKeyFrames()
            {
                KeyFrames = new ThicknessKeyFrameCollection()
                {
                    new LinearThicknessKeyFrame(beginThickness, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearThicknessKeyFrame(finalThickness, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _controlGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));

            storyboard.Completed += (s, e) =>
            {
                _controlGrid.Margin = finalThickness;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.ControlBarSlidOut = false;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after curosr enters OuterGrid. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void OuterGridMouseEnter(object sender, MouseEventArgs e)
        {
            ConfigurationManager.Instance.UIController.ControlBarCursorInRange = true;
            ConfigurationManager.Instance.UIController.RequestControlBarSlideIn();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after cursor leave OuterGrid. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void OuterGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            uiController.ControlBarCursorInRange = false;

            if (!uiController.VolumeBarSlidOut)
                uiController.RequestControlBarSlideOut();
        }

        #endregion SLIDE ANIMATION METHODS

        #region UI CONTROLLER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after broadcasting animation request by ui controller. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Request Animation Event Arguments. </param>
        private void OnUIControllerBroadcastedAnimationRequest(object sender, RequestAnimationEventArgs e)
        {
            if (e.Target == AnimationTarget.ControlBarSlide && e.Value is bool value)
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
