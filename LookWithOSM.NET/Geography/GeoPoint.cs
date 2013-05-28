namespace LookWithOSM.NET.Geography
{
    /// <summary>
    /// A point on Earth with coordinates
    /// </summary>
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPoint()
        {
        }

        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
