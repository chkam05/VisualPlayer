﻿using chkam05.VisualPlayer.Data.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chkam05.VisualPlayer.Converters
{
    public class LyricsMatchTypeNameConverter : IValueConverter
    {

        //  CONST

        private static readonly Dictionary<LyricsMatchType, string> _mapping = new Dictionary<LyricsMatchType, string>
        {
            { LyricsMatchType.FILE_NAME, "File Name" },
            { LyricsMatchType.FILE_METADATA, "Metadata" },
            { LyricsMatchType.FILE_CONTENT, "Content" }
        };


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
            if (value != null)
            {
                var enumValue = (LyricsMatchType)value;
                return _mapping[enumValue];
            }

            return string.Empty;
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
            var stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue) && _mapping.Any(m => m.Value == stringValue))
                return _mapping.Where(m => m.Value == stringValue).First().Key;

            return LyricsMatchType.FILE_NAME;
        }
    }
}
