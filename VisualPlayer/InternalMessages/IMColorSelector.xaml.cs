using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using static VisualPlayer.InternalMessages.Base.IMBase;
using VisualPlayer.InternalMessages.Base;
using VisualPlayer.InternalMessages.Enums;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Controls.Events;

namespace VisualPlayer.InternalMessages
{
    public partial class IMColorSelector : IMBase
    {

        //  VARIABLES

        private ObservableCollection<ThemeColor> _colorsCollection;
        private bool _enableHistory = true;
        private ObservableCollection<ThemeColor> _historyColorsCollection;
        private ThemeColor _selectedColor = null;


        //  GETTERS & SETTERS

        public ObservableCollection<ThemeColor> ColorsCollection
        {
            get => _colorsCollection;
            set
            {
                UpdateProperty(ref _colorsCollection, value);
                _colorsCollection.CollectionChanged += NotifyColorsCollectionChanged;
            }
        }

        public bool EnableHistory
        {
            get => _enableHistory;
            set => UpdateProperty(ref _enableHistory, value);
        }

        public ObservableCollection<ThemeColor> HistoryColorsCollection
        {
            get => _historyColorsCollection;
            set
            {
                UpdateProperty(ref _historyColorsCollection, value);
                _historyColorsCollection.CollectionChanged += NotifyHistoryColorsCollectionChanged;
            }
        }

        public ThemeColor SelectedColor
        {
            get => _selectedColor;
            set => UpdateProperty(ref _selectedColor, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMColorSelector class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        public IMColorSelector(IIMControl imControl) : base(imControl)
        {
            //  Initialize data.
            ColorsCollection = new ObservableCollection<ThemeColor>(BaseColors.GetBaseColors());
            HistoryColorsCollection = new ObservableCollection<ThemeColor>(BaseColors.GetDefaultColors(true));

            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message color selector. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="message"> Message. </param>
        /// <param name="icon"> Message title icon. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMColorSelector CreateMessage(IIMControl imControl, string title,
            CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var imBox = new IMColorSelector(imControl)
            {
                Title = title,
            };

            if (onClose != null)
                imBox.Closed = onClose;

            return imBox;
        }

        #endregion CLASS METHODS

        #region BUTTONS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking ok button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void OkCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Ok);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking cancel button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void CancelCustomButtonClick(object sender, RoutedEventArgs e)
        {
            InvokeClosedEventHandler(IMResult.Cancel);
        }

        #endregion BUTTONS INTERACTION METHODS

        #region COLOR SELECTORS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing color selection in color selector. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Color Selection Changed Event Arguments. </param>
        private void ColorSelectorSelectionChanged(object sender, ColorSelectionChangedEventArgs e)
        {
            var selectedColor = HistoryColorsCollection.FirstOrDefault(c => c.ColorCode == e.SelectedColor.ColorCode);

            if (selectedColor != null)
            {
                HistoryColorsCollection.Remove(selectedColor);
                HistoryColorsCollection.Insert(0, selectedColor);

                SelectedColor = selectedColor;
            }
            else
            {
                HistoryColorsCollection.Insert(0, e.SelectedColor.CloneModified(allowRemove: true));

                while (HistoryColorsCollection.Count() > 5)
                    HistoryColorsCollection.RemoveAt(HistoryColorsCollection.Count() - 1);

                SelectedColor = e.SelectedColor;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing history color selection in color selector. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Color Selection Changed Event Arguments. </param>
        private void HistoryColorSelectorSelectionChanged(object sender, ColorSelectionChangedEventArgs e)
        {
            HistoryColorsCollection.Remove(e.SelectedColor);
            HistoryColorsCollection.Insert(0, e.SelectedColor);

            SelectedColor = e.SelectedColor;
        }

        #endregion COLOR SELECTORS INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Notify colors collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(ColorsCollection));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Notify history colors collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyHistoryColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(HistoryColorsCollection));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
