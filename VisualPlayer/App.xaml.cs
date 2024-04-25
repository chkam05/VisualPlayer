using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VisualPlayer.Data.Configuration;
using VisualPlayer.Data.Player;
using VisualPlayer.InternalMessages;
using VisualPlayer.Utilities;
using VisualPlayer.Windows;

namespace VisualPlayer
{
    public partial class App : Application, ISingleInstanceApp
    {

        //  VARIABLES

        public AppHelper AppHelper { get; private set; }
        public ConfigurationManager ConfigurationManager { get; private set; }
        public Player Player { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> App class constructor. </summary>
        public App() : base()
        {
            this.AppHelper = new AppHelper();
        }

        #endregion CLASS METHODS

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            var applicationName = AppHelper.GetApplicationName();

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
            if (AppHelper.IsApplicationInstanceRunning())
            {
                Current.Shutdown(1);
                return;
            }

            //  Startup application.
            ConfigurationManager.Instance.LoadConfiguration();

            this.ConfigurationManager = ConfigurationManager.Instance;
            this.Player = Player.Instance;

            var args = e?.Args?.Any() == true ? e.Args : Enumerable.Empty<string>();

            MainWindow = new MainWindow(args);
            MainWindow.Show();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked druing application shutdown. </summary>
        /// <param name="e"> Exit Event Arguments. </param>
        protected override void OnExit(ExitEventArgs e)
        {
            ConfigurationManager.Instance.SaveConfiguration();
            AppHelper.Dispose();
        }

        #endregion APPLICATION METHODS

        #region SINGLE INSTANCE APP INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Signal external command line arguments. </summary>
        /// <param name="args"> Interfaced list of arguments. </param>
        /// <returns> True - method work completed successfully; False - otherwise. </returns>
        public bool SignalExternalCommandLineArgs(IEnumerable<string> args)
        {
            var window = this.MainWindow as ISingleInstanceApp;

            if (window == null)
                return false;

            return window.SignalExternalCommandLineArgs(args);
        }

        #endregion SINGLE INSTANCE APP INTERFACE METHODS

        #region WINDOW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get internal message control from window. </summary>
        /// <returns> Internal message control window. </returns>
        public IIMControl GetIMControlFromWindow()
        {
            foreach (var window in Windows)
            {
                if (window is IIMWindow imWindow)
                    return imWindow.IMControl;
            }

            return null;
        }

        #endregion WINDOW METHODS

    }
}
