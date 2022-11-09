using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Converters
{
    public class IPlayableNowPlayingConverter : IMultiValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert source values to a value for the binding target. </summary>
        /// <param name="values"> Array of values that the source bindings in MultiBinding produces. </param>
        /// <param name="targetType"> The type of the binding target property. </param>
        /// <param name="parameter"> Converter parameter to use. </param>
        /// <param name="culture"> Culture to use in the converter. </param>
        /// <returns> A converted value. If the method returns null, the valid null value is used. </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool nowPlaying = (bool) values[0];
            bool hasImage = values.Count() > 1 ? ((ImageSource)values[1] != null) : false;

            if (nowPlaying)
                return PackIconKind.Play;

            else if (hasImage)
                return PackIconKind.None;

            else
                return PackIconKind.MusicNote;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Converts a binding target value to the source binding values. </summary>
        /// <param name="value"> Value that the binding target produces. </param>
        /// <param name="targetTypes"> Array of types to convert to. </param>
        /// <param name="parameter"> Converter parameter to use. </param>
        /// <param name="culture"> Culture to use in the converter. </param>
        /// <returns> Array of values that have been converted from the target value back to the source values.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var packIconKind = (PackIconKind)value;

            if (packIconKind == PackIconKind.Play)
                return new object[] { true };

            return new object[] { false };
        }

    }
}
