using SereneApi.Handlers.Rest.Requests;
using System;

namespace SereneApi.Handlers.Rest.Routing
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        string BuildEndPoint(IRestApiRequest request);

        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// </summary>
        Uri BuildRoute(IRestApiRequest request);

        Uri GetUrl(IRestApiRequest apiRequest);
    }
}