using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Extensions.DependencyInjection.Types;
using System;
using SereneApi.Interfaces;
using SereneApi.Types;

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
        public static IRegisterApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiImplementation>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            RegisterApiHandlerExtensions registerExtensions = new RegisterApiHandlerExtensions();

            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(optionsAction, registerExtensions, services), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return registerExtensions;
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/> with an <see cref="IServiceProvider"/> to get already registered services</param>
        public static IRegisterApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            RegisterApiHandlerExtensions registerExtensions = new RegisterApiHandlerExtensions();

            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions(optionsAction, registerExtensions, p, services), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return registerExtensions;
        }

        private static IApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>> optionsAction, IRegisterApiHandlerExtensions registerExtensions, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>();

            optionsAction.Invoke(builder);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }

        private static IApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction, IRegisterApiHandlerExtensions registerExtensions, IServiceProvider serviceProvider, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>();

            optionsAction.Invoke(builder, serviceProvider);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }
    }
}
