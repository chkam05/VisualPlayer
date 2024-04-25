using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace VisualPlayer.InternalMessages.Converters
{
    public class IMProgressWidthConverter : IMultiValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Border border = (Border) values.FirstOrDefault(v => v is Border);
            double[] progress = values.Where(v => v is double).Select(v => (double)v).ToArray();

            if (border == null || progress.Length != 2)
                return 0;

            double progressMax = progress.Max();
            double progressValue = progress.Min();
            double maxWidth = border.ActualWidth;

            if (progressMax == 0)
                return maxWidth;

            return (maxWidth * progressValue) / progressMax;
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
