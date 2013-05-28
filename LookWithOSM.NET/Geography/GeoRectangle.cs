namespace LookWithOSM.NET.Geography
{
    /// <summary>
    /// A rectangle on Earth surface
    /// </summary>
    public class GeoRectangle
    {
        public GeoPoint SouthWestVertex { get; set; }
        public GeoPoint NorthEastVertex { get; set; }
    }
}
