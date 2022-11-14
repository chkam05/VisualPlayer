using chkam05.VisualPlayer.Converters.Static;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class PlaybackStateControlDescriptionConverter : IValueConverter
    {

        //  CONST

        private static readonly string PlayDesc = "Play";
        private static readonly string PlayingDest = "Playing";
        private static readonly string PauseDest = "Pause";
        private static readonly string PausedDest = "Paused";
        private static readonly string StopDesc = "Stop";
        private static readonly string StoppedDesc = "Stopped";


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
            var param = (PlaybackStateControlIconConverterParameter?)parameter;

            switch (playbackState)
            {
                case PlaybackState.Playing:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PlayingDest;
                    }

                    return PauseDest;

                case PlaybackState.Paused:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return PausedDest;
                    }

                    return PlayDesc;

                case PlaybackState.Stopped:
                default:
                    if (param.HasValue)
                    {
                        if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                            return StoppedDesc;
                    }

                    return PlayDesc;
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
            var description = (string)value;
            var param = (PlaybackStateControlIconConverterParameter?)parameter;

            if (description == PlayingDest || description == PlayDesc)
            {
                if (param.HasValue)
                {
                    if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                        return PlaybackState.Playing;
                }

                return PlaybackState.Stopped;
            }
            else if (description == PausedDest || description == PauseDest)
            {
                if (param.HasValue)
                {
                    if (param.Value == PlaybackStateControlIconConverterParameter.STATE)
                        return PlaybackState.Paused;
                }

                return PlaybackState.Playing;
            }
            else
            {
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
