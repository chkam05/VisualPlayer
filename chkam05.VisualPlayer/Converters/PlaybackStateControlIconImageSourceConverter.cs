using chkam05.VisualPlayer.Converters.Static;
using CSCore.SoundOut;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class PlaybackStateControlIconImageSourceConverter : IValueConverter
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
            var playbackState = (PlaybackState)value;
            var packIconKind = PackIconKind.Play;

            switch (playbackState)
            {
                case PlaybackState.Playing:
                    packIconKind = PackIconKind.Pause;
                    break;

                case PlaybackState.Paused:
                    packIconKind = PackIconKind.Play;
                    break;

                case PlaybackState.Stopped:
                default:
                    packIconKind = PackIconKind.Play;
                    break;
            }

            var imgConv = new PackIconKindImageSourceConverter();

            return imgConv.Convert(packIconKind, targetType, parameter, culture);
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
