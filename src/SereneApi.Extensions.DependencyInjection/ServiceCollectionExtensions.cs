using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Options.Factories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void OverrideConfigurationProvider<THandler>(this IServiceCollection services, Action<IHandlerConfigurationFactory> configurator) where THandler : ConfigurationProvider
        {
            services.TryAddSingleton<IConfigurationManager>(new ConfigurationManager());

            using ServiceProvider provider = services.BuildServiceProvider();

            IConfigurationManager configuration = provider.GetRequiredService<IConfigurationManager>();

            configuration.AmendConfiguration<THandler>(configurator);
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApiHandler">The API that will be extended.</typeparam>
        /// <param name="factory">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static void ExtendApi<TApiHandler>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsExtensions> factory) where TApiHandler : IApiHandler
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

            IApiOptionsExtensions extensions = serviceProvider.GetService<ApiOptionsFactory<TApiHandler>>();

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
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>([NotNull] this IServiceCollection services, [NotNull] Action<IApiOptionsFactory> factory) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            services.TryAddSingleton<IConfigurationManager>(new ConfigurationManager());

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if (services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ApiOptionsFactory<TApiHandler> optionsFactory = provider.GetRequiredService<IConfigurationManager>().BuildApiOptionsFactory<TApiHandler>();

            services.Add(new ServiceDescriptor(typeof(ApiOptionsFactory<TApiHandler>),
                p => CreateApiHandlerOptionsBuilder(factory, optionsFactory, services), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(IApiOptions<TApiHandler>), BuildApiHandlerOptions<TApiHandler>, ServiceLifetime.Scoped));

            return optionsFactory;
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
            [NotNull] Action<IApiOptionsFactory, IServiceProvider> factory)
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

            services.TryAddSingleton<IConfigurationManager>(new ConfigurationManager());

            ServiceDescriptor service = ServiceDescriptor.Scoped<TApi, TApiHandler>();

            if (services.Contains(service))
            {
                throw new ArgumentException($"The API {typeof(TApi).Name} has already been registered.");
            }

            services.Add(service);

            using ServiceProvider provider = services.BuildServiceProvider();

            ApiOptionsFactory<TApiHandler> optionsFactory = provider.GetRequiredService<IConfigurationManager>().BuildApiOptionsFactory<TApiHandler>();

            services.TryAdd(new ServiceDescriptor(typeof(ApiOptionsFactory<TApiHandler>),
                p => CreateApiHandlerOptionsBuilder(factory, optionsFactory, services, p), ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(typeof(IApiOptions<TApiHandler>),
                BuildApiHandlerOptions<TApiHandler>, ServiceLifetime.Scoped));

            return optionsFactory;
        }

        private static IApiOptionsBuilder CreateApiHandlerOptionsBuilder<TApiHandler>([NotNull] Action<IApiOptionsFactory> factory, ApiOptionsFactory<TApiHandler> optionsFactory, [NotNull] IServiceCollection services) where TApiHandler : IApiHandler
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            factory.Invoke(optionsFactory);

            optionsFactory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            optionsFactory.Dependencies.AddScoped<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                optionsFactory.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = (ServiceProvider)p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApiHandler>();
                });
            }

            return optionsFactory;
        }

        private static IApiOptionsBuilder CreateApiHandlerOptionsBuilder<TApiHandler>([NotNull] Action<IApiOptionsFactory, IServiceProvider> builder, [NotNull] ApiOptionsFactory<TApiHandler> optionsFactory, [NotNull] IServiceCollection services, [NotNull] IServiceProvider provider) where TApiHandler : IApiHandler
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            builder.Invoke(optionsFactory, provider);

            optionsFactory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            optionsFactory.Dependencies.AddTransient<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                optionsFactory.Dependencies.TryAddScoped<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApiHandler>();
                });
            }

            return optionsFactory;
        }

        /// <summary>
        /// Builds the <see cref="IApiOptions"/> for the specified <see cref="IApiHandler"/>
        /// </summary>
        private static IApiOptions<TApiHandler> BuildApiHandlerOptions<TApiHandler>([NotNull] IServiceProvider provider) where TApiHandler : IApiHandler
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            ApiOptionsFactory<TApiHandler> factory = provider.GetRequiredService<ApiOptionsFactory<TApiHandler>>();

            return (IApiOptions<TApiHandler>)factory.BuildOptions();
        }
    }
}
