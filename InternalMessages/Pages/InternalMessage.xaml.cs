using chkam05.InternalMessages.Controls;
using chkam05.InternalMessages.EventArgs;
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

namespace chkam05.InternalMessages.Pages
{
    public partial class InternalMessage : Page, IInternalMessage
    {

        //  EVENTS

        public event EventHandler<InternalMessageResultEventArgs> OnOptionSelect;


        //  VARIABLES

        private InternalMessagesContainer _container;


        #region GETTERS & SETTERS

        public string Message
        {
            get => MessageTextBlock.Text;
            set => MessageTextBlock.Text = value;
        }
        public new string Title
        {
            get => TitleTextBlock.Text;
            set => TitleTextBlock.Text = value;
        }
        public PackIconKind Icon
        {
            get => PackIcon.Kind;
            set => PackIcon.Kind = value;
        }
        public Brush IconColor
        {
            get => PackIcon.Foreground;
            set => PackIcon.Foreground = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessage class constructor. </summary>
        /// <param name="container"> Messages container. </param>
        public InternalMessage(InternalMessagesContainer container)
        {
            //  Setup variables.
            _container = container;

            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking OK button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void OkOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            //  Invoke external method.
            OnOptionSelect?.Invoke(this, 
                new InternalMessageResultEventArgs(InternalMessageResult.OK));

            //  Close message.
            Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking YES button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void YesOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            //  Invoke external method.
            OnOptionSelect?.Invoke(this,
                new InternalMessageResultEventArgs(InternalMessageResult.YES));

            //  Close message.
            Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after clicking NO button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void NoOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            //  Invoke external method.
            OnOptionSelect?.Invoke(this,
                new InternalMessageResultEventArgs(InternalMessageResult.NO));

            //  Close message.
            Close();
        }

        #endregion BUTTONS INTERACTION METHODS

        #region BUTTONS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get option button that depends on selected result type. </summary>
        /// <param name="resultType"> Result type. </param>
        /// <returns> Option button that depends on selected result type. </returns>
        private OptionButton GetButtonDependsOnResult(InternalMessageResult resultType)
        {
            switch (resultType)
            {
                case InternalMessageResult.OK:
                    return OkOptionsButton;

                case InternalMessageResult.YES:
                    return YesOptionsButton;

                case InternalMessageResult.NO:
                    return NoOptionsButton;

                default:
                    return null;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set option button background. </summary>
        /// <param name="resultType"> Result type that option button depends on. </param>
        /// <param name="background"> Option button new background. </param>
        public void SetButtonBackground(InternalMessageResult resultType, Brush background)
        {
            var optionsButton = GetButtonDependsOnResult(resultType);
            optionsButton.Background = background;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set option button foreground. </summary>
        /// <param name="resultType"> Result type that option button depends on. </param>
        /// <param name="foreground"> Option button new foreground. </param>
        public void SetButtonForeground(InternalMessageResult resultType, Brush foreground)
        {
            var optionsButton = GetButtonDependsOnResult(resultType);
            optionsButton.Foreground = foreground;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set option button icon. </summary>
        /// <param name="resultType"> Result type that option button depends on. </param>
        /// <param name="icon"> Option button new icon. </param>
        public void SetButtonIcon(InternalMessageResult resultType, PackIconKind icon)
        {
            var optionsButton = GetButtonDependsOnResult(resultType);
            optionsButton.Icon = icon;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set option button title. </summary>
        /// <param name="resultType"> Result type that option button depends on. </param>
        /// <param name="title"> Option button new title. </param>
        public void SetButtonTitle(InternalMessageResult resultType, string title)
        {
            var optionsButton = GetButtonDependsOnResult(resultType);
            optionsButton.Title = title;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show or hide option button that depends on selected result type. </summary>
        /// <param name="resultType"> Result type. </param>
        /// <param name="use"> True - show button; False - hide button. </param>
        public void UseButton(InternalMessageResult resultType, bool use = true)
        {
            var optionsButton = GetButtonDependsOnResult(resultType);
            optionsButton.Visibility = use ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion BUTTONS MANAGEMENT METHODS

        #region INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message and load previous message if was showen. </summary>
        /// <returns> Previous showen message or NULL. </returns>
        public IInternalMessage Close()
        {
            //  Close message and return previous message if was showen or null.
            return _container.Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if interface type is particular type of internal message. </summary>
        /// <param name="type"> Internal message type to check. </param>
        /// <returns> True - interface is type of internal message type; False - otherwise. </returns>
        public bool IsTypeOf(Type type) => this.GetType() == type;

        #endregion INTERFACE METHODS

    }
}
