using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualPlayer.Utilities
{
    public static class MathHelper
    {

        //  METHODS

        #region GEOMETRY

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

        #endregion GEOMETRY

        #region RANGE CHECK METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if double value is in range. </summary>
        /// <param name="value"> Value. </param>
        /// <param name="min"> Min available value. </param>
        /// <param name="max"> Max available value. </param>
        /// <returns> True - value is in range; False - otherwise. </returns>
        public static bool IsInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if float value is in range. </summary>
        /// <param name="value"> Value. </param>
        /// <param name="min"> Min available value. </param>
        /// <param name="max"> Max available value. </param>
        /// <returns> True - value is in range; False - otherwise. </returns>
        public static bool IsInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if int value is in range. </summary>
        /// <param name="value"> Value. </param>
        /// <param name="min"> Min available value. </param>
        /// <param name="max"> Max available value. </param>
        /// <returns> True - value is in range; False - otherwise. </returns>
        public static bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if long value is in range. </summary>
        /// <param name="value"> Value. </param>
        /// <param name="min"> Min available value. </param>
        /// <param name="max"> Max available value. </param>
        /// <returns> True - value is in range; False - otherwise. </returns>
        public static bool IsInRange(long value, long min, long max)
        {
            return value >= min && value <= max;
        }

        #endregion RANGE CHECK METHODS

    }
}
