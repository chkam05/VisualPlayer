using chkam05.VisualPlayer.Controls.Messages.Data;
using chkam05.VisualPlayer.Controls.Messages.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls.Messages
{
    public partial class MessagesControl : UserControl, IMessagesManager, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageLoadedEventArgs> OnMessageBack;
        public event EventHandler<MessageLoadedEventArgs> OnMessageLoaded;
        public event EventHandler OnShow;
        public event EventHandler OnHide;


        //  VARIABLES

        private List<IMessage> _loadedMessages;


        //  GETTERS & SETTERS

        public bool CanGoBack
        {
            get => _loadedMessages.Any() && _loadedMessages.IndexOf(LoadedMessage) > 0;
        }

        public IMessage LoadedMessage
        {
            get => _loadedMessages.LastOrDefault();
        }

        public int MessagesCount
        {
            get => _loadedMessages.Count;
        }

        public new bool IsVisible
        {
            get => Visibility == Visibility.Visible;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MessagesControl class constructor. </summary>
        public MessagesControl()
        {
            //  Setup Data Containers.
            _loadedMessages = new List<IMessage>();

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region CONTENT FRAME METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all loaded messages from history. </summary>
        private void ClearAllContent()
        {
            _loadedMessages.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove currently loaded message from content frame. </summary>
        private void ClearCurrentContent()
        {
            ContentFrame.Content = null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Metod invoked after loading message in frame. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Navigated Event Arguments. </param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //  Remove previous messages from content frame back entry.
            RemoveBackEntry();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove previous messages from content frame back entry. </summary>
        private void RemoveBackEntry()
        {
            //  Get previous messages from content frame navigation service.
            var backEntry = ContentFrame.NavigationService.RemoveBackEntry();

            //  While previous messages are available - try to remove it.
            while (backEntry != null)
                backEntry = ContentFrame.NavigationService.RemoveBackEntry();
        }

        #endregion CONTENT FRAME METHODS

        #region CREATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show box message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageType"> Message type. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        /// <returns> Message box. </returns>
        public BoxMessage CreateBoxMessage(string title, string message, BoxMessageType messageType = BoxMessageType.INFO, 
            EventHandler<MessageCloseEventArgs> messageCloseEvent = null)
        {
            BoxMessage messagePage = new BoxMessage(this, title, message, messageType, messageCloseEvent);
            LoadMessage(messagePage);

            if (!IsVisible)
                ShowInterface();

            return messagePage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show progress message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        /// <returns> Progress message. </returns>
        public ProgressMessage CreateProgressMessage(string title, string message, EventHandler<MessageCloseEventArgs> messageCloseEvent = null)
        {
            ProgressMessage messagePage = new ProgressMessage(this, title, message, messageCloseEvent);
            LoadMessage(messagePage);

            if (!IsVisible)
                ShowInterface();

            return messagePage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create and show await message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        /// <returns> Await message. </returns>
        public AwaitMessage CreateAwaitMessage(string title, string message, EventHandler<MessageCloseEventArgs> messageCloseEvent = null)
        {
            AwaitMessage messagePage = new AwaitMessage(this, title, message, messageCloseEvent);
            LoadMessage(messagePage);

            if (!IsVisible)
                ShowInterface();

            return messagePage;
        }

        #endregion CREATE METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show messages control user interface. </summary>
        public void ShowInterface()
        {
            if (Visibility != Visibility.Visible)
            {
                Visibility = Visibility.Visible;
                OnShow?.Invoke(this, new EventArgs());
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide messages control user interface. </summary>
        public void HideInterface()
        {
            Visibility = Visibility.Collapsed;
            OnHide?.Invoke(this, new EventArgs());

            //  Clear all pages.
            ClearAllContent();
            ClearCurrentContent();

            //  Invoke external event.
            var args = new MessageLoadedEventArgs(null);
            OnMessageBack?.Invoke(this, args);
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region NAVIGATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Go to previous message/messages by certain number of steps. </summary>
        /// <param name="backCount"> Number of steps back. </param>
        public void GoBack(int backCount = 1)
        {
            if (CanGoBack)
            {
                var currPageIndex = _loadedMessages.IndexOf(LoadedMessage);
                var destPageIndex = Math.Max(0, currPageIndex - backCount);

                //  Get previous page from list to load into content frame.
                var destPage = _loadedMessages[destPageIndex];

                //  Remove other pages loaded further.
                _loadedMessages.RemoveRange(destPageIndex + 1, MessagesCount - (destPageIndex + 1));

                //  Load previous page and update current page index.
                ContentFrame.Navigate(destPage);

                //  Invoke external event.
                var args = new MessageLoadedEventArgs(destPage);
                OnMessageBack?.Invoke(this, args);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Navigate to newly created message. </summary>
        /// <param name="message"> Message to load. </param>
        public void LoadMessage(IMessage message)
        {
            var messageToLoad = message as Page;

            if (messageToLoad != null)
            {
                //  Add page to history.
                _loadedMessages.Add(message);

                //  Load page.
                ContentFrame.Navigate(messageToLoad);

                //  Invoke external event.
                var args = new MessageLoadedEventArgs(message);
                OnMessageLoaded?.Invoke(this, args);
            }
        }

        #endregion NAVIGATION METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region PAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if message is already loaded. </summary>
        /// <param name="message"> Message to check. </param>
        /// <returns> True - message is already loaded; False - otherwise. </returns>
        public bool HasMessage(IMessage message)
        {
            return _loadedMessages.Contains(message);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if message with specified type is already loaded. </summary>
        /// <param name="messageType"> Message type to check. </param>
        /// <returns> True - message with this type is already loaded; False - otherwise. </returns>
        public bool HasMessage(Type messageType)
        {
            return _loadedMessages.Any(p => messageType.IsAssignableFrom(((Page)p).GetType()));
        }

        #endregion PAGES MANAGEMENT METHODS

    }
}
