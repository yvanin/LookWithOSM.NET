using System.Xml.Linq;
using LookWithOSM.NET.Geography;
using LookWithOSM.NET.HttpClients;

namespace LookWithOSM.NET.ApiClients
{
    /// <summary>
    /// Overpass API Client
    /// </summary>
    internal class OverpassApiClient : IOsmApiClient
    {
        private IHttpClient HttpClient { get; set; }

        public OverpassApiClient() : this(new DefaultHttpClient())
        {
        }

        public OverpassApiClient(IHttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public XDocument GetDataInBoundingBox(string apiUrl, GeoRectangle boundingBox)
        {
            var xmlQuery = PrepareQueryForObjectsInBoundingBox(boundingBox);

            try
            {
                var apiResponse = HttpClient.Post(apiUrl, xmlQuery.ToString(),
                                                  "Content-Type: application/xml; charset=utf-8");
                return XDocument.Parse(apiResponse);
            }
            catch
            {
                throw new OsmApiException();
            }
        }

        /// <example>
        /*
        <osm-script>
          <union into="_">
            <bbox-query e="36.2344722451331" n="49.99447425225" s="49.9899697477477" w="36.2274857548669"/>
            <recurse into="x" type="node-relation"/>
            <query type="way">
              <bbox-query e="36.2344722451331" n="49.99447425225" s="49.9899697477477" w="36.2274857548669"/>
            </query>
            <recurse into="x" type="way-node"/>
            <recurse type="way-relation"/>
          </union>
          <print/>
        </osm-script>
        */
        /// </example>
        private static XDocument PrepareQueryForObjectsInBoundingBox(GeoRectangle boundingBox)
        {
            return new XDocument(
                new XElement("osm-script",
                             new XElement("union",
                                          new XAttribute("into", "_"),
                                          new XElement("bbox-query",
                                                       new XAttribute("w", boundingBox.SouthWestVertex.Longitude),
                                                       new XAttribute("s", boundingBox.SouthWestVertex.Latitude),
                                                       new XAttribute("e", boundingBox.NorthEastVertex.Longitude),
                                                       new XAttribute("n", boundingBox.NorthEastVertex.Latitude)),
                                          new XElement("recurse",
                                                       new XAttribute("into", "x"),
                                                       new XAttribute("type", "node-relation")),
                                          new XElement("query",
                                                       new XAttribute("type", "way"),
                                                       new XElement("bbox-query",
                                                                    new XAttribute("w",
                                                                                   boundingBox.SouthWestVertex.Longitude),
                                                                    new XAttribute("s",
                                                                                   boundingBox.SouthWestVertex.Latitude),
                                                                    new XAttribute("e",
                                                                                   boundingBox.NorthEastVertex.Longitude),
                                                                    new XAttribute("n",
                                                                                   boundingBox.NorthEastVertex.Latitude))),
                                          new XElement("recurse",
                                                       new XAttribute("into", "x"),
                                                       new XAttribute("type", "way-node")),
                                          new XElement("recurse",
                                                       new XAttribute("type", "way-relation"))),
                             new XElement("print")));
        }
    }
}