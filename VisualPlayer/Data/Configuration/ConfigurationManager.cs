using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Data.Enums;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class ConfigurationManager : BaseViewModel
    {

        //  CONST

        private static string CONFIG_FILE_NAME = "config.json";


        //  VARIABLES

        private static ConfigurationManager _instance;
        private static object _instanceLock = new object();

        private Config _config;
        private ThemeManager _themeManager;
        private UserInterfaceController _uiController;


        //  GETTERS & SETTERS

        public static ConfigurationManager Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new ConfigurationManager();

                    return _instance;
                }
            }
        }

        public Config Config
        {
            get => _config;
            set => UpdateProperty(ref _config, value);
        }

        public Appearance Appearance
        {
            get => _config.Appearance;
            set
            {
                _config.Appearance = value;
                NotifyPropertyChanged(nameof(Appearance));
            }
        }

        public UserInterfaceController UIController
        {
            get => _uiController;
            set => UpdateProperty(ref _uiController, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Private ConfigurationManager singleton class constructor. </summary>
        private ConfigurationManager()
        {
            LoadConfiguration();

            _themeManager = SetupThemeManager();
            _uiController = new UserInterfaceController();
        }

        #endregion CLASS METHODS

        #region LOAD & SAVE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration. </summary>
        public void LoadConfiguration()
        {
            Config = LoadConfigurationFromFile(out _) ?? LoadDefaultConfiguration();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load default configuration. </summary>
        /// <returns> Default configuration. </returns>
        public Config LoadDefaultConfiguration()
        {
            return new Config();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load configuration from file. </summary>
        /// <returns> Configuration from file or null. </returns>
        private Config LoadConfigurationFromFile(out Exception exception)
        {
            string configFilePath = GetApplicationConfigFilePath();

            try
            {
                string configFileContent = File.ReadAllText(configFilePath);
                Config config = JsonConvert.DeserializeObject<Config>(configFileContent);

                exception = null;
                return config;
            }
            catch (Exception exc)
            {
                exception = exc;
                return null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save configuration. </summary>
        public void SaveConfiguration()
        {
            var configFileContent = JsonConvert.SerializeObject(Config, Formatting.Indented);
            var configPath = GetApplicationConfigPath();

            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            var configFilePath = GetApplicationConfigFilePath();

            File.WriteAllText(configFilePath, configFileContent);
        }

        #endregion LOAD & SAVE METHODS

        #region PATHS

        //  --------------------------------------------------------------------------------
        /// <summary> Get application configuration path. </summary>
        /// <returns> Application configuration path. </returns>
        public static string GetApplicationConfigPath()
        {
            string appData = Environment.GetEnvironmentVariable("APPDATA");
            string appName = AppHelper.GetApplicationName();

            return Path.Combine(appData, appName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get application configuration file path. </summary>
        /// <returns> Application configuration file path. </returns>
        public static string GetApplicationConfigFilePath()
        {
            var appData = GetApplicationConfigPath();

            return Path.Combine(appData, CONFIG_FILE_NAME);
        }

        #endregion PATHS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup theme manager. </summary>
        /// <returns> Theme manager. </returns>
        private ThemeManager SetupThemeManager()
        {
            var themeManager = new ThemeManager();

            themeManager.SystemThemeChanged += (s, e) =>
            {
                if (Appearance.UseSystemColor)
                {
                    var systemAccentColor = e.Color;
                    var systemAccentColorCode = ColorsHelper.GetColorCode(systemAccentColor);

                    Appearance.AccentColor = new ThemeColor(systemAccentColor, systemAccentColorCode);
                }

                if (Appearance.Theme == Theme.System)
                {
                    Appearance.CurrentTheme = e.Theme;
                }
            };

            return themeManager;
        }

        #endregion SETUP METHOD

    }
}
