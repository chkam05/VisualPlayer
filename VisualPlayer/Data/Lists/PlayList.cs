using chkam05.VisualPlayer.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Lists
{
    public class PlayList<T> : IPlayList<T> where T : BaseFile
    {

        //  VARIABLES

        private List<T> _dataContainer;


        #region GETTERS & SETTERS

        public T this[int i]
        {
            get => (i >= 0 && i < Count) ? _dataContainer[i] : null;
            set
            {
                if (i >= 0 && i < Count)
                    _dataContainer[i] = value;
            }
        }

        public int Count
        {
            get => _dataContainer.Count;
        }

        public List<T> List
        {
            get => _dataContainer.ToList();
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> PlayList class constructor. </summary>
        public PlayList()
        {
            //  Setup data container.
            _dataContainer = new List<T>();
        }

        #endregion CLASS METHODS

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add file to playlist. </summary>
        /// <param name="file"> File to add. </param>
        public void Add(T file)
        {
            _dataContainer.Add(file);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add range of files to playlist. </summary>
        /// <param name="files"> Enumerable range of files to add. </param>
        public void AddRange(IEnumerable<T> files)
        {
            foreach (var file in files)
                Add(file);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear playlist. </summary>
        public void Clear()
        {
            _dataContainer.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if playlist contains particular file. </summary>
        /// <param name="file"> File to check if exists in playlist. </param>
        /// <returns> True - file exists in playlist; False - otherwise. </returns>
        public bool Contains(T file)
        {
            return _dataContainer.Contains(file);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of specified item from playlist. </summary>
        /// <param name="file"> File located in playlist. </param>
        /// <returns> Index of specified file in playlist. </returns>
        public int IndexOf(T file)
        {
            return _dataContainer.Contains(file) ? _dataContainer.IndexOf(file) : -1;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Move file in playlist. </summary>
        /// <param name="itemIndex"> Index of file to move. </param>
        /// <param name="destIndex"> Destination index. </param>
        public void MoveItem(int itemIndex, int destIndex)
        {
            var moveItem = _dataContainer[itemIndex];

            if (itemIndex < destIndex)
            {
                _dataContainer.Insert(destIndex + 1, moveItem);
                _dataContainer.RemoveAt(itemIndex);
            }
            else
            {
                _dataContainer.Insert(destIndex, moveItem);
                _dataContainer.RemoveAt(itemIndex + 1);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove file at particular index from playlist. </summary>
        /// <param name="itemIndex"> File index to remove. </param>
        public void RemoveAt(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < _dataContainer.Count)
                _dataContainer.RemoveAt(itemIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove particular file from playlist. </summary>
        /// <param name="file"> File to remove. </param>
        public void Remove(T file)
        {
            if (_dataContainer.Contains(file))
                _dataContainer.Remove(file);
        }

        #endregion PLAYLIST METHODS

    }
}
