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
    public class PlaybackStateControlIconConverter : IValueConverter
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
            var playbackState = (PlaybackState) value;
            var param = (PlaybackStateControlIconConverterParameter?) parameter;

            switch (playbackState)
            {
                case PlaybackState.Playing:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PackIconKind.Play;
                    }

                    return PackIconKind.Pause;

                case PlaybackState.Paused:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PackIconKind.Pause;
                    }

                    return PackIconKind.Play;

                case PlaybackState.Stopped:
                default:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PackIconKind.Stop;
                    }

                    return PackIconKind.Play;
            }
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
            var packIconKind = (PackIconKind)value;
            var param = (PlaybackStateControlIconConverterParameter?)parameter;

            switch (packIconKind)
            {
                case PackIconKind.Play:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PlaybackState.Playing;
                    }

                    return PlaybackState.Stopped;

                case PackIconKind.Pause:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PlaybackState.Paused;
                    }

                    return PlaybackState.Playing;

                case PackIconKind.Stop:
                default:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PlaybackState.Stopped;
                    }

                    return PlaybackState.Playing;
            }
        }

    }
}
