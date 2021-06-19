using System.Net;

// ReSharper disable once CheckNamespace
namespace SereneApi.Abstractions.Response
{
    public static class HttpStatusCodeExtensions
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
