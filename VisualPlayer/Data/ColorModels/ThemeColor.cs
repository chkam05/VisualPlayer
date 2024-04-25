using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using VisualPlayer.Commands;
using VisualPlayer.Data.ColorModels.Events;
using VisualPlayer.Utilities;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.ColorModels
{
    public class ThemeColor : BaseViewModel, ICloneable
    {

        //  COMMANDS

        [JsonIgnore]
        public ICommand SelectItemCommand { get; set; }

        [JsonIgnore]
        public ICommand RemoveItemCommand { get; set; }


        //  DELEGATES

        public delegate void ThemeColorTransporterEventHandler(object sender, ThemeColorTransporterEventArgs e);


        //  EVENTS

        public ThemeColorTransporterEventHandler ClearItems;
        public ThemeColorTransporterEventHandler SelectItem;
        public ThemeColorTransporterEventHandler RemoveItem;


        //  VARIABLES

        private bool _allowRemove;
        private Color _color;
        private string _colorName;


        //  GETTERS & SETTERS

        public bool AllowRemove
        {
            get => _allowRemove;
            set => UpdateProperty(ref _allowRemove, value);
        }

        public Color Color
        {
            get => _color;
            set
            {
                UpdateProperty(ref _color, value);
                UpdateAdditionalProperties();
            }
        }

        [JsonIgnore]
        public string ColorCode
        {
            get => ColorsHelper.GetColorCode(Color);
        }

        public string ColorName
        {
            get => !string.IsNullOrEmpty(_colorName) ? _colorName : ColorCode;
            set => UpdateProperty(ref _colorName, value);
        }

        [JsonIgnore]
        public SolidColorBrush ColorBrush
        {
            get => new SolidColorBrush(Color);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ThemeColor class constructor. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="colorName"> Color name. </param>
        /// <param name="allowRemove"> Allow remove. </param>
        [JsonConstructor]
        public ThemeColor(Color color, string colorName = null, bool? allowRemove = false)
        {
            Color = color;
            ColorName = colorName;
            AllowRemove = allowRemove ?? false;

            SetupCommands();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create ThemeColor from color code. </summary>
        /// <param name="colorCode"> Color code. </param>
        /// <param name="colorName"> Color name. </param>
        /// <returns> Theme color. </returns>
        public static ThemeColor ThemeColorFromCode(string colorCode, string colorName)
        {
            Color color = ColorsHelper.GetColorFromCode(colorCode);
            string name = !string.IsNullOrEmpty(colorName) ? colorName : colorCode;

            return new ThemeColor(color, name);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Returns a string that represents the current object. </summary>
        /// <returns> String that represents the current object.</returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(ColorName) ? ColorName : ColorCode;
        }

        #endregion CLASS METHODS

        #region CLONE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clone. </summary>
        /// <returns> Object clone. </returns>
        public object Clone()
        {
            return new ThemeColor(Color, ColorName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clone and modify. </summary>
        /// <param name="color"> Color. </param>
        /// <param name="colorName"> Color name. </param>
        /// <param name="allowRemove"> Allow remove. </param>
        /// <returns> ThemeColor clone. </returns>
        public ThemeColor CloneModified(Color? color = null, string colorName = null, bool? allowRemove = null)
        {
            var clone = (ThemeColor)Clone();

            if (color.HasValue)
                clone.Color = color.Value;

            if (!string.IsNullOrEmpty(colorName))
                clone.ColorName = colorName;

            if (allowRemove.HasValue)
                clone.AllowRemove = allowRemove.Value;

            return clone;
        }

        #endregion CLONE METHODS

        #region CONTROL COMMANDS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Select item command execute method. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnSelectItemCommandExecuted(object sender)
        {
            SelectItem?.Invoke(sender, new ThemeColorTransporterEventArgs(this));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Remove item command execute method. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        private void OnRemoveItemCommandExecuted(object sender)
        {
            RemoveItem?.Invoke(sender, new ThemeColorTransporterEventArgs(this));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Setup commands. </summary>
        private void SetupCommands()
        {
            SelectItemCommand = new RelayCommand(OnSelectItemCommandExecuted);
            RemoveItemCommand = new RelayCommand(OnRemoveItemCommandExecuted);
        }

        #endregion CONTROL COMMANDS METHODS

        #region CONVERSION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create ThemeColor from AHSL color. </summary>
        /// <param name="ahslColor"> AHSL color. </param>
        /// <param name="name"> Name. </param>
        /// <returns> Theme Color. </returns>
        public static ThemeColor FromAHSLColor(AHSLColor ahslColor, string name)
        {
            return new ThemeColor(ahslColor.ToColor(), name);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert to AHSL color. </summary>
        /// <returns> AHSL color. </returns>
        public AHSLColor ToAHSLColor()
        {
            return AHSLColor.FromColor(Color);
        }

        #endregion CONVERSION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update additional properties. </summary>
        private void UpdateAdditionalProperties()
        {
            NotifyPropertyChanged(nameof(ColorCode));
            NotifyPropertyChanged(nameof(ColorBrush));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
