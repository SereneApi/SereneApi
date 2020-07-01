using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Extensions.DependencyInjection.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
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

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptionsBuilder<TApiImplementation>),
                p => CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(optionsAction, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>), p => BuildApiHandlerOptions<TApiImplementation>(p, services), ServiceLifetime.Scoped));

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

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptionsBuilder<TApiImplementation>),
                p => CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(optionsAction, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiImplementation>), p => BuildApiHandlerOptions<TApiImplementation>(p, services), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions),
                p => p.GetRequiredService<IApiHandlerOptions<TApiImplementation>>(), ServiceLifetime.Scoped));

            return extensions;
        }

        private static IApiHandlerOptionsBuilder<TApiImplementation> CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>> action, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            IApiHandlerExtensions<TApiDefinition> extensions = provider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();

            CoreOptions coreOptions = GetCoreOptions(extensions);

            coreOptions.Dependencies.AddSingleton(() => services, Binding.Unbound);

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(coreOptions.Dependencies);

            action.Invoke(builder);

            coreOptions.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiImplementation> clientFactory = new ClientFactory<TApiImplementation>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiHandlerOptionsBuilder<TApiImplementation> CreateApiHandlerOptionsBuilder<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder<TApiImplementation>, IServiceProvider> action, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            IApiHandlerExtensions<TApiDefinition> extensions = provider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();

            CoreOptions coreOptions = GetCoreOptions(extensions);

            coreOptions.Dependencies.AddSingleton(() => services, Binding.Unbound);

            ApiHandlerOptionsBuilder<TApiImplementation> builder = new ApiHandlerOptionsBuilder<TApiImplementation>(coreOptions.Dependencies);

            action.Invoke(builder, provider);

            coreOptions.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiImplementation> clientFactory = new ClientFactory<TApiImplementation>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiHandlerOptions<TApiImplementation> BuildApiHandlerOptions<TApiImplementation>(IServiceProvider provider, IServiceCollection services) where TApiImplementation : ApiHandler
        {
            ApiHandlerOptionsBuilder<TApiImplementation> builder = (ApiHandlerOptionsBuilder<TApiImplementation>)provider.GetRequiredService<IApiHandlerOptionsBuilder<TApiImplementation>>();

            return builder.BuildOptions(services);
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
