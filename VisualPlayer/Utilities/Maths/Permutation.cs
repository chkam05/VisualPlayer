using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities.Maths
{
    public class Permutation
    {

        //  VARIABLES

        private Dictionary<int, int> _permutation;
        private int _size;


        #region GETTERS & SETTERS

        public int this[int i]
        {
            get => _permutation != null && _permutation.ContainsKey(i)
                ? _permutation[i] : 0;
        }

        public int Size
        {
            get => _size;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Permutation class constructor. </summary>
        public Permutation(int size)
        {
            Create(size);
        }

        #endregion CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create permutation with specified size. </summary>
        public void Create(int size)
        {
            _permutation = new Dictionary<int, int>();
            _size = size;

            Random random = new Random();
            List<int> unused = Enumerable.Range(1, size - 1).ToList();

            int firstIndex = 0;
            int index = firstIndex;

            while (unused.Count > 0)
            {
                var randomValue = unused[random.Next(unused.Count)];

                _permutation.Add(index, randomValue);
                unused.Remove(randomValue);
                index = randomValue;
            }

            _permutation.Add(index, firstIndex);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add new element to permutation. </summary>
        public void AddElement()
        {
            if (_permutation == null)
            {
                _permutation = new Dictionary<int, int>();
                _size = 0;
            }

            if (_permutation.Any())
            {
                Random random = new Random();

                int newIndex = _size;
                int randomIndex = random.Next(0, _size - 1);
                int randomIndexValue = _permutation[randomIndex];

                _permutation.Add(newIndex, randomIndex);
                _permutation[randomIndex] = newIndex;
                _size++;
            }
            else
            {
                _permutation.Add(0, 0);
                _size = 1;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add certain number of elements to permutation. </summary>
        /// <param name="range"> Number of elements to add. </param>
        public void AddRange(int range)
        {
            foreach (int _ in Enumerable.Range(0, range))
                AddElement();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get value index. </summary>
        /// <param name="value"> Value which index will be returned. </param>
        /// <returns> Index of value. </returns>
        public int GetValueIndex(int value)
        {
            if (_permutation != null)
            {
                var index = _permutation.FirstOrDefault(kvp => kvp.Value == value).Key;
                return index >= 0 ? index : 0;
            }

            return 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove element from permutation. </summary>
        public void RemoveElement()
        {
            if (_permutation != null && _permutation.Any())
            {
                int index = _size - 1;
                int value = _permutation[index];

                int updateKey = _permutation.FirstOrDefault(kvp => kvp.Value == value).Key;
                
                _permutation[updateKey] = value;
                _permutation.Remove(index);
                _size--;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove certain number of elements from permutation. </summary>
        /// <param name="range"> Number of elements to remove. </param>
        public void RemoveRange(int range)
        {
            if (_permutation == null)
                return;

            foreach (int _ in Enumerable.Range(0, range))
            {
                if (_permutation.Any())
                    AddElement();
                else
                    break;
            }
        }

    }
}
