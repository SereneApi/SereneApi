using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Configuration;
using SereneApi.Core.Configuration;
using SereneApi.Core.Http;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class ApiConfigurationExtensions
    {
        /// <summary>
        /// Configures the APIs <see cref="IConnectionSettings"/> using the <see cref="IConfiguration"/>.
        /// </summary>
        /// <remarks>All configuration is retrieved from the <strong>ApiConfig</strong> section. When the ApiConfigurationKey is null the key will be retrieved from the handler name.</remarks>
        /// <param name="apiConfiguration"></param>
        /// <param name="configuration">Provides the configuration to be used.</param>
        /// <param name="apiConfigurationKey">Specifies the configuration section to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified api key was not found. </exception>
        /// <exception cref="ArgumentException">Thrown when the ApiConfiguration is null and the HandlerType cannot be distinguished.</exception>
        public static void UseConfiguration(this IApiConfiguration apiConfiguration, IConfiguration configuration, string apiConfigurationKey = null)
        {
            if (apiConfigurationKey == null)
            {
                using IDependencyProvider provider = apiConfiguration.Dependencies.BuildProvider();

                if (!provider.TryGetDependency(out HandlerConfiguration handlerConfiguration))
                {
                    throw new ArgumentException("Unable to retrieve HandlerType the apiConfigurationKey must be set.");
                }

                if (!handlerConfiguration.TryGetHandlerType(out Type handlerType))
                {
                    throw new ArgumentException("Unable to retrieve HandlerType the apiConfigurationKey must be set.");
                }

                apiConfigurationKey = GetApiName(handlerType);
            }

            apiConfiguration.AddConnectionSettings(configuration.GetApiConfig(apiConfigurationKey));
        }

        /// <summary>
        /// Configures the APIs <see cref="IConnectionSettings"/> using the <see cref="IConfiguration"/>.
        /// </summary>
        /// <remarks>All configuration is retrieved from the <strong>ApiConfig</strong> section.</remarks>
        /// <param name="apiConfiguration"></param>
        /// <param name="configuration">Provides the configuration to be used.</param>
        /// <param name="apiConfigurationKey">Specifies the configuration section to be used.</param>
        /// <param name="apiBaseAddressKey">Specifies the configuration section to be used as the Base Address.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified api key was not found. </exception>
        public static void UseConfiguration(this IApiConfiguration apiConfiguration, IConfiguration configuration, string apiConfigurationKey, string apiBaseAddressKey)
        {
            apiConfiguration.AddConnectionSettings(configuration.GetApiConfig(apiConfigurationKey, apiBaseAddressKey));
        }

        private static string GetApiName(Type handlerType)
        {
            return handlerType.Name.Replace("Handler", string.Empty);
        }
    }
}
