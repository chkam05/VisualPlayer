using chkam05.VisualPlayer.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace chkam05.VisualPlayer
{
    public partial class App : Application
    {

        //  VARIABLES

        private bool _foceClose = false;
        private Mutex _mutex;

        private string _configurationPath = string.Empty;

        public string ApplicationName { get; private set; }
        public ConfigManager ConfigManager { get; private set; }
        public ExceptionsHandler ExceptionsHandler { get; private set; }
        public PlayerCore PlayerCore { get; private set; }
        public SystemListener SystemListener { get; private set; }


        #region GETTERS & SETTERS

        public string ConfigurationPath
        {
            get => GetOrCreateConfigurationPath();
        }

        public string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Application class constructor. </summary>
        public App() : base()
        {
            //  Get application name.
            ApplicationName = GetApplicationName();

            //  Initialize extensions handler.
            ExceptionsHandler = ExceptionsHandler.Create(ApplicationName);

            //  Initialize files & paths.
            _configurationPath = Path.Combine(
                Environment.GetEnvironmentVariable("APPDATA"),
                ApplicationName);
        }

        #endregion CLASS METHODS

        #region APPLICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called when application is starting up. </summary>
        /// <param name="e"> Application startup event arguments. </param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //  Implementation of additional exception handling.
            SetupExceptionsHandling();

            //  Setup objects.
            ConfigManager = ConfigManager.Instance;
            PlayerCore = PlayerCore.Instance;
            SystemListener = SystemListener.Instance;
            
            //  Check if application is already running and shut it down if yes.
            if (IsApplicationAlreadyRunning())
            {
                _foceClose = true;
                Application.Current.Shutdown(1);
                return;
            }

            //  Startup application.
            base.OnStartup(e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when application is shutting down. </summary>
        /// <param name="e"> Application exit event arguments. </param>
        protected override void OnExit(ExitEventArgs e)
        {
            //  Relase mutex if it is in use.
            if (_mutex != null)
            {
                _mutex.Dispose();
                _mutex = null;
            }

            //  Save configuration if application is closing normally.
            if (!_foceClose)
                ConfigManager.Save();

            //  Close application.
            base.OnExit(e);
        }

        #endregion APPLICATION METHODS

        #region EXCEPTIONS HANDLING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Handle exception in classic Win32 way. </summary>
        /// <param name="exc"> Exception to handle. </param>
        private void ClassicExceptionHandle(Exception exc)
        {
            //  Create message box to show and respond to the exception.
            var messageBoxResult = MessageBox.Show(
                $"{exc.Message}{Environment.NewLine}Do you want to continue?",
                ApplicationName,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Error);

            //  React on exception.
            switch (messageBoxResult)
            {
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                case MessageBoxResult.No:

                    //  Force close application.
                    _foceClose = true;
                    Application.Current.Shutdown(1);
                    break;

                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:

                    //  Continue running the application.
                    break;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Handle exception. </summary>
        /// <param name="exception"> Exception to handle. </param>
        private void HandleException(Exception exception)
        {
            if (ExceptionsHandler != null)
                this.ExceptionsHandler.ReportException(exception);

            else
                ClassicExceptionHandle(exception);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Implementation of additional exception handling. </summary>
        private void SetupExceptionsHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleException((Exception) e.ExceptionObject);
            DispatcherUnhandledException += (s, e) =>
            {
                HandleException(e.Exception);
                e.Handled = true;
            };
        }

        #endregion EXCEPTIONS HANDLING METHODS

        #region FILES & PATHS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get configuration path, or create and get if not exists. </summary>
        /// <returns> Configuration path. </returns>
        private string GetOrCreateConfigurationPath()
        {
            if (!Directory.Exists(_configurationPath))
                Directory.CreateDirectory(_configurationPath);

            return _configurationPath;
        }

        #endregion FILES & PATHS MANAGEMENT METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get application assembly name. </summary>
        /// <returns> Application assembly name. </returns>
        private string GetApplicationName()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName.Name;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if application is already running to prevent launching it's copy. </summary>
        /// <returns> True - application is running; False - otherwise. </returns>
        private bool IsApplicationAlreadyRunning()
        {
            _mutex = new Mutex(true, ApplicationName, out bool isNewInstance);

            if (!isNewInstance)
            {
                _mutex.Dispose();
                _mutex = null;
            }

            return !isNewInstance;
        }

        #endregion UTILITY METHODS

    }
}
