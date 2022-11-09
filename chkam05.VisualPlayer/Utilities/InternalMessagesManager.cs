using chkam05.Tools.ControlsEx.Events;
using chkam05.Tools.ControlsEx.InternalMessages;
using chkam05.VisualPlayer.Data.Configuration;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static chkam05.Tools.ControlsEx.Events.Delegates;

namespace chkam05.VisualPlayer.Utilities
{
    public class InternalMessagesManager
    {

        //  VARIABLES

        public ConfigManager ConfigManager { get; private set; }
        public InternalMessagesExContainer Container { get; private set; }
        public List<IInternalMessageEx> Messages { get; private set; }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessagesManager class constructor. </summary>
        /// <param name="container"> InternalMessagesExContainer. </param>
        public InternalMessagesManager(InternalMessagesExContainer container, ConfigManager configManager)
        {
            Container = container;
            ConfigManager = configManager;
            Messages = new List<IInternalMessageEx>();
        }

        #endregion CLASS METHODS

        #region MESSAGES METHODS

        //  --------------------------------------------------------------------------------
        private void OnCloseMessage(object sender, InternalMessageCloseEventArgs e)
        {
            var message = (IInternalMessageEx)sender;

            if (Messages.Contains(message))
                Messages.Remove(message);
        }

        #endregion MESSAGES METHODS

        #region MESSAGES MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        public InternalMessageEx CreateAlertMessageBox(string title, string messageContent, bool autoShow = true)
        {
            var message = InternalMessageEx.CreateAlertMessage(Container, title, messageContent);
            message.OnClose += OnCloseMessage;
            Messages.Add(message);
            UpdateBaseConfiguration(message);

            if (autoShow)
                Container.ShowMessage(message);
            return message;
        }

        //  --------------------------------------------------------------------------------
        public InternalMessageEx CreateQuestionMessageBox(string title, string messageContent, 
            InternalMessageClose<InternalMessageCloseEventArgs> onCloseEvent, bool autoShow = true)
        {
            var message = InternalMessageEx.CreateQuestionMessage(Container, title, messageContent);
            message.OnClose += onCloseEvent;
            message.OnClose += OnCloseMessage;
            Messages.Add(message);
            UpdateBaseConfiguration(message);

            if (autoShow)
                Container.ShowMessage(message);
            return message;
        }

        //  --------------------------------------------------------------------------------
        public ProgressInternalMessageEx CreateProgressMessage(string title, string messageContent, bool autoShow = true)
        {
            var message = new ProgressInternalMessageEx(Container, title, messageContent);
            message.OnClose += OnCloseMessage;
            Messages.Add(message);
            UpdateProgressConfiguration(message);

            if (autoShow)
                Container.ShowMessage(message);
            return message;
        }

        #endregion MESSAGES MANAGEMENT METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        public void UpdateConfiguration()
        {
            foreach (var message in Messages)
            {
                if (message.GetType().IsSubclassOf(typeof(BaseAwaitInternalMessageEx)))
                    UpdateAwaitConfiguration(message as BaseAwaitInternalMessageEx);

                else if (message.GetType().IsSubclassOf(typeof(BaseProgressInternalMessageEx)))
                    UpdateProgressConfiguration(message as BaseProgressInternalMessageEx);

                else if (message.GetType().IsSubclassOf(typeof(ColorsPickerInternalMessageEx)))
                    UpdateColorPickerConfiguration(message as ColorsPickerInternalMessageEx);

                else if (message.GetType().IsSubclassOf(typeof(BaseFilesSelectorInternalMessageEx)))
                    UpdateFilesSelectorConfiguration(message as BaseFilesSelectorInternalMessageEx);

                else
                    UpdateBaseConfiguration(message as BaseInternalMessageEx<InternalMessageCloseEventArgs>);
            }
        }

        //  --------------------------------------------------------------------------------
        private void UpdateBaseConfiguration<T>(BaseInternalMessageEx<T> message) where T : InternalMessageCloseEventArgs
        {
            message.Background = ConfigManager.BackgroundColorBrush;
            message.BorderBrush = ConfigManager.AccentColorBrush;
            message.Foreground = ConfigManager.ForegroundColorBrush;
            message.ButtonBackground = ConfigManager.AccentColorBrush;
            message.ButtonBorderBrush = ConfigManager.AccentColorBrush;
            message.ButtonForeground = ConfigManager.AccentForegroundColorBrush;
            message.ButtonMouseOverBackground = ConfigManager.MouseOverBackgroundColorBrush;
            message.ButtonMouseOverBorderBrush = ConfigManager.MouseOverBorderColorBrush;
            message.ButtonMouseOverForeground = ConfigManager.MouseOverForegroundColorBrush;
            message.ButtonPressedBackground = ConfigManager.PressedBackgroundColorBrush;
            message.ButtonPressedBorderBrush = ConfigManager.PressedBorderColorBrush;
            message.ButtonPressedForeground = ConfigManager.PressedForegroundColorBrush;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateAwaitConfiguration(BaseAwaitInternalMessageEx message)
        {
            UpdateBaseConfiguration(message);

            message.IndicatorFill = ConfigManager.AccentColorBrush;
            message.IndicatorPen = ConfigManager.AccentColorBrush;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateColorPickerConfiguration(ColorsPickerInternalMessageEx message)
        {
            UpdateBaseConfiguration(message);

            message.ColorComponentMouseOverBackground = ConfigManager.MouseOverBackgroundColorBrush;
            message.ColorComponentMouseOverBorderBrush = ConfigManager.MouseOverBorderColorBrush;
            message.ColorComponentMouseOverForeground = ConfigManager.MouseOverForegroundColorBrush;
            message.ColorComponentSelectedBackground = ConfigManager.SelectedBackgroundColorBrush;
            message.ColorComponentSelectedBorderBrush = ConfigManager.SelectedBorderColorBrush;
            message.ColorComponentSelectedForeground = ConfigManager.SelectedForegroundColorBrush;
            message.ColorComponentSelectedTextBackground = ConfigManager.SelectedBackgroundColorBrush;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateFilesSelectorConfiguration(BaseFilesSelectorInternalMessageEx message)
        {
            UpdateBaseConfiguration(message);

            message.TextBoxMouseOverBackground = ConfigManager.MouseOverBackgroundColorBrush;
            message.TextBoxMouseOverBorderBrush = ConfigManager.MouseOverBorderColorBrush;
            message.TextBoxMouseOverForeground = ConfigManager.MouseOverForegroundColorBrush;
            message.TextBoxSelectedBackground = ConfigManager.SelectedBackgroundColorBrush;
            message.TextBoxSelectedBorderBrush = ConfigManager.SelectedBorderColorBrush;
            message.TextBoxSelectedForeground = ConfigManager.SelectedForegroundColorBrush;
            message.TextBoxSelectedTextBackground = ConfigManager.SelectedBackgroundColorBrush;
        }

        //  --------------------------------------------------------------------------------
        private void UpdateProgressConfiguration(BaseProgressInternalMessageEx message)
        {
            UpdateBaseConfiguration(message);

            message.ProgressBarBorderBrush = ConfigManager.AccentColorBrush;
            message.ProgressBarProgressBrush = ConfigManager.AccentColorBrush;
        }

        #endregion UPDATE METHODS

    }
}
