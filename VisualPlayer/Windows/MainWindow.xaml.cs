using MaterialDesignThemes.Wpf;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
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
using VisualPlayer.Controls;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Enums;
using VisualPlayer.Data.MainMenu;
using VisualPlayer.Data.Player;
using VisualPlayer.InternalMessages;
using VisualPlayer.Utilities;

namespace VisualPlayer.Windows
{
    public partial class MainWindow : BaseWindow, ISingleInstanceApp, IIMWindow
    {

        //  GETTERS & SETTERS

        public IIMControl IMControl
        {
            get => _imControl;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainWindow class constructors. </summary>
        /// <param name="args"> List of application startup arguments. </param>
        public MainWindow(IEnumerable<string> args)
        {
            //  Initialize user interface.
            InitializeComponent();

            //  Execute commands and handle options.
            HandleArguments(args);
        }

        #endregion CLASS METHODS

        #region ARGUMENTS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Handle application arguments and commands. </summary>
        /// <param name="args"> Arguments. </param>
        /// <returns> True - arguments handled; False - otherwise. </returns>
        private bool HandleArguments(IEnumerable<string> args)
        {
            if (args?.Any() ?? false)
            {
                var files = GetFilesFromArgs(args);

                if (files.Any())
                {
                    LoadFiles(files);
                    return true;
                }
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get files paths from application arguments. </summary>
        /// <param name="args"> Arguments. </param>
        /// <returns> Collection of files paths. </returns>
        private IEnumerable<string> GetFilesFromArgs(IEnumerable<string> args)
        {
            return args.Where(a => !string.IsNullOrEmpty(a) && SystemHelper.IsFile(a));
        }

        #endregion ARGUMENTS MANAGEMENT METHODS

        #region FILES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load files. </summary>
        /// <param name="filesPaths"> Collection of files paths. </param>
        private void LoadFiles(IEnumerable<string> filesPaths)
        {
            var asyncFilesLoader = Player.Instance.PlayListDataContext.GetAsyncFilesLoader();
            var filesCount = filesPaths.Count();
            var imProgress = IMProgress.CreateMessage(IMControl, "Loading files", "");
            
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

            IMControl.LoadMessage(imProgress);
            asyncFilesLoader.RunWorkerAsync(filesPaths);
        }

        #endregion FILES MANAGEMENT METHODS

        #region SINGLE INSTANCE APP COMMAND LINE ARGS HANDLE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Handle external command line arguments. </summary>
        /// <param name="args"> Arguments. </param>
        /// <returns> True - arguments handled; False - otherwise. </returns>
        public bool SignalExternalCommandLineArgs(IEnumerable<string> args) => HandleArguments(args);

        #endregion SINGLE INSTANCE APP COMMAND LINE ARGS HANDLE METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after dropping item into window. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Drag Event Arguments. </param>
        private void WindowDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filesPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filesPaths == null || !filesPaths.Any())
                    return;

                LoadFiles(filesPaths);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after requesting window close. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Cancel Event Arguments. </param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var windowConfig = ConfigurationManager.Instance.Config.Window;

            windowConfig.UpdatePosition(this.Left, this.Top);
            windowConfig.UpdateSize(this.Width, this.Height);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading window. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var windowConfig = ConfigurationManager.Instance.Config.Window;

            this.Left = windowConfig.PositionX;
            this.Top = windowConfig.PositionY;
            this.Width = windowConfig.SizeWidth;
            this.Height = windowConfig.SizeHeight;

            WindowHelper.AdjustWindowInScreen(this);
        }

        #endregion WINDOW METHODS

    }
}
