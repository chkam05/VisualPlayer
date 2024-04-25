using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class UserInterfaceConfig : BaseViewModel
    {

        //  VARIABLES

        private bool _contentControlAutoHide;
        private bool _controlBarAutoHide;
        private bool _nowPlayingStayOnTop;


        //  GETTERS & SETTERS

        public bool ContentControlAutoHide
        {
            get => _contentControlAutoHide;
            set => UpdateProperty(ref _contentControlAutoHide, value);
        }

        public bool ControlBarAutoHide
        {
            get => _controlBarAutoHide;
            set => UpdateProperty(ref _controlBarAutoHide, value);
        }

        public bool NowPlayingStayOnTop
        {
            get => _nowPlayingStayOnTop;
            set => UpdateProperty(ref _nowPlayingStayOnTop, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> UserInterfaceConfig class constructor. </summary>
        [JsonConstructor]
        public UserInterfaceConfig(
            bool? contentControlAutoHide = null,
            bool? controlBarAutoHide = null,
            bool? nowPlayingStayOnTop = null)
        {
            ContentControlAutoHide = contentControlAutoHide.HasValue ? contentControlAutoHide.Value : true;
            ControlBarAutoHide = controlBarAutoHide.HasValue ? controlBarAutoHide.Value : true;
            NowPlayingStayOnTop = nowPlayingStayOnTop.HasValue ? nowPlayingStayOnTop.Value : false;
        }

        #endregion CLASS METHODS

    }
}
