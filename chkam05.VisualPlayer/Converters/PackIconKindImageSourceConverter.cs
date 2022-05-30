using chkam05.VisualPlayer.Utilities;
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
    public class PackIconKindImageSourceConverter : IValueConverter
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
            var packIconKind = (PackIconKind)value;
            var parameters = ((string)parameter).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            double height = parameters.Length > 0 ? double.TryParse(parameters[0], out double h) ? h : 32 : 32;
            double width = parameters.Length > 1 ? double.TryParse(parameters[1], out double w) ? w : 32 : 32;
            Color background = parameters.Length > 2 ? ColorUtilities.ColorFromHex(parameters[2]) : Colors.Transparent;
            Color foreground = parameters.Length > 3 ? ColorUtilities.ColorFromHex(parameters[3], Colors.Black) : Colors.Black;

            return ControlsUtilities.DrawImageFromPackIconKind(packIconKind, width, height, 
                new SolidColorBrush(background), new SolidColorBrush(foreground));
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
            throw new NotImplementedException();
        }

    }
}
