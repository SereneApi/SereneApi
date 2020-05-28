using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi;
using SereneApi.DependencyInjection;
using SereneApi.Types;
using System;
using SereneApi.DependencyInjection.Types;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IServiceProvider, ApiHandlerOptionsBuilder<TApiImplementation>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddTransient<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(optionsAction, p, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }

        private static ApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiImplementation>(Action<IServiceProvider, ApiHandlerOptionsBuilder<TApiImplementation>> optionsAction, IServiceProvider serviceProvider, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>();

            optionsAction.Invoke(serviceProvider, builder);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }


    }
}
