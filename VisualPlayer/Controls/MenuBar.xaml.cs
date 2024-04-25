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
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.Data.MainMenu;
using VisualPlayer.Utilities;

namespace VisualPlayer.Controls
{
    public partial class MenuBar : UserControl
    {

        //  VARIABLES

        private MainMenuDataContext _dataContext;
        private bool _isAnimating = false;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MenuBar class constructor. </summary>
        public MenuBar()
        {
            //  Setup data context.
            _dataContext = MainMenuManager.Instance.DataContext;
            ConfigurationManager.Instance.UIController.BroadcastAnimationRequest += OnUIControllerBroadcastedAnimationRequest;

            //  Initialize user interface.
            InitializeComponent();

            //  Set data context.
            DataContext = _dataContext;
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in MainMenu ListView. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is CustomListView listView && listView?.SelectedItem is MainMenuItem selectedItem)
            {
                selectedItem.InvokeClick();
                listView.SelectedItem = null;
            }
        }

        #endregion INTERACTION METHODS

        #region RESIZE ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if expand animation can be started. </summary>
        /// <returns> True - expand animation can be started; False - otherwise. </returns>
        private bool CanExpand()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return !uiController.MainMenuExpanded
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if shrink animation can be started. </summary>
        /// <returns> True - shrink animation can be started; False - otherwise. </returns>
        private bool CanShrink()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return uiController.MainMenuExpanded
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create expand menu Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void Expand(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanExpand())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            double beginWidth = _outerGrid.ActualWidth;
            double finalWidth = ResourcesHelper.GetResource(this, "MenuBar.OuterGrid.MaxWidth", out double menuBarMaxWidth)
                ? menuBarMaxWidth : 160;

            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(200);

            var animation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    new LinearDoubleKeyFrame(beginWidth, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearDoubleKeyFrame(finalWidth, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _outerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));

            storyboard.Completed += (s, e) =>
            {
                _outerGrid.Width = finalWidth;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.MainMenuExpanded = true;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create shrink menu Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void Shrink(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanShrink())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            double beginWidth = _outerGrid.ActualWidth;
            double finalWidth = ResourcesHelper.GetResource(this, "MenuBar.OuterGrid.MinWidth", out double menuBarMaxWidth)
                ? menuBarMaxWidth : 48;

            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(0);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(200);

            var animation = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    new LinearDoubleKeyFrame(beginWidth, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearDoubleKeyFrame(finalWidth, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _outerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));

            storyboard.Completed += (s, e) =>
            {
                _outerGrid.Width = finalWidth;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.MainMenuExpanded = false;
            storyboard.Begin();
        }

        #endregion RESIZE ANIMATION METHODS

        #region UI CONTROLLER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after broadcasting animation request by ui controller. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Request Animation Event Arguments. </param>
        private void OnUIControllerBroadcastedAnimationRequest(object sender, RequestAnimationEventArgs e)
        {
            if (e.Target == AnimationTarget.MainMenuExtend && e.Value is bool value)
            {
                if (value)
                    Expand(e.AnimationId);
                else
                    Shrink(e.AnimationId);
            }
        }

        #endregion UI CONTROLLER INTERACTION METHODS

    }
}
