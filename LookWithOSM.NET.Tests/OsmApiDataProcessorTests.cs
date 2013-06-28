using System;
using System.Collections.Generic;
using System.Linq;
using LookWithOSM.NET.ApiClients;
using LookWithOSM.NET.Geography;
using LookWithOSM.NET.OSM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LookWithOSM.NET.Tests
{
    [TestClass]
    [DeploymentItem("TestData.xml")]
    public class OsmApiDataProcessorTests
    {
        private const double Epsilon = 1;

        [TestMethod]
        public void GetObjectsOfImportance()
        {
            var boundingBox = new GeoRectangle
                {
                    NorthEastVertex = new GeoPoint(),
                    SouthWestVertex = new GeoPoint()
                };
            var xmlData = new OverpassApiClient(new TestHttpClient()).GetDataInBoundingBox(null, boundingBox);
            var objects = OsmApiDataProcessor.GetObjectsOfImportance(xmlData);

            AssertSingleNodes(objects.SingleNodes);
            AssertBuildingsWays(objects.BuildingsWays);
        }

        private static void AssertSingleNodes(ICollection<OsmNode> singleNodes)
        {
            Assert.IsNotNull(singleNodes);
            Assert.IsTrue(singleNodes.Count > 0);
            Assert.IsTrue(singleNodes.All(x => Math.Abs(x.Latitude - 0) > Epsilon && Math.Abs(x.Longitude - 0) > Epsilon));
            Assert.IsTrue(singleNodes.All(x => x.Tags != null));
            Assert.IsTrue(singleNodes.All(x => x.Tags.ContainsKey(Constants.OsmXml.Values.Amenity)));
        }

        private static void AssertBuildingsWays(ICollection<OsmWay> buildingsWays)
        {
            Assert.IsNotNull(buildingsWays);
            Assert.IsTrue(buildingsWays.Count > 0);
            Assert.IsTrue(buildingsWays.All(x => x.Nodes != null));
            Assert.IsTrue(buildingsWays.All(x => x.Nodes.Count > 2));
            Assert.IsTrue(buildingsWays.All(x => x.Nodes.All(y => Math.Abs(y.Latitude - 0) > Epsilon && Math.Abs(y.Longitude - 0) > Epsilon)));
            Assert.IsTrue(buildingsWays.All(x => x.Tags != null));
            Assert.IsTrue(buildingsWays.All(x => x.Tags.ContainsKey(Constants.OsmXml.Values.AddrHousenumber)));
            Assert.IsTrue(buildingsWays.All(x => x.Tags.ContainsKey(Constants.OsmXml.Values.AddrStreet)));
        }
    }
}