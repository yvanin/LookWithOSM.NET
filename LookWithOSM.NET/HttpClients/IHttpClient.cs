namespace LookWithOSM.NET.HttpClients
{
    internal interface IHttpClient
    {
        string Post(string url, string data, params string[] headers);
    }
}