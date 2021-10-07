using chkam05.InternalMessages.Pages;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

namespace chkam05.InternalMessages
{
    public partial class InternalMessagesContainer : UserControl
    {

        //  VARIABLES

        private bool _clearPrevious = false;
        private List<IInternalMessage> _history;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessagesContainer class constructor. </summary>
        public InternalMessagesContainer()
        {
            //  Initialize data containers.
            _history = new List<IInternalMessage>();

            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message and load previous message if was showen. </summary>
        /// <returns> Previous showen message or NULL. </returns>
        public IInternalMessage Close()
        {
            //  Load previous message if is available.
            if (ContentFrame.CanGoBack && _history.Count > 1)
            {
                //  Go back.
                ContentFrame.NavigationService.GoBack();

                //  Get current message and remove it.
                var previousPage = _history.LastOrDefault();

                if (previousPage != null)
                    _history.Remove(previousPage);

                //  Return previous message.
                return _history.LastOrDefault();
            }

            //  Clear history and frame content.
            ContentFrame.NavigationService.RemoveBackEntry();
            ContentFrame.Content = null;
            _history.Clear();

            //  Hide messages container.
            HideInterface();

            //  Return no message.
            return null;
        }

        #endregion INTERACTION METHODS

        #region MESSAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when page (message) is loded in content frame. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Navigation event arguments. </param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (_clearPrevious)
            {
                //  Get previous message to clear.
                var previousPage = ContentFrame.NavigationService.RemoveBackEntry();

                //  Keep trying clear previous message.
                while (previousPage != null)
                    previousPage = ContentFrame.RemoveBackEntry();
            }

            //  Reset clear previous page flag.
            _clearPrevious = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load and show message page. </summary>
        /// <param name="messagePage"> Message page to load. </param>
        private void LoadMessage(Page messagePage)
        {
            //  Clear messages history if clear previous flag is set.
            if (_clearPrevious)
                _history.Clear();

            //  Show messages container.
            ShowInterface();

            //  Load and show message page in content frame.
            ContentFrame.NavigationService.Navigate(messagePage);

            //  Add message page to history.
            _history.Add((IInternalMessage)messagePage);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show messages container if is not visible. </summary>
        private void ShowInterface()
        {
            if (this.Visibility != Visibility.Visible)
                this.Visibility = Visibility.Visible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide messages container if is visible. </summary>
        private void HideInterface()
        {
            if (this.Visibility != Visibility.Collapsed)
                this.Visibility = Visibility.Collapsed;
        }

        #endregion MESSAGES MANAGEMENT METHODS

        #region MESSAGES CREATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show information message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowInfo(string title, string message, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessage(this)
            {
                Icon = PackIconKind.Information,
                IconColor = Foreground,
                Message = message,
                Title = title,
            };

            //  Load message.
            LoadMessage(internalMessage);

            //  Configure buttons.
            internalMessage.UseButton(InternalMessageResult.OK);

            return internalMessage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show yes no question message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowYesNoQuestion(string title, string message, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessage(this)
            {
                Icon = PackIconKind.HelpCircle,
                IconColor = Foreground,
                Message = message,
                Title = title,
            };

            //  Load message.
            LoadMessage(internalMessage);

            //  Configure internal message buttons.
            internalMessage.UseButton(InternalMessageResult.YES);
            internalMessage.UseButton(InternalMessageResult.NO);

            return internalMessage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show exclamation message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowExclamation(string title, string message, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessage(this)
            {
                Icon = PackIconKind.Alert,
                IconColor = new SolidColorBrush(Colors.Yellow),
                Message = message,
                Title = title,
            };

            //  Load message.
            LoadMessage(internalMessage);

            //  Configure buttons.
            internalMessage.UseButton(InternalMessageResult.OK);

            return internalMessage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show error message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowError(string title, string message, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessage(this)
            {
                Icon = PackIconKind.Error,
                IconColor = new SolidColorBrush(Colors.Red),
                Message = message,
                Title = title,
            };

            //  Load message.
            LoadMessage(internalMessage);

            //  Configure buttons.
            internalMessage.UseButton(InternalMessageResult.OK);

            return internalMessage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show loader message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Title icon. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowLoader(string title, string message, 
            PackIconKind icon = PackIconKind.Hourglass, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessageLoader(this)
            {
                Icon = icon,
                IconColor = Foreground,
                Message = message,
                Title = title
            };

            //  Load message.
            LoadMessage(internalMessage);

            return internalMessage;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show progress message. </summary>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Title icon. </param>
        /// <param name="clearPrevious"> Clear previous messages flag. </param>
        /// <returns> Message interface. </returns>
        public IInternalMessage ShowProgress(string title, string message,
            PackIconKind icon = PackIconKind.Hourglass, bool clearPrevious = true)
        {
            _clearPrevious = clearPrevious;

            //  Create message with specified configuration.
            var internalMessage = new InternalMessageProgress(this)
            {
                Icon = icon,
                IconColor = Foreground,
                Message = message,
                ProgressMax = 100,
                ProgressValue = 0,
                Title = title
            };

            //  Load message.
            LoadMessage(internalMessage);

            return internalMessage;
        }

        #endregion MESSAGES CREATION METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  Configure content frame.
            ContentFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        #endregion USER CONTROL METHODS

    }
}
