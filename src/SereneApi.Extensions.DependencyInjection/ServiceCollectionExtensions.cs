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
        private static readonly object _getConfigurationManagerLock = new();
        private static ApiConfigurationManager ConfigurationManager { get; set; }

        public static void AmendConfigurationProvider<TConfigurationProvider>(this IServiceCollection services, Action<IApiConfiguration> configuration) where TConfigurationProvider : HandlerConfigurationProvider
        {
            GetConfigurationManager(services).AmendConfigurationProvider<TConfigurationProvider>(configuration);
        }

        public static void ExtendApi<TApiHandler>(this IServiceCollection services, Action<IApiConfiguration> configuration) where TApiHandler : IApiHandler
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            GetConfigurationManager(services).AddApiPostConfiguration<TApiHandler>(configuration);
        }

        public static void RegisterApi<TApi, TApiHandler>(this IServiceCollection services, Action<IApiConfiguration> configuration) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddScoped<TApi, TApiHandler>();
            services.AddScoped(p => p.GetRequiredService<ApiConfigurationManager>().BuildApiOptions<TApiHandler>());

            ApiConfigurationManager configurationManager = GetConfigurationManager(services);

            configurationManager.AddApiPreConfiguration<TApiHandler>(c =>
            {
                if (c.Dependencies.HasDependency<ILoggerFactory>())
                {
                    c.Dependencies.AddScoped<ILogger>(p => p.GetRequiredDependency<ILoggerFactory>().CreateLogger<TApiHandler>());
                }
            });

            configurationManager.AddApiConfiguration<TApiHandler>(configuration);
        }

        public static void RegisterApi<TApi, TApiHandler>(this IServiceCollection services, Action<IApiConfiguration, IServiceProvider> configuration) where TApi : class where TApiHandler : class, IApiHandler, TApi
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddScoped<TApi, TApiHandler>();
            services.AddScoped(p => p.GetRequiredService<ApiConfigurationManager>().BuildApiOptions<TApiHandler>());

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

                configuration.Invoke(c, serviceProvider);
            });
        }

        #region Private Methods

        private static ApiConfigurationManager GetConfigurationManager(IServiceCollection services)
        {
            lock (_getConfigurationManagerLock)
            {
                // We don't want to call TryAdd as it generates a new instance.
                if (services.Any(x => x.ServiceType == typeof(ApiConfigurationManager)))
                {
                    return ConfigurationManager;
                }

                ConfigurationManager = new ApiConfigurationManager(f =>
                {
                    f.Dependencies.AddSingleton(() => services, Binding.Unbound);
                    f.Dependencies.AddSingleton<IServiceProvider>(p => p.GetRequiredDependency<IServiceCollection>().BuildServiceProvider());

                    if (services.Any(x => x.ServiceType == typeof(ILoggerFactory)))
                    {
                        f.Dependencies.AddSingleton(p => p.GetRequiredDependency<IServiceProvider>().GetRequiredService<ILoggerFactory>());
                    }
                });

                // We're using an implementation instance as DI won't replace it. If we use the
                // standard AddSingleton<A, B> a new instance of HandlerConfiguration Manager will
                // be created.
                services.AddSingleton(ConfigurationManager);

                return ConfigurationManager;
            }
        }

        #endregion Private Methods
    }
}