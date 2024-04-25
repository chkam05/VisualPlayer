using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages.Enums;

namespace VisualPlayer.InternalMessages
{
    public partial class IMProgress : IMBase
    {

        //  VARIABLES

        private bool _allowCancel = true;
        private PackIconKind _icon = PackIconKind.None;
        private bool _isFinished = false;
        private string _message = string.Empty;
        private double _progressMax = 1.0f;
        private double _progress = 0.0f;


        //  GETTERS & SETTERS

        public bool AllowCancel
        {
            get => _allowCancel;
            set => UpdateProperty(ref _allowCancel, value);
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public bool IsFinished
        {
            get => _isFinished;
            private set => UpdateProperty(ref _isFinished, value);
        }

        public string Message
        {
            get => _message;
            set => UpdateProperty(ref _message, value);
        }

        public double ProgressMax
        {
            get => _progressMax;
            set
            {
                UpdateProperty(ref _progressMax, Math.Max(0, value));

                if (_progress > _progressMax)
                    Progress = _progressMax;
            }
        }

        public double Progress
        {
            get => _progress;
            set => UpdateProperty(ref _progress, Math.Max(0, Math.Min(value, _progressMax)));
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMProgress class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        public IMProgress(IIMControl imControl) : base(imControl)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message progress. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Message title icon. </param>
        /// <param name="onClose"> On close method. </param>
        /// <param name="allowCancel"> Allow cancel. </param>
        /// <returns> Internal message box. </returns>
        public static IMProgress CreateMessage(IIMControl imControl, string title, string message,
            PackIconKind icon = PackIconKind.ProgressHelper,
            CloseEventHandler<IIMCloseEventArgs> onClose = null,
            bool allowCancel = true)
        {
            var imBox = new IMProgress(imControl)
            {
                AllowCancel = allowCancel,
                Icon = icon,
                Message = message,
                Title = title,
            };

            if (onClose != null)
                imBox.Closed = onClose;

            return imBox;
        }

        #endregion CLASS METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after ok cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Ok);
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

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Finish progress. </summary>
        /// <param name="progress"> Optional progress value (default ProgressMax). </param>
        /// <param name="message"> Optional message. </param>
        public void Finish(double? progress = null, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message;

            if (progress.HasValue)
                Progress = Math.Max(0, Math.Min(ProgressMax, progress.Value));

            IsFinished = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update progress. </summary>
        /// <param name="progress"> Progress value. </param>
        /// <param name="message"> Optional message. </param>
        public void UpdateProgress(double progress, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message;

            Progress = Math.Max(0, Math.Min(ProgressMax, progress));

            if (Progress >= ProgressMax)
                IsFinished = true;
        }

        #endregion INTERACTION METHODS

    }
}
