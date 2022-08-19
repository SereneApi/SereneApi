using SereneApi.Handlers.Soap.Requests;
using System;
using SereneApi.Handlers.Soap.Requests.Types;

namespace SereneApi.Handlers.Soap.Routing
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// </summary>
        Uri BuildRoute(ISoapApiRequest request);

        Uri GetUrl(ISoapApiRequest request);
    }
}