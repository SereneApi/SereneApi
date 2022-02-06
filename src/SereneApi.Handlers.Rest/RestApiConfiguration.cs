using SereneApi.Handlers.Rest.Routing;
using DeltaWare.Dependencies.Abstractions;
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

            configuration.Dependencies.AddScoped(() => routeFactory);
        }

        public static void UseRouteFactory<TRouteFactory>(this IApiConfiguration configuration) where TRouteFactory : IRouteFactory
        {
            configuration.Dependencies.AddScoped<IRouteFactory, TRouteFactory>();
        }
    }
}