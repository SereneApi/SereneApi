using SereneApi.Core.Responses;
using System.Net;

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
