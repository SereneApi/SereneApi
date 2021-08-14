using DeltaWare.Dependencies.Abstractions;
using SereneApi.Handlers.Rest.Routing;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Options.Factories
{
    public static class ApiOptionsFactoryExtensions
    {
        public static void UseRouteFactory(this IApiOptionsFactory optionsFactory, IRouteFactory routeFactory)
        {
            if (routeFactory == null)
            {
                throw new ArgumentNullException(nameof(routeFactory));
            }

            if (optionsFactory is not ApiOptionsFactory factory)
            {
                throw new ArgumentException();
            }

            factory.Dependencies.AddScoped(() => routeFactory);
        }
    }
}
