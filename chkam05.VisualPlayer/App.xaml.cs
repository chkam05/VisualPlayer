using chkam05.VisualPlayer.Core;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Fonts;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Utilities.Data;
using chkam05.VisualPlayer.Windows;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer
{
    public partial class App : Application, ISingleInstanceApp
    {

        //  VARIABLES

        public FontsManager FontsManager { get; private set; }


        //  METHODS

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Main command line application method. </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            var applicationName = ApplicationHelper.Instance.GetApplicationName();

            if (SingleInstance<App>.InitializeAsFirstInstance(applicationName))
            {
                App app = new App();
                app.Run();

                // Allow single instance code to perform cleanup operations.
                SingleInstance<App>.Cleanup();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked during application startup. </summary>
        /// <param name="e"> Startup Event Arguments. </param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //  Check if application instance is running.
            if (ApplicationHelper.Instance.IsApplicationInstanceRunning())
            {
                Current.Shutdown(1);
                return;
            }

            //  Startup application.
            ConfigManager.Instance.LoadConfiguration();
            MainWindow = new MainWindow(e?.Args?.ToList());
            MainWindow.Show();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked druing application shutdown. </summary>
        /// <param name="e"> Exit Event Arguments. </param>
        protected override void OnExit(ExitEventArgs e)
        {
            ConfigManager.Instance.SaveConfiguration();
            ApplicationHelper.Instance.Dispose();
            Player.Instnace.Dispose();
        }

        #endregion APPLICATION METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Application class constructor. </summary>
        public App() : base()
        {
            FontsManager = FontsManager.Instance;
        }

        #endregion CLASS METHODS

        #region SINGLE INSTANCE APP INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Signal external command line arguments. </summary>
        /// <param name="args"> Interfaced list of arguments. </param>
        /// <returns> True - method work completed successfully; False - otherwise. </returns>
        public bool SignalExternalCommandLineArgs(List<string> args)
        {
            var window = MainWindow as MainWindow;

            if (window != null)
                return window.ProcessCommandLineArgs(args);
            else
                return false;
        }

        #endregion SINGLE INSTANCE APP INTERFACE METHODS

    }
}
