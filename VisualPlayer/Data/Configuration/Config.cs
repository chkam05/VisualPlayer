using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class Config : BaseViewModel
    {

        //  VARIABLES

        private Appearance _appearance;
        private bool _enableDebug;
        private UserInterfaceConfig _userInterface;
        private WindowConfig _window;


        //  GETTERS & SETTERS

        public Appearance Appearance
        {
            get => _appearance;
            set => UpdateProperty(ref _appearance, value);
        }

        public bool EnableDebug
        {
            get => _enableDebug;
            set => UpdateProperty(ref _enableDebug, value);
        }

        public UserInterfaceConfig UserInterface
        {
            get => _userInterface;
            set => UpdateProperty(ref _userInterface, value);
        }

        public WindowConfig Window
        {
            get => _window;
            set => UpdateProperty(ref _window, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Config class constructor. </summary>
        [JsonConstructor]
        public Config(
            Appearance appearance = null,
            bool? enableDebug = null,
            UserInterfaceConfig userInterface = null,
            WindowConfig window = null)
        {
        #if DEBUG
            EnableDebug = enableDebug ?? true;
        #else
            EnableDebug = enableDebug ?? false;
        #endif

            Appearance = appearance ?? new Appearance();
            UserInterface = userInterface ?? new UserInterfaceConfig();
            Window = window ?? new WindowConfig();
        }

        #endregion CLASS METHODS

    }
}
