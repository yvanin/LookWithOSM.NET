using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LookWithOSM.NET.Tests
{
    [TestClass]
    public class SystemTests
    {
        private const string OverpassApiUrl = "http://overpass-api.de/api/interpreter";

        [TestMethod]
        public void LookAt_North_East()
        {
            var lookWithOsm = new LookWithOsm(OverpassApiUrl);

            var objects = lookWithOsm.At(TestConstants.NEPoint, Geometry.ToRad(35)).ToArray();

            Assert.IsNotNull(objects);
            Assert.AreEqual(2, objects.Length);
        }
    }
}