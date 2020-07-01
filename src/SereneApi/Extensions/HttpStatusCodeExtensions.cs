using SereneApi.Abstraction.Enums;
using System.Net;
using SereneApi.Abstractions.Enums;

namespace SereneApi.Extensions
{
    internal static class HttpStatusCodeExtensions
    {
        public static Status ToStatus(this HttpStatusCode source)
        {
            try
            {
                return (Status)(int)source;
            }
            catch
            {
                return Status.Unknown;
            }
        }
    }
}
