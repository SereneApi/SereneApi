using DeltaWare.SereneApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IServiceProvider, ApiHandlerOptionsBuilder<TApiImplementation>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddTransient<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(p, services, optionsAction), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }

        private static ApiHandlerOptions<TApiHandler> CreateApiHandlerOptions<TApiHandler>(IServiceProvider serviceProvider, IServiceCollection services, Action<IServiceProvider, ApiHandlerOptionsBuilder<TApiHandler>> optionsAction) where TApiHandler : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiHandler> builder = new ApiHandlerOptionsBuilder<TApiHandler>(new ApiHandlerOptions<TApiHandler>());

            optionsAction?.Invoke(serviceProvider, builder);

            return builder.Options;
        }
    }
}
