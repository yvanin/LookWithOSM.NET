using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LookWithOSM.NET.Geography;

namespace LookWithOSM.NET
{
    /// <summary>
    /// Algorithms for calculations with locations on Earth surface
    /// </summary>
    internal static class GeoCalc
    {
        private const double EarthRadius = 6371000;

        /// <summary>
        /// Returns length of 1° of latitude on Earth surface in meters
        /// </summary>
        private static double LengthOfLatitudeDegree(double latitude)
        {
            return 111000;
        }

        /// <summary>
        /// Returns length of 1° of longitude on Earth surface in meters
        /// </summary>
        private static double LengthOfLongitudeDegree(double latitude)
        {
            return 111320*Math.Cos(Geometry.ToRad(latitude));
        }

        /// <summary>
        /// Calculates bounding box given a center point and the dimensions
        /// </summary>
        /// <param name="centerPoint">coordinates of a center point</param>
        /// <param name="height">bounding box's height in meters</param>
        /// <param name="width">bounding box's width in meters</param>
        public static GeoRectangle GetBoundingBox(GeoPoint centerPoint, double height, double width)
        {
            Debug.Assert(centerPoint != null, "centerPoint == null");

            return new GeoRectangle
                {
                    SouthWestVertex =
                        new GeoPoint(centerPoint.Latitude - ((height/2)/LengthOfLatitudeDegree(centerPoint.Latitude)),
                                     centerPoint.Longitude - ((width/2)/LengthOfLongitudeDegree(centerPoint.Latitude))),
                    NorthEastVertex =
                        new GeoPoint(centerPoint.Latitude + ((height/2)/LengthOfLatitudeDegree(centerPoint.Latitude)),
                                     centerPoint.Longitude + ((width/2)/LengthOfLongitudeDegree(centerPoint.Latitude)))
                };
        }

        /// <summary>
        /// Calculates constant bearing (rhumb line) in radians between two points
        /// source: http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        public static double GetBearing(GeoPoint point1, GeoPoint point2)
        {
            Debug.Assert(point1 != null, "point1 == null");
            Debug.Assert(point2 != null, "point2 == null");

            var dLon = Geometry.ToRad(point2.Longitude - point1.Longitude);
            var dPhi = Math.Log(Math.Tan(Geometry.ToRad(point2.Latitude)/2 + Math.PI/4)/
                                Math.Tan(Geometry.ToRad(point1.Latitude)/2 + Math.PI/4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2*Math.PI - dLon) : (2*Math.PI + dLon);
            return Math.Atan2(dLon, dPhi);
        }

        /// <summary>
        /// Calculates distance between two points
        /// source: http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        public static double GetDistance(GeoPoint point1, GeoPoint point2)
        {
            Debug.Assert(point1 != null, "point1 == null");
            Debug.Assert(point2 != null, "point2 == null");

            var dLat = Geometry.ToRad(point2.Latitude - point1.Latitude);
            var dLon = Geometry.ToRad(point2.Longitude - point1.Longitude);
            var lat1 = Geometry.ToRad(point1.Latitude);
            var lat2 = Geometry.ToRad(point2.Latitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadius * c;
        }

        /// <summary>
        /// Calculates the coordinates of a destination point given a start point and a distance along constant bearing
        /// source: http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startPoint">coordinates of a start point</param>
        /// <param name="bearing">constant bearing in radians</param>
        /// <param name="distance">distance in meters</param>
        public static GeoPoint GetDestination(GeoPoint startPoint, double bearing, double distance)
        {
            Debug.Assert(startPoint != null, "startPoint == null");

            var startLat = Geometry.ToRad(startPoint.Latitude);
            var startLong = Geometry.ToRad(startPoint.Longitude);

            var destLat = Math.Asin(Math.Sin(startLat)*Math.Cos(distance/EarthRadius)
                                    + Math.Cos(startLat)*Math.Sin(distance/EarthRadius)*Math.Cos(bearing));
            var destLong = startLong + Math.Atan2(Math.Sin(bearing)*Math.Sin(distance/EarthRadius)*Math.Cos(startLat),
                                                  Math.Cos(distance/EarthRadius) - Math.Sin(startLat)*Math.Sin(destLat));

            return new GeoPoint
                {
                    Latitude = Geometry.ToDegrees(destLat),
                    Longitude = Geometry.ToDegrees(destLong)
                };
        }

        /// <summary>
        /// Determines whether a point is located inside a polygon
        /// source: http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// </summary>
        public static bool IsPointInsidePolygon(GeoPoint point, IEnumerable<GeoPoint> polygonVertices)
        {
            Debug.Assert(point != null, "point == null");
            Debug.Assert(polygonVertices != null, "polygonVertices == null");

            var polygon = polygonVertices.ToArray();

            /* Preliminary Check */
            /* Check if a point is within the boundaries of a polygon */
            var minLat = polygon[0].Latitude;
            var maxLat = minLat;
            var minLong = polygon[0].Longitude;
            var maxLong = minLong;

            foreach (var vertex in polygon)
            {
                if (vertex.Latitude < minLat)
                    minLat = vertex.Latitude;
                else if (vertex.Latitude > maxLat)
                    maxLat = vertex.Latitude;

                if (vertex.Longitude < minLong)
                    minLong = vertex.Longitude;
                else if (vertex.Longitude > maxLong)
                    maxLong = vertex.Longitude;
            }

            if (point.Latitude < minLat || point.Latitude > maxLat
                || point.Longitude < minLong || point.Longitude > maxLong)
                return false;

            /* Core Check */
            var inside = false;

            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Latitude > point.Latitude) != (polygon[j].Latitude > point.Latitude)) &&
                    (point.Longitude < (polygon[j].Longitude - polygon[i].Longitude)*(point.Latitude - polygon[i].Latitude)/
                                       (polygon[j].Latitude - polygon[i].Latitude) + polygon[i].Longitude))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}