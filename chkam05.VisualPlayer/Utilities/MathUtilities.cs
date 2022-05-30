using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chkam05.VisualPlayer.Utilities
{
    public static class MathUtilities
    {

        //  METHODS

        #region CIRCLE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert degrees to radians. </summary>
        /// <param name="degrees"> Degrees. </param>
        /// <returns> Radians. </returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert radians to degrees. </summary>
        /// <param name="radians"> Radians. </param>
        /// <returns> Degrees. </returns>
        public static double ConvertRadiansToDegrees(double radians)
        {
            return (180 / Math.PI) * radians;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Find point on circle with radius and center point by angle. </summary>
        /// <param name="centerPoint"> Center point. </param>
        /// <param name="radius"> Circle radius. </param>
        /// <param name="angle"> Angle. </param>
        /// <returns> Point on circle. </returns>
        public static Point FindPointOnCircle(Point centerPoint, double radius, double angle)
        {
            var radians = ConvertDegreesToRadians(angle);

            return new Point(
                centerPoint.X + (radius * Math.Cos(radians)),
                centerPoint.Y + (radius * Math.Sin(radians)));
        }

        #endregion CIRCLE METHODS

        #region SCALING METHODS

        //  --------------------------------------------------------------------------------
        public static double CalculateTransformScale(Size oryginalSize, Size destSize)
        {
            double x = (destSize.Width * 100 / oryginalSize.Width) / 100;
            double y = (destSize.Height * 100 / oryginalSize.Height) / 100;

            return Math.Min(x, y);
        }

        #endregion SCALING METHODS

    }
}
