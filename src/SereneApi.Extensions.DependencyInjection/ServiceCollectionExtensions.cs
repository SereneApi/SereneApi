using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Configuration.Adapters;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
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
        public static IConfigurationExtensions ConfigureSereneApi([NotNull] this IServiceCollection services, [AllowNull] ISereneApiConfiguration configuration = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            configuration ??= SereneApiConfiguration.Default;

            ServiceDescriptor service = ServiceDescriptor.Singleton(configuration);

            if (services.Contains(service))
            {
                throw new ArgumentException("This can only be called once, or must be called before API Registration.");
            }

            services.AddSingleton(configuration);

            return configuration.GetExtensions();
        }

        /// <summary>
        /// Configures the default configuration for all APIs.
        /// </summary>
        /// <param name="builder">The prevalent configuration for all APIs.</param>
        /// <exception cref="ArgumentException">Thrown if this is called after API registration or if it is called twice.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>These values can be overriden during API Registration.</remarks>
        public static IConfigurationExtensions ConfigureSereneApi([NotNull] this IServiceCollection services, [NotNull] Action<ISereneApiConfigurationBuilder> builder)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            SereneApiConfiguration configuration = SereneApiConfiguration.Default;

            builder.Invoke(configuration);

            return services.ConfigureSereneApi(configuration);
        }

        /// <summary>
        /// Gets an instance of <see cref="IApiAdapter"/>, this is used to collect information encompassing SereneApi.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>Should be called after ConfigureSereneApi if any configuration is required.</remarks>
        public static void ConfigureApiAdapters([NotNull] this IServiceCollection services, [NotNull] Action<IApiAdapter> configurator)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            services.TryAddSingleton((ISereneApiConfiguration)SereneApiConfiguration.Default);

            using ServiceProvider provider = services.BuildServiceProvider();

            ISereneApiConfiguration sereneApiConfiguration = provider.GetRequiredService<ISereneApiConfiguration>();

            configurator.Invoke(sereneApiConfiguration.GetAdapter());
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static IApiOptionsExtensions ExtendApi<TApi>([NotNull] this IServiceCollection services) where TApi : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            return (IApiOptionsExtensions)serviceProvider.GetService<IApiOptionsBuilder<TApi>>();
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <param name="factory">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static void ExtendApi<TApi>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsExtensions> factory) where TApi : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IApiOptionsExtensions extensions = (IApiOptionsExtensions)serviceProvider.GetService<IApiOptionsBuilder<TApi>>();

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
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsBuilder<TApi>> builder) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            services.TryAddSingleton((ISereneApiConfiguration)SereneApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if (services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ISereneApiConfiguration configuration = provider.GetRequiredService<ISereneApiConfiguration>();

            ApiOptionsFactory<TApi> factory = configuration.BuildOptionsFactory<ApiOptionsFactory<TApi>>();

            services.Add(new ServiceDescriptor(typeof(IApiOptionsBuilder<TApi>),
                p => CreateApiHandlerOptionsBuilder(builder, factory, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(IApiOptions<TApi>), BuildApiHandlerOptions<TApi>, ServiceLifetime.Scoped));

            return factory;
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
            [NotNull] Action<IApiOptionsBuilder<TApi>, IServiceProvider> factory)
            where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            services.TryAddSingleton((ISereneApiConfiguration)SereneApiConfiguration.Default);

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if (services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ISereneApiConfiguration configuration = provider.GetRequiredService<ISereneApiConfiguration>();

            ApiOptionsFactory<TApi> builder = configuration.BuildOptionsFactory<ApiOptionsFactory<TApi>>();

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptionsBuilder<TApi>),
                p => CreateApiHandlerOptionsBuilder(factory, builder, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptions<TApi>),
                BuildApiHandlerOptions<TApi>, ServiceLifetime.Scoped));

            return builder;
        }

        /// <summary>
        /// Creates and configured the <see cref="IApiOptionsBuilder{TApi}"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptionsBuilder{TApi}"/> will be built for.</typeparam>
        /// <param name="factory">Configures the user supplied values for <see cref="IApiConfiguration"/>.</param>
        /// <param name="factory">Used to invoke the factory.</param>
        /// <param name="services">Injected into the <see cref="IDependencyCollection"/> for <see cref="IApiOptions"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is supplied.</exception>
        private static IApiOptionsBuilder<TApi> CreateApiHandlerOptionsBuilder<TApi>([NotNull] Action<IApiOptionsBuilder<TApi>> builder, [NotNull] ApiOptionsFactory<TApi> factory, [NotNull] IServiceCollection services) where TApi : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            builder.Invoke(factory);

            factory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            factory.Dependencies.AddTransient<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                factory.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = (ServiceProvider)p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApi>();
                });
            }

            return factory;
        }

        /// <summary>
        /// Creates and configured the <see cref="IApiOptionsBuilder{TApi}"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptionsBuilder{TApi}"/> will be built for.</typeparam>
        /// <param name="factory">Configures the user supplied values for <see cref="IApiConfiguration"/>.</param>
        /// <param name="factory">Used to invoke the factory.</param>
        /// <param name="services">Injected into the <see cref="IDependencyCollection"/> for <see cref="IApiOptions"/>.</param>
        /// <param name="provider">Used to invoke the factory.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is supplied.</exception>
        private static IApiOptionsBuilder<TApi> CreateApiHandlerOptionsBuilder<TApi>([NotNull] Action<IApiOptionsBuilder<TApi>, IServiceProvider> builder, [NotNull] ApiOptionsFactory<TApi> factory, [NotNull] IServiceCollection services, [NotNull] IServiceProvider provider) where TApi : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            builder.Invoke(factory, provider);

            factory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            factory.Dependencies.AddTransient<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                factory.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApi>();
                });
            }

            return factory;
        }

        /// <summary>
        /// Builds <see cref="IApiOptions"/> for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that the <see cref="IApiOptions"/> will be built for.</typeparam>
        /// <param name="provider">Used to get the <see cref="IApiOptionsBuilder{TApi}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        private static IApiOptions<TApi> BuildApiHandlerOptions<TApi>([NotNull] IServiceProvider provider) where TApi : class
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            ApiOptionsFactory<TApi> factory = (ApiOptionsFactory<TApi>)provider.GetRequiredService<IApiOptionsBuilder<TApi>>();

            return factory.BuildOptions();
        }
    }
}
