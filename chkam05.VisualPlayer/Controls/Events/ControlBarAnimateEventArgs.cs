using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class ControlBarAnimateEventArgs : EventArgs
    {

        //  VARIABLES

        public ControlBarState? TargetState { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlBarAnimateEventArgs class constructor. </summary>
        /// <param name="targetState"> Target state. </param>
        public ControlBarAnimateEventArgs(ControlBarState? targetState)
        {
            TargetState = targetState;
        }

        #endregion CLASS METHODS

    }
}
