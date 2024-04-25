using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages.Enums;

namespace VisualPlayer.InternalMessages
{
    public partial class IMBox : IMBase
    {

        //  CONST

        private static readonly IMResult[] DEFAULT_BUTTONS = new IMResult[] { IMResult.Ok };


        //  VARIABLES

        private ObservableCollection<IMResult> _buttons;
        private PackIconKind _icon = PackIconKind.None;
        private string _message = string.Empty;


        //  GETTERS & SETTERS

        public ObservableCollection<IMResult> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                _buttons.CollectionChanged += OnButtonsCollectionChanged;
                NotifyPropertyChanged(nameof(Buttons));
            }
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public string Message
        {
            get => _message;
            set => UpdateProperty(ref _message, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMBox class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        public IMBox(IIMControl imControl) : base(imControl)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="buttons"> Buttons collection. </param>
        /// <param name="icon"> Message title icon. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateMessage(IIMControl imControl, string title, string message,
            IEnumerable<IMResult> buttons = null,
            PackIconKind icon = PackIconKind.InfoCircleOutline,
            CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var imBox = new IMBox(imControl)
            {
                Buttons = new ObservableCollection<IMResult>((buttons?.Any() ?? false) ? buttons : DEFAULT_BUTTONS),
                Icon = icon,
                Message = message,
                Title = title,
            };

            if (onClose != null)
                imBox.Closed = onClose;

            return imBox;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create info internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateInfo(IIMControl imControl, string title, string message, CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var buttons = new[] { IMResult.Ok };
            var icon = PackIconKind.InfoCircleOutline;

            return CreateMessage(imControl, title, message, buttons, icon, onClose);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create alert internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateAlert(IIMControl imControl, string title, string message, CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var buttons = new[] { IMResult.Ok };
            var icon = PackIconKind.AlertOutline;

            return CreateMessage(imControl, title, message, buttons, icon, onClose);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create error internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateError(IIMControl imControl, string title, string message, CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var buttons = new[] { IMResult.Ok };
            var icon = PackIconKind.CloseOctagonOutline;

            return CreateMessage(imControl, title, message, buttons, icon, onClose);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create ok/cancel internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Message title icon. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateOkCancel(IIMControl imControl, string title, string message,
            PackIconKind icon = PackIconKind.InfoCircleOutline, CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var buttons = new[] { IMResult.Ok, IMResult.Cancel };

            return CreateMessage(imControl, title, message, buttons, icon, onClose);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create question (yes/no) internal message box. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMBox CreateQuestion(IIMControl imControl, string title, string message, CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var buttons = new[] { IMResult.Yes, IMResult.No };
            var icon = PackIconKind.QuestionMarkCircleOutline;

            return CreateMessage(imControl, title, message, buttons, icon, onClose);
        }

        #endregion CLASS METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ok button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Ok);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking yes button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void YesCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Yes);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking no button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void NoCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.No);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CancelCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Cancel);
        }

        #endregion BUTTONS INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after buttons collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        protected virtual void OnButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Buttons));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
