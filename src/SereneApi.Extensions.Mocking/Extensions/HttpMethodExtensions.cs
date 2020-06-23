
using SereneApi;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    internal static class HttpMethodExtensions
    {
        /// <summary>
        /// Converts a <see cref="HttpMethod"/> to a <see cref="Method"/>.
        /// </summary>
        /// <param name="method">The <see cref="HttpMethod"/> to be converted.</param>
        public static Method ToMethod(this HttpMethod method)
        {
            string methodString = method.Method;

            return methodString switch
            {
                "GET" => Method.GET,
                "POST" => Method.POST,
                "DELETE" => Method.DELETE,
                "PATCH" => Method.PATCH,
                "PUT" => Method.PUT,
                _ => Method.NONE
            };
        }
    }
}
