using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Serializers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;


namespace chkam05.VisualPlayer.Data.PlayLists
{
    public class PlayList : IPlayList<IPlayable>, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ObservableCollection<IPlayable> _playList;
        private Permutation _permutation;


        //  GETTERS & SETTERS

        public ObservableCollection<IPlayable> DataContext
        {
            get => _playList;
            set
            {
                _playList = value;
                OnPropertyChanged(nameof(DataContext));
            }
        }

        public IPlayable this[int i]
        {
            get => i >= 0 && i < Count ? DataContext[i] : null;
            set
            {
                if (i >= 0 && i < Count)
                    DataContext[i] = value;
            }
        }

        public int Count
        {
            get => DataContext.Count;
        }

        public Type[] ItemTypes
        {
            get
            {
                return DataContext.Select(s => s.GetType()).Distinct().ToArray();
            }
        }

        public IPlayable Selected
        {
            get => DataContext.FirstOrDefault(i => i.NowPlaying) ?? null;
        }

        public int SelectedIndex
        {
            get
            {
                var selected = Selected;
                return selected != null ? DataContext.IndexOf(selected) : -1;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayList class constructor. </summary>
        public PlayList()
        {
            //  Setup data containers.
            _playList = new ObservableCollection<IPlayable>();

            //  Setup initial data.
            _permutation = new Permutation();
        }

        #endregion CLASS METHODS

        #region FILES MANAGEMENT

        //  --------------------------------------------------------------------------------
        /// <summary> Load playlist from file. </summary>
        /// <param name="filePath"> Playlist file path. </param>
        public void LoadFromFile(string filePath)
        {
            using (var serializer = new PlayListSerializer())
            {
                serializer.LoadFromFile(filePath);
                var loadedItems = serializer.Deserialize();

                if (loadedItems != null && loadedItems.Any())
                {
                    Clear();
                    loadedItems.ForEach(item =>
                    {
                        if (item.Exists)
                        {
                            item.LoadMetadata();
                            Add(item);
                        }
                    });
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Save playlist to file. </summary>
        /// <param name="filePath"> Path to file where playlist will be saved. </param>
        public void SaveToFile(string filePath)
        {
            using (var serializer = new PlayListSerializer())
            {
                serializer.Serialize(DataContext);
                serializer.SaveToFile(filePath);
            }
        }

        #endregion FILES MANAGEMENT

        #region LIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add item to the end of the playlist. </summary>
        /// <param name="item"> Item to add to the playlist. </param>
        public void Add(IPlayable item)
        {
            DataContext.Add(item);
            _permutation.IncreaseCollection();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all items from the playlist. </summary>
        public void Clear()
        {
            DataContext.Clear();
            _permutation.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if item exists in playlist. </summary>
        /// <param name="item"> Item to check if exists. </param>
        /// <returns> True - item exists in playlist; False - otherwise. </returns>
        public bool Contains(IPlayable item)
        {
            return DataContext.Contains(item);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of specified item in playlist. </summary>
        /// <param name="item"> Item. </param>
        /// <returns> Index of specified item or -1. </returns>
        public int IndexOf(IPlayable item)
        {
            if (DataContext.Contains(item))
                return DataContext.IndexOf(item);

            return -1;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Inserts item into the playlist at the specified index. </summary>
        /// <param name="index"> Index at which item should be inserted. </param>
        /// <param name="item"> Item to insert to the playlist. </param>
        public void Insert(int index, IPlayable item)
        {
            DataContext.Insert(GetCorrectIndex(index), item);
            _permutation.IncreaseCollection();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get item with specified index from the playlist. </summary>
        /// <param name="index"> Item index. </param>
        public IPlayable GetItem(int index)
        {
            if (index >= 0 && index < Count)
                return DataContext[GetCorrectIndex(index)];

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if playlist contains any item. </summary>
        /// <returns> True - contains any item; False - otherwise. </returns>
        public bool HasItems()
        {
            return Count > 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove specified item from playlist. </summary>
        /// <param name="item"> Item to remove form playlist. </param>
        public void Remove(IPlayable item)
        {
            if (DataContext.Contains(item))
            {
                DataContext.Remove(item);
                _permutation.DecreaseCollection();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove item from playlist with specified index. </summary>
        /// <param name="index"> Item index to remove from playlist. </param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
            {
                DataContext.RemoveAt(index);
                _permutation.DecreaseCollection();
            }
        }

        #endregion LIST METHODS

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

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set an item as selected. </summary>
        /// <param name="item"> Item to set as select. </param>
        /// <returns> True - item selected; False - otherwise. </returns>
        public bool Select(IPlayable item)
        {
            if (DataContext.Contains(item))
            {
                Unselect();
                item.NowPlaying = true;
                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set an item with specified index as selected. </summary>
        /// <param name="index"> Index of specified item to set as selected. </param>
        /// <returns> Newly selected item or null. </returns>
        public IPlayable Select(int index)
        {
            if (index >= 0 && index < Count)
            {
                Unselect();
                DataContext[index].NowPlaying = true;
                return DataContext[index];
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set next item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        public IPlayable SelectNext()
        {
            return Select(SelectedIndex + 1);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set next random item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        public IPlayable RandomNext()
        {
            return Select(_permutation.GetNextIndex(SelectedIndex));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set previous item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        public IPlayable SelectPrevious()
        {
            return Select(SelectedIndex - 1);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set previous random item to selected item as selected. </summary>
        /// <returns> Newly selected random item or null. </returns>
        public IPlayable RandomPrevious()
        {
            return Select(_permutation.GetPreviousIndex(SelectedIndex));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set currently selected item as not selected. </summary>
        public void Unselect()
        {
            if (Selected != null)
                Selected.NowPlaying = false;
        }

        #endregion PLAYLIST METHODS

        #region SHUFFLE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate random sequence of items. </summary>
        public void Shuffle()
        {
            _permutation.Generate(Count);
        }

        #endregion SHUFFLE METHODS

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
