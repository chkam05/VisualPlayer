using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualPlayer.Converters
{
    public class VolumeControlButtonIconKindConverter : IMultiValueConverter
    {

        //  CONST

        private const int REQUIRED_ITEMS_COUNT = 2;


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < REQUIRED_ITEMS_COUNT)
                return PackIconKind.VolumeMedium;

            var volumePosition = values.FirstOrDefault(v => v is double) as double?;
            var isMute = values.FirstOrDefault(v => v is bool) as bool?;

            if (isMute.HasValue && isMute.Value)
                return PackIconKind.VolumeMute;

            if (volumePosition.HasValue)
                return MapVolumePositionToIconKind(volumePosition.Value);

            return PackIconKind.VolumeMedium;
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Map volume position to PackIconKind. </summary>
        /// <param name="volumePosition"> Volume position. </param>
        /// <returns> PackIconKind. </returns>
        private PackIconKind MapVolumePositionToIconKind(double volumePosition)
        {
            if (volumePosition > 66.6)
                return PackIconKind.VolumeHigh;

            else if (volumePosition < 33.3)
                return PackIconKind.VolumeLow;

            else
                return PackIconKind.VolumeMedium;
        }

        #endregion UTILITY METHODS

    }
}
