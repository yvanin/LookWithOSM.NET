using System;

namespace LookWithOSM.NET
{
    /// <summary>
    /// Geometric calculations
    /// </summary>
    internal static class Geometry
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// Convert radians to degrees (as bearing: 0°...360°)
        /// </summary>
        public static double ToBearingDegrees(double radians)
        {
            return (ToDegrees(radians) + 360) % 360;
        }
    }
}
