using DeltaWare.Dependencies.Abstractions;
using DeltaWare.Dependencies.Abstractions.Enums;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Handler;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static readonly object GetConfigurationManagerLock = new();
        private static ApiConfigurationManager ConfigurationManagerInstance { get; set; }

        public static IServiceCollection AmendConfigurationProvider<TConfigurationProvider>(this IServiceCollection services, Action<IApiConfiguration> configuration) where TConfigurationProvider : HandlerConfigurationProvider
        {
            GetConfigurationManager(services).AmendConfigurationProvider<TConfigurationProvider>(configuration);

            return services;
        }

        /// <summary>
        /// Modifies the <see cref="IApiConfiguration"/> of a registered <typeparamref name="TApiHandler"/>.
        /// </summary>
        /// <typeparam name="TApiHandler">The API Handler to have its <see cref="IApiConfiguration"/> modified.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> with the <typeparamref name="TApiHandler"/> to have its <see cref="IApiConfiguration"/> modified.</param>
        /// <param name="configurationFactory">The factory that creates the <see cref="IApiConfiguration"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ExtendApi<TApiHandler>(this IServiceCollection services, Action<IApiConfiguration> configurationFactory) where TApiHandler : IApiHandler
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationFactory == null)
            {
                throw new ArgumentNullException(nameof(configurationFactory));
            }

            GetConfigurationManager(services).AddApiPostConfiguration<TApiHandler>(configurationFactory);

            return services;
        }

        /// <summary>
        /// Modifies the <see cref="IApiConfiguration"/> of a registered <typeparamref name="TApiHandler"/>.
        /// </summary>
        /// <typeparam name="TApiHandler">The API Handler to have its <see cref="IApiConfiguration"/> modified.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> with the <typeparamref name="TApiHandler"/> to have its <see cref="IApiConfiguration"/> modified.</param>
        /// <param name="configurationFactory">The factory that creates the <see cref="IApiConfiguration"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection ExtendApi<TApiHandler>(this IServiceCollection services, Action<IApiConfiguration, IServiceProvider> configurationFactory) where TApiHandler : IApiHandler
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationFactory == null)
            {
                throw new ArgumentNullException(nameof(configurationFactory));
            }

            GetConfigurationManager(services).AddApiPostConfiguration<TApiHandler>(c =>
            {
                using IDependencyProvider provider = c.Dependencies.BuildProvider();

                IServiceProvider serviceProvider = provider.GetRequiredDependency<IServiceProvider>();

                configurationFactory.Invoke(c, serviceProvider);
            });

            return services;
        }

        /// <summary>
        /// Registers a scoped API of the type specified in <typeparamref name="TApi"/> with a
        /// handler type specified in <typeparamref name="TApiHandler"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TApi">The type of the API to add.</typeparam>
        /// <typeparam name="TApiHandler">The type of the implementation handler to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="configurationFactory">The factory that creates the <see cref="IApiConfiguration"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection RegisterApi<TApi, TApiHandler>(this IServiceCollection services, Action<IApiConfiguration> configurationFactory) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationFactory == null)
            {
                throw new ArgumentNullException(nameof(configurationFactory));
            }

            services.AddScoped<TApi, TApiHandler>();
            services.AddScoped(p => p.GetRequiredService<ApiConfigurationManager>().BuildApiSettings<TApiHandler>());

            ApiConfigurationManager configurationManager = GetConfigurationManager(services);

            configurationManager.AddApiPreConfiguration<TApiHandler>(c =>
            {
                if (c.Dependencies.HasDependency<ILoggerFactory>())
                {
                    c.Dependencies.AddScoped<ILogger>(p => p.GetRequiredDependency<ILoggerFactory>().CreateLogger<TApiHandler>());
                }
            });

            configurationManager.AddApiConfiguration<TApiHandler>(configurationFactory);

            return services;
        }


        /// <summary>
        /// Registers a scoped API of the type specified in <typeparamref name="TApi"/> with a
        /// implementation handler type specified in <typeparamref name="TApiHandler"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TApi">The type of the API to add.</typeparam>
        /// <typeparam name="TApiHandler">The type of the handler to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="configurationFactory">The factory that creates the <see cref="IApiConfiguration"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection RegisterApi<TApi, TApiHandler>(this IServiceCollection services, Action<IApiConfiguration, IServiceProvider> configurationFactory) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationFactory == null)
            {
                throw new ArgumentNullException(nameof(configurationFactory));
            }

            services.AddScoped<TApi, TApiHandler>();
            services.AddScoped(p => p.GetRequiredService<ApiConfigurationManager>().BuildApiSettings<TApiHandler>());

            ApiConfigurationManager configurationManager = GetConfigurationManager(services);

            configurationManager.AddApiPreConfiguration<TApiHandler>(c =>
            {
                if (c.Dependencies.HasDependency<ILoggerFactory>())
                {
                    c.Dependencies.AddScoped<ILogger>(p => p.GetRequiredDependency<ILoggerFactory>().CreateLogger<TApiHandler>());
                }
            });

            configurationManager.AddApiConfiguration<TApiHandler>(c =>
            {
                using IDependencyProvider provider = c.Dependencies.BuildProvider();

                IServiceProvider serviceProvider = provider.GetRequiredDependency<IServiceProvider>();

                configurationFactory.Invoke(c, serviceProvider);
            });

            return services;
        }

        #region Private Methods

        private static ApiConfigurationManager GetConfigurationManager(IServiceCollection services)
        {
            lock (GetConfigurationManagerLock)
            {
                // We don't want to call TryAdd as it generates a new instance.
                if (services.Any(x => x.ServiceType == typeof(ApiConfigurationManager)))
                {
                    return ConfigurationManagerInstance;
                }

                ConfigurationManagerInstance = new ApiConfigurationManager(f =>
                {
                    f.Dependencies.AddSingleton(() => services, Binding.Unbound);
                    f.Dependencies.AddSingleton<IServiceProvider>(p => p.GetRequiredDependency<IServiceCollection>().BuildServiceProvider());

                    if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
                    {
                        f.Dependencies.AddSingleton(p => p.GetRequiredDependency<IServiceProvider>().GetRequiredService<ILoggerFactory>());
                    }
                });

                // We're using an implementation instance as DI won't replace it. If we use the
                // standard AddSingleton<A, B> a new instance of HandlerConfiguration Manager
                // will be created.
                services.AddSingleton(ConfigurationManagerInstance);

                return ConfigurationManagerInstance;
            }
        }

        #endregion Private Methods
    }
}