using chkam05.Tools.ControlsEx.InternalMessages;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.PlayLists;
using chkam05.VisualPlayer.Utilities.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace chkam05.VisualPlayer.Utilities
{
    public class FilesManager
    {

        //  CONST

        private static readonly string ALL_FILES_FILTER = "All Files (*.*)|*.*";

        public static readonly List<FileConfig> FilesTypes = new List<FileConfig>
        {
            new FileConfig("MP3", ".mp3", FileGroup.MUSIC, FileKind.MP3),
            new FileConfig("VPL", ".vpl", FileGroup.PLAYLIST, FileKind.VPL),
            new FileConfig("XML", ".xml", FileGroup.PLAYLIST, FileKind.XML),
            new FileConfig("VPT", ".vpt", FileGroup.LYRICS, FileKind.VPT),
            new FileConfig("XML", ".xml", FileGroup.LYRICS, FileKind.XML),
            new FileConfig("JSON", ".json", FileGroup.SETTINGS, FileKind.JSON)
        };


        //  VARIABLES

        private static FilesManager _instance;

        public string ConfigurationFilePath { get; private set; }
        public string EqualizerStoragePath { get; private set; }
        public string LyricsStoragePath { get; private set; }
        public string PlayListCache { get; private set; }
        public string StoragePath { get; private set; }
        public string VisualisationsStoragePath { get; private set; }


        //  GETTERS & SETTERS

        public static FilesManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FilesManager();

                return _instance;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FilesManager private class constructor. </summary>
        private FilesManager()
        {
            PrepareLocalStorage();
        }

        #endregion CLASS METHODS

        #region DIALOG OPEN METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load files via open files dialog. </summary>
        /// <param name="filesGroup"> Files group to load. </param>
        /// <param name="title"> Open files dialog title. </param>
        /// <param name="multiSelect"> Files multiselection option. </param>
        /// <returns> List of loaded files paths. </returns>
        public List<string> DialogOpenFiles(FileGroup filesGroup, string title = "Select files to load.", bool multiSelect = true)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "",
                Filter = GenerateFilter(filesGroup, true),
                InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE"),
                Multiselect = multiSelect,
                Title = title
            };

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileNames.ToList();

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get save file path via save file dialog. </summary>
        /// <param name="filesGroup"> Files group available to save. </param>
        /// <param name="title"> Save files dialog title. </param>
        /// <param name="defaultFileName"> Default save file name. </param>
        /// <returns> Path to file where it can be saved. </returns>
        public string DialogGetSaveFilePath(FileGroup filesGroup, string title = "Save to file", string defaultFileName = "")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                DefaultExt = "",
                FileName = defaultFileName,
                Filter = GenerateFilter(filesGroup, false),
                InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE"),
                Title = title
            };

            if (saveFileDialog.ShowDialog() == true)
                return saveFileDialog.FileName;

            return null;
        }

        #endregion DIALOG OPEN METHODS

        #region LISTING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of files from directory. </summary>
        /// <param name="path"> Directory path. </param>
        /// <param name="filter"> List of acceptable files types. </param>
        /// <returns> List of files paths. </returns>
        public static List<string> GetFilesList(string path, IEnumerable<FileConfig> filter = null, bool subDirectories = false)
        {
            var result = new List<string>();

            if (Directory.Exists(path))
            {
                result.AddRange(
                    Directory.GetFiles(path).Where(f 
                        => filter == null 
                        || (filter != null && filter.Any(e => MatchExtensions(f, e.Extension))))
                );

                if (subDirectories)
                {
                    result.AddRange(
                        Directory.GetDirectories(path)
                            .SelectMany(p => GetFilesList(p, filter, subDirectories))
                    );
                }
            }

            return result;
        }

        #endregion LISTING METHODS

        #region LOCAL DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create local storage for application in AppData Roaming directory. </summary>
        private void PrepareLocalStorage()
        {
            var appDataPath = Environment.GetEnvironmentVariable("APPDATA");
            var appName = ApplicationHelper.Instance.GetApplicationName();
            var appPath = Path.GetDirectoryName(ApplicationHelper.Instance.GetApplicationLocation());

            StoragePath = Path.Combine(appDataPath, appName);

            ConfigurationFilePath = Path.Combine(StoragePath, "config.json");
            EqualizerStoragePath = Path.Combine(StoragePath, "Equalizer");
            LyricsStoragePath = Path.Combine(StoragePath, "Lyrics");
            PlayListCache = Path.Combine(StoragePath, "PlaylistCache.vpl");
            VisualisationsStoragePath = Path.Combine(StoragePath, "Visualisations");

            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            if (!Directory.Exists(EqualizerStoragePath))
                Directory.CreateDirectory(EqualizerStoragePath);

            if (!Directory.Exists(LyricsStoragePath))
                Directory.CreateDirectory(LyricsStoragePath);

            if (!Directory.Exists(VisualisationsStoragePath))
                Directory.CreateDirectory(VisualisationsStoragePath);
        }

        #endregion LOCAL DATA MANAGEMENT METHODS

        #region OPEN FILES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Asynchronously load files to playlist. </summary>
        /// <param name="filesPaths"> List of files paths. </param>
        /// <param name="filesGroup"> Files group to load. </param>
        /// <param name="result"> Output playlist where files will be loaded. </param>
        /// <param name="dispatcherHandler"> Dispatcher handler for invoke action outside. </param>
        /// <param name="progressMessage"> Progress message box. </param>
        public void LoadPlayableFilesAsync(List<string> filesPaths, FileGroup filesGroup, IPlayList<IPlayable> result, 
            DispatcherHandler dispatcherHandler, ProgressInternalMessageEx progressMessage = null)
        {
            int filesCount = filesPaths.Count;
            var suitableExtensions = GetSuitableExtensions(filesGroup);

            var bgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };

            //  Loading files work method.
            bgWorker.DoWork += (s, e) =>
            {
                int filesCounter = 0;

                foreach (var filePath in filesPaths)
                    if (suitableExtensions.Contains(Path.GetExtension(filePath).ToLower()))
                    {
                        var file = new SongFile(filePath);
                        filesCounter++;

                        dispatcherHandler.TryInvoke(() => result.Add(file));
                        bgWorker.ReportProgress(filesCounter, file);
                    }
            };

            //  Loading files notify method.
            bgWorker.ProgressChanged += (s, e) =>
            {
                if (progressMessage != null)
                    dispatcherHandler.TryInvoke(() =>
                    {
                        progressMessage.Message = (e.UserState as IFile).FileName;
                        progressMessage.Progress = (filesCount > 0)
                            ? e.ProgressPercentage * 100 / filesCount
                            : 0;
                    });
            };

            //  Loading files finished method.
            bgWorker.RunWorkerCompleted += (s, e) =>
            {
                if (progressMessage != null)
                    dispatcherHandler.TryInvoke(() => progressMessage.Close());
            };

            bgWorker.RunWorkerAsync();
        }

        #endregion OPEN FILES METHODS

        #region STATIC FILE TYPE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get file configuration by file extension. </summary>
        /// <param name="extension"> File extension. </param>
        /// <returns> File configuration with specified extension. </returns>
        public static FileConfig GetFileTypeByExtension(string extension)
        {
            return FilesTypes.FirstOrDefault(t => t.Extension == extension.ToLower());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get file configuration by file kind. </summary>
        /// <param name="fileKind"> File kind. </param>
        /// <returns> File configuration with specified file kind. </returns>
        public static FileConfig GetFileTypeByKind(FileKind fileKind)
        {
            return FilesTypes.FirstOrDefault(t => t.Kind == fileKind);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get files configurations by files group. </summary>
        /// <param name="filesGroup"> Files group. </param>
        /// <returns> Files configurations with specified files group. </returns>
        public static IEnumerable<FileConfig> GetFileTypesByGroup(FileGroup filesGroup)
        {
            return FilesTypes.Where(t => t.Group == filesGroup);
        }

        #endregion STATIC FILE TYPE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate filter for open files dialog. </summary>
        /// <param name="filesGroup"> Files group. </param>
        /// <param name="allowAllFiles"> Allow all files in filter. </param>
        /// <returns> Filter for open files dialog. </returns>
        private string GenerateFilter(FileGroup filesGroup, bool allowAllFiles = false)
        {
            var sb = new StringBuilder();
            var typesFiltered = GetFileTypesByGroup(filesGroup);

            if (typesFiltered != null && typesFiltered.Any())
                foreach (var fileType in typesFiltered)
                    sb.Append($"{fileType.Name} (*{fileType.Extension})|*{fileType.Extension}|");

            if (allowAllFiles)
                sb.Append($"{ALL_FILES_FILTER}");

            var result = sb.ToString();

            return !string.IsNullOrEmpty(result) && result.Last() == '|' 
                ? result.Substring(0, result.Length - 1) : result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get list of suitable files extensions for files group. </summary>
        /// <param name="filesGroup"> Files group. </param>
        /// <returns> List of suitable files extensions in files group. </returns>
        private List<string> GetSuitableExtensions(FileGroup filesGroup)
        {
            var result = GetFileTypesByGroup(filesGroup)
                ?.Select(t => t.Extension);

            return result != null && result.Any() ? result.ToList() : new List<string>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file has specified extension. </summary>
        /// <param name="filePath"> Path to file to check. </param>
        /// <param name="extension"> Extension. </param>
        /// <returns> True - file has specified extension; False - otherwise. </returns>
        private static bool MatchExtensions(string filePath, string extension)
        {
            return Path.GetExtension(filePath).ToLower() == extension.ToLower();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file match required file type. </summary>
        /// <param name="filePath"> Path to file to check. </param>
        /// <param name="fileTypeInfo"> File configuration and type informations. </param>
        /// <returns> True - file is match requirements; False - otherwise. </returns>
        public static bool MatchFileType(string filePath, FileConfig fileTypeInfo)
        {
            return Path.GetExtension(filePath).ToLower() == fileTypeInfo?.Extension;
        }

        #endregion UTILITY METHODS

    }
}
