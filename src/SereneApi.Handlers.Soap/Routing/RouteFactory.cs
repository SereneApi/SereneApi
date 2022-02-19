using SereneApi.Handlers.Soap.Requests;
using System;

namespace SereneApi.Handlers.Soap.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class RouteFactory : IRouteFactory
    {
        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        public RouteFactory()
        {
        }

        #endregion Constructors

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute(ISoapApiRequest request)
        {
            string route = $"{request.ResourcePath}/{request.Resource}";

            if (request.Version != null)
            {
                route += $"/{request.Version.GetVersionString()}";
            }

            return new Uri(route, UriKind.Relative);
        }
    }
}