using chkam05.VisualPlayer.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Lists
{
    public interface IPlayList<T> where T : BaseFile
    {

        //  VARIABLES

        T this[int i] { get; set; }
        int Count { get; }
        List<T> List { get; }


        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add file to playlist. </summary>
        /// <param name="file"> File to add. </param>
        void Add(T file);

        //  --------------------------------------------------------------------------------
        /// <summary> Add range of files to playlist. </summary>
        /// <param name="files"> Enumerable range of files to add. </param>
        void AddRange(IEnumerable<T> files);

        //  --------------------------------------------------------------------------------
        /// <summary> Clear playlist. </summary>
        void Clear();

        //  --------------------------------------------------------------------------------
        /// <summary> Check if playlist contains particular file. </summary>
        /// <param name="file"> File to check if exists in playlist. </param>
        /// <returns> True - file exists in playlist; False - otherwise. </returns>
        bool Contains(T file);

        //  --------------------------------------------------------------------------------
        /// <summary> Get index of specified item from playlist. </summary>
        /// <param name="file"> File located in playlist. </param>
        /// <returns> Index of specified file in playlist. </returns>
        int IndexOf(T file);

        //  --------------------------------------------------------------------------------
        /// <summary> Move file in playlist. </summary>
        /// <param name="itemIndex"> Index of file to move. </param>
        /// <param name="destIndex"> Destination index. </param>
        void MoveItem(int itemIndex, int destIndex);

        //  --------------------------------------------------------------------------------
        /// <summary> Remove file at particular index from playlist. </summary>
        /// <param name="itemIndex"> File index to remove. </param>
        void RemoveAt(int itemIndex);

        //  --------------------------------------------------------------------------------
        /// <summary> Remove particular file from playlist. </summary>
        /// <param name="file"> File to remove. </param>
        void Remove(T file);

    }
}
