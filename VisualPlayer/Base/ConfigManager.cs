using chkam05.VisualPlayer.Base.EventArgs;
using chkam05.VisualPlayer.Data.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chkam05.VisualPlayer.Base
{
    public class ConfigManager
    {

        //  EVENTS

        public event EventHandler<ConfigSaveEventArgs> OnConfigSave;
        public event EventHandler<ConfigUpdateEventArgs> OnConfigUpdate;

        
        //  CONST

        private const string _configFileName = "config.json";


        //  VARIABLES

        private static ConfigManager _instance;

        private string _configFilePath;
        private string _configPath;

        public AppConfig Config { get; private set; }


        #region GETTERS & SETTERS

        //  --------------------------------------------------------------------------------
        /// <summary> Get current configuration files path. </summary>
        public string ConfigurationPath
        {
            get
            {
                if (string.IsNullOrEmpty(_configPath))
                    _configPath = Path.Combine(
                        Environment.GetEnvironmentVariable("APPDATA"),
                        (Application.Current as App).ApplicationName);

                return _configPath;
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ConfigManager class constructor. </summary>
        private ConfigManager()
        {
            //  Prepare basic configuration of the application. 
            var setupComplete = Setup();

            //  If setup has been completed without loading basic configuration, 
            //  load configuation from file.
            if (setupComplete)
                Load();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get or create instance of ConfigMananger class. </summary>
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigManager();

                return _instance;
            }
        }

        #endregion CLASS METHODS

        #region CONFIGURATION FILE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration from static configuration file from AppData. </summary>
        public void Load()
        {
            var application = (App)Application.Current;

            try
            {
                using (var fileStream = File.Open(_configFilePath, FileMode.Open))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string rawConfiguration = streamReader.ReadToEnd();
                        Config = JsonConvert.DeserializeObject<AppConfig>(rawConfiguration);
                    }
                }

                //  If configuration is not loaded, throw to load default configuration.
                if (Config == null)
                    throw new Exception("For unknown reasons, the configuration was not loaded.");
            }
            catch (Exception exc)
            {
                //  Loading configuration is not possible.
                LoadDefaultConfiguration();

                exc = new Exception(
                    $"Detected problem while loading configuration.{Environment.NewLine}" +
                    $"Default configuration has been loaded.{Environment.NewLine}" +
                    $"{exc.Message}");

                var exceptionHandler = ExceptionsHandler.Instance;

                if (exceptionHandler != null)
                    exceptionHandler.ReportException(exc);
                else
                    throw exc;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration to static configuration file in AppData. </summary>
        public void Save()
        {
            var application = (App)Application.Current;

            try
            {
                //  Check and create configuration directory if does not exists.
                CheckConfigurationPath();

                using (var fileStream = File.Open(_configFilePath, FileMode.Create))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        var rawConfiguration = JsonConvert.SerializeObject(Config);
                        streamWriter.Write(rawConfiguration);
                    }
                }

                OnConfigSave?.Invoke(this, new ConfigSaveEventArgs());
            }
            catch (Exception exc)
            {
                exc = new Exception(
                    $"Detected problem while saving configuration.{Environment.NewLine}" +
                    $"{exc.Message}");

                var exceptionHandler = ExceptionsHandler.Instance;

                if (exceptionHandler != null)
                    exceptionHandler.ReportException(exc);
                else
                    throw exc;
            }
        }

        #endregion CONFIGURATION FILE MANAGEMENT METHODS

        #region CONFIGURATION PROPERTIES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get value from class property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Value of this property. </returns>
        public object Get<T>(string propertyName) where T : IAppConfig
        {
            var tType = typeof(T);

            if (tType == typeof(AppConfig))
                return Config.Get(propertyName);

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if class contains particular property name. </summary>
        /// <param name="propertyName"> Property name to check. </param>
        /// <returns> True - object contains property; False - otherwise. </returns>
        public bool Has<T>(string propertyName) where T : IAppConfig
        {
            var tType = typeof(T);

            if (tType == typeof(AppConfig))
                return Config.Has(propertyName);

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Manual invoke configuration update method. </summary>
        /// <param name="name"> Updating value name. </param>
        public void InvokeConfigUpdate<T>(string name) where T : IAppConfig
        {
            var tType = typeof(T);

            if (tType == typeof(AppConfig) && Config.Has(name))
            {
                object value = Get<T>(name);
                OnConfigUpdate?.Invoke(Config, new ConfigUpdateEventArgs(name, value));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set class property value. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set to selected property. </param>
        public bool Set<T>(string propertyName, object value) where T : IAppConfig
        {
            var tType = typeof(T);

            if (tType == typeof(AppConfig))
                return Config.Set(propertyName, value);

            return false;
        }

        #endregion CONFIGURATION PROPERTIES METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Prepare basic configuration of the application. </summary>
        private bool Setup()
        {
            var application = (chkam05.VisualPlayer.App) Application.Current;

            //  Setup configuration directory path.
            _configPath = Path.Combine(
                Environment.GetEnvironmentVariable("APPDATA"),
                application.ApplicationName);

            //  Setup main configuration file path.
            _configFilePath = Path.Combine(_configPath, _configFileName);

            //  Check and create configuration files and directory if does not exists.
            var pathExist = CheckConfigurationPath();
            var fileExist = CheckConfigurationFile();

            return pathExist && fileExist;
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create main configuration file if does not exists and load default configuration. </summary>
        /// <returns> True - configuration file exists; False - otherwise. </returns>
        private bool CheckConfigurationFile()
        {
            var fileExists = File.Exists(_configFilePath);

            if (!fileExists)
            {
                //  Load default configuration and save it.
                LoadDefaultConfiguration();
                Save();
            }

            return fileExists;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create configuration files directory if does not exists. </summary>
        /// <returns> True - configuration files directory exists; False - otherwise. </returns>
        private bool CheckConfigurationPath()
        {
            var directoryExists = Directory.Exists(ConfigurationPath);

            if (!directoryExists)
                Directory.CreateDirectory(ConfigurationPath);

            return directoryExists;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load default configuration and save it to file. </summary>
        private void LoadDefaultConfiguration()
        {
            Config = AppConfig.Default;
        }

        #endregion UTILITY METHODS

    }
}
