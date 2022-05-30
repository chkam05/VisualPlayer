using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class DoublePercentStringConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Converts a value. </summary>
        /// <param name="value"> Value produced by the binding source. </param>
        /// <param name="targetType"> Type of the binding target property. </param>
        /// <param name="parameter"> Converter parameter to use. </param>
        /// <param name="culture"> Culture to use in the converter. </param>
        /// <returns> Converted value. If the method returns null, the valid null value is used. </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = (double)value;
            double maxValue = 1.0;

            if (!string.IsNullOrEmpty(parameter as string))
                double.TryParse(parameter as string, out maxValue);

            return $"{(int)(doubleValue * 100 / maxValue)}%";
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Converts a value. </summary>
        /// <param name="value"> Value that is produced by the binding target. </param>
        /// <param name="targetType"> Type to convert to. </param>
        /// <param name="parameter"> Converter parameter to use. </param>
        /// <param name="culture"> Culture to use in the converter. </param>
        /// <returns> Converted value. If the method returns null, the valid null value is used. </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;
            double maxValue = 1.0;

            if (!string.IsNullOrEmpty(stringValue))
            {
                if (stringValue.Contains('%'))
                    stringValue = stringValue.Remove('%');

                if (double.TryParse(stringValue, out double doubleValue))
                    return doubleValue * maxValue / 100;
            }

            return 0;
        }

    }
}
