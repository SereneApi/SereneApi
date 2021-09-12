using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Options.Factories;
using System;
using System.Linq;

namespace SereneApi.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApiHandler">The API that will be extended.</typeparam>
        /// <param name="factory">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public static void ExtendApi<TApiHandler>(this IServiceCollection services, Action<IApiOptionsExtensions> factory) where TApiHandler : IApiHandler
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

        public static void OverrideConfigurationFactory<TConfigurationProvider>(this IServiceCollection services, Action<IHandlerConfigurationFactory> configurator) where TConfigurationProvider : ConfigurationFactory
        {
            services.TryAddSingleton<IConfigurationManager>(new ConfigurationManager());

            using ServiceProvider provider = services.BuildServiceProvider();

            IConfigurationManager configuration = provider.GetRequiredService<IConfigurationManager>();

            configuration.AmendConfiguration<TConfigurationProvider>(configurator);
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the
        /// specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">
        /// The Handler which will be configured and perform API calls.
        /// </typeparam>
        /// <param name="factory">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified API has already been registered.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        /// <remarks>
        /// By default an <see cref="ILoggerFactory"/> is added to the API to create an <seealso cref="ILogger"/>.
        /// </remarks>
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(this IServiceCollection services, Action<IApiOptionsFactory> factory) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            RegisterApi<TApi, TApiHandler>(services);

            using ServiceProvider provider = services.BuildServiceProvider();

            ApiOptionsFactory<TApiHandler> optionsFactory = provider.GetRequiredService<IConfigurationManager>().BuildApiOptionsFactory<TApiHandler>();

            services.Add(new ServiceDescriptor(typeof(ApiOptionsFactory<TApiHandler>),
                _ => CreateApiHandlerOptionsBuilder(factory, optionsFactory, services), ServiceLifetime.Singleton));

            return optionsFactory;
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the
        /// specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">
        /// The Handler which will be configured and perform API calls.
        /// </typeparam>
        /// <param name="factory">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified API has already been registered.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        /// <remarks>
        /// By default an <see cref="ILoggerFactory"/> is added to the API to create an <seealso cref="ILogger"/>.
        /// </remarks>
        public static IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(
            this IServiceCollection services,
            Action<IApiOptionsFactory, IServiceProvider> factory)
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

            RegisterApi<TApi, TApiHandler>(services);

            using ServiceProvider provider = services.BuildServiceProvider();

            ApiOptionsFactory<TApiHandler> optionsFactory = provider.GetRequiredService<IConfigurationManager>().BuildApiOptionsFactory<TApiHandler>();

            services.Add(new ServiceDescriptor(typeof(ApiOptionsFactory<TApiHandler>),
                p => CreateApiHandlerOptionsFactory(factory, optionsFactory, services, p), ServiceLifetime.Singleton));

            return optionsFactory;
        }

        /// <summary> Builds the <see cref="IApiOptions"/> for the specified <see
        /// cref="IApiHandler"optionsFactory/> </summary>
        private static IApiOptions<TApiHandler> BuildApiHandlerOptions<TApiHandler>(IServiceProvider provider) where TApiHandler : IApiHandler
        {
            ApiOptionsFactory<TApiHandler> factory = provider.GetRequiredService<ApiOptionsFactory<TApiHandler>>();

            return (IApiOptions<TApiHandler>)factory.BuildOptions();
        }

        private static IApiOptionsBuilder CreateApiHandlerOptionsBuilder<TApiHandler>(Action<IApiOptionsFactory> factory, ApiOptionsFactory<TApiHandler> optionsFactory, IServiceCollection services) where TApiHandler : IApiHandler
        {
            factory.Invoke(optionsFactory);

            optionsFactory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            optionsFactory.Dependencies.AddSingleton<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                optionsFactory.Dependencies.TryAddSingleton<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApiHandler>();
                });
            }

            return optionsFactory;
        }

        private static IApiOptionsBuilder CreateApiHandlerOptionsFactory<TApiHandler>(Action<IApiOptionsFactory, IServiceProvider> builder, ApiOptionsFactory<TApiHandler> optionsFactory, IServiceCollection services, IServiceProvider provider) where TApiHandler : IApiHandler
        {
            builder.Invoke(optionsFactory, provider);

            optionsFactory.Dependencies.AddSingleton(() => services, Binding.Unbound);
            optionsFactory.Dependencies.AddSingleton<IServiceProvider>(p => p.GetDependency<IServiceCollection>().BuildServiceProvider());

            if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
            {
                optionsFactory.Dependencies.TryAddSingleton<ILogger>(p =>
                {
                    IServiceProvider serviceProvider = p.GetDependency<IServiceProvider>();

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    return loggerFactory?.CreateLogger<TApiHandler>();
                });
            }

            return optionsFactory;
        }

        private static void RegisterApi<TApi, TApiHandler>(IServiceCollection services) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            services.TryAddSingleton<IConfigurationManager>(new ConfigurationManager());
            services.AddScoped<TApi, TApiHandler>();
            services.Add(new ServiceDescriptor(typeof(IApiOptions<TApiHandler>), BuildApiHandlerOptions<TApiHandler>, ServiceLifetime.Scoped));
        }
    }
}