using chkam05.VisualPlayer.Data.Static;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class RepeatControlDescriptionConverter : IValueConverter
    {

        //  CONST

        private static readonly string RepeatAll = "All";
        private static readonly string RepeatOne = "One";
        private static readonly string RepeatOff = "Repeat";


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
            var repeatMode = (Repeat)value;

            switch (repeatMode)
            {
                case Repeat.ALL:
                    return RepeatAll;

                case Repeat.ONE:
                    return RepeatOne;

                case Repeat.NORMAL:
                default:
                    return RepeatOff;
            }
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
            var description = (string)value;

            if (description == RepeatAll)
                return Repeat.ALL;

            else if (description == RepeatOne)
                return Repeat.ONE;

            else
                return Repeat.NORMAL;
        }

    }
}
