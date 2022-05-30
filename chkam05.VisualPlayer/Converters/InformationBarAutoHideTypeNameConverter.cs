using chkam05.VisualPlayer.Controls.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class InformationBarAutoHideTypeNameConverter : IValueConverter
    {

        //  CONST

        private static readonly Dictionary<InformationBarAutoHide, string> _mapping = 
            new Dictionary<InformationBarAutoHide, string>
            {
                { InformationBarAutoHide.AUTOHIDE, "Auto hide" },
                { InformationBarAutoHide.STAY_3S, "3 sec." },
                { InformationBarAutoHide.STAY_5S, "5 sec." },
                { InformationBarAutoHide.STAY_10S, "10 sec." },
                { InformationBarAutoHide.INFINITE, "Stay on top" },
            };


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
            if (value != null)
            {
                var enumValue = (InformationBarAutoHide)value;
                return _mapping[enumValue];
            }

            return string.Empty;
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
            var stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue) && _mapping.Any(m => m.Value == stringValue))
                return _mapping.Where(m => m.Value == stringValue).First().Key;

            return InformationBarAutoHide.STAY_5S;
        }
    }
}
