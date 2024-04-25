using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public static class StringUtilities
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if character is letter or number. </summary>
        /// <param name="c"> Character. </param>
        /// <returns> True - character is letter or number; False - otherwise. </returns>
        public static bool IsAlphaNumericChar(char c)
        {
            return (c >= 'A' && c <= 'Z')
                || (c >= 'a' && c <= 'z')
                || (c >= '0' && c <= '9');
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Limit text to only letters and numbers. </summary>
        /// <param name="text"> Text to limit. </param>
        /// <returns> Text with only letters and numbers. </returns>
        public static string LimitToAlphaNumeric(string text)
        {
            return string.Concat(text.ToList().FindAll(c => IsAlphaNumericChar(c)));
        }

    }
}
