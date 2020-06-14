
// Do not change namespace
// ReSharper disable once CheckNamespace
using SereneApi;

namespace System.Net.Http
{
    internal static class HttpMethodExtensions
    {
        public static Method ToMethod(this HttpMethod method)
        {
            string methodString = method.Method;

            return methodString switch
            {
                "GET" => Method.Get,
                "POST" => Method.Post,
                "DELETE" => Method.Delete,
                "PATCH" => Method.Patch,
                "PUT" => Method.Put,
                _ => Method.None
            };
        }
    }
}
