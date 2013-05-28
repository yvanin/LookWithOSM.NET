using System.Net;

namespace LookWithOSM.NET.HttpClients
{
    internal class DefaultHttpClient : IHttpClient
    {
        public string Post(string url, string data, params string[] headers)
        {
            using (var webClient = new WebClient())
            {
                foreach (var header in headers)
                {
                    webClient.Headers.Add(header);
                }
                return webClient.UploadString(url, data);
            }
        }
    }
}