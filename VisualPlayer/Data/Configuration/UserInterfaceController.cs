using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Enums;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class UserInterfaceController : BaseViewModel
    {

        //  DELEGATES

        public delegate void BoradcastAnimationRequestEventHandler(object sender, RequestAnimationEventArgs e);


        //  EVENTS

        public BoradcastAnimationRequestEventHandler BroadcastAnimationRequest;


        //  VARIABLES

        private List<AnimationRequest> _animationRequests;
        private bool _anyAnimationRunning = false;

        private bool _contentControlSlidOut = true;
        private bool _contentVisible = false;
        private bool _controlBarCursorInRange = false;
        private bool _controlBarSlidOut = true;
        private bool _mainMenuExpanded = false;
        private bool _nowPlayingExpanded = true;
        private bool _nowPlayingContextMenuItemOpen = false;
        private bool _volumeBarSlidOut = true;


        //  GETTERS & SETTERS

        public bool AnyAnimationIsRunning
        {
            get => _anyAnimationRunning;
            set => UpdateProperty(ref _anyAnimationRunning, value);
        }

        public bool ContentControlSlidOut
        {
            get => _contentControlSlidOut;
            set => UpdateProperty(ref _contentControlSlidOut, value);
        }

        public bool ContentVisible
        {
            get => _contentVisible;
            set => UpdateProperty(ref _contentVisible, value);
        }

        public bool ControlBarCursorInRange
        {
            get => _controlBarCursorInRange;
            set => UpdateProperty(ref _controlBarCursorInRange, value);
        }

        public bool ControlBarSlidOut
        {
            get => _controlBarSlidOut;
            set => UpdateProperty(ref _controlBarSlidOut, value);
        }

        public bool MainMenuExpanded
        {
            get => _mainMenuExpanded;
            set => UpdateProperty(ref _mainMenuExpanded, value);
        }

        public bool NowPlayingExpanded
        {
            get => _nowPlayingExpanded;
            set => UpdateProperty(ref _nowPlayingExpanded, value);
        }

        public bool NowPlayingContextMenuItemOpen
        {
            get => _nowPlayingContextMenuItemOpen;
            set => UpdateProperty(ref _nowPlayingContextMenuItemOpen, value);
        }

        public bool VolumeBarSlidOut
        {
            get => _volumeBarSlidOut;
            set => UpdateProperty(ref _volumeBarSlidOut, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UserInterfaceController class constructor. </summary>
        public UserInterfaceController()
        {
            _animationRequests = new List<AnimationRequest>();
        }

        #endregion CLASS METHODS

        #region ANIMATIONS CONTROLLER

        //  --------------------------------------------------------------------------------
        /// <summary> Request animation. </summary>
        /// <param name="target"> Animation target. </param>
        /// <param name="value"> Value. </param>
        /// <param name="outOfOrder"> Run animation immediately. </param>
        private void RequestAnimation(AnimationTarget target, object value, bool outOfOrder = false)
        {
            var animation = new AnimationRequest(target, value, outOfOrder);

            if (!AllowAnimation(new[] { animation }))
                return;

            if (animation.OutOfOrder)
            {
                InsertAnimation(animation, 0, true);
            }
            else
            {
                AddAnimation(animation);
            }

            if (!AnyAnimationIsRunning)
                RunNextAnimation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request animations. </summary>
        /// <param name="animations"> Animations. </param>
        private void RequestAnimations(IEnumerable<AnimationRequest> animations)
        {
            if (!AllowAnimation(animations))
                return;

            var animationsToRun = new List<AnimationRequest>();
            var ignoreOutOfOrder = false;
            var insertIndex = 0;

            foreach (var animation in animations)
            {
                if (animation.OutOfOrder && !ignoreOutOfOrder)
                {
                    if (InsertAnimation(animation, insertIndex, false))
                    {
                        animationsToRun.Add(animation);
                        insertIndex++;
                    }
                }
                else
                {
                    if (insertIndex > 0)
                    {
                        if (InsertAnimation(animation, insertIndex))
                            insertIndex++;
                    }
                    else
                    {
                        AddAnimation(animation);
                    }
                    
                    ignoreOutOfOrder = true;
                }
            }

            if (animationsToRun.Any())
                animationsToRun.ForEach(a => RunAnimation(a));

            if (!AnyAnimationIsRunning)
                RunNextAnimation();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add animation. </summary>
        /// <param name="animation"> Animation. </param>
        /// <param name="run"> Run animation immediately. </param>
        private void AddAnimation(AnimationRequest animation, bool run = false)
        {
            if (_animationRequests.Any(a => a.Target == animation.Target))
                return;

            _animationRequests.Add(animation);

            if (run)
                RunAnimation(animation);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Insert animation. </summary>
        /// <param name="animation"> Animation. </param>
        /// <param name="index"> Animation index. </param>
        /// <param name="run"> Run animation immediately. </param>
        private bool InsertAnimation(AnimationRequest animation, int index, bool run = false)
        {
            if (_animationRequests.Any(a => a.Target == animation.Target))
                return false;

            _animationRequests.Insert(index, animation);

            if (run)
                RunAnimation(animation);

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Run animation. </summary>
        /// <param name="animation"> Animation to run. </param>
        private void RunAnimation(AnimationRequest animation)
        {
            AnyAnimationIsRunning = true;
            animation.IsRunning = true;
            InvokeBroadcastAnimationRequest(animation);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Run next animation. </summary>
        /// <param name="animationIndex"> Next animation index. </param>
        private void RunNextAnimation(int? animationIndex = null)
        {
            if (!_animationRequests.Any())
            {
                AnyAnimationIsRunning = false;
                return;
            }

            var index = animationIndex.HasValue ? animationIndex.Value : 0;

            if (index < _animationRequests.Count)
            {
                var animation = _animationRequests[index];

                if (!animation.IsRunning)
                    RunAnimation(animation);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Invoked broadcast animation request. </summary>
        /// <param name="animation"> Animation. </param>
        private void InvokeBroadcastAnimationRequest(AnimationRequest animation)
        {
            var eventArgs = new RequestAnimationEventArgs(
                animation.Id,
                animation.Target,
                animation.Value);

            BroadcastAnimationRequest?.Invoke(this, eventArgs);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Notify animation finished. </summary>
        /// <param name="animationId"> Animation id. </param>
        public void NotifyAnimationFinished(string animationId)
        {
            var animation = _animationRequests.FirstOrDefault(a => a.Id == animationId);

            if (animation != null)
            {
                var animationIndex = _animationRequests.IndexOf(animation);
                _animationRequests.Remove(animation);
                RunNextAnimation(animationIndex);
            }

            RunNextAnimation();
        }

        #endregion ANIMATION CONTROLLER

        #region ANIMATION REQUESTS

        //  --------------------------------------------------------------------------------
        /// <summary> Request content control to slide in. </summary>
        public void RequestContentControlSlideIn()
        {
            if (ContentControlSlidOut)
                return;

            RequestAnimation(AnimationTarget.ContentControlSlide, true, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request content control to slide out. </summary>
        public void RequestContentControlSlideOut()
        {
            if (!ContentControlSlidOut || ContentVisible)
                return;

            if (NowPlayingExpanded)
            {
                if (NowPlayingContextMenuItemOpen)
                    return;

                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.NowPlayingVisibility, false, true),
                    new AnimationRequest(AnimationTarget.ContentControlSlide, false, false)
                });
            }
            else if (MainMenuExpanded)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.MainMenuExtend, false, true),
                    new AnimationRequest(AnimationTarget.ContentControlSlide, false, false)
                });
            }
            else
            {
                RequestAnimation(AnimationTarget.ContentControlSlide, false, true);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request content to hide. </summary>
        public void RequestContentHide()
        {
            if (ContentVisible)
                RequestAnimation(AnimationTarget.ContentVisibility, false, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request content to show. </summary>
        public void RequestContentShow()
        {
            if (ContentVisible)
                return;

            if (!ContentControlSlidOut)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.ContentControlSlide, true, true),
                    new AnimationRequest(AnimationTarget.ContentVisibility, true, false)
                });
            }
            else if (NowPlayingExpanded)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.NowPlayingVisibility, false, true),
                    new AnimationRequest(AnimationTarget.ContentVisibility, true, false),
                });
            }
            else
            {
                RequestAnimation(AnimationTarget.ContentVisibility, true, true);
            }

            if (!ControlBarSlidOut)
                RequestAnimation(AnimationTarget.ControlBarSlide, true, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request control bar to slide in. </summary>
        public void RequestControlBarSlideIn()
        {
            if (ControlBarSlidOut)
                return;

            RequestAnimations(new[]
            {
                new AnimationRequest(AnimationTarget.ControlBarSlide, true, true),
                new AnimationRequest(AnimationTarget.ContentControlExtend, ContentControlExpandState.ControlBar, true)
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request control bar to slide out. </summary>
        public void RequestControlBarSlideOut()
        {
            if (VolumeBarSlidOut || ControlBarCursorInRange || !ControlBarSlidOut || ContentVisible)
                return;

            RequestAnimations(new[]
            {
                new AnimationRequest(AnimationTarget.ControlBarSlide, false, true),
                new AnimationRequest(AnimationTarget.ContentControlExtend, ContentControlExpandState.Default, true)
            });
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request menu bar to expand. </summary>
        public void RequestMenuBarExpand()
        {
            if (MainMenuExpanded)
                return;

            if (!ContentControlSlidOut)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.ContentControlSlide, true, true),
                    new AnimationRequest(AnimationTarget.MainMenuExtend, true, true),
                });
            }
            else if (NowPlayingExpanded)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.NowPlayingVisibility, false, true),
                    new AnimationRequest(AnimationTarget.MainMenuExtend, true, true),
                });
            }
            else
            {
                RequestAnimation(AnimationTarget.MainMenuExtend, true, true);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request menu bar to shrink. </summary>
        public void RequestMenuBarShrink()
        {
            if (!MainMenuExpanded)
                return;

            RequestAnimation(AnimationTarget.MainMenuExtend, false, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request now playing to slide in. </summary>
        public void RequestNowPlayingSlideIn()
        {
            if (NowPlayingExpanded || ContentVisible)
                return;

            if (!ContentControlSlidOut)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.ContentControlSlide, true, true),
                    new AnimationRequest(AnimationTarget.NowPlayingVisibility, true, true),
                });
            }
            else if (MainMenuExpanded)
            {
                RequestAnimations(new[]
                {
                    new AnimationRequest(AnimationTarget.MainMenuExtend, false, true),
                    new AnimationRequest(AnimationTarget.NowPlayingVisibility, true, true),
                });
            }
            else
            {
                RequestAnimation(AnimationTarget.NowPlayingVisibility, true, true);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request now playing to slide out. </summary>
        public void RequestNowPlayingSlideOut()
        {
            if (!NowPlayingExpanded || NowPlayingContextMenuItemOpen)
                return;

            RequestAnimation(AnimationTarget.NowPlayingVisibility, false, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request volume bar to slide in. </summary>
        public void RequestVolumeBarSlideIn()
        {
            if (VolumeBarSlidOut || !ControlBarSlidOut)
                return;

            RequestAnimation(AnimationTarget.VolumeBarSlide, true, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Request volume bar to slide out. </summary>
        public void RequestVolumeBarSlideOut()
        {
            if (VolumeBarSlidOut)
                RequestAnimation(AnimationTarget.VolumeBarSlide, false, true);
        }

        #endregion ANIMATION REQUESTS

        #region SPECIAL RULES

        //  --------------------------------------------------------------------------------
        /// <summary> Allow animations by checking special rules. </summary>
        /// <param name="animations"> Animations. </param>
        /// <returns> True - animations allowed; False - otherwise. </returns>
        private bool AllowAnimation(IEnumerable<AnimationRequest> animations)
        {
            if (!IsAnyControlBarSlideOutAnimation(animations))
                return true;

            if (IsAnyVolumeBarSlideOutAnimationInQueue())
                return false;

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if any animation is control bar slide out animation. </summary>
        /// <param name="animations"> Animations. </param>
        /// <returns> True - There is control bar slide out animation; False - otherwise. </returns>
        private bool IsAnyControlBarSlideOutAnimation(IEnumerable<AnimationRequest> animations)
        {
            return animations.Any(a => a.Target == AnimationTarget.ControlBarSlide && (a.Value as bool?) == false);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if any volume bar slide out animation is waiting for run. </summary>
        /// <returns> True - Volume bar slide out animation is wiating to run; False - otherwise. </returns>
        private bool IsAnyVolumeBarSlideOutAnimationInQueue()
        {
            return _animationRequests.Any(a => a.Target == AnimationTarget.VolumeBarSlide && (a.Value as bool?) == false);
        }

        #endregion SPECIAL RULES

    }
}
