using SereneApi.Core.Http.Responses;
using System.Net;

namespace SereneApi.Extensions.Mocking.Rest.Extensions
{
    internal static class StatusExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this Status source)
        {
            return (HttpStatusCode)(int)source;
        }
    }
}