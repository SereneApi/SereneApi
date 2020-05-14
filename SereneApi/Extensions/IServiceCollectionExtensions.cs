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
                p => CreateApiHandlerOptions(p, services, optionsAction), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }

        private static ApiHandlerOptions<TApiHandler> CreateApiHandlerOptions<TApiHandler>(IServiceProvider serviceProvider, IServiceCollection services, Action<IServiceProvider, ApiHandlerOptionsBuilder<TApiHandler>> optionsAction) where TApiHandler : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiHandler> builder = new ApiHandlerOptionsBuilder<TApiHandler>(new ApiHandlerOptions<TApiHandler>());

            optionsAction?.Invoke(serviceProvider, builder);

            ApiHandlerOptions<TApiHandler> options = builder.Options;

            // Client override has been provided. We do not need to initiate a client Factory.
            if (options.HttpClient != null)
            {
                return options;
            }

            services.AddHttpClient(options.HandlerType.ToString(), client =>
            {
                client.BaseAddress = new Uri($"{options.Source}/{options.ResourcePrecursor}{options.Resource}");
                client.Timeout = options.Timeout;

                options.RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);

            });

            options.AddHttpClientFactory(serviceProvider.GetRequiredService<IHttpClientFactory>());


            return options;
        }
    }
}
