using System.Xml.Linq;
using LookWithOSM.NET.Geography;

namespace LookWithOSM.NET.ApiClients
{
    /// <summary>
    /// A client of OSM API
    /// </summary>
    internal interface IOsmApiClient
    {
        /// <summary>
        /// Gets all structures in a bounding box
        /// </summary>
        XDocument GetDataInBoundingBox(string apiUrl, GeoRectangle boundingBox);
    }
}