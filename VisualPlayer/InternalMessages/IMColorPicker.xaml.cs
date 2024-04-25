using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VisualPlayer.Data.ColorModels;
using VisualPlayer.InternalMessages.Base;
using System.Collections.Specialized;
using VisualPlayer.Controls.Events;
using VisualPlayer.InternalMessages.Enums;
using VisualPlayer.Controls;

namespace VisualPlayer.InternalMessages
{
    public partial class IMColorPicker : IMBase
    {

        //  VARIABLES

        private bool _upDownModificationLock = false;
        private bool _enableHistory = true;
        private ObservableCollection<ThemeColor> _historyColorsCollection;
        private ThemeColor _selectedColor = null;


        //  GETTERS & SETTERS

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
            set
            {
                UpdateProperty(ref _selectedColor, value);
                UpdateUpDownValues(value.Color);
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> IMColorPicker class constructor. </summary>
        /// <param name="imControl"> Internal message control interface. </param>
        public IMColorPicker(IIMControl imControl) : base(imControl)
        {
            //  Initialize data.
            HistoryColorsCollection = new ObservableCollection<ThemeColor>(BaseColors.GetDefaultColors(allowRemove: true));

            //  Initialize user interface.
            InitializeComponent();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create internal message color selector. </summary>
        /// <param name="imControl"> Internal message control. </param>
        /// <param name="title"> Message title. </param>
        /// <param name="onClose"> On close method. </param>
        /// <returns> Internal message box. </returns>
        public static IMColorPicker CreateMessage(IIMControl imControl, string title,
            CloseEventHandler<IIMCloseEventArgs> onClose = null)
        {
            var imBox = new IMColorPicker(imControl)
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
        private void AddCustomButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedColor = HistoryColorsCollection.FirstOrDefault(c => c.ColorCode == SelectedColor.ColorCode);

            if (selectedColor != null)
            {
                HistoryColorsCollection.Remove(selectedColor);
                HistoryColorsCollection.Insert(0, selectedColor);
            }
            else
            {
                HistoryColorsCollection.Insert(0, SelectedColor);

                while (HistoryColorsCollection.Count() > 10)
                    HistoryColorsCollection.RemoveAt(HistoryColorsCollection.Count() - 1);
            }
        }
        
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
        /// <summary> Method invoked after changing color component value in up down. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Custom Up Down Value Changed Event Arguments. </param>
        private void ColorComponentUpDownValueChanged(object sender, CustomUpDownValueChangedEventArgs e)
        {
            if (!_upDownModificationLock && e.UIModified && sender is CustomUpDown upDown)
            {
                _upDownModificationLock = true;

                switch (upDown.Name)
                {
                    case "_redUpDown":
                    case "_greenUpDown":
                    case "_blueUpDown":
                        byte red = (byte)_redUpDown.Value;
                        byte green = (byte)(byte)_greenUpDown.Value;
                        byte blue = (byte)(byte)_blueUpDown.Value;

                        var color = Color.FromArgb(255, red, green, blue);

                        DoInUpDownModificationLock(() =>
                        {
                            SelectedColor = new ThemeColor(color);
                            _colorPicker.SelectedColor = SelectedColor;
                        });
                        break;

                    case "_hueUpDown":
                    case "_lightnessUpDown":
                    case "_saturationUpDown":
                        int hue = (int)_hueUpDown.Value;
                        int lightness = (int)_lightnessUpDown.Value;
                        int saturation = (int)_saturationUpDown.Value;

                        var ahslColor = new AHSLColor(255, hue, saturation, lightness);

                        DoInUpDownModificationLock(() =>
                        {
                            SelectedColor = new ThemeColor(ahslColor.ToColor());
                            _colorPicker.SelectedColor = SelectedColor;
                        });
                        break;
                }

                _upDownModificationLock = false;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing color selection in color selector. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Color Selection Changed Event Arguments. </param>
        private void ColorSelectorSelectionChanged(object sender, ColorSelectionChangedEventArgs e)
        {
            SelectedColor = e.SelectedColor;
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
            _colorPicker.SelectedColor = e.SelectedColor;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update up down value. </summary>
        /// <param name="color"> Color. </param>
        private void UpdateUpDownValues(Color color)
        {
            var ahslColor = AHSLColor.FromColor(color);

            DoInUpDownModificationLock(() =>
            {
                _redUpDown.Value = color.R;
                _greenUpDown.Value = color.G;
                _blueUpDown.Value = color.B;

                _hueUpDown.Value = ahslColor.H;
                _lightnessUpDown.Value = ahslColor.L;
                _saturationUpDown.Value = ahslColor.S;
            });
        }

        #endregion COLOR SELECTORS INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Notify history colors collection changed. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Notify Collection Changed Event Arguments. </param>
        private void NotifyHistoryColorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(HistoryColorsCollection));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke method in up down modification lock. </summary>
        /// <param name="f"> Method. </param>
        private void DoInUpDownModificationLock(Action f)
        {
            _upDownModificationLock = true;
            f?.Invoke();
            _upDownModificationLock = false;
        }

        #endregion UTILITY METHODS

    }
}
