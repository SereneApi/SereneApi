using SereneApi.Abstractions.Request;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Extensions
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
