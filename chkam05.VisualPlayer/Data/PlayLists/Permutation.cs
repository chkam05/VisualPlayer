using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.PlayLists
{
    public class Permutation
    {

        //  VARIABLES

        private Dictionary<int, int> _permutation;


        //  GETTERS & SETTERS

        public int Size
        {
            get => _permutation.Count;
        }

        public int this[int value]
        {
            get => _permutation.ContainsKey(value) ? _permutation[value] : -1;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Permutation class constructor. </summary>
        public Permutation()
        {
            _permutation = new Dictionary<int, int>();
        }

        #endregion CLASS METHODS

        #region GENERATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Generate set of Permutation with certain size. </summary>
        /// <param name="size"> Size of Permutation. </param>
        public void Generate(int size = 0)
        {
            _permutation = new Dictionary<int, int>();

            if (size > 1)
            {
                var start = 0;
                var next = 0;
                var items = Enumerable.Range(1, size - 1).ToList();
                var random = new Random();

                for (int i = 0; i < size; i++)
                {
                    if (items.Any())
                        next = items[random.Next(0, items.Count())];
                    else
                        next = 0;

                    _permutation[start] = next;
                    start = next;

                    items.Remove(next);
                }
            }
            else if (size > 0)
            {
                _permutation[0] = 0;
            }
        }

        #endregion GENERATION METHODS

        #region ITEMS MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Increase the set of Permutation by one. </summary>
        public void IncreaseCollection()
        {
            if (!_permutation.Any())
            {
                _permutation[0] = 0;
                return;
            }

            var random = new Random();
            var randomIndex = random.Next(0, Size);

            _permutation[Size] = _permutation[randomIndex];
            _permutation[randomIndex] = Size;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear set of Permutation. </summary>
        public void Clear()
        {
            _permutation = new Dictionary<int, int>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Decrease the set of Permutation by one. </summary>
        public void DecreaseCollection()
        {
            if (_permutation.Any())
            {
                var lastIndex = Size - 1;
                var previousIndex = TryGetIndexOfValue(lastIndex);

                if (previousIndex.HasValue)
                {
                    _permutation[previousIndex.Value] = _permutation[lastIndex];
                    _permutation.Remove(lastIndex);
                }
            }
        }

        #endregion ITEMS MANAGEMENT METHODS

        #region SELECTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get next permutation index. </summary>
        /// <param name="fromIndex"> Permutation index. </param>
        /// <returns> Next permutation index or next index. </returns>
        public int GetNextIndex(int fromIndex)
        {
            if (_permutation.TryGetValue(fromIndex, out int nextIndex))
                return nextIndex;

            return fromIndex + 1;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get previous permutation index. </summary>
        /// <param name="fromIndex"> Permutation index. </param>
        /// <returns> Previous premutation index or previous index. </returns>
        public int GetPreviousIndex(int fromIndex)
        {
            var previousIndex = TryGetIndexOfValue(fromIndex);
            return previousIndex.HasValue ? previousIndex.Value : fromIndex--;
        }

        #endregion SELECTION METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Try get index from value. </summary>
        /// <param name="value"> Indexed value. </param>
        /// <returns> Index of value or null. </returns>
        private int? TryGetIndexOfValue(int value)
        {
            if (_permutation.Any())
            {
                return _permutation.Where(kvp => kvp.Value == value)
                    .Select(kvp => (KeyValuePair<int, int>?)kvp)
                    .FirstOrDefault()?.Key;
            }

            return null;
        }

        #endregion UTILITY METHODS

    }
}
