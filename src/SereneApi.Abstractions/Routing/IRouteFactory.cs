using SereneApi.Abstractions.Requests;
using System;

namespace SereneApi.Abstractions.Routing
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        string BuildEndPoint(IApiRequest request);

        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// </summary>
        Uri BuildRoute(IApiRequest request);
    }
}
