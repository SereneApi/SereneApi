
using System.Net;

// Do note change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Abstraction.Enums
{
    public static class StatusExtensions
    {
        public static bool IsSuccessCode(this Status status)
        {
            return status == Status.Ok ||
                   status == Status.Created ||
                   status == Status.Accepted ||
                   status == Status.NoContent;
        }

        public static HttpStatusCode ToHttpStatusCode(this Status source)
        {
            return (HttpStatusCode)(int)source;
        }
    }
}
