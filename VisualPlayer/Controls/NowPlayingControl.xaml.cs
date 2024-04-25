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
    public partial class NowPlayingControl : UserControl
    {

        //  VARIABLES

        private NowPlayingDataContext _dataContext;
        private bool _isAnimating = false;
        private FrameworkElement _draggingElement = null;
        private object _draggingItem = null;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NowPlayingControl class constructor. </summary>
        public NowPlayingControl()
        {
            //  Setup data context.
            _dataContext = Player.Instance.PlayListDataContext;
            ConfigurationManager.Instance.UIController.BroadcastAnimationRequest += OnUIControllerBroadcastedAnimationRequest;

            //  Initialize user interface.
            InitializeComponent();

            //  Set data context.
            DataContext = _dataContext;
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Instance.UIController.RequestNowPlayingSlideOut();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unclicking more button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void MoreButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender is Button button)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        #endregion INTERACTION METHODS

        #region NOW PLAYING ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing visibility of context menus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private void ContextMenuIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;
            ConfigurationManager.Instance.UIController.NowPlayingContextMenuItemOpen = isVisible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on now playing item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Args. </param>
        private void NowPlayingListViewItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var nowPlayingItem = (e.Source as FrameworkElement)?.DataContext as NowPlayingItem;
            _dataContext.SelectedItem = nowPlayingItem;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing mouse button when cursor is over now playing list view item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Args. </param>
        private void NowPlayingListViewItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is CustomListViewItem customListViewItem && e.ClickCount == 1)
            {
                if (customListViewItem.DataContext is NowPlayingItem item)
                {
                    _draggingElement = customListViewItem;
                    _draggingItem = item;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing mouse button when cursor is over now playing list view item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Args. </param>
        private void NowPlayingListViewItemPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is CustomListViewItem customListViewItem)
            {
                if (customListViewItem.DataContext is NowPlayingItem item)
                {
                    if (_draggingItem == item)
                    {
                        _draggingElement = null;
                        _draggingItem = null;
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after dropping item in now playing list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Drage Event Arguments. </param>
        private void NowPlayingListViewDrop(object sender, DragEventArgs e)
        {
            var customListView = sender as CustomListView;

            if (customListView == null)
                return;

            if (_draggingItem != null && e.Data.GetDataPresent(typeof(NowPlayingItem)))
            {
                var droppedItem = e.Data.GetData(typeof(NowPlayingItem)) as NowPlayingItem;
                int targetIndex = -1;

                if (_dataContext.Items.Count > 0)
                {
                    Point position = e.GetPosition(customListView);
                    double itemHeight = _draggingElement.ActualHeight
                        + _draggingElement.Margin.Top
                        + _draggingElement.Margin.Bottom;
                    double fullHeight = customListView.Items.Count * itemHeight;

                    if (fullHeight > 0)
                    {
                        targetIndex = Math.Max(0, (int)Math.Floor((customListView.Items.Count * position.Y) / fullHeight));
                    }
                }

                _dataContext.MoveItem(droppedItem, targetIndex);
            }

            _draggingItem = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after leaving now playing list view by cursor. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Event Args. </param>
        private void NowPlayingListMouseLeave(object sender, MouseEventArgs e)
        {
            _draggingElement = null;
            _draggingItem = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when cursor is moving over now playing list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Args. </param>
        private void NowPlayingListPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingElement != null && _draggingItem != null)
                DragDrop.DoDragDrop(_draggingElement, _draggingItem, DragDropEffects.Move);
        }

        #endregion NOW PLAYING ITEMS INTERACTION METHODS

        #region SLIDE ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide in animation can be started. </summary>
        /// <returns> True - slide in animation can be started; False - otherwise. </returns>
        private bool CanSlideIn()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return uiController.ContentControlSlidOut
                && !uiController.ContentVisible
                && !uiController.NowPlayingExpanded
                && !_isAnimating;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if slide out animation can be started. </summary>
        /// <returns> True - slide out animation can be started; False - otherwise. </returns>
        private bool CanSlideOut()
        {
            var uiController = ConfigurationManager.Instance.UIController;

            return uiController.NowPlayingExpanded
                && !uiController.NowPlayingContextMenuItemOpen
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

            ResourcesHelper.GetResource(this, "NowPlayingControl.OuterGrid.MinWidth", out double finalWidth);

            double beginWidth = _outerGrid.Width;

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
                _outerGrid.MinWidth = finalWidth;
                _isAnimating = false;
                uiController.NotifyAnimationFinished(animationId);
            };

            _isAnimating = true;
            uiController.NowPlayingExpanded = true;
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

            double beginWidth = _outerGrid.Width;
            double finalWidth = 0f;
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
            _outerGrid.MinWidth = 0;
            uiController.NowPlayingExpanded = false;
            storyboard.Begin();
        }

        #endregion SLIDE ANIMATION METHODS

        #region UI CONTROLLER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after broadcasting animation request by ui controller. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Request Animation Event Arguments. </param>
        private void OnUIControllerBroadcastedAnimationRequest(object sender, RequestAnimationEventArgs e)
        {
            if (e.Target == AnimationTarget.NowPlayingVisibility && e.Value is bool value)
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
