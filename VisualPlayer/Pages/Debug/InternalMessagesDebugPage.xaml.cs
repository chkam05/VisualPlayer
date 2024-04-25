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
using VisualPlayer.Controls;
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages;
using VisualPlayer.Pages.Base;
using static VisualPlayer.InternalMessages.Base.IMBase;
using VisualPlayer.InternalMessages.Enums;
using System.Threading;

namespace VisualPlayer.Pages.Debug
{
    public partial class InternalMessagesDebugPage : BasePage
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessagesDebugPage class constructor. </summary>
        /// <param name="contentViewer"> Content viewer interface. </param>
        public InternalMessagesDebugPage(IContentViewer contentViewer) : base(contentViewer)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region HEADER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ContentViewer.GoBack();
        }

        #endregion HEADER INTERACTION METHODS

        #region ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message ok settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageOkSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Test";
            string message = "This is internal message test, with OK button.";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMBox.CreateInfo(imControl, title, message);

            imControl.LoadMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message ok/cancel settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageOkCancelSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Test";
            string message = "This is internal message test, with OK and Cancel buttons.";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMBox.CreateOkCancel(imControl, title, message, PackIconKind.ListStatus);

            imControl.LoadMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message yes/no settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageYesNoSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Test";
            string message = "This is internal message test, with Yes and No buttons.";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMBox.CreateQuestion(imControl, title, message);

            imControl.LoadMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message progress settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageProgressSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            var bgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bgWorker.DoWork += (s, ew) =>
            {
                var im = (IMProgress)ew.Argument;
                var worker = (BackgroundWorker)s;

                for (int progress = 0; progress < im.ProgressMax; progress++)
                {
                    if (worker.CancellationPending || ew.Cancel)
                        break;

                    progress++;
                    worker.ReportProgress(progress, im);
                    Thread.Sleep(250);
                }

                ew.Result = im;
            };

            bgWorker.ProgressChanged += (s, ep) =>
            {
                var im = (IMProgress)ep.UserState;
                string progressMessage = $"This is progress internal message test. [{ep.ProgressPercentage} %]";

                im.UpdateProgress(ep.ProgressPercentage, progressMessage);
            };

            bgWorker.RunWorkerCompleted += (s, ep) =>
            {
                var im = (IMProgress)ep.Result;

                if (!ep.Cancelled)
                    im.Finish(message: "Progress finished.");
            };

            CloseEventHandler<IIMCloseEventArgs> closeEventHandler = (s, ec) =>
            {
                var im = (IMProgress)s;

                if (ec.Result == IMResult.Cancel && bgWorker.IsBusy)
                    bgWorker.CancelAsync();
            };

            string title = "Internal Message Test";
            string message = "This is progress internal message test. [0 %]";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var imProgress = IMProgress.CreateMessage(imControl, title, message, onClose: closeEventHandler);

            imProgress.ProgressMax = 100;
            imProgress.Progress = 0;

            imControl.LoadMessage(imProgress);
            bgWorker.RunWorkerAsync(imProgress);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message await settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageAwaitSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            var bgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bgWorker.DoWork += (s, ew) =>
            {
                var im = (IMAwait)ew.Argument;
                var worker = (BackgroundWorker)s;

                for (int progress = 0; progress < 100; progress++)
                {
                    if (worker.CancellationPending || ew.Cancel)
                        break;

                    progress++;
                    worker.ReportProgress(progress, im);
                    Thread.Sleep(250);
                }

                ew.Result = im;
            };

            bgWorker.ProgressChanged += (s, ep) =>
            {
                var im = (IMAwait)ep.UserState;
                string progressMessage = $"This is progress internal message test. [{ep.ProgressPercentage} %]";

                im.UpdateProgress(progressMessage);
            };

            bgWorker.RunWorkerCompleted += (s, ep) =>
            {
                var im = (IMAwait)ep.Result;

                if (!ep.Cancelled)
                    im.Finish(message: "Progress finished.");
            };

            CloseEventHandler<IIMCloseEventArgs> closeEventHandler = (s, ec) =>
            {
                var im = (IMAwait)s;

                if (ec.Result == IMResult.Cancel && bgWorker.IsBusy)
                    bgWorker.CancelAsync();
            };

            string title = "Internal Message Test";
            string message = "This is progress internal message test. [0 %]";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var imAwait = IMAwait.CreateMessage(imControl, title, message, onClose: closeEventHandler);

            imControl.LoadMessage(imAwait);
            bgWorker.RunWorkerAsync(imAwait);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message color selector settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageColorSelectorSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Color Selector Test";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMColorSelector.CreateMessage(imControl, title);

            imControl.LoadMessage(im);
        }
        
        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message color picker settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageColorPickerSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Color Picker Test";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMColorPicker.CreateMessage(imControl, title);

            imControl.LoadMessage(im);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal message files selector (open files) settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessageFilesSelectorOpenSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            string title = "Internal Message Files Selector Test";

            var imControl = ((App)App.Current).GetIMControlFromWindow();
            var im = IMFilesSelector.CreateOpenFilesMessage(imControl, title);

            imControl.LoadMessage(im);
        }

        #endregion ITEMS INTERACTION METHODS

    }
}
