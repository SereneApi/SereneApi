using System;
using System.Net.Http;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestMethod
    {
        /// <summary>
        /// Specifies the <see cref="HttpMethod"/> of the request.
        /// </summary>
        /// <param name="httpMethod">The <see cref="ArgumentException"/> to be used.</param>
        /// <exception cref="HttpMethod">Thrown when an invalid <see cref="HttpMethod"/> is provided.</exception>
        IApiRequestResource UsingMethod(HttpMethod httpMethod);
    }
}