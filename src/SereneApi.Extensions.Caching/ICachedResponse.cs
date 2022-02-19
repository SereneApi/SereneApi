using System.Net.Http;

namespace SereneApi.Extensions.Caching
{
    public interface ICachedResponse
    {
        HttpResponseMessage GenerateHttpResponse();
    }
}