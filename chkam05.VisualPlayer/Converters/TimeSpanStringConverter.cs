using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class TimeSpanStringConverter : IMultiValueConverter
    {

        //  CONST

        private static readonly string formatStandard = "hh\\:mm\\:ss";
        private static readonly string formatWithMs = "hh\\:mm\\:ss\\.fff";

        private static readonly string defaultStandard = "00:00:00";
        private static readonly string defaultWithMs = "00:00:00.000";


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
            var timeSpan = values[0] != DependencyProperty.UnsetValue ? (TimeSpan)values[0] : new TimeSpan(0);
            var advancedTimeMode = values[1] != DependencyProperty.UnsetValue ? (bool)values[1] : false;
            var format = advancedTimeMode ? formatWithMs : formatStandard;

            if (timeSpan != null)
                return timeSpan.ToString(format);

            return advancedTimeMode ? defaultWithMs : defaultStandard;
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
            var data = value as string;

            if (!string.IsNullOrEmpty(data))
            {
                TimeSpan timeSpan = new TimeSpan(0);

                if (TimeSpan.TryParseExact(data, formatWithMs, CultureInfo.InvariantCulture, out timeSpan))
                    return new object[] { timeSpan, true };

                if (TimeSpan.TryParseExact(data, defaultStandard, CultureInfo.InvariantCulture, out timeSpan))
                    return new object[] { timeSpan, false };
            }

            return new object[] { new TimeSpan(0), false };
        }

    }
}
