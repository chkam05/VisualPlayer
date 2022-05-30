using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class VolumeControlAnimationFinishEventArgs : EventArgs
    {

        //  VARIABLES

        public bool State { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VolumeControlAnimationFinishEventArgs class constructor. </summary>
        /// <param name="state"> State. </param>
        public VolumeControlAnimationFinishEventArgs(bool state)
        {
            State = state;
        }

        #endregion CLASS METHODS

    }
}
