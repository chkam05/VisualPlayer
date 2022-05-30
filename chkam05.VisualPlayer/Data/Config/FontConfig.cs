using chkam05.VisualPlayer.Data.Fonts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Config
{
    public class FontConfig : INotifyPropertyChanged, ICloneable
    {

        //  CONST

        public const double HEADER_DIFFRENCE_MAX = 10;
        public const double HEADER_DIFFRENCE_MIN = 8;
        public const double SPACING_MAX = 8.0;
        public const double SPACING_MIN = 2.0;

        public static readonly FontConfig StandardFontConfig = new FontConfig()
        {
            FontFamily = FontsManager.Instance.GetFontByName("Segoe UI"),
            FontSize = 16,
            FontStyle = FontStyles.Normal,
            FontStretch = FontStretches.Normal,
            FontWeight = FontWeights.Normal,
            HeaderDiffrence = 10,
            Spacing = 3,
        };

        public static readonly FontConfig ElectronicFontConfig = new FontConfig()
        {
            FontFamily = FontsManager.Instance.GetFontByName("LED Sled Straight"),
            FontSize = 18,
            FontStyle = FontStyles.Normal,
            FontStretch = FontStretches.Normal,
            FontWeight = FontWeights.Normal,
            HeaderDiffrence = 10,
            Spacing = 2,
        };

        public static readonly FontConfig ElectronicOldFontConfig = new FontConfig()
        {
            FontFamily = FontsManager.Instance.GetFontByNameAndSubName("Digital dream", "Narrow"),
            FontSize = 16,
            FontStyle = FontStyles.Normal,
            FontStretch = FontStretches.Normal,
            FontWeight = FontWeights.Normal,
            HeaderDiffrence = 8,
            Spacing = 8,
        };


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private FontContainer _fontFamily = FontsManager.Instance.GetFontByName("Segoe UI");
        private int _fontSize = 12;
        private FontStyle _fontStyle = FontStyles.Normal;
        private FontStretch _fontStretch = FontStretches.Normal;
        private FontWeight _fontWeight = FontWeights.Normal;
        private int _headerDiffrence = 8;
        private double _spacing = 2.0;


        //  GETTERS & SETTERS

        public FontContainer FontFamily
        {
            get => _fontFamily;
            set
            {
                _fontFamily = value;
                OnPropertyChanged(nameof(FontFamily));
            }
        }

        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                _fontStyle = value;
                OnPropertyChanged(nameof(FontStyle));
            }
        }

        public FontStretch FontStretch
        {
            get => _fontStretch;
            set
            {
                _fontStretch = value;
                OnPropertyChanged(nameof(FontStretch));
            }
        }

        public FontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                _fontWeight = value;
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        public int HeaderDiffrence
        {
            get => _headerDiffrence;
            set
            {
                _headerDiffrence = value;
                OnPropertyChanged(nameof(HeaderDiffrence));
            }
        }

        public double Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                OnPropertyChanged(nameof(Spacing));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FontConfig class constructor. </summary>
        public FontConfig()
        {
            //
        }

        #endregion CLASS METHODS

        #region CLONEABLE INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates a new object that is a copy of the current instance. </summary>
        /// <returns> New object that is a copy of this instance. </returns>
        public object Clone()
        {
            return new FontConfig()
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStyle = FontStyle,
                FontStretch = FontStretch,
                FontWeight = FontWeight
            };
        }

        #endregion CLONEABLE INTERFACE METHODS

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

        #region UPDATE DATA METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update configuration data from another config. </summary>
        /// <param name="config"> Configuration data to update. </param>
        public void Update(FontConfig config)
        {
            FontFamily = config.FontFamily;
            FontSize = config.FontSize;
            FontStyle = config.FontStyle;
            FontStretch = config.FontStretch;
            FontWeight = config.FontWeight;
            HeaderDiffrence = config.HeaderDiffrence;
            Spacing = config.Spacing;
        }

        #endregion UPDATE DATA METHODS

    }
}
