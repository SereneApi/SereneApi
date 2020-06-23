using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Extensions.DependencyInjection.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net.Http;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Allows a registered <see cref="ApiHandler"/> to be extended upon.
        /// </summary>
        /// <typeparam name="TApiDefinition">The <see cref="ApiHandler"/> Definition.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the specified <see cref="ApiHandler"/> has not been registered.</exception>
        public static IApiHandlerExtensions ExtendApiHandler<TApiDefinition>(this IServiceCollection services) where TApiDefinition : class
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IApiHandlerExtensions<TApiDefinition> extensions = serviceProvider.GetService<IApiHandlerExtensions<TApiDefinition>>();

            if(extensions == null)
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            return extensions;
        }

        /// <summary>
        /// Allows a registered <see cref="ApiHandler"/> to be extended upon.
        /// </summary>
        /// <typeparam name="TApiDefinition">The <see cref="ApiHandler"/> Definition.</typeparam>
        /// /// <exception cref="ArgumentException">Thrown if the specified <see cref="ApiHandler"/> has not been registered.</exception>
        public static void ExtendApiHandler<TApiDefinition>(this IServiceCollection services, Action<IApiHandlerExtensions> extensionsAction) where TApiDefinition : class
        {
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IApiHandlerExtensions<TApiDefinition> extensions = serviceProvider.GetService<IApiHandlerExtensions<TApiDefinition>>();

            if(extensions == null)
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            extensionsAction.Invoke(extensions);
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/></param>
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiImplementation>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            ApiHandlerExtensions<TApiDefinition> extensions = new ApiHandlerExtensions<TApiDefinition>();

            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerExtensions<TApiDefinition>), extensions));

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptionsBuilder<TApiImplementation>),
                p => CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(optionsAction, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>), BuildApiHandlerOptions<TApiImplementation>, ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return extensions;
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/> with an <see cref="IServiceProvider"/> to get already registered services</param>
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            ApiHandlerExtensions<TApiDefinition> extensions = new ApiHandlerExtensions<TApiDefinition>();

            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerExtensions<TApiDefinition>), extensions));

            services.TryAdd(new ServiceDescriptor(typeof(ApiHandlerOptionsBuilder<TApiImplementation>),
                p => CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(optionsAction, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>), BuildApiHandlerOptions<TApiImplementation>, ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return extensions;
        }

        private static ApiHandlerOptionsBuilder<TApiImplementation> CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>> action, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            ApiHandlerExtensions<TApiDefinition> extensions = provider.GetRequiredService<ApiHandlerExtensions<TApiDefinition>>();

            CoreOptions coreOptions = GetCoreOptions(extensions);

            DependencyCollection dependencies = (DependencyCollection)coreOptions.DependencyCollection.Clone();

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(dependencies, services);

            action.Invoke(builder);

            DependencyInjectionClientFactory<TApiImplementation> factory = new DependencyInjectionClientFactory<TApiImplementation>(builder.DependencyCollection);

            factory.Initiate();

            builder.DependencyCollection.AddDependency<IClientFactory>(factory);

            return builder;
        }

        private static ApiHandlerOptionsBuilder<TApiImplementation> CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> action, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            ApiHandlerExtensions<TApiDefinition> extensions = provider.GetRequiredService<ApiHandlerExtensions<TApiDefinition>>();

            CoreOptions coreOptions = GetCoreOptions(extensions);

            DependencyCollection dependencies = (DependencyCollection)coreOptions.DependencyCollection.Clone();

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(dependencies, services);

            action.Invoke(builder, provider);

            DependencyInjectionClientFactory<TApiImplementation> factory = new DependencyInjectionClientFactory<TApiImplementation>(builder.DependencyCollection);

            factory.Initiate();

            builder.DependencyCollection.AddDependency<IClientFactory>(factory);

            return builder;
        }

        private static IApiHandlerOptions<TApiImplementation> BuildApiHandlerOptions<TApiImplementation>(IServiceProvider provider) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = provider.GetRequiredService<ApiHandlerOptionsBuilder<TApiImplementation>>();

            builder.DependencyCollection.AddDependency(provider.GetRequiredService<IHttpClientFactory>());

            return builder.BuildOptions();
        }

        private static CoreOptions GetCoreOptions(IApiHandlerExtensions extensions)
        {
            if(extensions is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
