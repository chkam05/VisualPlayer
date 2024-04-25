using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.InternalMessages.Enums;

namespace VisualPlayer.InternalMessages.Base
{
    public class IMCloseEventArgs : EventArgs, IIMCloseEventArgs
    {

        //  VARIABLES

        public IMResult Result { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMCloseEventArgs class constructor. </summary>
        /// <param name="result"> Result. </param>
        public IMCloseEventArgs(IMResult result) : base()
        {
            Result = result;
        }

        #endregion CLASS METHODS

    }
}
