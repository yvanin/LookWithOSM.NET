using System.Collections.Generic;

namespace LookWithOSM.NET.OSM
{
    /// <summary>
    /// OSM API Way
    /// </summary>
    internal class OsmWay
    {
        public long Id { get; set; }
        public IList<OsmNode> Nodes { get; set; }
        public IDictionary<string, string> Tags { get; set; }
    }
}
