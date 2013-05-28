## Look With OSM .NET

Basic provider of OpenStreetMap data for Augmented Reality purposes.
It can detect what a person sees given his or her physical position and direction of a view.

Usage:

``` csharp
var lookWithOsm = new LookWithOsm("http://overpass-api.de/api/interpreter");
lookWithOsm.At(new GeoPoint(51.50918, -0.12129), Geometry.ToRad(90));
```