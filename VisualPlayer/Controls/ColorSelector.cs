using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VisualPlayer.Controls.Events;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Data.ColorModels.Events;

namespace VisualPlayer.Controls
{
    public class ColorSelector : Control
    {

        //  DELEGATES

        public delegate void ColorSelectionChangedEventHandler(object sender, ColorSelectionChangedEventArgs e);


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ColorsCollectionProperty = DependencyProperty.Register(
            nameof(ColorsCollection),
            typeof(ObservableCollection<ThemeColor>),
            typeof(ColorSelector),
            new PropertyMetadata(
                new ObservableCollection<ThemeColor>(BaseColors.GetBaseColors()),
                ColorsCollectionPropertyChangedCallback));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ColorSelector),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.Register(
            nameof(ShowContextMenu),
            typeof(bool),
            typeof(ColorSelector),
            new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(ThemeColor),
            typeof(ColorSelector),
            new PropertyMetadata(null));


        //  EVENTS

        public event ColorSelectionChangedEventHandler SelectionChanged;


        //  VARIABLES

        private bool _waitingDoubleClick = false;


        //  GETTERS & SETTERS

        public ObservableCollection<ThemeColor> ColorsCollection
        {
            get => (ObservableCollection<ThemeColor>)GetValue(ColorsCollectionProperty);
            set => SetValue(ColorsCollectionProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public bool ShowContextMenu
        {
            get => (bool)GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, value);
        }

        public ThemeColor SelectedColor
        {
            get => (ThemeColor)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static ColorSelector class constructor. </summary>
        static ColorSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSelector),
                new FrameworkPropertyMetadata(typeof(ColorSelector)));
        }

        #endregion CLASS METHODS

        #region COMPONENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after selecting color item in list view. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Selection Changed Event Arguments. </param>
        private void ListViewColorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_waitingDoubleClick && e.AddedItems.Count > 0 && e.AddedItems[0] is ThemeColor themeColor)
            {
                SelectedColor = themeColor;
                InvokeSelectionChanged(themeColor);

                _waitingDoubleClick = false;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after mouse button down. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ListViewPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _waitingDoubleClick = true;
        }

        #endregion COMPONENT METHODS

        #region ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Methon invoked after selecting item from item context menu. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Theme Color Transport Event Arguments. </param>
        private void OnItemSelect(object sender, ThemeColorTransporterEventArgs e)
        {
            SelectedColor = e.ThemeColor;
            InvokeSelectionChanged(e.ThemeColor);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Methon invoked after removing item from item context menu. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Theme Color Transport Event Arguments. </param>
        private void OnItemRemove(object sender, ThemeColorTransporterEventArgs e)
        {
            var themeColor = ColorsCollection.FirstOrDefault(c => c.ColorCode == e.ThemeColor.ColorCode);

            if (themeColor != null)
                ColorsCollection.Remove(themeColor);
        }

        #endregion ITEMS INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke selection changed event. </summary>
        /// <param name="themeColor"> External theme color. </param>
        private void InvokeSelectionChanged(ThemeColor themeColor = null)
        {
            SelectionChanged?.Invoke(this, new ColorSelectionChangedEventArgs(themeColor ?? SelectedColor));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing colors collection property. </summary>
        /// <param name="s"> Dependency object in which method is invoked. </param>
        /// <param name="e"> Dependency Property Changed Event Arguments. </param>
        private static void ColorsCollectionPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is ColorSelector instance)
            {
                var oldCollection = e.OldValue as ObservableCollection<ThemeColor>;
                var newCollection = e.NewValue as ObservableCollection<ThemeColor>;

                if (oldCollection != null)
                {
                    oldCollection.CollectionChanged -= instance.ColorsCollectionChanged;

                    foreach (var themeColor in oldCollection)
                        instance.UnsetThemeColorEvents(themeColor);
                }

                if (newCollection != null)
                {
                    newCollection.CollectionChanged += instance.ColorsCollectionChanged;

                    foreach (var themeColor in oldCollection)
                        instance.SetThemeColorEvents(themeColor);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after colors collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void ColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ThemeColor themeColor)
                        UnsetThemeColorEvents(themeColor);
                }
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ThemeColor themeColor)
                        SetThemeColorEvents(themeColor);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set theme color events. </summary>
        /// <param name="themeColor"> Theme color. </param>
        private void SetThemeColorEvents(ThemeColor themeColor)
        {
            themeColor.SelectItem += OnItemSelect;
            themeColor.RemoveItem += OnItemRemove;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Unset theme color events. </summary>
        /// <param name="themeColor"> Theme color. </param>
        private void UnsetThemeColorEvents(ThemeColor themeColor)
        {
            themeColor.SelectItem -= OnItemSelect;
            themeColor.RemoveItem -= OnItemRemove;
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            var listView = GetListView("_listView");

            SetListViewPreviewMouseDown(listView, ListViewPreviewMouseDown);
            SetListViewSelectionChanged(listView, ListViewColorSelectionChanged);

            if (ColorsCollection != null)
            {
                ColorsCollection.CollectionChanged += ColorsCollectionChanged;

                foreach (var themeColor in ColorsCollection)
                    SetThemeColorEvents(themeColor);
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get ListView from ContentTemplate. </summary>
        /// <param name="listViewName"> ListView name. </param>
        /// <returns> ListView or null. </returns>
        private ListView GetListView(string listViewName)
        {
            return this.Template.FindName(listViewName, this) as ListView;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set list view preview mouse down event handler. </summary>
        /// <param name="listView"> ListView. </param>
        /// <param name="handler"> Preview mouse down event handler. </param>
        private void SetListViewPreviewMouseDown(ListView listView, MouseButtonEventHandler handler)
        {
            if (listView != null && handler != null)
                listView.PreviewMouseDown += handler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set list view selection changed event handler. </summary>
        /// <param name="listView"> List view. </param>
        /// <param name="handler"> Selection changed event handler. </param>
        private void SetListViewSelectionChanged(ListView listView, SelectionChangedEventHandler handler)
        {
            if (listView != null && handler != null)
                listView.SelectionChanged += handler;
        }

        #endregion TEMPLATE METHODS

    }
}
