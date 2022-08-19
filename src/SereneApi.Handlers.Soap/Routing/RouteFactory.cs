using SereneApi.Handlers.Soap.Requests;
using System;
using SereneApi.Core.Http;

namespace SereneApi.Handlers.Soap.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class RouteFactory : IRouteFactory
    {
        private readonly IConnectionSettings _connectionSettings;

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        public RouteFactory(IConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }
        
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

        public Uri GetUrl(ISoapApiRequest request)
        {
            string url = $"{_connectionSettings.BaseAddress}{request.Route}";

            return new Uri(url);
        }
    }
}