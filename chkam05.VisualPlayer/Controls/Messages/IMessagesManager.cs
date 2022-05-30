using chkam05.VisualPlayer.Controls.Messages.Data;
using chkam05.VisualPlayer.Controls.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Messages
{
    public interface IMessagesManager
    {

        //  EVENTS

        event EventHandler<MessageLoadedEventArgs> OnMessageBack;
        event EventHandler<MessageLoadedEventArgs> OnMessageLoaded;
        event EventHandler OnShow;
        event EventHandler OnHide;


        //  GETTERS & SETTERS

        bool CanGoBack { get; }
        IMessage LoadedMessage { get; }
        int MessagesCount { get; }
        bool IsVisible { get; }


        //  METHODS

        #region CREATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show box message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageType"> Message type. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        BoxMessage CreateBoxMessage(string title, string message,
            BoxMessageType messageType = BoxMessageType.INFO,
            EventHandler<MessageCloseEventArgs> messageCloseEvent = null);

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show progress message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        /// <returns> Progress message. </returns>
        ProgressMessage CreateProgressMessage(string title, string message,
            EventHandler<MessageCloseEventArgs> messageCloseEvent = null);

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show await message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        /// <returns> Await message. </returns>
        AwaitMessage CreateAwaitMessage(string title, string message,
            EventHandler<MessageCloseEventArgs> messageCloseEvent = null);

        #endregion CREATE METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show message control user interface. </summary>
        void ShowInterface();

        //  --------------------------------------------------------------------------------
        /// <summary> Hide message control user interface. </summary>
        void HideInterface();

        #endregion INTERFACE MANAGEMENT METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Go to previous message/messages by certain number of steps. </summary>
        /// <param name="backCount"> Number of steps back. </param>
        void GoBack(int backCount = 1);

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate to newly created message. </summary>
        /// <param name="message"> Message to load. </param>
        void LoadMessage(IMessage message);

        #endregion NAVIGATION METHODS

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if message is already loaded. </summary>
        /// <param name="message"> Message to check. </param>
        /// <returns> True - message is already loaded; False - otherwise. </returns>
        bool HasMessage(IMessage message);

        //  --------------------------------------------------------------------------------
        /// <summary> Check if message with specified type is already loaded. </summary>
        /// <param name="messageType"> Message type to check. </param>
        /// <returns> True - message with this type is already loaded; False - otherwise. </returns>
        bool HasMessage(Type messageType);

        #endregion PAGES MANAGEMENT METHODS

    }
}
