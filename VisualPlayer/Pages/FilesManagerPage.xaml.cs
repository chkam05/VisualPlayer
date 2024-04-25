using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using VisualPlayer.Controls;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.Player;
using VisualPlayer.InternalMessages;
using VisualPlayer.Pages.Base;
using VisualPlayer.Pages.Debug;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels.Files;

namespace VisualPlayer.Pages
{
    public partial class FilesManagerPage : BasePage
    {

        //  VARIABLES

        private FilesManagerDataContext _dataContext;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesManagerPage class constructor. </summary>
        /// <param name="contentViewer"> Content viewer interface. </param>
        public FilesManagerPage(IContentViewer contentViewer) : base(contentViewer)
        {
            //  Setup data.
            _dataContext = new FilesManagerDataContext();
            DataContext = _dataContext;

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region ADDRESS BAR INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked address bar custom text box got focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddressBarCustomTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _dataContext.PathFocused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after address bar custom text box lost focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void AddressBarCustomTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            _dataContext.PathFocused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after preview key down in address bar text box. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void AddressBarCustomTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        #endregion ADDRESS BAR INTERACTION METHODS

        #region COMPONENTS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting item in files tree viewer. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> FilesTreeViewer Item Selected Event Arguments. </param>
        private void FilesTreeViewerItemSelected(object sender, FilesTreeViewerItemSelectedEventArgs e)
        {
            if (e.TreeViewFileItem != null)
                _dataContext.Path = e.TreeViewFileItem.Path;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting items in files viewer. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> FilesViewer Items Selected Event Arguments. </param>
        private void FilesViewerItemsSelected(object sender, FilesViewerItemsSelectedEventArgs e)
        {
            if (e.FilesItems?.Any() ?? false)
            {
                var directories = e.FilesItems.Where(f => f.IsDirectory);
                var files = e.FilesItems.Where(f => !f.IsDirectory);

                List<string> finalFiles = new List<string>();

                if (files.Any())
                {
                    if (directories.Any())
                        finalFiles.AddRange(directories.SelectMany(d => GetFilesFromSubDirectories(d.Path)));

                    finalFiles.AddRange(files.Select(f => f.Path));
                }
                else if (directories.Any())
                {
                    if (directories.Count() == 1)
                        _dataContext.Path = directories.First().Path;
                    else
                        finalFiles.AddRange(directories.SelectMany(d => GetFilesFromSubDirectories(d.Path)));
                }

                if (finalFiles.Any())
                {
                    var imControl = ((App)Application.Current).GetIMControlFromWindow();
                    var asyncFilesLoader = Player.Instance.PlayListDataContext.GetAsyncFilesLoader();
                    var filesCount = finalFiles.Count;
                    var imProgress = IMProgress.CreateMessage(imControl, "Loading files", "");

                    var errorMessages = new List<string>();
                    int loadedFiles = 0;

                    imProgress.ProgressMax = 100;

                    asyncFilesLoader.ProgressChanged += (o, e1) =>
                    {
                        var userState = (object[])e1.UserState;
                        var validationResult = (bool)userState[0];
                        var filePath = (string)userState[1];
                        var errorMessage = (string)userState[2];

                        if (validationResult)
                        {
                            imProgress.UpdateProgress(e1.ProgressPercentage, Path.GetFileName(filePath));
                            loadedFiles++;
                        }
                        else
                        {
                            imProgress.UpdateProgress(e1.ProgressPercentage, errorMessage);
                            errorMessages.Add(errorMessage);
                        }
                    };

                    asyncFilesLoader.RunWorkerCompleted += (o, e1) =>
                    {
                        var errorMessage = string.Join(Environment.NewLine, errorMessages);

                        imProgress.Finish(100, $"Loaded {loadedFiles}/{filesCount} files.{Environment.NewLine}{errorMessage}");
                    };

                    imControl.LoadMessage(imProgress);
                    asyncFilesLoader.RunWorkerAsync(finalFiles);
                }
            }
        }

        #endregion COMPONENTS INTERACTION METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get files paths from dicrectory and subdirectories. </summary>
        /// <param name="path"> Directory path. </param>
        /// <returns> Collection of files paths. </returns>
        private IEnumerable<string> GetFilesFromSubDirectories(string path)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(path))
            {
                var directories = SystemHelper.GetDirectories(path,
                    _dataContext.SearchPhrase,
                    _dataContext.ShowHiddenFiles,
                    _dataContext.ShowSystemFiles);

                var files = SystemHelper.GetFiles(path,
                    _dataContext.SearchPhrase,
                    _dataContext.FileExtension?.Extensions,
                    _dataContext.ShowHiddenFiles,
                    _dataContext.ShowSystemFiles);

                result.AddRange(directories.SelectMany(d => GetFilesFromSubDirectories(d.FullName)));
                result.AddRange(files.Select(f => f.FullName));
            }

            return result;
        }

        #endregion FILES MANAGEMENT METHODS

        #region HEADER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ContentViewer.GoBack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking options button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OptionsButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && sender is Button button)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        #endregion HEADER INTERACTION METHODS

        #region NAVIGATION BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationBackCustomButtonClick(object sender, RoutedEventArgs e)
        {
            _dataContext.NavigateBack();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation forward button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationForwardCustomButtonClick(object sender, RoutedEventArgs e)
        {
            _dataContext.NavigateForward();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation go button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NavigationGoCustomButtonClick(object sender, RoutedEventArgs e)
        {
            _dataContext.Path = _addressBarTextBox.Text;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking navigation search button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchCustomButtonClick(object sender, RoutedEventArgs e)
        {
            _dataContext.SearchPhrase = _searchBarTextBox.Text;
        }

        #endregion NAVIGATION BUTTONS INTERACTION METHODS

        #region SEARCH BAR INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked search bar custom text box got focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchBarCustomTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _dataContext.SearchFocused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked search bar custom text box lost focus. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void SearchBarCustomTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            _dataContext.SearchFocused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after preview key down in search bar text box. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Key Event Arguments. </param>
        private void SearchBarCustomTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                ((Control)sender).MoveFocus(request);
            }
        }

        #endregion SEARCH BAR INTERACTION METHODS

    }
}
