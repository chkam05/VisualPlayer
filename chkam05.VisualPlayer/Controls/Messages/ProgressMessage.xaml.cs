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
    public partial class ProgressMessage : Page, IProgressMessage, INotifyPropertyChanged
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(ProgressMessage),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PackIconKindProperty = DependencyProperty.Register(
            nameof(PackIconKind),
            typeof(PackIconKind),
            typeof(ProgressMessage),
            new PropertyMetadata(PackIconKind.ProgressClock));

        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register(
            nameof(ProgressValue),
            typeof(double),
            typeof(ProgressMessage),
            new PropertyMetadata(0.0));


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

        public PackIconKind PackIconKind
        {
            get => (PackIconKind)GetValue(PackIconKindProperty);
            set
            {
                SetValue(PackIconKindProperty, value);
                OnPropertyChanged(nameof(PackIconKind));
            }
        }

        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set
            {
                SetValue(ProgressValueProperty, Math.Max(Math.Min(value, 100.0), 0.0));
                OnPropertyChanged(nameof(ProgressValue));//ProgressBarStyle
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ProgressMessage class constructor. </summary>
        /// <param name="messagesManager"> Messages manager where page will be presented. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="messageCloseEvent"> Message close event handler. </param>
        public ProgressMessage(IMessagesManager messagesManager, string title, string message,
            EventHandler<MessageCloseEventArgs> messageCloseEvent)
        {
            //  Setup modules.
            Configuration = Configuration.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            MessagesManager = messagesManager;
            Message = message;
            OnMessageClose = messageCloseEvent;
            Result = MessageResult.NONE;
            Title = title;
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
        /// <summary> Get mapped progress value. </summary>
        /// <param name="maxValue"> Max value to get. </param>
        /// <returns> Mapped progress value. </returns>
        public double GetMapProgressValue(double maxValue)
        {
            return (maxValue > 0 && ProgressValue > 0) ? ProgressValue * maxValue / 100 : 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set progress value by mapping value. </summary>
        /// <param name="value"> Value to map. </param>
        /// <param name="maxValue"> Max value to set. </param>
        public void SetMapProgressValue(double value, double maxValue)
        {
            ProgressValue = (maxValue > 0 && value > 0) ? value * 100 / maxValue : 0;
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

    }
}
