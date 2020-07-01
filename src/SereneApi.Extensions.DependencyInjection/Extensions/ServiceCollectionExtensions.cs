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
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiDefinition>> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptionsBuilder<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(optionsAction, services), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerExtensions<TApiDefinition>), CreateApiHandlerExtensions<TApiDefinition>, ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiDefinition>), p => BuildApiHandlerOptions<TApiDefinition>(p, services), ServiceLifetime.Scoped));

            //services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions), p => p.GetRequiredService<IApiHandlerOptions<TApiDefinition>>(), ServiceLifetime.Scoped));

            using ServiceProvider provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();
        }

        /// <summary>
        /// Registers the given handler as a service in the <see cref="IServiceCollection"/>. This method is used alongside Dependency Injection
        /// </summary>
        /// <typeparam name="TApiDefinition">The definition of the API.</typeparam>
        /// <typeparam name="TApiImplementation">The implementation of the <see cref="ApiHandler"/> to be registered as a service</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
        /// <param name="optionsAction">An action to configure the <see cref="ApiHandler"/> with an <see cref="IServiceProvider"/> to get already registered services</param>
        public static IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(this IServiceCollection services, Action<IApiHandlerOptionsBuilder<TApiDefinition>, IServiceProvider> optionsAction) where TApiDefinition : class where TApiImplementation : ApiHandler, TApiDefinition
        {
            services.TryAddScoped<TApiDefinition, TApiImplementation>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptionsBuilder<TApiDefinition>),
                p => CreateApiHandlerOptionsBuilder(optionsAction, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerExtensions<TApiDefinition>), CreateApiHandlerExtensions<TApiDefinition>, ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiHandlerOptions<TApiDefinition>), p => BuildApiHandlerOptions<TApiDefinition>(p, services), ServiceLifetime.Scoped));

            //services.Add(new ServiceDescriptor(typeof(IApiHandlerOptions), p => p.GetRequiredService<IApiHandlerOptions<TApiDefinition>>(), ServiceLifetime.Scoped));

            using ServiceProvider provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IApiHandlerExtensions<TApiDefinition>>();
        }

        private static IApiHandlerOptionsBuilder<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IApiHandlerOptionsBuilder<TApiDefinition>> action, IServiceCollection services) where TApiDefinition : class
        {
            ApiHandlerOptionsBuilder<TApiDefinition> builder = new ApiHandlerOptionsBuilder<TApiDefinition>();

            action.Invoke(builder);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiDefinition> clientFactory = new ClientFactory<TApiDefinition>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiHandlerOptionsBuilder<TApiDefinition> CreateApiHandlerOptionsBuilder<TApiDefinition>(Action<IApiHandlerOptionsBuilder<TApiDefinition>, IServiceProvider> action, IServiceCollection services, IServiceProvider provider) where TApiDefinition : class
        {
            ApiHandlerOptionsBuilder<TApiDefinition> builder = new ApiHandlerOptionsBuilder<TApiDefinition>();

            action.Invoke(builder, provider);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                ClientFactory<TApiDefinition> clientFactory = new ClientFactory<TApiDefinition>(p);

                if(!clientFactory.IsConfigured)
                {
                    clientFactory.Configure();
                }

                return clientFactory;
            });

            return builder;
        }

        private static IApiHandlerExtensions<TApiDefinition> CreateApiHandlerExtensions<TApiDefinition>(IServiceProvider provider) where TApiDefinition : class
        {
            IApiHandlerOptionsBuilder<TApiDefinition> builder = provider.GetRequiredService<IApiHandlerOptionsBuilder<TApiDefinition>>();

            CoreOptions options = GetCoreOptions(builder);

            return new ApiHandlerExtensions<TApiDefinition>(options.Dependencies);
        }

        private static IApiHandlerOptions<TApiDefinition> BuildApiHandlerOptions<TApiDefinition>(IServiceProvider provider, IServiceCollection services) where TApiDefinition : class
        {
            ApiHandlerOptionsBuilder<TApiDefinition> builder = (ApiHandlerOptionsBuilder<TApiDefinition>)provider.GetRequiredService<IApiHandlerOptionsBuilder<TApiDefinition>>();

            return builder.BuildOptions(services);
        }

        private static CoreOptions GetCoreOptions(IApiHandlerOptionsBuilder builder)
        {
            if(builder is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
