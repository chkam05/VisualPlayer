using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VisualPlayer.Converters
{
    public class CornerRadiusModifConverter : IValueConverter
    {

        //  CONST

        private const string BOTTOM_LEFT = "B";
        private const string BOTTOM_RIGHT = "R";
        private const string TOP_LEFT = "L";
        private const string TOP_RIGHT = "T";
        private const char NULL_MODIFIER = 'N';

        private static readonly char[] SHIFT_CHARACTERS = new char[]
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', '-'
        };


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cornerRadius = (CornerRadius)value;
            var modifier = ((string)parameter)?.ToUpper();

            if (string.IsNullOrEmpty(modifier))
                return cornerRadius;

            var topLeft = Modif(cornerRadius.TopLeft, modifier, TOP_LEFT);
            var topRight = Modif(cornerRadius.TopRight, modifier, TOP_RIGHT);
            var bottomRight = Modif(cornerRadius.BottomRight, modifier, BOTTOM_RIGHT);
            var bottomLeft = Modif(cornerRadius.BottomLeft, modifier, BOTTOM_LEFT);

            return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region MODIFICATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Modif corner radius value. </summary>
        /// <param name="value"> Corner radius value. </param>
        /// <param name="modifier"> Modifier string. </param>
        /// <param name="modif"> Modificating param. </param>
        /// <returns> Modified value. </returns>
        private double Modif(double value, string modifier, string modif)
        {
            int modifIndex = modifier.IndexOf(modif);

            if (modifIndex < 0)
                return 0;

            if (modifIndex + 1 < modifier.Length)
            {
                string shift = "";

                for (int i = modifIndex + 1; i < modifier.Length; i++)
                {
                    if (modifier[i] == NULL_MODIFIER)
                        return 0;

                    if (!SHIFT_CHARACTERS.Contains(modifier[i]))
                        break;

                    shift += modifier[i];
                }

                if (!string.IsNullOrEmpty(shift) && double.TryParse(shift, out double shiftValue))
                    return value + shiftValue;
            }

            return value;
        }

        #endregion MODIFICATION METHODS

    }
}
