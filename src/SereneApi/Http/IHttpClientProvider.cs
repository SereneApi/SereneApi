using System.Net.Http;

namespace SereneApi.Http
{
    internal interface IHttpClientProvider
    {
        HttpClient GetHttpClient();
    }
}
