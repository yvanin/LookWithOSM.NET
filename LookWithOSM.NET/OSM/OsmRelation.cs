using System.Collections.Generic;

namespace LookWithOSM.NET.OSM
{
    /// <summary>
    /// OSM API Relation
    /// </summary>
    internal class OsmRelation
    {
        public long Id { get; set; }
        public ICollection<long> MemberIds { get; set; }
        public IDictionary<string, string> Tags { get; set; }
    }
}
