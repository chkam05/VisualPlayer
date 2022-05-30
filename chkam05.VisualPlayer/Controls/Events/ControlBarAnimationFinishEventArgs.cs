using chkam05.VisualPlayer.Controls.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class ControlBarAnimationFinishEventArgs : EventArgs
    {

        //  VARIABLES

        public ControlBarState State { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlBarAnimationFinishEventArgs class constructor. </summary>
        /// <param name="state"> State. </param>
        public ControlBarAnimationFinishEventArgs(ControlBarState state)
        {
            State = state;
        }

        #endregion CLASS METHODS

    }
}
