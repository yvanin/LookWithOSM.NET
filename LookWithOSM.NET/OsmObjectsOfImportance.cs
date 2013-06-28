using System.Collections.Generic;
using LookWithOSM.NET.OSM;

namespace LookWithOSM.NET
{
    /// <summary>
    /// Collection of OSM API objects used for further processing
    /// </summary>
    internal class OsmObjectsOfImportance
    {
        /// <summary>
        /// Nodes of importance that are not included in any Way
        /// </summary>
        public ICollection<OsmNode> SingleNodes { get; set; }
        /// <summary>
        /// Ways of buildings
        /// </summary>
        public ICollection<OsmWay> BuildingsWays { get; set; }
    }
}
