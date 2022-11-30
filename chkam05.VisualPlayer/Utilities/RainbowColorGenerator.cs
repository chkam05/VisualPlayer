using chkam05.VisualPlayer.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Utilities
{
    public static class RainbowColorGenerator
    {

        //  METHODS

        #region COLOR GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get next rainbow color. </summary>
        /// <param name="color"> Current color. </param>
        /// <param name="jump"> Value to jump in color rainbow palette. </param>
        /// <returns> Color. </returns>
        public static Color GetNextColor(Color color, int jump)
        {
            return GetNextColor(AHSLColor.FromColor(color), jump).ToColor();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get next rainbow color. </summary>
        /// <param name="color"> Current AHLS color. </param>
        /// <param name="jump"> Value to jump in color rainbow palette. </param>
        /// <returns> AHLS color. </returns>
        public static AHSLColor GetNextColor(AHSLColor color, int jump)
        {
            return ColorUtilities.UpdateColor(color, h: color.H + jump);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create rainbow linear gradient brush. </summary>
        /// <param name="color"> Gradient start color. </param>
        /// <param name="jump"> Value to jump in color rainbow palette. </param>
        /// <param name="size"> Size of object to fill with gradient. </param>
        /// <param name="maxSize"> Max side of object to fill gradient. </param>
        /// <returns> Linear gradient brush. </returns>
        public static LinearGradientBrush GetRainbowGradient(Color color, int jump, int size, double maxSize)
        {
            return GetRainbowGradient(AHSLColor.FromColor(color), jump, size, maxSize);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create rainbow linear gradient brush. </summary>
        /// <param name="color"> AHLS gradient start color. </param>
        /// <param name="jump"> Value to jump in color rainbow palette. </param>
        /// <param name="size"> Size of object to fill with gradient. </param>
        /// <param name="maxSize"> Max side of object to fill gradient. </param>
        /// <returns> Linear gradient brush. </returns>
        public static LinearGradientBrush GetRainbowGradient(AHSLColor color, int jump, int size, double maxSize, bool reverse = false)
        {
            var limit = Math.Min(AHSLColor.HUE_MAX, jump);

            int checkPointSize = (int)(maxSize / (4 * (maxSize / AHSLColor.HUE_MAX + 1)));
            int checkPointHue = (int)(checkPointSize * limit / maxSize);
            int partHue = (int)(checkPointSize * limit / maxSize);
            int finalHue = (int)(size * limit / maxSize);

            var grandients = new GradientStopCollection()
            {
                new GradientStop(color.ToColor(), 0.0),
            };

            for (int i = 0; i < (int)(size / checkPointSize); i++)
            {
                grandients.Add(new GradientStop(
                    ColorUtilities.UpdateColor(color, h: color.H + partHue * i).ToColor(),
                    CalculateGradientPoint(checkPointSize * i, size)));
            }

            grandients.Add(new GradientStop(
                ColorUtilities.UpdateColor(color, h: color.H + finalHue).ToColor(), 1.0));

            double startY = reverse ? 0.0 : 1.0;
            double endY = reverse ? 1.0 : 0.0;

            return new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, startY),
                EndPoint = new Point(0.5, endY),
                GradientStops = grandients
            };
        }

        #endregion COLOR GET METHODS

        #region GRADIENT UTILITIES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate gradient point in object. </summary>
        /// <param name="point"> Point in object. </param>
        /// <param name="size"> Object size. </param>
        /// <returns> Gradient point in object. </returns>
        private static double CalculateGradientPoint(int point, int size)
        {
            return 100d * point / size / 100;
        }

        #endregion GRADIENT UTILITIES METHODS

    }
}
