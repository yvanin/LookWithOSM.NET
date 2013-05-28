using System.Linq;
using LookWithOSM.NET.ApiClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LookWithOSM.NET.Tests
{
    [TestClass]
    [DeploymentItem("TestData.xml")]
    public class LookWithOsmTests
    {
        [TestMethod]
        public void At()
        {
            var lookWithOsm = new LookWithOsm(null, LookWithOsmAdjustment.Default, new OverpassApiClient(new TestHttpClient()));

            var objects = lookWithOsm.At(TestConstants.NEPoint, Geometry.ToRad(35)).ToArray();

            Assert.IsNotNull(objects);
            Assert.AreEqual(2, objects.Length);
        }
    }
}