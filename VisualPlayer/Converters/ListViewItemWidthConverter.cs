using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VisualPlayer.Controls;
using VisualPlayer.Utilities;

namespace VisualPlayer.Converters
{
    public class ListViewItemWidthConverter : IMultiValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var listViewWidth = ObjectHelper.GetValue<double>(values, 0);
            var listViewPadding = ObjectHelper.GetValue<Thickness?>(values, null);
            var scrollBarSize = ObjectHelper.GetValue<double>(values, 0, 1);
            var listView = ObjectHelper.GetValue<CustomListView>(values, null);

            if (listViewWidth <= 0 || !listViewPadding.HasValue)
                return Double.NaN;

            var substract = listViewPadding.Value.Left + listViewPadding.Value.Right + scrollBarSize;

            if (listView != null && ObjectHelper.GetListViewVerticalCustomScrollBarVisibile(listView) == Visibility.Visible)
                substract += scrollBarSize;

            return Math.Max(0, listViewWidth - substract);
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
