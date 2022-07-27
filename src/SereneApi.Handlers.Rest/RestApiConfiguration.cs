using SereneApi.Handlers.Rest.Routing;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class RestApiConfiguration
    {
        public static void UseRouteFactory(this IApiConfiguration configuration, IRouteFactory routeFactory)
        {
            if (routeFactory == null)
            {
                throw new ArgumentNullException(nameof(routeFactory));
            }

            configuration.Dependencies
                .Register(() => routeFactory)
                .AsScoped();
        }

        public static void UseRouteFactory<TRouteFactory>(this IApiConfiguration configuration) where TRouteFactory : IRouteFactory
        {
            configuration.Dependencies.Register<TRouteFactory>()
                .DefineAs<IRouteFactory>()
                .AsScoped();
        }
    }
}