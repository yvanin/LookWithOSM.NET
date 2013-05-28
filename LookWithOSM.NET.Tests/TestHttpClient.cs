using System.IO;
using LookWithOSM.NET.HttpClients;

namespace LookWithOSM.NET.Tests
{
    public class TestHttpClient : IHttpClient
    {
        public string Post(string url, string data, params string[] headers)
        {
            return File.ReadAllText("TestData.xml");
        }
    }
}