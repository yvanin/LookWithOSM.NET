using System.Collections.Generic;
using LookWithOSM.NET.Geography;

namespace LookWithOSM.NET.OSM
{
    /// <summary>
    /// OSM API Node
    /// </summary>
    internal class OsmNode : GeoPoint
    {
        public long Id { get; set; }
        public IDictionary<string, string> Tags { get; set; }
    }
}
