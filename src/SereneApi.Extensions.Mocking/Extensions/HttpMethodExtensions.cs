using System.Net.Http;
using SereneApi.Abstractions.Request;

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
            string methodstring = method.Method;

            return methodstring switch
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
