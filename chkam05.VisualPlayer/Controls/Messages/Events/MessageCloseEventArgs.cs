using chkam05.VisualPlayer.Controls.Messages.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Messages.Events
{
    public class MessageCloseEventArgs : EventArgs
    {

        //  VARIABLES

        public MessageResult MessageResult { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessageClosedEventArgs class constructor. </summary>
        /// <param name="messageResult"> Message result. </param>
        public MessageCloseEventArgs(MessageResult messageResult)
        {
            MessageResult = messageResult;
        }

        #endregion CLASS METHODS

    }
}
