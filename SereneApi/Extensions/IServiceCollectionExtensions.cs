using DeltaWare.SereneApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;

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
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(new ApiHandlerOptions<TApiImplementation>());

            optionsAction.Invoke(serviceProvider, builder);

            return builder.BuildApiOptions(services);
        }


    }
}
