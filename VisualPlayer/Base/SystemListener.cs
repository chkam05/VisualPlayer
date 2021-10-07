using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventArgs = System.EventArgs;


namespace chkam05.VisualPlayer.Base
{
    public class SystemListener
    {

        //  EVENTS

        /// <summary> Event occurs when the user changes the display settings. </summary>
        public event EventHandler DisplaySettingsChangedHanlder;

        /// <summary> Event occurs when the display settings are changing. </summary>
        public event EventHandler DisplaySettingsChangingHandler;

        /// <summary> Event occurs before the thread that listens for system events is terminated. </summary>
        public event EventHandler EventsThreadShutdownHandler;

        /// <summary> Event occurs when the user adds fonts to or removes fonts from the system. </summary>
        public event EventHandler InstalledFontsChangedHandler;

        /// <summary> Event occurs when the user switches to an application that uses a different palette. </summary>
        public event EventHandler PaletteChangedHandler;

        /// <summary> Event occurs when the user suspends or resumes the system. </summary>
        public event EventHandler PowerModeChangedHandler;

        /// <summary> Event occurs when the user is logging off or shutting down the system. </summary>
        public event EventHandler SessionEndedHandler;

        /// <summary> Event occurs when the user is trying to log off or shut down the system. </summary>
        public event EventHandler SessionEndingHandler;

        /// <summary> Event occurs when the currently logged-in user has changed. </summary>
        public event EventHandler SessionSwitchHandler;

        /// <summary> Event occurs when the user changes the time on the system clock. </summary>
        public event EventHandler TimeChangedHandler;

        /// <summary> Event occurs when a windows timer interval has expired. </summary>
        public event EventHandler TimerElapsedHandler;

        /// <summary> Event occurs when a user preference has changed. </summary>
        public event EventHandler UserPreferenceChangedHandler;

        /// <summary> Event occurs when a user preference is changing. </summary>
        public event EventHandler UserPreferenceChangingHandler;


        //  VARIABLES

        private static SystemListener _instance;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SystemListener class constructor. </summary>
        private SystemListener()
        {
            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
            SystemEvents.DisplaySettingsChanging += DisplaySettingsChanging;
            SystemEvents.EventsThreadShutdown += EventsThreadShutdown;
            SystemEvents.InstalledFontsChanged += InstalledFontsChanged;
            SystemEvents.PaletteChanged += PaletteChanged;
            SystemEvents.PowerModeChanged += PowerModeChanged;
            SystemEvents.SessionEnded += SessionEnded;
            SystemEvents.SessionEnding += SessionEnding;
            SystemEvents.SessionSwitch += SessionSwitch;
            SystemEvents.TimeChanged += TimeChanged;
            SystemEvents.TimerElapsed += TimerElapsed;
            SystemEvents.UserPreferenceChanged += UserPreferenceChanged;
            SystemEvents.UserPreferenceChanging += UserPreferenceChanging;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get or create instance of SystemListener class. </summary>
        public static SystemListener Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SystemListener();

                return _instance;
            }
        }

        #endregion CLASS METHODS

        #region LISTENER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user changes the display settings. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            DisplaySettingsChangedHanlder?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the display settings are changing. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void DisplaySettingsChanging(object sender, System.EventArgs e)
        {
            DisplaySettingsChangingHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called before the thread that listens for system events is terminated. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void EventsThreadShutdown(object sender, System.EventArgs e)
        {
            EventsThreadShutdownHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user adds fonts to or removes fonts from the system. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void InstalledFontsChanged(object sender, System.EventArgs e)
        {
            InstalledFontsChangedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user switches to an application that uses a different palette. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void PaletteChanged(object sender, System.EventArgs e)
        {
            PaletteChangedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user suspends or resumes the system. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void PowerModeChanged(object sender, System.EventArgs e)
        {
            PowerModeChangedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user is logging off or shutting down the system. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void SessionEnded(object sender, System.EventArgs e)
        {
            SessionEndedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user is trying to log off or shut down the system. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void SessionEnding(object sender, System.EventArgs e)
        {
            SessionEndingHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the currently logged-in user has changed. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void SessionSwitch(object sender, System.EventArgs e)
        {
            SessionSwitchHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when the user changes the time on the system clock. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void TimeChanged(object sender, System.EventArgs e)
        {
            TimeChangedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when a windows timer interval has expired. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void TimerElapsed(object sender, System.EventArgs e)
        {
            TimerElapsedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when a user preference has changed. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void UserPreferenceChanged(object sender, System.EventArgs e)
        {
            UserPreferenceChangedHandler?.Invoke(sender, e);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when a user preference is changing. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void UserPreferenceChanging(object sender, System.EventArgs e)
        {
            UserPreferenceChangingHandler?.Invoke(sender, e);
        }

        #endregion LISTENER METHODS

    }
}
