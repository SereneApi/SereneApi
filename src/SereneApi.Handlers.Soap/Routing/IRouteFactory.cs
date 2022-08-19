using SereneApi.Handlers.Soap.Requests;

/* Unmerged change from project 'SereneApi.Handlers.Soap (net6.0)'
Before:
using System;
using SereneApi.Handlers.Soap.Requests.Types;
After:
using SereneApi.Handlers.Soap.Requests.Types;
using System;
*/
using System;

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