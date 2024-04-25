using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Controls
{
    public class FilesTreeViewer : Control
    {

        //  DELEGATES

        public delegate void ItemSelectedEventHandler(object sender, FilesTreeViewerItemSelectedEventArgs e);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(FilesTreeViewer),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(ObservableCollection<TreeViewFileItem>),
            typeof(FilesTreeViewer),
            new PropertyMetadata(new ObservableCollection<TreeViewFileItem>()));


        //  EVENTS

        public event ItemSelectedEventHandler ItemSelected;


        //  GETTERS & SETTERS

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public ObservableCollection<TreeViewFileItem> Items
        {
            get => (ObservableCollection<TreeViewFileItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesTreeViewer class constructor. </summary>
        public FilesTreeViewer()
        {
            if (Items == null || !Items.Any())
                Items = new ObservableCollection<TreeViewFileItem>(GetDefaultValues());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Static FilesTreeViewer class constructor. </summary>
        static FilesTreeViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilesTreeViewer),
                new FrameworkPropertyMetadata(typeof(FilesTreeViewer)));
        }

        #endregion CLASS METHODS

        #region DATA METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get default values. </summary>
        /// <returns> Collection of default tree view file items. </returns>
        private IEnumerable<TreeViewFileItem> GetDefaultValues()
        {
            return SystemHelper.GetDrives().Select(d =>
            {
                var treeViewFileItem = TreeViewFileItem.CreateTreeViewFileItem(d);

                LoadChildItems(treeViewFileItem);

                treeViewFileItem.Expanded += OnItemExpand;

                return treeViewFileItem;
            });
        }

        #endregion DATA METHODS

        #region COMPONENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in tree view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void TreeViewSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on tree view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TreeViewPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            if (treeView.SelectedItem is TreeViewFileItem selectedItem)
            {
                ItemSelected?.Invoke(this, new FilesTreeViewerItemSelectedEventArgs(selectedItem));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing mouse button on tree view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TreeViewPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            Point mousePosition = e.GetPosition(treeView);
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeView, mousePosition);

            if (hitTestResult != null && hitTestResult.VisualHit is FrameworkElement frameworkElement)
            {
                var targetTreeView = ObjectHelper.FindParentElementByTemplate<CustomTreeView>(frameworkElement);

                if (targetTreeView != null && targetTreeView == treeView)
                {
                    var selectedItem = targetTreeView.SelectedItem as TreeViewFileItem;

                    if (selectedItem == null)
                        return;

                    var treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(selectedItem) as CustomTreeViewItem;

                    if (treeViewItem != null)
                        treeViewItem.IsSelected = false;
                }
            }
        }

        #endregion COMPONENT METHODS

        #region ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Collapse recurecny all child tree view file items. </summary>
        /// <param name="item"> Tree view file item. </param>
        private void CollapseRecurency(TreeViewFileItem item)
        {
            var expandedChildItems = item.ChildItems.Where(i => i.IsExpanded);

            foreach (var childItem in expandedChildItems)
            {
                CollapseRecurency(childItem);
                childItem.IsExpanded = false;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after expand or collapse tree view file item. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> TreeViewFileItem Expand Event Arguments. </param>
        private void OnItemExpand(object sender, TreeViewFileItemExpandEventArgs e)
        {
            if (e.IsExpanded)
            {
                foreach (var childItem in e.TreeViewFileItem.ChildItems)
                    LoadChildItems(childItem);
            }
            else
            {
                CollapseRecurency(e.TreeViewFileItem);

                foreach (var childItem in e.TreeViewFileItem.ChildItems)
                    RemoveChildItems(childItem);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load child items. </summary>
        /// <param name="item"> Tree view file item. </param>
        private void LoadChildItems(TreeViewFileItem item)
        {
            var directories = SystemHelper.GetDirectories(item.Path);

            if (directories != null)
            {
                foreach (var dir in directories)
                {
                    if (!SystemHelper.IsDirectory(dir))
                        continue;

                    if (SystemHelper.IsFileHidden(dir))
                        continue;

                    if (SystemHelper.IsFileSystem(dir))
                        continue;

                    var childItem = TreeViewFileItem.CreateTreeViewFileItem(dir);

                    childItem.Expanded += OnItemExpand;

                    item.ChildItems.Add(childItem);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove child items. </summary>
        /// <param name="item"> Tree view file item. </param>
        private void RemoveChildItems(TreeViewFileItem item)
        {
            foreach (var childItem in item.ChildItems)
                childItem.Expanded -= OnItemExpand;

            item.ChildItems.Clear();
        }

        #endregion ITEMS INTERACTION METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            var treeView = GetTreeView("_treeView");
            SetTreeViewSelectedItemChanged(treeView, TreeViewSelectionChanged);
            SetTreeViewPreviewMouseDoubleClick(treeView, TreeViewPreviewMouseDoubleClick);
            SetPreviewMouseDown(treeView, TreeViewPreviewMouseDown);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get TreeView from ContentTemplate. </summary>
        /// <param name="treeViewName"> Tree view name. </param>
        /// <returns> TreeView or null. </returns>
        private TreeView GetTreeView(string treeViewName)
        {
            return this.Template.FindName(treeViewName, this) as TreeView;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set framework element preview mouse down event handler. </summary>
        /// <param name="frameworkElement"> Framework element. </param>
        /// <param name="handler"> Mouse button event handler. </param>
        private void SetPreviewMouseDown(FrameworkElement frameworkElement, MouseButtonEventHandler handler)
        {
            frameworkElement.PreviewMouseDown += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set tree view preview mouse double click event handler. </summary>
        /// <param name="treeView"> Tree view. </param>
        /// <param name="handler"> Mouse button event handler. </param>
        private void SetTreeViewPreviewMouseDoubleClick(TreeView treeView, MouseButtonEventHandler handler)
        {
            treeView.PreviewMouseDoubleClick += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set tree view selection item changed event handler. </summary>
        /// <param name="treeView"> Tree view. </param>
        /// <param name="handler"> Routed property changed event handler. </param>
        private void SetTreeViewSelectedItemChanged(TreeView treeView, RoutedPropertyChangedEventHandler<object> handler)
        {
            if (treeView != null && handler != null)
                treeView.SelectedItemChanged += handler;
        }

        #endregion TEMPLATE METHODS

    }
}
