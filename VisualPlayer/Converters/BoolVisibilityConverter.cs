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
    public class BoolVisibilityConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = (bool)value;
            var hideParam = (string)parameter;

            if (visible)
            {
                return Visibility.Visible;
            }
            else if (!string.IsNullOrEmpty(hideParam))
            {
                if (hideParam.ToLower() == "hidden")
                    return Visibility.Hidden;

                else if (hideParam.ToLower() == "collapsed")
                    return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? true : false;
        }

    }
}
