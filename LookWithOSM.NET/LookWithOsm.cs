using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookWithOSM.NET.ApiClients;
using LookWithOSM.NET.Geography;
using LookWithOSM.NET.OSM;

namespace LookWithOSM.NET
{
    /// <summary>
    /// OpenStreetMap data provider for Augmented Reality
    /// </summary>
    public class LookWithOsm
    {
        private string ApiUrl { get; set; }
        private IOsmApiClient ApiClient { get; set; }
        private LookWithOsmAdjustment Adjustment { get; set; }

        public LookWithOsm(string apiUrl)
            : this(apiUrl, SupportedApi.OverpassApi, LookWithOsmAdjustment.Default)
        {
        }

        public LookWithOsm(string apiUrl, LookWithOsmAdjustment adjustment) :
            this(apiUrl, SupportedApi.OverpassApi, adjustment)
        {
        }

        internal LookWithOsm(string apiUrl, SupportedApi api, LookWithOsmAdjustment adjustment)
        {
            ApiUrl = apiUrl;
            switch (api)
            {
                case SupportedApi.OverpassApi:
                    ApiClient = new OverpassApiClient();
                    break;
            }
            Adjustment = adjustment;
        }

        internal LookWithOsm(string apiUrl, LookWithOsmAdjustment adjustment, IOsmApiClient apiClient)
        {
            ApiUrl = apiUrl;
            ApiClient = apiClient;
            Adjustment = adjustment;
        }

        /// <summary>
        /// Determines what a person sees given person's position and direction of a view
        /// </summary>
        /// <param name="position">coordinates of a viewer</param>
        /// <param name="bearing">direction at which viewer looks (bearing) in radians</param>
        /// <returns>list of objects' descriptions</returns>
        public IEnumerable<string> At(GeoPoint position, double bearing)
        {
            var boundingBox = GeoCalc.GetBoundingBox(position, Adjustment.BoundingBoxHeight, Adjustment.BoundingBoxWidth);
            var mapData = ApiClient.GetDataInBoundingBox(ApiUrl, boundingBox);
            var objectsOfImportance = OsmApiDataProcessor.GetObjectsOfImportance(mapData);
            return FilterObjectsByPositionAndBearing(objectsOfImportance, position, bearing);
        }

        /// <summary>
        /// Filters out only objects in a person's direction of a view
        /// </summary>
        internal IEnumerable<string> FilterObjectsByPositionAndBearing(OsmObjectsOfImportance objects, GeoPoint position, double bearing)
        {
            var testingPoints = GetPointsAlongBearing(position, bearing);

            // filter objects
            var filterSingleNodesTask = Task.Factory.StartNew(() => FilterSingleNodes(objects.SingleNodes, testingPoints));
            var filterBuildingWaysTask = Task.Factory.StartNew(() =>
            {
                // filter buildings' ways by testing if some point along a person's direction of view is inside the way
                foreach (var point in testingPoints)
                {
                    foreach (var way in objects.BuildingWays)
                    {
                        if (GeoCalc.IsPointInsidePolygon(point, way.Nodes))
                            return new {TestingPoint = point, Way = way};
                    }
                }
                return null;
            });
            Task.WaitAll(filterSingleNodesTask, filterBuildingWaysTask);

            // print out objects' descriptions
            var building = filterBuildingWaysTask.Result;
            if (building != null)
            {
                yield return BuildingWayDescription(building.Way);
            }
            foreach (var node in filterSingleNodesTask.Result)
            {
                if (building == null ||
                    GeoCalc.GetDistance(node, building.TestingPoint) < Adjustment.MaxDeviationFromBearingBuildingIntersection)
                    yield return SingleNodeDescription(node);
            }
        }

        /// <summary>
        /// Returns a collection of physical locations along a person's direction of a view ordered by distance (ascending)
        /// </summary>
        private IEnumerable<GeoPoint> GetPointsAlongBearing(GeoPoint position, double bearing)
        {
            var pointsCount =
                (int)
                Math.Sqrt(Adjustment.BoundingBoxHeight*Adjustment.BoundingBoxHeight +
                          Adjustment.BoundingBoxWidth*Adjustment.BoundingBoxWidth)/2*Adjustment.TestByBearingStepLength;

            var points = new GeoPoint[pointsCount];
            for (var i = 0; i < pointsCount; i++)
            {
                var destination = GeoCalc.GetDestination(position, bearing, Adjustment.TestByBearingStepLength * i + 1);
                points[i] = new GeoPoint(destination.Latitude, destination.Longitude);
            }

            return points;
        }

        /// <summary>
        /// Filters single nodes by testing whether they are close to the points along a person's direction of view
        /// </summary>
        private ICollection<OsmNode> FilterSingleNodes(ICollection<OsmNode> singleNodes, IEnumerable<GeoPoint> testingPoints)
        {
            var result = new List<OsmNode>();
            foreach (var point in testingPoints)
            {
                foreach (var node in singleNodes)
                {
                    if (!result.Contains(node) && GeoCalc.GetDistance(point, node) < Adjustment.MaxDeviationFromBearing)
                    {
                        result.Add(node);
                    }
                }
            }
            return result;
        }

        private static string BuildingWayDescription(OsmWay buildingWay)
        {
            return String.Format("{0} {1}", buildingWay.Tags[Constants.OsmXml.Values.AddrHousenumber],
                                 buildingWay.Tags[Constants.OsmXml.Values.AddrStreet]);
        }

        private static string SingleNodeDescription(OsmNode node)
        {
            var result = node.Tags[Constants.OsmXml.Values.Amenity];
            if (node.Tags.ContainsKey(Constants.OsmXml.Values.Name))
                result += " " + node.Tags[Constants.OsmXml.Values.Name];
            else if (node.Tags.Count == 2)
                result += " " + node.Tags.First(x => x.Key != Constants.OsmXml.Values.Amenity).Value;
            return result;
        }
    }
}