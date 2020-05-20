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

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(new ApiHandlerOptions<TApiImplementation>());

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                optionsAction?.Invoke(serviceProvider, builder);
            }

            ApiHandlerOptions<TApiImplementation> options = builder.Options;

            if (options.HttpClient == null)
            {
                services.AddHttpClient(options.HandlerType.ToString(), client =>
                {
                    client.BaseAddress = new Uri($"{options.Source}/{options.ResourcePrecursor}{options.Resource}");
                    client.Timeout = options.Timeout;

                    options.RequestHeaderBuilder.Invoke(client.DefaultRequestHeaders);
                });

                // NOT A fan of this not being disposed... Disposing of this also disposed the IHttpClientFactory
                options.AddHttpClientFactory(services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>());
            }

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptions<TApiImplementation>),
                p => options, ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }
    }
}
