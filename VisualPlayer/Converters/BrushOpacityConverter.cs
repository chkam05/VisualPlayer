using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisualPlayer.Converters
{
    public class BrushOpacityConverter : IMultiValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = ((Brush)values[0]).Clone();
            double opacity = (double)values[1];

            brush.Opacity = opacity;

            return brush;
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Brush brush = ((Brush)value).Clone();
            double opacity = brush.Opacity;

            brush.Opacity = 1f;

            return new object[] { brush, opacity };
        }

    }
}
