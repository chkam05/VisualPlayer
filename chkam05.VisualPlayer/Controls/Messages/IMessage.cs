using chkam05.VisualPlayer.Controls.Messages.Data;
using chkam05.VisualPlayer.Controls.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Messages
{
    public interface IMessage
    {

        //  EVENTS

        event EventHandler<MessageCloseEventArgs> OnMessageClose;


        //  VARIABLES

        IMessagesManager MessagesManager { get; }
        string Message { get; set; }
        MessageResult Result { get; }
        string Title { get; set; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message. </summary>
        void CloseMessage();

    }
}
