using chkam05.VisualPlayer.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Config
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

        public string ColorCode
        {
            get => ColorUtilities.ColorToHex(_color);
        }

        public string ColorName
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(ColorName));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo private class constructor for configuration. </summary>
        [JsonConstructor]
        private ColorInfo()
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor. </summary>
        /// <param name="color"> Color object. </param>
        /// <param name="name"> Color name. </param>
        public ColorInfo(Color color, string name = null)
        {
            Color = color;
            ColorName = !string.IsNullOrEmpty(name) ? name : ColorUtilities.ColorToHex(color);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ColorInfo class constructor. </summary>
        /// <param name="colorCode"> Hexadecimal color code. </param>
        /// <param name="name"> Color name. </param>
        public ColorInfo(string colorCode, string name = null)
        {
            Color = ColorUtilities.ColorFromHex(colorCode);
            ColorName = !string.IsNullOrEmpty(name) ? name : colorCode;
        }

        #endregion CLASS METHODS

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
