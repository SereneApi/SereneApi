using SereneApi.Core.Requests;
using System;

namespace SereneApi.Core.Routing
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// </summary>
        Uri BuildRoute(IApiRequest request);
    }
}
