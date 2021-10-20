using chkam05.Visualisations;
using chkam05.VisualPlayer.Base.EventArgs;
using chkam05.VisualPlayer.Data.States;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Configuration
{
    public class AppConfig : IAppConfig
    {

        //  EVENTS

        public event EventHandler<ConfigUpdateEventArgs> OnConfigUpdate;


        //  VARIABLES

        public bool PlayerAutoPlayOnAdd { get; set; }
        public bool PlayerKeepPlayListOnRestart { get; set; }
        public bool PlayerOSDEnabled { get; set; }
        public double PlayerOSDTime { get; set; }
        public int PlayerRepeat { get; set; }
        public bool PlayerShuffle { get; set; }

        public double PlayListWidth { get; set; }
        public SideBarState SideBarState { get; set; }

        public bool UseSystemColor { get; set; }
        public Color ThemeColor { get; set; }
        public List<Color> UsedThemeColors { get; set; }

        public VisualisationColorMode VisualisationColorMode { get; set; }
        public Color VisualisationColor { get; set; }
        public List<Color> UsedVisualisationColors { get; set; }
        public bool VisualisationEnabled { get; set; }
        public bool VisualisationLogoEnabled { get; set; }
        public VisualisationType VisualisationType { get; set; }

        public Point WinPosition { get; set; }
        public Size WinSize { get; set; }


        #region GETTERS & SETTERS

        public object this[string propertyName]
        {
            get => Get(propertyName);
            set => Set(propertyName, value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AppConfig class constructor. </summary>
        public AppConfig()
        {
            UsedThemeColors = new List<Color>();
            UsedVisualisationColors = new List<Color>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create AppConfig (configuration) class instance with default values. </summary>
        /// <returns> AppConfig class instance with default values. </returns>
        public static AppConfig Default
        {
            get
            {
                var defaultThemeColor = Color.FromRgb(0, 120, 208);
                var windowSize = new Size(800, 450);

                var appConfig = new AppConfig()
                {
                    PlayerAutoPlayOnAdd = false,
                    PlayerKeepPlayListOnRestart = false,
                    PlayerOSDEnabled = true,
                    PlayerOSDTime = 5000,
                    PlayerRepeat = 0,
                    PlayerShuffle = false,
                    PlayListWidth = 320,
                    SideBarState = SideBarState.COLLAPSED,
                    UseSystemColor = false,
                    ThemeColor = defaultThemeColor,
                    UsedThemeColors = new List<Color>() { defaultThemeColor },
                    VisualisationColorMode = VisualisationColorMode.APPLICATION,
                    VisualisationColor = defaultThemeColor,
                    UsedVisualisationColors = new List<Color>() { defaultThemeColor },
                    VisualisationEnabled = true,
                    VisualisationLogoEnabled = false,
                    VisualisationType = VisualisationType.StripesVisualisation,
                    WinPosition = WindowUtils.GetWindowCenterPosition(windowSize),
                    WinSize = windowSize,
                };

                return appConfig;
            }
        }

        #endregion CLASS METHODS

        #region PROPERTIES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get value from class property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Value of this property. </returns>
        public object Get(string propertyName)
        {
            return Has(propertyName)
                ? this.GetType().GetProperty(propertyName).GetValue(this)
                : null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if class contains particular property name. </summary>
        /// <param name="propertyName"> Property name to check. </param>
        /// <returns> True - object contains property; False - otherwise. </returns>
        public bool Has(string propertyName)
        {
            return this.GetType().GetProperty(propertyName) != null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set class property value. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set to selected property. </param>
        public bool Set(string propertyName, object value)
        {
            if (Has(propertyName))
            {
                try
                {
                    this.GetType().GetProperty(propertyName).SetValue(this, value);
                    OnConfigUpdate?.Invoke(this, new ConfigUpdateEventArgs(propertyName, value));
                    return true;
                }
                catch
                {
                    //  Nothing to do, configuration not changed.
                }
            }

            return false;
        }

        #endregion PROPERTIES METHODS

    }
}
