using chkam05.VisualPlayer.Data.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.PlayLists
{
    public interface IPlayList<T> where T : IPlayable
    {

        //  GETTERS & SETTERS

        ObservableCollection<T> DataContext { get; set; }
        IPlayable this[int i] { get; set; }
        int Count { get; }
        T Selected { get; }
        int SelectedIndex { get; }


        //  METHODS

        #region LIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add item to the end of the playlist. </summary>
        /// <param name="item"> Item to add to the playlist. </param>
        void Add(T item);

        //  --------------------------------------------------------------------------------
        /// <summary> Remove all items from the playlist. </summary>
        void Clear();

        //  --------------------------------------------------------------------------------
        /// <summary> Check if item exists in playlist. </summary>
        /// <param name="item"> Item to check if exists. </param>
        /// <returns> True - item exists in playlist; False - otherwise. </returns>
        bool Contains(IPlayable item);

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of specified item in playlist. </summary>
        /// <param name="item"> Item. </param>
        /// <returns> Index of specified item or -1. </returns>
        int IndexOf(T item);

        //  --------------------------------------------------------------------------------
        /// <summary> Inserts item into the playlist at the specified index. </summary>
        /// <param name="index"> Index at which item should be inserted. </param>
        /// <param name="item"> Item to insert to the playlist. </param>
        void Insert(int index, T item);

        //  --------------------------------------------------------------------------------
        /// <summary> Get item with specified index from the playlist. </summary>
        /// <param name="index"> Item index. </param>
        T GetItem(int index);

        //  --------------------------------------------------------------------------------
        /// <summary> Check if playlist contains any item. </summary>
        /// <returns> True - contains any item; False - otherwise. </returns>
        bool HasItems();

        //  --------------------------------------------------------------------------------
        /// <summary> Remove specified item from playlist. </summary>
        /// <param name="item"> Item to remove form playlist. </param>
        void Remove(T item);

        //  --------------------------------------------------------------------------------
        /// <summary> Remove item from playlist with specified index. </summary>
        /// <param name="index"> Item index to remove from playlist. </param>
        void RemoveAt(int index);

        #endregion LIST METHODS

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set an item as selected. </summary>
        /// <param name="item"> Item to set as select. </param>
        /// <returns> True - item selected; False - otherwise. </returns>
        bool Select(T item);

        //  --------------------------------------------------------------------------------
        /// <summary> Set an item with specified index as selected. </summary>
        /// <param name="index"> Index of specified item to set as selected. </param>
        /// <returns> Newly selected item or null. </returns>
        T Select(int index);

        //  --------------------------------------------------------------------------------
        /// <summary> Set next item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        T SelectNext();

        //  --------------------------------------------------------------------------------
        /// <summary> Set next random item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        T RandomNext();

        //  --------------------------------------------------------------------------------
        /// <summary> Set previous item to selected item as selected. </summary>
        /// <returns> Newly selected item or null. </returns>
        T SelectPrevious();

        //  --------------------------------------------------------------------------------
        /// <summary> Set previous random item to selected item as selected. </summary>
        /// <returns> Newly selected random item or null. </returns>
        T RandomPrevious();

        //  --------------------------------------------------------------------------------
        /// <summary> Set currently selected item as not selected. </summary>
        void Unselect();

        #endregion PLAYLIST METHODS

    }
}
