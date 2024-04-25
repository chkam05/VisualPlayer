using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.InternalMessages.Base;

namespace VisualPlayer.InternalMessages
{
    public interface IIMControl
    {

        //  GETTERS & SETTERS

        bool CanGoBack { get; }

        IMBase CurrentMessage { get; }

        int CurrentMessageIndex { get; }

        int MessagesCount { get; }


        //  METHODS

        /// <summary> Load internal message. </summary>
        /// <param name="internalMessage"> Internal message. </param>
        void LoadMessage(IMBase internalMessage);

    }
}
