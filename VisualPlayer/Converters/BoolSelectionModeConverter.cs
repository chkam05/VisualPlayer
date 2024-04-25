using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace VisualPlayer.Converters
{
    public class BoolSelectionModeConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selectionMode = (bool)value;

            if (selectionMode)
            {
                return SelectionMode.Multiple;
            }
            
            return SelectionMode.Single;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SelectionMode)value == SelectionMode.Multiple ? true : false;
        }

    }
}
