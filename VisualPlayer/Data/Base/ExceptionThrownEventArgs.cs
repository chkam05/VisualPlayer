using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Data.Base
{
    public class ExceptionThrownEventArgs : EventArgs
    {

        //  VARIABLES

        public string Context { get; private set; }
        public string Message { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayerExceptionThrownEventArgs class constructor. </summary>
        /// <param name="context"> Context. </param>
        /// <param name="exception"> Exception. </param>
        public ExceptionThrownEventArgs(string context, string message) : base()
        {
            Context = context;
            Message = !string.IsNullOrEmpty(message)
                ? message
                : "An unknown error has been thrown.";
        }

        #endregion CLASS METHODS

    }
}
