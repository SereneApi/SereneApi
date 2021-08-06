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
        IApiRequestParameters AgainstEndpoint(string endpoint);
        
        /// <summary>
        /// Specifies the Endpoint to be used for the request.
        /// </summary>
        /// <param name="endpoint">The version to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when a string is either empty or only contains null spaces.</exception>
        [Obsolete("This has been superseded by AgainstEndpoint and will soon be removed.")]
        // TODO: Remove in future update
        IApiRequestParameters WithEndpoint(string endpoint);
    }
}
