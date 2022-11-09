using chkam05.VisualPlayer.Controls;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Config.Events;
using chkam05.VisualPlayer.Data.Configuration;
using chkam05.VisualPlayer.Data.Configuration.Events;
using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.Lyrics.Events;
using chkam05.VisualPlayer.Serializers;
using chkam05.VisualPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace chkam05.VisualPlayer.Data.Lyrics
{
    public class LyricsManager : INotifyPropertyChanged
    {

        //  CONST

        public readonly Dictionary<string, string> ConfigurationMapping = new Dictionary<string, string>
        {
            { nameof(Config.Configuration.AutoLoadLyrics), nameof(AutoLoad) },
            { nameof(Config.Configuration.LyricsMatchType), nameof(MatchType) }
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<LyricsItemUpdateEventArgs> OnItemChange;


        //  VARIABLES

        private static LyricsManager _instance;

        private bool _autoLoad = false;
        private Lyrics _currentLyrics = null;
        private bool _editMode = false;
        private IFile _lodedFile;
        private ObservableCollection<Lyrics> _lyrics;
        private bool _lyricsChanged = false;
        private LyricsMatchingType _matchType;
        private TimeSpan _tolerance = TimeSpan.FromMilliseconds(50);

        public ILyricsController Controller { get; set; }


        //  GETTERS & SETTERS

        public static LyricsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LyricsManager();

                return _instance;
            }
        }

        public bool AutoLoad
        {
            get => _autoLoad;
            set
            {
                _autoLoad = value;
                OnPropertyChanged(nameof(AutoLoad));
            }
        }

        public int Count
        {
            get => DataContext.Count;
        }

        public ObservableCollection<Lyrics> DataContext
        {
            get => _lyrics;
            set
            {
                _lyrics = value;
                _lyrics.CollectionChanged += ContentCollectionChanged;
                OnPropertyChanged(nameof(DataContext));
            }
        }

        public bool EditMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
            }
        }

        public bool HasLyrics
        {
            get => DataContext != null && DataContext.Any();
        }

        public IFile LoadedFile
        {
            get => _lodedFile;
            set
            {
                _lodedFile = value;
                OnPropertyChanged(nameof(LoadedFile));
            }
        }
        
        public bool LyricsChanged
        {
            get => _lyricsChanged;
            set
            {
                _lyricsChanged = value;
                OnPropertyChanged(nameof(LyricsChanged));
            }
        }

        public LyricsMatchingType MatchType
        {
            get => _matchType;
            set
            {
                _matchType = value;
                OnPropertyChanged(nameof(MatchType));
            }
        }

        public PropertyInfo[] Properties
        {
            get => ObjectUtilities.GetObjectProperties(this.GetType());
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LyricsContainer private class constructor. </summary>
        private LyricsManager()
        {
            DataContext = new ObservableCollection<Lyrics>();
            LyricsChanged = false;
        }

        #endregion CLASS METHODS

        #region FILES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Create empty lyrics container. </summary>
        public void CreateEmpty()
        {
            DataContext.Clear();
            LyricsChanged = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if lyrics exists in library. </summary>
        /// <returns> True - lyrics exists in library; False - otherwise. </returns>
        public bool ExistsInLibrary()
        {
            if (LoadedFile != null)
            {
                var filter = FilesManager.GetFileTypesByGroup(FileGroup.LYRICS);
                var lyricsStoragePath = FilesManager.Instance.LyricsStoragePath;

                //  Get list of lyrics files matched by similar names to related file.
                var lyricsFilesPaths = FilesManager.GetFilesList(lyricsStoragePath, filter, false);
                var limitedLyricsFilesPaths = MatchLyricsFile(LoadedFile.FilePath, lyricsFilesPaths);

                //  Select correct lyrics file by matching them to related file.
                foreach (var lyricsFilePath in limitedLyricsFilesPaths)
                {
                    try
                    {
                        using (var serializer = new LyricsSerializer())
                        {
                            serializer.LoadFromFile(lyricsFilePath);

                            //  Match lyrics and related files.
                            if (MatchLyricsFile(serializer, LoadedFile))
                                return true;
                        }
                    }
                    catch (Exception)
                    {
                        //  Problem with file or serialization.
                    }
                }
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load lyrics from file. </summary>
        /// <param name="filePath"> Lyrics file path. </param>
        public bool LoadFromFile(string filePath)
        {
            if (File.Exists(filePath) && LoadedFile != null)
            {
                try
                {
                    using (var serializer = new LyricsSerializer())
                    {
                        serializer.LoadFromFile(filePath);
                        var lyrics = serializer.Deserialize();

                        if (lyrics != null)
                        {
                            DataContext.Clear();
                            lyrics.OrderBy(o => o.StartTime).ToList().ForEach(l => DataContext.Add(l));
                        }
                    }

                    LyricsChanged = false;
                    return true;
                }
                catch (Exception)
                {
                    //  Problem with file or serialization.
                }
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load lyrics from library. </summary>
        /// <returns> True - lyrics loaded from library; False - otherwise. </returns>
        public bool LoadFromLibrary()
        {
            if (LoadedFile != null)
            {
                var filter = FilesManager.GetFileTypesByGroup(FileGroup.LYRICS);
                var lyricsStoragePath = FilesManager.Instance.LyricsStoragePath;

                //  Get list of lyrics files matched by similar names to related file.
                var lyricsFilesPaths = FilesManager.GetFilesList(lyricsStoragePath, filter, false);
                var limitedLyricsFilesPaths = MatchLyricsFile(LoadedFile.FilePath, lyricsFilesPaths);

                //  Select correct lyrics file by matching them to related file.
                foreach (var lyricsFilePath in limitedLyricsFilesPaths)
                {
                    try
                    {
                        using (var serializer = new LyricsSerializer())
                        {
                            serializer.LoadFromFile(lyricsFilePath);

                            //  Match lyrics and related files.
                            if (MatchLyricsFile(serializer, LoadedFile))
                            {
                                var lyrics = serializer.Deserialize();

                                if (lyrics != null)
                                {
                                    DataContext.Clear();
                                    lyrics.OrderBy(o => o.StartTime).ToList().ForEach(l => DataContext.Add(l));
                                }

                                LyricsChanged = false;
                                return true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //  Problem with file or serialization.
                    }
                }
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save lyrics to file. </summary>
        /// <param name="filePath"> Path to file where playlist will be saved. </param>
        public void SaveToFile(string filePath)
        {
            if (LoadedFile != null)
            {
                using (var serializer = new LyricsSerializer(LoadedFile))
                {
                    serializer.Serialize(DataContext);
                    serializer.SaveToFile(filePath);

                    LyricsChanged = false;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save lyrics to library. </summary>
        public void SaveToLibrary()
        {
            if (LoadedFile != null)
            {
                var ext = FilesManager.GetFileTypeByKind(FileKind.VPT).Extension;
                var lyricsStoragePath = FilesManager.Instance.LyricsStoragePath;
                var lyricsFilePath = Path.Combine(lyricsStoragePath, LoadedFile.FileName + ext);

                using (var serializer = new LyricsSerializer(LoadedFile))
                {
                    serializer.Serialize(DataContext);
                    serializer.SaveToFile(lyricsFilePath);
                }

                LyricsChanged = false;
            }
        }

        #endregion FILES MANAGEMENT

        #region LYRICS MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Add lyrics to the end of the lyrics collection. </summary>
        /// <param name="lyrics"> Lyrics to add to the lyrics collection. </param>
        public void Add(Lyrics lyrics)
        {
            DataContext.Add(lyrics);
            LyricsChanged = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Arrange lyrics by their start time. </summary>
        public void ArrangeLyricsByTime()
        {
            DataContext = new ObservableCollection<Lyrics>(DataContext.OrderBy(l => l.StartTime));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all lyrics from the lyrics collection. </summary>
        public void Clear()
        {
            DataContext.Clear();
            LyricsChanged = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if lyrics exists in lyrics collection. </summary>
        /// <param name="lyrics"> Lyrics to check if exists. </param>
        /// <returns> True - lyrics exists in lyrics collection; False - otherwise. </returns>
        public bool Contains(Lyrics lyrics)
        {
            return DataContext.Contains(lyrics);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get lyrics with specified index from the lyrics collection. </summary>
        /// <param name="index"> Lyrics index. </param>
        public Lyrics GetLyrics(int index)
        {
            if (index >= 0 && index < Count)
                return DataContext[GetCorrectIndex(index)];

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get next lyrics from lyrics collection after specified lyrics. </summary>
        /// <param name="lyrics"> Lyrics before next. </param>
        /// <returns> Next lyrics after lyrics in lyrics collection. </returns>
        public Lyrics GetNextLyrics(Lyrics lyrics)
        {
            if (Contains(lyrics))
            {
                int itemIndex = IndexOf(lyrics);

                if (itemIndex < Count - 1)
                    return GetLyrics(itemIndex + 1);
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Show next lyrics in lyrics text controller. </summary>
        /// <param name="playingFile"> Currently playing file. </param>
        /// <param name="playbackTime"> Current playback time. </param>
        /// <param name="dispatcher"> Dispatcher to invoke action outside. </param>
        public void GetNextLyrics(IFile playingFile, TimeSpan playbackTime, Dispatcher dispatcher)
        {
            bool lyricsChanged = false;

            //  Check if controller is assigned.
            if (Controller != null)
            {
                //  Check if any lyrics are loaded.
                if (HasLyrics && playingFile == LoadedFile)
                {
                    var lyrics = DataContext.FirstOrDefault(l
                        => playbackTime >= (l.StartTime - _tolerance)
                        && playbackTime < l.EndTime);

                    //  Update with new lyrics.
                    if (_currentLyrics != lyrics)
                    {
                        _currentLyrics = lyrics;
                        lyricsChanged = true;
                    }
                }
                else
                {
                    //  Clear lyrics if nothing is loaded.
                    if (_currentLyrics != null)
                    {
                        _currentLyrics = null;
                        lyricsChanged = true;
                    }
                }

                //  Update lyrics in controller.
                if (lyricsChanged)
                {
                    dispatcher?.Invoke(() =>
                    {
                        try
                        {
                            Controller.Text = _currentLyrics?.Text;
                        }
                        catch (Exception)
                        {
                            //
                        }
                    });
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get previous lyrics from lyrics collection before specified lyrics. </summary>
        /// <param name="lyrics"> Lyrics after previous. </param>
        /// <returns> Previous lyrics before lyrics in lyrics collection. </returns>
        public Lyrics GetPreviousLyrics(Lyrics lyrics)
        {
            if (Contains(lyrics))
            {
                int itemIndex = IndexOf(lyrics);

                if (itemIndex > 0)
                    return GetLyrics(itemIndex - 1);
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of specified lyrics in lyrics collection. </summary>
        /// <param name="lyrics"> Lyrics. </param>
        /// <returns> Index of specified lyrics or -1. </returns>
        public int IndexOf(Lyrics lyrics)
        {
            if (DataContext.Contains(lyrics))
                return DataContext.IndexOf(lyrics);

            return -1;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Inserts lyrics into the lyrics collection at the specified index. </summary>
        /// <param name="index"> Index at which lyrics should be inserted. </param>
        /// <param name="lyrics"> Lyrics to insert to the lyrics collection. </param>
        public void Insert(int index, Lyrics lyrics)
        {
            DataContext.Insert(GetCorrectIndex(index), lyrics);
            LyricsChanged = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove specified lyrics from lyrics collection. </summary>
        /// <param name="lyrics"> Lyrics to remove form lyrics collection. </param>
        public void Remove(Lyrics lyrics)
        {
            if (DataContext.Contains(lyrics))
            {
                DataContext.Remove(lyrics);
                LyricsChanged = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove lyrics from lyrics collection with specified index. </summary>
        /// <param name="index"> Lyrics index to remove from lyrics collection. </param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                DataContext.RemoveAt(index);
                LyricsChanged = true;
            }
        }

        #endregion LYRICS MANAGEMENT

        #region MATCH LYRICS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if lyrics matching the file. </summary>
        /// <param name="serializer"> Loading lyrics serializer. </param>
        /// <param name="correspondingFile"> File to match with lyrics. </param>
        /// <returns> True - lyrics match the file; False - otherwise. </returns>
        private bool MatchLyricsFile(LyricsSerializer serializer, IFile correspondingFile)
        {
            switch (MatchType)
            {
                case LyricsMatchingType.FILE_CONTENT:
                    return serializer.IsCorrectFileId(correspondingFile);

                case LyricsMatchingType.FILE_METADATA:
                    return serializer.IsCorrectFileName(correspondingFile);

                case LyricsMatchingType.FILE_NAME:
                    return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file math any of lyrics files outwardly. </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="lyricsFiles"> List of lyrics files paths. </param>
        /// <returns> List of matched files. </returns>
        private List<string> MatchLyricsFile(string filePath, List<string> lyricsFilesPaths)
        {
            string lFileName = StringUtilities.LimitToAlphaNumeric(
                Path.GetFileNameWithoutExtension(filePath).ToLower());

            return lyricsFilesPaths.Where(l => StringUtilities.LimitToAlphaNumeric(
                Path.GetFileNameWithoutExtension(l).ToLower()) == lFileName).ToList();
        }

        #endregion MATCH LYRICS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing lyrics collection. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Lyrics item in e.OldItems)
                    item.PropertyChanged -= ItemUpdate;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Lyrics item in e.NewItems)
                    item.PropertyChanged += ItemUpdate;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after chaning any value in lyrics in lyrics collection. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Property Changed Event Arguments. </param>
        private void ItemUpdate(object sender, PropertyChangedEventArgs e)
        {
            var lyrics = sender as Lyrics;

            if (lyrics != null)
            {
                if (e.PropertyName == nameof(Lyrics.StartTime))
                {
                    //  Correct end time.
                    if (lyrics.StartTime > lyrics.EndTime)
                        lyrics.EndTime = lyrics.StartTime;
                }
                else if (e.PropertyName == nameof(Lyrics.EndTime))
                {
                    //  Correct start time.
                    if (lyrics.StartTime > lyrics.EndTime)
                        lyrics.StartTime = lyrics.EndTime;
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set visualisation property. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <param name="value"> Value to set. </param>
        /// <returns> True - value set; False - otherwise. </returns>
        public bool SetProperty(string propertyName, object value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property != null && property.PropertyType == value.GetType())
            {
                property.SetValue(this, value);
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update lyrics configuration from settings. </summary>
        /// <param name="config"> Configuration with lyrics settings. </param>
        public void UpdateConfiguration(ConfigManager configManager)
        {
            if (configManager != null)
            {
                AutoLoad = configManager.LyricsAutoLoad;
                MatchType = configManager.LyricsMatchingType;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update single visualisation configuration from settings. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Configuration Update Event Arguments. </param>
        public void UpdateConfiguration(object sender, ConfigUpdateEventArgs e)
        {
            var configManager = (ConfigManager)sender;
            UpdateConfiguration(configManager);
        }

        #endregion UPDATE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Correct item index. </summary>
        /// <param name="index"> Requested item index in list. </param>
        /// <returns> Requested index or near available. </returns>
        private int GetCorrectIndex(int index)
        {
            return Math.Max(0, Math.Min(index, Count - 1));
        }

        #endregion UTILITY METHODS

    }
}
