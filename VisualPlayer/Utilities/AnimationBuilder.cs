using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace chkam05.VisualPlayer.Utilities
{
    public class AnimationBuilder : IDisposable
    {

        //  EVENTS

        public event EventHandler OnAnimationComplete;


        //  VARIABLES

        public Storyboard Storyboard { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AnimationBuilder class constructor. </summary>
        public AnimationBuilder()
        {
            //  Create new Storyboard.
            Storyboard = new Storyboard();

            //  Assign on animation complete method.
            Storyboard.Completed += (sender, e) => OnAnimationComplete?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Free all variables and recycle AnimationBuilder instance. </summary>
        public void Dispose()
        {
            Storyboard = null;
        }

        #endregion CLASS METHODS

        #region ANIMATIONS BUILD METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and add animation based on double key frames. </summary>
        /// <param name="keyFrames"> Key frames collection. </param>
        /// <param name="target"> Animation target object. </param>
        /// <param name="targetProperty"> Animation target object property. </param>
        /// <param name="fillBehavior"> Property ending behaviour. </param>
        public void AddDoubleAnimationUsingKeyFrames(
            DoubleKeyFrameCollection keyFrames,
            DependencyObject target,
            PropertyPath targetProperty,
            FillBehavior fillBehavior = FillBehavior.Stop)
        {
            //  Create animation.
            var animation = new DoubleAnimationUsingKeyFrames();

            //  Setup animation.
            animation.FillBehavior = fillBehavior;
            animation.KeyFrames = keyFrames;

            //  Set animation target object and property
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, targetProperty);

            //  Add animation into StoryBoard.
            Storyboard.Children.Add(animation);
        }

        #endregion ANIMATIONS BUILD METHODS

        #region FRAMES BUILD METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create animation double key frame. </summary>
        /// <param name="frameTimeInSeconds"> Frame position on timeline in seconds. </param>
        /// <param name="value"> Frame double value to change. </param>
        /// <returns> Easing Double Key Frame. </returns>
        public static EasingDoubleKeyFrame CreateEasingDoubleKeyFrame(
            double frameTimeInSeconds,
            double value)
        {
            //  Create double key frame.
            return new EasingDoubleKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frameTimeInSeconds)),
                Value = value
            };
        }

        #endregion FRAMES BUILD METHODS

        #region STORYBOARD MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all animations from storyboard. </summary>
        public void ClearAnimations()
        {
            Storyboard.Children.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Run animations from storyboard. </summary>
        public void Run()
        {
            Storyboard.Begin();
        }

        #endregion STORYBOARD MANAGEMENT METHODS

    }
}
