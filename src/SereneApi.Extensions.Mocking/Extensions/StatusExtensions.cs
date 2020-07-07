using System.Net;
using SereneApi.Abstractions.Response;

namespace SereneApi.Extensions.Mocking.Extensions
{
    internal static class StatusExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this Status source)
        {
            return (HttpStatusCode)(int)source;
        }
    }
}
