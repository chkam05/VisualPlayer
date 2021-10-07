using chkam05.VisualPlayer.Data.Files;
using chkam05.VisualPlayer.Data.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace chkam05.VisualPlayer.Data.Lists
{
    public class NowPlaying<T> : ISerializable, IPlayList<T> where T : BaseFile
    {

        //  CONST

        private static readonly string _serializePostfix = "_PLAYLIST";
        private static readonly string _serializeDataName = "DATA";
        private static readonly string _serializeVersionName = "VERSION";


        //  EVENTS

        public event EventHandler<T> OnAddItem;
        public event EventHandler<IEnumerable<T>> OnAddItems;
        public event EventHandler<T> OnRemoveItem;
        public event EventHandler<IEnumerable<T>> OnRemoveItems;


        //  VARIABLES

        private ObservableCollection<T> _dataContainer;
        private ListView _visualContainer;

        private int _selectedItemIndex = -1;
        private T _selectedItem = null;


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

        public Type DataType
        {
            get => typeof(T);
        }

        public List<T> List
        {
            get => _dataContainer.ToList();
        }

        public int SelectedItemIndex
        {
            get
            {
                if (_selectedItemIndex < 0)
                    SelectItem(0);

                return _selectedItemIndex;
            }
        }

        public T SelectedItem
        {
            get
            {
                if (_selectedItem == null)
                    SelectItem(0);

                return _selectedItem;
            }
        }

        public ObservableCollection<T> VisualList
        {
            get => _dataContainer;
            set => _dataContainer = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> NowPlaying class constructor. </summary>
        public NowPlaying()
        {
            //  Setup data container.
            _dataContainer = new ObservableCollection<T>();
        }

        #endregion CLASS METHODS

        #region PLAYLIST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Add file to playlist. </summary>
        /// <param name="file"> File to add. </param>
        public void Add(T file)
        {
            Add(file, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add file to playlist. </summary>
        /// <param name="file"> File to add. </param>
        /// <param name="invokeEvent"> Invoke external method. </param>
        private bool Add(T file, bool invokeEvent)
        {
            if (_visualContainer != null)
            {
                _visualContainer.Dispatcher.Invoke(() =>
                {
                    _dataContainer.Add(file);
                    RefreshVisualContainer();
                });
            }
            else
            {
                _dataContainer.Add(file);
            }

            //  Invoke external method.
            if (invokeEvent)
                OnAddItem?.Invoke(this, file);

            return true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Add range of files to playlist. </summary>
        /// <param name="files"> Enumerable range of files to add. </param>
        public void AddRange(IEnumerable<T> files)
        {
            //  Added items.
            List<T> addedFiles = new List<T>();

            //  Add items.
            foreach (var file in files)
            {
                if (Add(file, false))
                    addedFiles.Add(file);
            }

            //  Invoke external method.
            OnAddItems?.Invoke(this, addedFiles);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear playlist. </summary>
        public void Clear()
        {
            _dataContainer.Clear();
            _selectedItem = null;
            _selectedItemIndex = -1;

            if (_visualContainer != null)
                RefreshVisualContainer();
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
        /// <summary> Get index of specified file from playlist. </summary>
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

            //  Fix selected item index.
            FixSelection();

            //  Update visual component interface.
            if (_visualContainer != null)
                RefreshVisualContainer();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove file at particular index from playlist. </summary>
        /// <param name="itemIndex"> File index to remove. </param>
        public void RemoveAt(int itemIndex)
        {
            RemoveAt(itemIndex, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove file at particular index from playlist. </summary>
        /// <param name="itemIndex"> File index to remove. </param>
        /// <param name="invokeEvent"> Invoke external method. </param>
        private bool RemoveAt(int itemIndex, bool invokeEvent)
        {
            if (itemIndex >= 0 && itemIndex < _dataContainer.Count)
            {
                var file = _dataContainer[itemIndex];

                _dataContainer.RemoveAt(itemIndex);

                //  Fix selected item index.
                FixSelection();

                //  Update visual component interface.
                if (_visualContainer != null)
                    RefreshVisualContainer();

                //  Invoke external method.
                if (invokeEvent)
                    OnRemoveItem?.Invoke(this, file);

                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove particular file from playlist. </summary>
        /// <param name="file"> File to remove. </param>
        public void Remove(T file)
        {
            Remove(file, true);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove particular file from playlist. </summary>
        /// <param name="file"> File to remove. </param>
        /// <param name="invokeEvent"> Invoke external method. </param>
        private bool Remove(T file, bool invokeEvent)
        {
            if (_dataContainer.Contains(file))
            {
                _dataContainer.Remove(file);

                //  Fix selected item index.
                FixSelection();

                //  Update visual component interface.
                if (_visualContainer != null)
                    RefreshVisualContainer();

                //  Invoke external method.
                if (invokeEvent)
                    OnRemoveItem?.Invoke(this, file);

                return true;
            }

            return false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove range of files from playlist. </summary>
        /// <param name="files"> Enumerable range of files to remove. </param>
        public void RemoveRange(IEnumerable<T> files)
        {
            //  Removed items.
            List<T> removedFiles = new List<T>();

            //  Remove items.
            foreach (var file in files)
            {
                if (Remove(file, false))
                    removedFiles.Add(file);
            }

            //  Invoke external method.
            OnRemoveItems?.Invoke(this, removedFiles);
        }

        #endregion PLAYLIST METHODS

        #region SERIALIZATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Serialize object to XML. </summary>
        /// <returns> Serialized XElement object. </returns>
        public XElement SerializeToXML()
        {
            var app = (App)Application.Current;
            var xmlName = app.ApplicationName + _serializePostfix;

            XElement root = new XElement(xmlName);
            root.Add(new XAttribute(nameof(DataType), DataType.Name));
            root.Add(new XAttribute(_serializeVersionName, app.Version));

            XElement data = new XElement(_serializeDataName);

            if (typeof(ISerializable).IsAssignableFrom(DataType))
            {
                foreach (var item in _dataContainer)
                {
                    var serializedObject = item.SerializeToXML();
                    data.Add(serializedObject);
                }
            }

            root.Add(data);

            return root;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Deserialize and set object data from XML. </summary>
        /// <param name="xmlObject"> Serialized XML object. </param>
        public void DeserializeFromXML(XElement xmlObject)
        {
            var app = (App)Application.Current;
            var xmlName = app.ApplicationName + _serializePostfix;

            if (xmlObject.Name != xmlName)
                throw new ArgumentException("XML object does not represents this kind of playlist object.");

            if (xmlObject.Attribute(nameof(DataType)).Value != DataType.Name)
                throw new ArgumentException("XML data does not represents kind of playlist data objects.");

            //  Remove current data from playlist.
            Clear();

            var data = xmlObject.Element(_serializeDataName);

            if (typeof(ISerializable).IsAssignableFrom(DataType) && data.HasElements)
            {
                foreach (var serializedObject in data.Elements())
                {
                    T item = (T) typeof(T).GetMethod("FromXML").Invoke(null, new object[] { serializedObject });

                    if (item.Exists)
                        Add(item);
                }
            }

            if (_dataContainer.Any(f => (f as IPlayableFile).IsPlaying))
            {
                _selectedItem = _dataContainer.FirstOrDefault(f => (f as IPlayableFile).IsPlaying);
                FixSelection();
            }
        }

        #endregion SERIALIZATION METHODS

        #region SORTING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Sort files in playlist by method and order. </summary>
        /// <typeparam name="TKey"> Sorting value type. </typeparam>
        /// <param name="sortMethod"> Sorting method. </param>
        /// <param name="sortOrder"> Sorting order. </param>
        public void SortBy<TKey>(Expression<Func<T, TKey>> sortMethod, SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                default:
                case SortOrder.ASCENDING:
                    _dataContainer = new ObservableCollection<T>(_dataContainer.OrderBy(sortMethod.Compile()));
                    break;

                case SortOrder.DESCENDING:
                    _dataContainer = new ObservableCollection<T>(_dataContainer.OrderByDescending(sortMethod.Compile()));
                    break;
            }

            //  Fix selected item index.
            FixSelection();

            //  Update visual component interface.
            if (_visualContainer != null)
                UpdateVisualContainer();
        }

        #endregion SORTING METHODS

        #region SPECIAL NOWPLAYING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Fix selection after changing any file index in playlist. </summary>
        private void FixSelection()
        {
            if (_selectedItem != null)
            {
                if (!_dataContainer.Contains(_selectedItem))
                {
                    _selectedItem = null;
                    _selectedItemIndex = -1;
                    return;
                }
                    
                _selectedItemIndex = IndexOf(_selectedItem);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Select file from playlist by it's index. </summary>
        /// <param name="itemIndex"> Index of file to select. </param>
        /// <returns> Selected file. </returns>
        public T SelectItem(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < Count)
            {
                //  Diselect actually selected item.
                if (_selectedItem != null && _selectedItem is IPlayableFile)
                    (_selectedItem as IPlayableFile).IsPlaying = false;

                _selectedItem = _dataContainer[itemIndex];
                _selectedItemIndex = itemIndex;

                if (_selectedItem is IPlayableFile)
                    (_selectedItem as IPlayableFile).IsPlaying = true;

                //  Refresh data in visual container.
                RefreshVisualContainer();

                return _selectedItem;
            }

            return null;
        }

        #endregion SPECIAL NOWPLAYING METHODS

        #region VISUAL CONTAINER METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Assing visual list container to playlist. </summary>
        /// <param name="listView"> Visual list component. </param>
        /// <param name="asBinding"> Assign data from playlist as binding in visual list component. </param>
        public void SetVisualContainer(ListView listView, bool asBinding = false)
        {
            _visualContainer = listView;

            if (asBinding)
            {
                var binding = new Binding("DataContext")
                {
                    Source = _dataContainer
                };

                listView.SetBinding(ListView.ItemsSourceProperty, binding);
            }
            else
            {
                listView.ItemsSource = _dataContainer;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Refresh data in visual list container. </summary>
        public void RefreshVisualContainer()
        {
            if (_visualContainer != null)
                _visualContainer.Items.Refresh();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update visual list container by reassign data. </summary>
        public void UpdateVisualContainer()
        {
            if (_visualContainer != null)
            {
                _visualContainer.ItemsSource = null;
                _visualContainer.ItemsSource = _dataContainer;
            }
        }

        #endregion VISUAL CONTAINER METHODS

    }
}
