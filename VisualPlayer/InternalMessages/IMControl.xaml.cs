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
using VisualPlayer.Data.Configuration;
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.Pages.Base;

namespace VisualPlayer.InternalMessages
{
    public partial class IMControl : UserControl, IIMControl
    {

        //  VARIABLES

        private List<IMBase> _internalMessages;


        //  GETTERS & SETTERS

        public bool CanGoBack
        {
            get => _internalMessages.Any() && CurrentMessageIndex > 0;
        }

        public IMBase CurrentMessage
        {
            get => _contentFrame.Content as IMBase;
        }

        public int CurrentMessageIndex
        {
            get => CurrentMessage != null ? _internalMessages.IndexOf(CurrentMessage) : -1;
        }

        public int MessagesCount
        {
            get => _internalMessages.Count;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMControl class constructor. </summary>
        public IMControl()
        {
            //  Setup data containers.
            _internalMessages = new List<IMBase>();

            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERNAL INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after navigating to internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Navigation Event Arguments. </param>
        private void FrameNavigated(object sender, NavigationEventArgs e)
        {
            RemoveBackEntry();
        }

        #endregion INTERNAL INTERACTION METHODS

        #region MESSAGES INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after closing internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Internal Message Close Event Args Interface. </param>
        private void OnInternalMessageClosed(object sender, IIMCloseEventArgs e)
        {
            if (sender is IMBase imBase && imBase != null)
            {
                if (imBase == CurrentMessage)
                {
                    if (CanGoBack)
                    {
                        var destMsgIndex = CurrentMessageIndex - 1;
                        var destMsg = _internalMessages[destMsgIndex];

                        _internalMessages.Remove(imBase);
                        _contentFrame.Navigate(destMsg);
                    }
                    else
                    {
                        if (Visibility == Visibility.Visible)
                            Visibility = Visibility.Collapsed;

                        _internalMessages.Remove(imBase);
                    }
                }
                else
                {
                    _internalMessages.Remove(imBase);
                }
            }
        }

        #endregion MESSAGES INTERACTION METHODS

        #region MESSAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load internal message. </summary>
        /// <param name="internalMessage"> Internal message. </param>
        public void LoadMessage(IMBase internalMessage)
        {
            internalMessage.Closed += OnInternalMessageClosed;

            _internalMessages.Add(internalMessage);
            _contentFrame.Navigate(internalMessage);

            if (Visibility != Visibility.Visible)
                Visibility = Visibility.Visible;
        }

        #endregion MESSAGES MANAGEMENT METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Remove history from frame navigation service. </summary>
        private void RemoveBackEntry()
        {
            var backEntry = _contentFrame.NavigationService.RemoveBackEntry();

            if (backEntry != null)
                RemoveBackEntry();
        }

        #endregion UTILITY METHODS

    }
}
