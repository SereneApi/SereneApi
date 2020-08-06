using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.DependencyInjection.Factories;
using SereneApi.Extensions.DependencyInjection.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the default configuration for all APIs.
        /// </summary>
        /// <param name="configuration">The prevalent configuration for all APIs.</param>
        /// <exception cref="ArgumentException">Thrown if this is called after API registration or if it is called twice.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>These values can be overriden during API Registration.</remarks>
        public static IDefaultApiConfigurationExtensions ConfigureSereneApi([NotNull] this IServiceCollection services, [AllowNull] IDefaultApiConfiguration configuration = null)
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            configuration ??= DefaultApiConfiguration.Default;

            ServiceDescriptor service = ServiceDescriptor.Singleton(configuration);

            if(services.Contains(service))
            {
                throw new ArgumentException("This can only be called once, or must be called before API Registration.");
            }

            services.AddSingleton(configuration);

            return configuration.GetExtensions();
        }

        /// <summary>
        /// Configures the default configuration for all APIs.
        /// </summary>
        /// <param name="factory">The prevalent configuration for all APIs.</param>
        /// <exception cref="ArgumentException">Thrown if this is called after API registration or if it is called twice.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>These values can be overriden during API Registration.</remarks>
        public static IDefaultApiConfigurationExtensions ConfigureSereneApi([NotNull] this IServiceCollection services, [NotNull] Action<IDefaultApiConfigurationBuilder> factory)
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            DefaultApiConfiguration configuration = (DefaultApiConfiguration)DefaultApiConfiguration.Default;

            factory.Invoke(configuration);

            return services.ConfigureSereneApi(configuration);
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static IApiOptionsExtensions ExtendApi<TApi>([NotNull] this IServiceCollection services) where TApi : class
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            return (IApiOptionsExtensions)serviceProvider.GetService<IApiOptionsConfigurator<TApi>>();
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <param name="factory">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static void ExtendApi<TApi>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsExtensions> factory) where TApi : class
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IApiOptionsExtensions extensions = (IApiOptionsExtensions)serviceProvider.GetService<IApiOptionsConfigurator<TApi>>();

            factory.Invoke(extensions);
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">The Handler which will be configured and perform API calls.</typeparam>
        /// <param name="factory">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">Thrown when the specified API has already been registered.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        /// <remarks>By default an <see cref="ILoggerFactory"/> is added to the API to create an <seealso cref="ILogger"/>.</remarks>
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsConfigurator<TApi>> factory) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            services.TryAddSingleton(DefaultApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if(services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            IDefaultApiConfiguration configuration = provider.GetRequiredService<IDefaultApiConfiguration>();

            ApiApiOptionsBuilder<TApi> builder = configuration.GetOptionsBuilder<ApiApiOptionsBuilder<TApi>>();

            services.Add(new ServiceDescriptor(typeof(IApiOptionsConfigurator<TApi>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(IApiOptions<TApi>), BuildApiHandlerOptions<TApi>, ServiceLifetime.Scoped));

            return builder;
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">The Handler which will be configured and perform API calls.</typeparam>
        /// <param name="factory">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">Thrown when the specified API has already been registered.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        /// <remarks>By default an <see cref="ILoggerFactory"/> is added to the API to create an <seealso cref="ILogger"/>.</remarks>
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(
            [NotNull] this IServiceCollection services,
            [NotNull] Action<IApiOptionsConfigurator<TApi>, IServiceProvider> factory)
            where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            services.TryAddSingleton(DefaultApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if(services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            IDefaultApiConfiguration configuration = provider.GetRequiredService<IDefaultApiConfiguration>();

            ApiApiOptionsBuilder<TApi> builder = configuration.GetOptionsBuilder<ApiApiOptionsBuilder<TApi>>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptionsConfigurator<TApi>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptions<TApi>),
                BuildApiHandlerOptions<TApi>, ServiceLifetime.Scoped));

            return builder;
        }

        /// <summary>
        /// Creates and configured the <see cref="IApiOptionsConfigurator"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptionsConfigurator"/> will be built for.</typeparam>
        /// <param name="factory">Configures the user supplied values for <see cref="IDefaultApiConfiguration"/>.</param>
        /// <param name="builder">Used to invoke the factory.</param>
        /// <param name="services">Injected into the <see cref="IDependencyCollection"/> for <see cref="IApiOptions"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is supplied.</exception>
        private static IApiOptionsConfigurator<TApi> CreateApiHandlerOptionsBuilder<TApi>([NotNull] Action<IApiOptionsConfigurator<TApi>> factory, [NotNull] ApiApiOptionsBuilder<TApi> builder, [NotNull] IServiceCollection services) where TApi : class
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            factory.Invoke(builder);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddTransient<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if(services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                builder.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = (ServiceProvider)p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApi>();
                });
            }

            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                DefaultClientFactory<TApi> defaultClientFactory = new DefaultClientFactory<TApi>(p);

                if(!defaultClientFactory.IsConfigured)
                {
                    defaultClientFactory.Configure();
                }

                return defaultClientFactory;
            });

            return builder;
        }

        /// <summary>
        /// Creates and configured the <see cref="IApiOptionsConfigurator"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptionsConfigurator"/> will be built for.</typeparam>
        /// <param name="factory">Configures the user supplied values for <see cref="IDefaultApiConfiguration"/>.</param>
        /// <param name="builder">Used to invoke the factory.</param>
        /// <param name="services">Injected into the <see cref="IDependencyCollection"/> for <see cref="IApiOptions"/>.</param>
        /// <param name="provider">Used to invoke the factory.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is supplied.</exception>
        private static IApiOptionsConfigurator<TApi> CreateApiHandlerOptionsBuilder<TApi>([NotNull] Action<IApiOptionsConfigurator<TApi>, IServiceProvider> factory, [NotNull] ApiApiOptionsBuilder<TApi> builder, [NotNull] IServiceCollection services, [NotNull] IServiceProvider provider) where TApi : class
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if(services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if(provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            factory.Invoke(builder, provider);

            builder.Dependencies.AddSingleton(() => services, Binding.Unbound);
            builder.Dependencies.AddTransient<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if(services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                builder.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = (ServiceProvider)p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApi>();
                });
            }

            builder.Dependencies.AddScoped<IClientFactory>(p =>
            {
                DefaultClientFactory<TApi> defaultClientFactory = new DefaultClientFactory<TApi>(p);

                if(!defaultClientFactory.IsConfigured)
                {
                    defaultClientFactory.Configure();
                }

                return defaultClientFactory;
            });

            return builder;
        }

        /// <summary>
        /// Builds <see cref="IApiOptions"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptions"/> will be built for.</typeparam>
        /// <param name="provider">Used to get the <see cref="IApiOptionsConfigurator"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        private static IApiOptions<TApi> BuildApiHandlerOptions<TApi>([NotNull] IServiceProvider provider) where TApi : class
        {
            if(provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            ApiApiOptionsBuilder<TApi> builder = (ApiApiOptionsBuilder<TApi>)provider.GetRequiredService<IApiOptionsConfigurator<TApi>>();

            return builder.BuildOptions();
        }
    }
}
