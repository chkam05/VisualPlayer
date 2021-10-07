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
    public partial class InternalMessageProgress : Page, IInternalMessage
    {

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

        public Brush ProgressColor
        {
            get => ProgressBar.Foreground;
            set => ProgressBar.Foreground = value;
        }

        public double ProgressMax
        {
            get => ProgressBar.Maximum;
            set => ProgressBar.Maximum = value;
        }

        public double ProgressValue
        {
            get => ProgressBar.Value;
            set => ProgressBar.Value = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessageProgress class constructor. </summary>
        /// <param name="container"> Messages container. </param>
        public InternalMessageProgress(InternalMessagesContainer container)
        {
            //  Setup variables.
            _container = container;

            InitializeComponent();
        }

        #endregion CLASS METHODS

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
