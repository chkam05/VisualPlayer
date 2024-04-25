using System.Windows.Media;

namespace VisualPlayer.Utilities
{
    public static class ColorsHelper
    {

        //  METHODS

        #region CONVERSION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert color to hexadecimal color code. </summary>
        /// <param name="color"> Color. </param>
        /// <returns> Hexadecimal color code. </returns>
        public static string GetColorCode(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create color from hexadecimal color code. </summary>
        /// <param name="colorCode"> Hexadecimal color code. </param>
        /// <returns> Color. </returns>
        public static Color GetColorFromCode(string colorCode)
        {
            return (Color)ColorConverter.ConvertFromString(colorCode);
        }

        #endregion CONVERSION METHODS

    }
}
