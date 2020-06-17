using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Extensions.DependencyInjection.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

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

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerExtensions<TApiDefinition>), extensions));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions<TApiDefinition, TApiImplementation>(optionsAction, p, services), ServiceLifetime.Scoped));

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

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerExtensions<TApiDefinition>), extensions));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>),
                p => CreateApiHandlerOptions<TApiDefinition, TApiImplementation>(optionsAction, p, services), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return extensions;
        }

        private static IApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>> optionsAction, IServiceProvider serviceProvider, IServiceCollection services) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            IApiHandlerExtensions<TApiDefinition> extensions = serviceProvider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();

            CoreOptions options = GetCoreOptions(extensions);

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>((DependencyCollection)options.DependencyCollection.Clone());

            optionsAction.Invoke(builder);

            builder.AddServicesCollection(services);

            return builder.BuildOptions();
        }

        private static IApiHandlerOptions<TApiImplementation> CreateApiHandlerOptions<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> optionsAction, IServiceProvider serviceProvider, IServiceCollection services) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            IApiHandlerExtensions<TApiDefinition> extensions = serviceProvider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();

            CoreOptions options = GetCoreOptions(extensions);

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>((DependencyCollection)options.DependencyCollection.Clone());

            optionsAction.Invoke(builder, serviceProvider);

            builder.AddServicesCollection(services);

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
