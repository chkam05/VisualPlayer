using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class InformationBarAnimationFinishEventArgs : EventArgs
    {

        //  VARIABLES

        public InformationBarState State { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InformationBarAnimationFinishEventArgs class constructor. </summary>
        /// <param name="state"> State. </param>
        public InformationBarAnimationFinishEventArgs(InformationBarState state)
        {
            State = state;
        }

        #endregion CLASS METHODS

    }
}
