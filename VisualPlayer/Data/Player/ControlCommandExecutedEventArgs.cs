using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Data.Player
{
    public class ControlCommandExecutedEventArgs : EventArgs
    {

        //  VARIABLES

        public ControlCommand Command { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControllCommandEventArgs class constructor. </summary>
        /// <param name="command"> Control command type. </param>
        public ControlCommandExecutedEventArgs(ControlCommand command) : base()
        {
            Command = command;
        }

        #endregion CLASS METHODS

    }
}
