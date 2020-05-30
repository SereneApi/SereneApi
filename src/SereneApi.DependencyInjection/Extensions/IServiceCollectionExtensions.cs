using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi;
using SereneApi.DependencyInjection.Types;
using SereneApi.Types;
using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/></param>
        public static void AddApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<ApiHandlerOptionsBuilder<TApiImplementation>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddTransient<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(optionsAction, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/> with an <see cref="IServiceProvider"/> to get already registered services</param>
        public static void AddApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<ApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddTransient<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(optionsAction, p, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ApiHandlerOptions),
                p => p.GetRequiredService<ApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Singleton));
        }

        private static ApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiImplementation>(Action<ApiHandlerOptionsBuilder<TApiImplementation>> optionsAction, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>();

            optionsAction.Invoke(builder);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }

        private static ApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiImplementation>(Action<ApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction, IServiceProvider serviceProvider, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>();

            optionsAction.Invoke(builder, serviceProvider);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }


    }
}
