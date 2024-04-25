using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualPlayer.Converters
{
    public class StringsCombineConverter : IMultiValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var strings = values.Where(v => v is string str && !string.IsNullOrEmpty(str)).ToArray();
            var separator = (parameter as string) ?? "; ";

            return string.Join(separator, strings);
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            var separator = (parameter as string) ?? "; ";

            return text.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

    }
}
