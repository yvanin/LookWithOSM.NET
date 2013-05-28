namespace LookWithOSM.NET
{
    internal static class Constants
    {
        public static class OsmXml
        {
            public static class Tags
            {
                public const string Member = "member";
                public const string Nd = "nd";
                public const string Node = "node";
                public const string Relation = "relation";
                public const string Tag = "tag";
                public const string Way = "way";
            }

            public static class Attributes
            {
                public const string Id = "id";
                public const string K = "k";
                public const string V = "v";
                public const string Lat = "lat";
                public const string Lon = "lon";
                public const string Ref = "ref";
                public const string Role = "role";
            }

            public static class Values
            {
                public const string AddrHousenumber = "addr:housenumber";
                public const string AddrStreet = "addr:street";
                public const string Amenity = "amenity";
                public const string House = "house";
                public const string Name = "name";
                public const string Street = "street";
                public const string Type = "type";
            }
        }
    }
}