using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels.Files;
using static VisualPlayer.Controls.FilesTreeViewer;

namespace VisualPlayer.Controls
{
    public class FilesViewer : Control
    {

        //  DELEGATES

        public delegate void ItemsSelectedEventHandler(object sender, FilesViewerItemsSelectedEventArgs e);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(FilesViewer),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty FileExtensionProperty = DependencyProperty.Register(
            nameof(FileExtension),
            typeof(FileExtension),
            typeof(FilesViewer),
            new PropertyMetadata(
                null,
                FileExtensionPropertyChangedCallback));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(ObservableCollection<FileItem>),
            typeof(FilesViewer),
            new PropertyMetadata(
                new ObservableCollection<FileItem>(),
                ItemsPropertyChangedCallback));

        public static readonly DependencyProperty ItemsSizeProperty = DependencyProperty.Register(
            nameof(ItemsSize),
            typeof(int),
            typeof(FilesViewer),
            new PropertyMetadata(
                0,
                ItemsSizePropertyChangedCallback));

        public static readonly DependencyProperty MultipleSelectionProperty = DependencyProperty.Register(
            nameof(MultipleSelection),
            typeof(bool),
            typeof(FilesViewer),
            new PropertyMetadata(false));

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            nameof(Path),
            typeof(string),
            typeof(FilesViewer),
            new PropertyMetadata(
                null,
                PathPropertyChangedCallback));

        public static readonly DependencyProperty SearchPhraseProperty = DependencyProperty.Register(
            nameof(SearchPhrase),
            typeof(string),
            typeof(FilesViewer),
            new PropertyMetadata(
                null,
                SearchPhrasePropertyChangedCallback));

        public static readonly DependencyProperty ShowHiddenFilesProperty = DependencyProperty.Register(
            nameof(ShowHiddenFiles),
            typeof(bool),
            typeof(FilesViewer),
            new PropertyMetadata(
                false,
                ShowHiddenFilesPropertyChangedCallback));

        public static readonly DependencyProperty ShowSystemFilesProperty = DependencyProperty.Register(
            nameof(ShowSystemFiles),
            typeof(bool),
            typeof(FilesViewer),
            new PropertyMetadata(
                false,
                ShowSystemFilesPropertyChangedCallback));


        //  EVENTS

        public event ItemsSelectedEventHandler ItemsSelected;


        //  VARIABLES

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public FileExtension FileExtension
        {
            get => (FileExtension)GetValue(FileExtensionProperty);
            set => SetValue(FileExtensionProperty, value);
        }

        public ObservableCollection<FileItem> Items
        {
            get => (ObservableCollection<FileItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public int ItemsSize
        {
            get => (int)GetValue(ItemsSizeProperty);
            set => SetValue(ItemsSizeProperty, value);
        }

        public bool MultipleSelection
        {
            get => (bool)GetValue(MultipleSelectionProperty);
            set => SetValue(MultipleSelectionProperty, value);
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public string SearchPhrase
        {
            get => (string)GetValue(SearchPhraseProperty);
            set => SetValue(SearchPhraseProperty, value);
        }

        public bool ShowHiddenFiles
        {
            get => (bool)(GetValue(ShowHiddenFilesProperty));
            set => SetValue(ShowHiddenFilesProperty, value);
        }

        public bool ShowSystemFiles
        {
            get => (bool)GetValue(ShowSystemFilesProperty);
            set => SetValue(ShowSystemFilesProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesViewer class constructor. </summary>
        public FilesViewer()
        {
            if (Items == null || !Items.Any())
                Items = new ObservableCollection<FileItem>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Static FilesViewer class constructor. </summary>
        static FilesViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilesViewer),
                new FrameworkPropertyMetadata(typeof(FilesViewer)));
        }

        #endregion CLASS METHODS

        #region COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing key on list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void ListViewPreviewKeyUp(object sender, KeyEventArgs e)
        {
            ListView listView = sender as ListView;

            if (e.Key == Key.Enter)
            {
                if (listView.SelectedItems != null && listView.SelectedItems.Count > 0)
                {
                    var selectedItems = listView.SelectedItems.Cast<FileItem>().ToList();
                    ItemsSelected?.Invoke(this, new FilesViewerItemsSelectedEventArgs(selectedItems));
                }
                else
                {
                    ItemsSelected?.Invoke(this, new FilesViewerItemsSelectedEventArgs(Enumerable.Empty<FileItem>()));
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after double clicking on list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ListViewPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            
            if (listView.SelectedItems != null && listView.SelectedItems.Count > 0)
            {
                var selectedItems = listView.SelectedItems.Cast<FileItem>().ToList();
                ItemsSelected?.Invoke(this, new FilesViewerItemsSelectedEventArgs(selectedItems));
            }
            else
            {
                ItemsSelected?.Invoke(this, new FilesViewerItemsSelectedEventArgs(Enumerable.Empty<FileItem>()));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing mouse button on list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ListViewPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            Point mousePosition = e.GetPosition(listView);
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(listView, mousePosition);

            if (hitTestResult != null && hitTestResult.VisualHit is FrameworkElement frameworkElement)
            {
                var targetListView = ObjectHelper.FindParentElementByTemplate<CustomListView>(frameworkElement);

                if (targetListView != null && targetListView == listView)
                {
                    listView.SelectedItem = null;
                    listView.SelectedItems.Clear();
                }
            }
        }

        #endregion COMPONENTS INTERACTION METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load/Refresh files list. </summary>
        /// <param name="path"> Catalog path. </param>
        /// <param name="searchPhrase"> Files search phrase. </param>
        /// <param name="fileExtension"> File extension. </param>
        /// <param name="showHiddenFiles"> Show hidden files. </param>
        /// <param name="showSystemFiles"> Show system files. </param>
        private void LoadFilesList(string path = null, string searchPhrase = null, FileExtension fileExtension = null,
            bool? showHiddenFiles = null, bool? showSystemFiles = null)
        {
            if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(Path))
            {
                var drives = SystemHelper.GetDrives();

                Items = new ObservableCollection<FileItem>(
                    drives.Select(dr => FileItem.CreateFileItem(dr)));
            }
            else
            {
                var filesAndDirectories = SystemHelper.GetFilesAndDirectories(
                    !string.IsNullOrEmpty(path) ? path : Path,
                    !string.IsNullOrEmpty(searchPhrase) ? searchPhrase : SearchPhrase,
                    (fileExtension != null ? fileExtension : FileExtension)?.Extensions,
                    showHiddenFiles.HasValue ? showHiddenFiles.Value : ShowHiddenFiles,
                    showSystemFiles.HasValue ? showSystemFiles.Value : ShowSystemFiles);

                Items = new ObservableCollection<FileItem>(
                    filesAndDirectories.Select(fd => FileItem.CreateFileItem(fd)));
            }
        }

        #endregion FILES MANAGEMENT METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing file extension property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void FileExtensionPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var fileExtension = e.NewValue as FileExtension ?? FileExtension.GetDefaultFileExtension();
                instance.LoadFilesList(fileExtension: fileExtension);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing items collection property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ItemsPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var oldItemsCollection = e.OldValue as ObservableCollection<FileItem>;

                if (oldItemsCollection != null)
                    oldItemsCollection.CollectionChanged -= instance.ItemsCollectionChanged;

                var newItemsCollection = e.NewValue as ObservableCollection<FileItem>;

                if (newItemsCollection != null)
                    newItemsCollection.CollectionChanged += instance.ItemsCollectionChanged;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after items collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing items size property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ItemsSizePropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                //
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing path property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void PathPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var path = e.NewValue as string;
                instance.LoadFilesList(path);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing search phrase property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void SearchPhrasePropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var searchPhrase = e.NewValue as string ?? string.Empty;
                instance.LoadFilesList(searchPhrase: searchPhrase);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing show hidden files property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ShowHiddenFilesPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var showHiddenFiles = e.NewValue as bool?;
                instance.LoadFilesList(showHiddenFiles: showHiddenFiles ?? false);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing show system files property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ShowSystemFilesPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is FilesViewer instance)
            {
                var showSystemFiles = e.NewValue as bool?;
                instance.LoadFilesList(showSystemFiles: showSystemFiles ?? false);
            }
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            var listView = GetListView("_listView");
            SetListViewSelectedItemChanged(listView, ListViewSelectionChanged);
            SetListViewPreviewMouseDoubleClick(listView, ListViewPreviewMouseDoubleClick);
            SetPreviewMouseDown(listView, ListViewPreviewMouseDown);
            SetPreviewKeyUp(listView, ListViewPreviewKeyUp);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get ListView from ContentTemplate. </summary>
        /// <param name="listViewName"> List view name. </param>
        /// <returns> TreeView or null. </returns>
        private ListView GetListView(string listViewName)
        {
            return this.Template.FindName(listViewName, this) as ListView;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set framework element preview key up event handler. </summary>
        /// <param name="frameworkElement"> Framework element. </param>
        /// <param name="handler"> Key event handler. </param>
        private void SetPreviewKeyUp(FrameworkElement frameworkElement, KeyEventHandler handler)
        {
            frameworkElement.PreviewKeyUp += handler;
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
        /// <summary> Set list view preview mouse double click event handler. </summary>
        /// <param name="listView"> List view. </param>
        /// <param name="handler"> Mouse button event handler. </param>
        private void SetListViewPreviewMouseDoubleClick(ListView listView, MouseButtonEventHandler handler)
        {
            listView.PreviewMouseDoubleClick += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set list view selection item changed event handler. </summary>
        /// <param name="listView"> List view. </param>
        /// <param name="handler"> Selection changed event handler. </param>
        private void SetListViewSelectedItemChanged(ListView listView, SelectionChangedEventHandler handler)
        {
            if (listView != null && handler != null)
                listView.SelectionChanged += handler;
        }

        #endregion TEMPLATE METHODS

    }
}
