using chkam05.VisualPlayer.Controls.Messages.Data;
using chkam05.VisualPlayer.Controls.Messages.Events;
using chkam05.VisualPlayer.Data.Config;
using MaterialDesignThemes.Wpf;
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
    public partial class BoxMessage : Page, IMessage, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(BoxMessage),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register(
            nameof(MessageType),
            typeof(BoxMessageType),
            typeof(BoxMessage),
            new PropertyMetadata(BoxMessageType.INFO));

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(BoxMessage),
            new PropertyMetadata(PackIconKind.InformationCircle));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageCloseEventArgs> OnMessageClose;


        //  VARIABLES

        public Configuration Configuration { get; private set; }
        public IMessagesManager MessagesManager { get; private set; }
        public MessageResult Result { get; private set; }


        //  GETTERS & SETTERS

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set
            {
                SetValue(MessageProperty, value);
                OnPropertyChanged(nameof(Message));
            }
        }

        public BoxMessageType MessageType
        {
            get => (BoxMessageType)GetValue(MessageTypeProperty);
            set
            {
                SetValue(MessageTypeProperty, value);
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public PackIconKind PackIconKind
        {
            get => (PackIconKind)GetValue(PackIconKindProperty);
            set
            {
                SetValue(PackIconKindProperty, value);
                OnPropertyChanged(nameof(PackIconKind));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BoxMessage class constructor. </summary>
        /// <param name="messagesManager"> Messages manager where page will be presented. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageType"> Message type. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        public BoxMessage(IMessagesManager messagesManager, string title, string message, 
            BoxMessageType messageType, EventHandler<MessageCloseEventArgs> messageCloseEvent)
        {
            //  Setup modules.
            Configuration = Configuration.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            MessagesManager = messagesManager;
            Message = message;
            MessageType = messageType;
            OnMessageClose = messageCloseEvent;
            Result = MessageResult.NONE;
            Title = title;

            SetupIcon();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message. </summary>
        public void CloseMessage()
        {
            if (MessagesManager.CanGoBack)
                MessagesManager.GoBack();
            else
                MessagesManager.HideInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing OK action button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.OK;
            CloseMessage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing YES action button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.YES;
            CloseMessage();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing NO action button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.NO;
            CloseMessage();
        }

        #endregion INTERACTION METHODS

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

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading message page. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            OnMessageClose?.Invoke(this, new MessageCloseEventArgs(Result));
        }

        #endregion PAGE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup icon by message type. </summary>
        private void SetupIcon()
        {
            switch (MessageType)
            {
                case BoxMessageType.INFO:
                    PackIconKind = PackIconKind.InfoCircleOutline;
                    break;

                case BoxMessageType.ALERT:
                    PackIconKind = PackIconKind.AlertOutline;
                    break;

                case BoxMessageType.ERROR:
                    PackIconKind = PackIconKind.ErrorOutline;
                    break;

                case BoxMessageType.QUESTION:
                    PackIconKind = PackIconKind.QuestionMarkCircleOutline;
                    break;
            }
        }

        #endregion SETUP METHODS

    }
}
