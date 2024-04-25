using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Base;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Player
{
    public class Player : BaseViewModel
    {

        //  DELEGATES

        public delegate void PlayerExceptionThrownEventHandler(object sender, ExceptionThrownEventArgs e);


        //  EVENTS

        public PlayerExceptionThrownEventHandler PlayerExceptionThrown;


        //  VARIABLES

        private static Player _instance;
        private static object _instanceLock = new object();

        private ControlBarDataContext _controlBarDataContext;
        private NowPlayingDataContext _playListDataContext;


        //  GETTERS & SETTERS

        public static Player Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new Player();

                    return _instance;
                }
            }
        }

        public ControlBarDataContext ControlBarDataContext
        {
            get => _controlBarDataContext;
            set => UpdateProperty(ref _controlBarDataContext, value);
        }

        public NowPlayingDataContext PlayListDataContext
        {
            get => _playListDataContext;
            set => UpdateProperty(ref _playListDataContext, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Player constructor. </summary>
        private Player()
        {
            _controlBarDataContext = new ControlBarDataContext();
            _playListDataContext = new NowPlayingDataContext();
        }

        #endregion CLASS METHODS

    }
}
