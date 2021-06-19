using System;

namespace SereneApi.Abstractions.Requests.Builder
{
    public interface IApiRequestEndpoint : IApiRequestParameters
    {
        /// <summary>
        /// Specifies the Endpoint to be used for the request.
        /// </summary>
        /// <param name="endpoint">The version to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when a string is either empty or only contains null spaces.</exception>
        IApiRequestParameters WithEndpoint(string endpoint);
    }
}
