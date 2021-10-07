using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lists;
using chkam05.VisualPlayer.Utilities.Files.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Files
{
    public class AsyncFileLoader<T> : IDisposable where T : BaseFile
    {

        //  EVENTS

        public event EventHandler<FilesLoaderOnLoadEventArgs> OnFileLoaded;
        public event EventHandler<FilesLoaderOnCompleteEventArgs> OnWorkComplete;


        //  VARIABLES

        private List<string> _errorFilesPaths;
        private List<string> _loadedFilesPaths;
        private BackgroundWorker _loader;
        private int _loadingBreak = 100;

        public int LoadingBreakInMiliseconds
        {
            get => _loadingBreak;
            set => _loadingBreak = Math.Max(10, Math.Min(value, 1000));
        }


        #region GETTERS & SETTERS

        public bool IsWorking
        {
            get => _loader != null && _loader.IsBusy;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> AsyncFileLoader class constructor. </summary>
        public AsyncFileLoader()
        {
            //  Initialize data containers.
            _errorFilesPaths = new List<string>();
            _loadedFilesPaths = new List<string>();

            //  Setup background worker.
            _loader = new BackgroundWorker();
            _loader.DoWork += LoadingDoWork;
            _loader.ProgressChanged += LoadingProgressChanges;
            _loader.RunWorkerCompleted += LoadingWorkComplete;
            _loader.WorkerReportsProgress = true;
            _loader.WorkerSupportsCancellation = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked when instance is destroying. </summary>
        public void Dispose()
        {
            //  Stop loading operation if is on.
            Stop();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load files async into playlist. </summary>
        /// <param name="destPlayList"> Destination playlist. </param>
        /// <param name="filesPaths"> List of files paths. </param>
        /// <param name="loadMetadata"> Auto load file metadata at creation. </param>
        /// <returns> True - loading started; False - otherwise. </returns>
        public bool LoadFilesAsync(IPlayList<T> destPlayList, string[] filesPaths, bool loadMetadata = true)
        {
            //  Check if argumets are correct.
            if (destPlayList == null || filesPaths == null)
                return false;

            //  Start loading files.
            _loader.RunWorkerAsync(new List<object> { destPlayList, filesPaths, loadMetadata });

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Stop loading operation. </summary>
        public void Stop()
        {
            if (_loader != null && _loader.IsBusy)
                _loader.CancelAsync();
        }

        #endregion INTERACTION METHODS

        #region LOADING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Try to create file object and load into destination playlist. </summary>
        /// <param name="filePath"> Path of file to load. </param>
        /// <param name="fileIndex"> Index of file to load. </param>
        /// <param name="destPlayList"> Destination playlist. </param>
        /// <param name="loadMetadata"> Auto load file metadata at creation. </param>
        /// <returns> Data transfer object with results. </returns>
        private FilesLoaderDataRelay TryCreateAndLoadFile(string filePath, int fileIndex, 
            IPlayList<T> destPlayList, bool loadMetadata)
        {
            var result = new FilesLoaderDataRelay()
            {
                FilePath = filePath,
                FileIndex = fileIndex
            };

            try
            {
                //  Create file object.
                var file = (T)Activator.CreateInstance(typeof(T), 
                    new object[] { filePath, loadMetadata });

                //  Load created file object.
                destPlayList.Add(file);
                _loadedFilesPaths.Add(filePath);
            }
            catch (Exception exc)
            {
                //  Report error.
                result.Exception = exc;
                _errorFilesPaths.Add(filePath);
            }

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after loading files. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Run worker completed event arguments. </param>
        private void LoadingWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            //  Invoke outside on loading complete event.
            OnWorkComplete?.Invoke(this, 
                new FilesLoaderOnCompleteEventArgs(_errorFilesPaths, _loadedFilesPaths));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method that load files async. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void LoadingDoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = (List<object>) e.Argument;
            var destPlayList = (IPlayList<T>) arguments[0];
            var filesPaths = (string[]) arguments[1];
            var loadMetadata = (bool) arguments[2];

            int filesCount = filesPaths.Count();
            int fileIndex = 0;
            int progress = 0;

            while (fileIndex < filesCount)
            {
                string filePath = filesPaths[fileIndex];

                //  Exit method if loader work is cancelled.
                if (_loader.CancellationPending)
                    return;

                //  Create data transfer object.
                var dataRelay = TryCreateAndLoadFile(filePath, fileIndex, destPlayList, loadMetadata);

                //  Update file index and progress.
                fileIndex++;
                progress = Math.Min((fileIndex * 100) / filesCount, 100);

                //  Notify progress change.
                _loader.ReportProgress(progress, dataRelay);

                //  Take a break.
                Thread.Sleep(_loadingBreak);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after loading single file. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Progress change event arguments. </param>
        private void LoadingProgressChanges(object sender, ProgressChangedEventArgs e)
        {
            //  Get data transfer object.
            var dataRelay = (FilesLoaderDataRelay)e.UserState;

            //  Invoke outside on load file event.
            OnFileLoaded?.Invoke(this, new FilesLoaderOnLoadEventArgs(
                dataRelay.FilePath,
                dataRelay.FileIndex,
                e.ProgressPercentage,
                dataRelay.IsError,
                dataRelay.ErrorMessage));
        }

        #endregion LOADING METHODS

    }
}
