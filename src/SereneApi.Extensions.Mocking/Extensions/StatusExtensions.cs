
using SereneApi.Abstractions.Responses;
using System.Net;

// Do note change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Abstractions.Enums
{
    internal static class StatusExtensions
    {
        public static HttpStatusCode ToHttpStatusCode(this Status source)
        {
            return (HttpStatusCode)(int)source;
        }
    }
}
