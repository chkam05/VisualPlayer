using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Messages.Events
{
    public class MessageLoadedEventArgs : EventArgs
    {

        //  VARIABLES

        public IMessage Message { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessageLoadedEventArgs class constructor. </summary>
        /// <param name="message"> Loaded message. </param>
        public MessageLoadedEventArgs(IMessage message)
        {
            Message = message;
        }

        #endregion CLASS METHODS

    }
}
