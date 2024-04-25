using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class SerialPortStateStringConverter : IValueConverter
    {

        //  CONST

        public static readonly string CONNECTED_STATE = "Disconnect";
        public static readonly string DISCONNECTED_STATE = "Connect";


        //  METHODS

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isConnected = (bool)value;
            return isConnected ? CONNECTED_STATE : DISCONNECTED_STATE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
