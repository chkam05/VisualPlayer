using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VisualPlayer.InternalMessages.Enums;

namespace VisualPlayer.InternalMessages.Converters
{
    public class IMButtonVisibilityConverter : IValueConverter
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var buttons = value as ObservableCollection<IMResult>;
            var buttonType = parameter as IMResult?;

            if (!buttonType.HasValue)
                throw new ArgumentException($"Invalid button type parameter.");

            return (buttons?.Any(b => b == buttonType.Value) ?? false)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        //  --------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
