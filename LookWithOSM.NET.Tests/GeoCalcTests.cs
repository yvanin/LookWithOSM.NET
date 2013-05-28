using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LookWithOSM.NET.Tests
{
    [TestClass]
    public class GeoCalcTests
    {
        [TestMethod]
        public void GetBoundingBox()
        {
            var bBox = GeoCalc.GetBoundingBox(TestConstants.NEPoint, 2000, 2000);

            Assert.AreEqual(45, Geometry.ToBearingDegrees(GeoCalc.GetBearing(bBox.SouthWestVertex, TestConstants.NEPoint)), 1);
            Assert.AreEqual(225, Geometry.ToBearingDegrees(GeoCalc.GetBearing(bBox.NorthEastVertex, TestConstants.NEPoint)), 1);
            Assert.AreEqual(2828, GeoCalc.GetDistance(bBox.SouthWestVertex, bBox.NorthEastVertex), 3);
        }
    }
}