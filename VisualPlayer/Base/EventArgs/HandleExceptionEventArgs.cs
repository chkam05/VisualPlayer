using chkam05.InternalMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Base.EventArgs
{
    public class HandleExceptionEventArgs
    {

        //  VARIABLES

        public Exception Exception { get; private set; }
        public IInternalMessage InternalMessage { get; private set; }


        #region GETTERS & SETTERS

        public bool HasInternalMessage
        {
            get => InternalMessage != null;
        }

        public Type InternalMessageType
        {
            get => InternalMessage?.GetType();
        }

        public string Message
        {
            get => Exception.Message;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> HandleExceptionEventArgs class constructor. </summary>
        /// <param name="exc"> Raised exception. </param>
        /// <param name="internalMessage"> Attached internal message. </param>
        public HandleExceptionEventArgs(Exception exc, IInternalMessage internalMessage)
        {
            Exception = exc;
            InternalMessage = internalMessage;
        }

        #endregion CLASS METHODS

    }
}
