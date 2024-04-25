using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Events
{
    public class VolumeControlAnimateEventArgs : EventArgs
    {

        //  VARIABLES

        public bool TargetState { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VolumeControlAnimateEventArgs class constructor. </summary>
        /// <param name="targetState"> Target state. </param>
        public VolumeControlAnimateEventArgs(bool targetState)
        {
            TargetState = targetState;
        }

        #endregion CLASS METHODS

    }
}
