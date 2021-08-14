using DeltaWare.Dependencies.Abstractions;
using SereneApi.Handlers.Soap.Requests;
using System;

namespace SereneApi.Handlers.Soap.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class RouteFactory : IRouteFactory
    {
        private readonly IDependencyProvider _dependencies;

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public RouteFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        #endregion

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute(ISoapApiRequest request)
        {
            string route = $"{request.ResourcePath}/{request.Resource}";

            if (request.Version != null)
            {
                route += $"/{request.Version.GetVersionString()}";
            }

            if (!string.IsNullOrWhiteSpace(request.Service))
            {
                route += $"/{request.Service}";
            }

            return new Uri(route, UriKind.Relative);
        }
    }
}
