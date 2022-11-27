using chkam05.Tools.ControlsEx.Colors;
using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace chkam05.VisualPlayer.Data.Configuration
{
    public class ColorInfo : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private Color _color = Colors.Transparent;
        private string _name;


        //  GETTERS & SETTERS

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        [JsonIgnore]
        public string ColorCode
        {
            get => ColorUtilities.ColorToHex(_color);
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor. </summary>
        public ColorInfo()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor. </summary>
        /// <param name="colorCode"> Hexadecimal color code. </param>
        /// <param name="colorName"> Color name. </param>
        public ColorInfo(string colorCode, string colorName)
        {
            Color = ColorUtilities.ColorFromHex(colorCode);
            Name = colorName;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor from ColorPaletteItem object. </summary>
        /// <param name="colorPaletteItem"> ColorPaletteItem. </param>
        public ColorInfo(ColorPaletteItem colorPaletteItem)
        {
            if (colorPaletteItem != null)
            {
                Color = colorPaletteItem.Color;
                Name = colorPaletteItem.Name;
            }
        }

        #endregion CLASS METHODS

        #region CAST METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert ColorPaletteItems list of objects to ColorInfo list. </summary>
        /// <param name="colorPaletteItems"> List of ColorPaletteItems objects. </param>
        /// <returns></returns>
        public static List<ColorInfo> ToColorPaletteItems(IEnumerable<ColorPaletteItem> colorPaletteItems)
        {
            return colorPaletteItems?.Select(i => new ColorInfo(i)).ToList() ?? new List<ColorInfo>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert ColorInfo object to ColorPaletteItem. </summary>
        /// <returns> ColorPaletteItem. </returns>
        public ColorPaletteItem ToColorPaletteItem()
        {
            return new ColorPaletteItem(Color, Name);
        }

        #endregion CAST METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
