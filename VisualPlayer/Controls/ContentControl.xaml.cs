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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.Data.MainMenu;
using VisualPlayer.Pages;
using VisualPlayer.Pages.Base;
using VisualPlayer.Pages.Settings;
using VisualPlayer.Utilities;

namespace VisualPlayer.Controls
{
    public partial class ContentControl : UserControl
    {

        //  VARIABLES

        private bool _isAnimating = false;
        private bool _isAnimatingExtend = false;


        //  GETTERS & SETTERS

        public IContentViewer ContentViewer
        {
            get => _contentViewer;
        }


        //  METHODS
                       
        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ContentControl class constructor. </summary>
        public ContentControl()
        {
            //  Setup data context.
            ConfigurationManager.Instance.UIController.BroadcastAnimationRequest += OnUIControllerBroadcastedAnimationRequest;

            //  Setup main menu.
            SetupMainMenu();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if extend animation can be started. </summary>
        /// <returns> True - extend animation can be started; False - otherwise. </returns>
        private bool CanAnimateExtend()
        {
            return !_isAnimatingExtend;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide in animation can be started. </summary>
        /// <returns> True - slide in animation can be started; False - otherwise. </returns>
        private bool CanAnimateSlideIn()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return !uiController.ContentControlSlidOut
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide out animation can be started. </summary>
        /// <returns> True - slide out animation can be started; False - otherwise. </returns>
        private bool CanAnimateSlideOut()
        {
            var uiConfig = ConfigurationManager.Instance.Config.UserInterface;
            var uiController = ConfigurationManager.Instance.UIController;

            return uiConfig.ContentControlAutoHide
                && uiController.ContentControlSlidOut
                && !uiController.ContentVisible
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if content can be hidden. </summary>
        /// <returns> True - content can be hidden; False - otherwise. </returns>
        private bool CanHideContent()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return uiController.ContentVisible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if content can be showed. </summary>
        /// <returns> True - content can be showed; False - otherwise. </returns>
        private bool CanShowContent()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return !uiController.ContentVisible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide content. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void ContentHide(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanHideContent())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            _contentViewer.Visibility = Visibility.Collapsed;
            _outerGridControlRow.Width = new GridLength(1, GridUnitType.Auto);
            _outerGridCursorDetectionRow.Width = new GridLength(
                ResourcesHelper.GetResource(this, "ContentControl.CursorDetectionRow.Width", out double cursorDetectionRowWidth)
                    ? cursorDetectionRowWidth : 40);

            Grid.SetColumnSpan(this, 1);

            uiController.ContentVisible = false;
            uiController.NotifyAnimationFinished(animationId);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show content. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void ContentShow(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanShowContent())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            uiController.ContentVisible = true;

            Grid.SetColumnSpan(this, 2);

            _outerGridControlRow.Width = new GridLength(1, GridUnitType.Star);
            _outerGridCursorDetectionRow.Width = new GridLength(0);
            _contentViewer.Visibility = Visibility.Visible;

            uiController.NotifyAnimationFinished(animationId);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create extend Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        /// <param name="targetMargin"> Target margin. </param>
        /// <param name="animationTime"> Animation time. </param>
        /// <param name="startMilisecondsShift"> Animation start time shift. </param>
        private void ExtendTo(string animationId, Thickness targetMargin, double animationTime = 200, double startMilisecondsShift = 0)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanAnimateExtend())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            Thickness beginThickness = ObjectHelper.CloneThickness(_outerGrid.Margin);
            TimeSpan beginTimeSpan = TimeSpan.FromMilliseconds(startMilisecondsShift);
            TimeSpan finalTimeSpan = TimeSpan.FromMilliseconds(animationTime);

            var animation = new ThicknessAnimationUsingKeyFrames()
            {
                KeyFrames = new ThicknessKeyFrameCollection()
                {
                    new LinearThicknessKeyFrame(beginThickness, KeyTime.FromTimeSpan(beginTimeSpan)),
                    new LinearThicknessKeyFrame(targetMargin, KeyTime.FromTimeSpan(finalTimeSpan)),
                }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, _outerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));

            storyboard.Completed += (s, e) =>
            {
                _outerGrid.Margin = targetMargin;
                _isAnimatingExtend = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimatingExtend = true;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create slide in Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void SlideIn(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanAnimateSlideIn())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            Thickness beginThickness = ObjectHelper.CloneThickness(_innerGrid.Margin);
            Thickness finalThickness = ResourcesHelper.GetResource(this, "ContentControl.Margin", out Thickness _innerGridMargin)
                ? _innerGridMargin : new Thickness(8);

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

            Storyboard.SetTarget(animation, _innerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));

            storyboard.Completed += (s, e) =>
            {
                _innerGrid.Margin = finalThickness;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.ContentControlSlidOut = true;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create slide in Storyboard and begin animation. </summary>
        /// <param name="animationId"> Animation identifier. </param>
        private void SlideOut(string animationId)
        {
            var uiController = ConfigurationManager.Instance.UIController;

            if (!CanAnimateSlideOut())
            {
                uiController.NotifyAnimationFinished(animationId);
                return;
            }

            var innerGridMargin = ResourcesHelper.GetResource(this, "ContentControl.InnerGrid.Margin", out Thickness _innerGridMargin)
                ? _innerGridMargin : new Thickness(0);
            var innerGridWidth = _innerGrid.ActualWidth;

            Thickness beginThickness = ObjectHelper.CloneThickness(_innerGrid.Margin);
            Thickness finalThickness = ObjectHelper.ModifyThickness(innerGridMargin,
                left: -(innerGridWidth + innerGridMargin.Left));

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

            Storyboard.SetTarget(animation, _innerGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));

            storyboard.Completed += (s, e) =>
            {
                _innerGrid.Margin = finalThickness;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.ContentControlSlidOut = false;
            storyboard.Begin();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after curosr enters OuterGrid. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void OuterGridMouseEnter(object sender, MouseEventArgs e)
        {
            ConfigurationManager.Instance.UIController.RequestContentControlSlideIn();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after cursor leave OuterGrid. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void OuterGridMouseLeave(object sender, MouseEventArgs e)
        {
            var uiConfig = ConfigurationManager.Instance.Config.UserInterface;
            var uiController = ConfigurationManager.Instance.UIController;

            if (uiController.NowPlayingExpanded && uiConfig.NowPlayingStayOnTop)
                return;

            ConfigurationManager.Instance.UIController.RequestContentControlSlideOut();
        }

        #endregion ANIMATION METHODS

        #region MAIN MENU METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting info menu item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Main Menu Item Click Event Arguments. </param>
        private void MainMenuFilesManagerItemClick(object sender, MainMenuItemClickEventArgs e)
        {
            ContentViewer.LoadPage(new FilesManagerPage(ContentViewer));
            ConfigurationManager.Instance.UIController.RequestContentShow();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting setting menu item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Main Menu Item Click Event Arguments. </param>
        private void MainMenuSettingsItemClick(object sender, MainMenuItemClickEventArgs e)
        {
            ContentViewer.LoadPage(new SettingsMainPage(ContentViewer));
            ConfigurationManager.Instance.UIController.RequestContentShow();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting info menu item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Main Menu Item Click Event Arguments. </param>
        private void MainMenuInfoItemClick(object sender, MainMenuItemClickEventArgs e)
        {
            //
        }

        #endregion MAIN MENU METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup main menu. </summary>
        private void SetupMainMenu()
        {
            var menuDataContext = MainMenuManager.Instance.DataContext;

            var menuItems = new MainMenuItem[]
            {
                new MainMenuItem("Files Manager", PackIconKind.FolderMultipleOutline, "Manage files", MainMenuFilesManagerItemClick),
                new MainMenuItem("Settings", PackIconKind.Gear, "Setup application", MainMenuSettingsItemClick),
                new MainMenuItem("About", PackIconKind.InfoCircleOutline, "Information about application", MainMenuInfoItemClick),
            };

            menuDataContext.AddItems(menuItems, MainMenuPosition.Top);
        }

        #endregion SETUP METHODS

        #region UI CONTROLLER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after broadcasting animation request by ui controller. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Request Animation Event Arguments. </param>
        private void OnUIControllerBroadcastedAnimationRequest(object sender, RequestAnimationEventArgs e)
        {
            if (e.Target == AnimationTarget.ContentControlExtend && e.Value is ContentControlExpandState state)
            {
                switch (state)
                {
                    case ContentControlExpandState.Default:
                        var marginDefault = ResourcesHelper.GetResource(this, "ContentControl.OuterGrid.Margin", out Thickness thicknessDefault)
                            ? thicknessDefault : new Thickness(0, 0, 0, -40);

                        ExtendTo(e.AnimationId, marginDefault, 250, 0);
                        break;

                    case ContentControlExpandState.ControlBar:
                        var marginControlBar = ResourcesHelper.GetResource(this, "ContentControl.OuterGrid.MarginWithControlBar", out Thickness thicknessControlBar)
                            ? thicknessControlBar : new Thickness(0, 0, 0, -48);

                        ExtendTo(e.AnimationId, marginControlBar, 175, 0);
                        break;
                }
            }
            else if (e.Target == AnimationTarget.ContentControlSlide && e.Value is bool contentControlSlide)
            {
                if (contentControlSlide)
                    SlideIn(e.AnimationId);
                else
                    SlideOut(e.AnimationId);
            }
            else if (e.Target == AnimationTarget.ContentVisibility && e.Value is bool contentVisible)
            {
                if (contentVisible)
                    ContentShow(e.AnimationId);
                else
                    ContentHide(e.AnimationId);
            }
        }

        #endregion UI CONTROLLER INTERACTION METHODS

    }
}
