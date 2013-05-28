using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using LookWithOSM.NET.OSM;

namespace LookWithOSM.NET
{
    internal static class OsmApiDataProcessor
    {
        /// <summary>
        /// Extract nodes and ways of importance from OSM API Response XML
        /// </summary>
        public static OsmObjectsOfImportance GetObjectsOfImportance(XDocument xmlData)
        {
            var extractSingleNodesTask = Task.Factory.StartNew(() => ExtractSingleNodes(xmlData));
            var osmWays = ExtractBuildingWays(xmlData);

            return new OsmObjectsOfImportance
            {
                SingleNodes = extractSingleNodesTask.Result,
                BuildingWays = osmWays
            };
        }

        /// <summary>
        /// Extracts all Nodes of importance, that are not included in any Way, from OSM API Response XML
        /// </summary>
        private static ICollection<OsmNode> ExtractSingleNodes(XDocument xmlData)
        {
            Debug.Assert(xmlData.Root != null, "xmlData.Root == null");

            var nodes = new List<OsmNode>();
            foreach (var nodeElement in xmlData.Root.Descendants(Constants.OsmXml.Tags.Node))
            {
                var tags = nodeElement.Elements(Constants.OsmXml.Tags.Tag).ToArray();
                if (tags.All(t => t.Attribute(Constants.OsmXml.Attributes.K).Value != Constants.OsmXml.Values.Amenity))
                    continue;

                var osmNode = new OsmNode
                {
                    Id = Convert.ToInt64(nodeElement.Attribute(Constants.OsmXml.Attributes.Id).Value),
                    Latitude = Convert.ToDouble(nodeElement.Attribute(Constants.OsmXml.Attributes.Lat).Value),
                    Longitude = Convert.ToDouble(nodeElement.Attribute(Constants.OsmXml.Attributes.Lon).Value),
                    Tags = new Dictionary<string, string>()
                };
                foreach (var tagElement in tags)
                {
                    osmNode.Tags.Add(tagElement.Attribute(Constants.OsmXml.Attributes.K).Value,
                                     tagElement.Attribute(Constants.OsmXml.Attributes.V).Value);
                }
                nodes.Add(osmNode);
            }
            return nodes;
        }

        /// <summary>
        /// Extracts all Ways of buildings from OSM API Response XML
        /// </summary>
        private static ICollection<OsmWay> ExtractBuildingWays(XDocument xmlData)
        {
            Debug.Assert(xmlData.Root != null, "xmlData.Root == null");

            var extractAllNodesTask = Task.Factory.StartNew(() => ExtractAllNodes(xmlData));
            var extractStreetRelationsTask = Task.Factory.StartNew(() => ExtractStreetRelations(xmlData));
            Task.WaitAll(extractAllNodesTask, extractStreetRelationsTask);

            var osmNodes = extractAllNodesTask.Result;
            var osmStreetRelations = extractStreetRelationsTask.Result;

            var osmWays = new List<OsmWay>();
            foreach (var wayElement in xmlData.Root.Descendants(Constants.OsmXml.Tags.Way))
            {
                var tags = wayElement.Elements(Constants.OsmXml.Tags.Tag).ToArray();
                if (tags.All(
                    t => t.Attribute(Constants.OsmXml.Attributes.K).Value != Constants.OsmXml.Values.AddrHousenumber))
                    continue;

                var osmWay = new OsmWay
                {
                    Id = Convert.ToInt64(wayElement.Attribute(Constants.OsmXml.Attributes.Id).Value),
                    Nodes = new List<OsmNode>(),
                    Tags = new Dictionary<string, string>()
                };
                foreach (var tagElement in tags)
                {
                    osmWay.Tags.Add(tagElement.Attribute(Constants.OsmXml.Attributes.K).Value,
                                    tagElement.Attribute(Constants.OsmXml.Attributes.V).Value);
                }
                foreach (var nodeElement in wayElement.Elements(Constants.OsmXml.Tags.Nd))
                {
                    osmWay.Nodes.Add(
                        osmNodes[Convert.ToInt64(nodeElement.Attribute(Constants.OsmXml.Attributes.Ref).Value)]);
                }

                if (!osmWay.Tags.ContainsKey(Constants.OsmXml.Values.AddrStreet))
                {
                    var streetRelation = osmStreetRelations.First(x => x.MemberIds.Contains(osmWay.Id));
                    osmWay.Tags.Add(Constants.OsmXml.Values.AddrStreet,
                                    streetRelation.Tags[Constants.OsmXml.Values.Name]);
                }

                osmWays.Add(osmWay);
            }
            return osmWays;
        }

        /// <summary>
        /// Extracts all Nodes from OSM API Response XML
        /// </summary>
        private static IDictionary<long, OsmNode> ExtractAllNodes(XDocument xmlData)
        {
            Debug.Assert(xmlData.Root != null, "xmlData.Root == null");

            var osmNodes = new SortedDictionary<long, OsmNode>();
            foreach (var nodeElement in xmlData.Root.Descendants(Constants.OsmXml.Tags.Node))
            {
                var osmNode = new OsmNode
                {
                    Id = Convert.ToInt64(nodeElement.Attribute(Constants.OsmXml.Attributes.Id).Value),
                    Latitude = Convert.ToDouble(nodeElement.Attribute(Constants.OsmXml.Attributes.Lat).Value),
                    Longitude = Convert.ToDouble(nodeElement.Attribute(Constants.OsmXml.Attributes.Lon).Value)
                };
                osmNodes.Add(osmNode.Id, osmNode);
            }
            return osmNodes;
        }

        /// <summary>
        /// Extracts all Relations of type "street" from OSM API Response XML
        /// </summary>
        private static List<OsmRelation> ExtractStreetRelations(XDocument xmlData)
        {
            Debug.Assert(xmlData.Root != null, "xmlData.Root == null");

            var osmStreetRelations = new List<OsmRelation>();
            foreach (var relationElement in xmlData.Root.Descendants(Constants.OsmXml.Tags.Relation))
            {
                var relationTags = relationElement.Elements(Constants.OsmXml.Tags.Tag).ToArray();
                if (!relationTags.Any(t =>
                                      t.Attribute(Constants.OsmXml.Attributes.K).Value == Constants.OsmXml.Values.Type &&
                                      t.Attribute(Constants.OsmXml.Attributes.V).Value == Constants.OsmXml.Values.Street))
                    continue;

                var osmRelation = new OsmRelation
                {
                    Id = Convert.ToInt64(relationElement.Attribute(Constants.OsmXml.Attributes.Id).Value),
                    MemberIds = new SortedSet<long>(),
                    Tags = new Dictionary<string, string>()
                };
                foreach (var memberElement in relationElement.Elements(Constants.OsmXml.Tags.Member))
                {
                    if (memberElement.Attribute(Constants.OsmXml.Attributes.Role).Value == Constants.OsmXml.Values.House)
                    {
                        osmRelation.MemberIds.Add(
                            Convert.ToInt64(memberElement.Attribute(Constants.OsmXml.Attributes.Ref).Value));
                    }
                }
                foreach (var tagElement in relationTags)
                {
                    osmRelation.Tags.Add(tagElement.Attribute(Constants.OsmXml.Attributes.K).Value,
                                         tagElement.Attribute(Constants.OsmXml.Attributes.V).Value);
                }
                osmStreetRelations.Add(osmRelation);
            }
            return osmStreetRelations;
        }
    }
}
