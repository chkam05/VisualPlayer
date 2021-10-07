using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.InternalMessages.EventArgs
{
    public class InternalMessageResultEventArgs
    {

        //  VARIABLES

        public InternalMessageResult Result { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessageResultEventArgs class constructor. </summary>
        /// <param name="resultType"> Message result type. </param>
        public InternalMessageResultEventArgs(InternalMessageResult resultType)
        {
            Result = resultType;
        }

        #endregion CLASS METHODS

    }
}
