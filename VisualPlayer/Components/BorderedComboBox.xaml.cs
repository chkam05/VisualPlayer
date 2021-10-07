using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Components
{
    public partial class BorderedComboBox : UserControl
    {

        //  EVENTS

        public event EventHandler<int> OnItemSelect;


        //  VARIABLES

        private bool _focused = false;
        private List<string> _items;


        #region GETTERS & SETTERS

        public string this[int i]
        {
            get => (i >= 0 && i < _items.Count) ? _items[i] : null;
            set
            {
                if (i >= 0 && i < _items.Count)
                    _items[i] = value;

                UpdateList(false);
            }
        }

        public new Brush Background
        {
            get => ContentBorder.Background;
            set => ContentBorder.Background = value;
        }

        public new Brush BorderBrush
        {
            get => ContentBorder.BorderBrush;
            set => ContentBorder.BorderBrush = value;
        }

        public new double BorderThickness
        {
            get => ContentBorder.BorderThickness.Left;
            set => ContentBorder.BorderThickness = new Thickness(value);
        }

        public List<string> Items
        {
            get => _items;
            set
            {
                _items = value != null ? value : new List<string>();
                UpdateList(true);
            }
        }

        public new bool IsEnabled
        {
            get => DataComboBox.IsEnabled;
            set => DataComboBox.IsEnabled = value;
        }

        public int SelectedItemIndex
        {
            get => DataComboBox.SelectedIndex;
            set
            {
                if (value < 0 || value >= _items.Count)
                    ClearSelection();
                else
                    DataComboBox.SelectedIndex = value;
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BorderedComboBox class constructor. </summary>
        public BorderedComboBox()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region DATA MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update changes in data list in combobox. </summary>
        /// <param name="clearSelection"> Diselect current selected item. </param>
        private void UpdateList(bool clearSelection = false)
        {
            var selectedIndex = DataComboBox.SelectedIndex;

            if (clearSelection)
                ClearSelection();

            DataComboBox.ItemsSource = null;
            DataComboBox.ItemsSource = _items;

            if (!clearSelection)
                DataComboBox.SelectedIndex = Math.Max(0, Math.Max(selectedIndex, _items.Count - 1));
        }

        #endregion DATA MANAGEMENT METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clear selection from combobox. </summary>
        public void ClearSelection()
        {
            DataComboBox.SelectedIndex = -1;
        }

        #endregion INTERACTION METHODS

        #region INTERFACE INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when combobox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DataComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _focused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when combobox is unselected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void DataComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _focused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when combobox item is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Text change event arguments. </param>
        private void DataComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_focused)
                OnItemSelect(this, (sender as ComboBox).SelectedIndex);
        }

        #endregion INTERFACE INTERACTION METHODS

    }
}
