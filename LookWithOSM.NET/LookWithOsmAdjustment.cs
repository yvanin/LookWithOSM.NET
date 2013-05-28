namespace LookWithOSM.NET
{
    public struct LookWithOsmAdjustment
    {
        /// <summary>
        /// Height of an area requested from OSM
        /// </summary>
        public double BoundingBoxHeight;
        /// <summary>
        /// Width of an area requested from OSM
        /// </summary>
        public double BoundingBoxWidth;
        /// <summary>
        /// Length in meters of each step from viewer along bearing until the object is encountered
        /// </summary>
        public int TestByBearingStepLength;
        /// <summary>
        /// Distance in meters of acceptable deviation of a single object from a point on bearing
        /// </summary>
        public double MaxDeviationFromBearing;
        /// <summary>
        /// Distance in meters of acceptable deviation of a single object from a first point on bearing inside building
        /// </summary>
        public double MaxDeviationFromBearingBuildingIntersection;

        public static LookWithOsmAdjustment Default = new LookWithOsmAdjustment
            {
                BoundingBoxHeight = 500,
                BoundingBoxWidth = 500,
                TestByBearingStepLength = 3,
                MaxDeviationFromBearing = 10,
                MaxDeviationFromBearingBuildingIntersection = 10
            };
    }
}