using chkam05.VisualPlayer.Controls.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class SideBarAnimationFinishEventArgs : EventArgs
    {

        //  VARIABLES

        public SideBarMenuState State { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SideBarAnimationFinishEventArgs class constructor. </summary>
        /// <param name="state"> State. </param>
        public SideBarAnimationFinishEventArgs(SideBarMenuState state)
        {
            State = state;
        }

        #endregion CLASS METHODS

    }
}
