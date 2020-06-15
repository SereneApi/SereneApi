using SereneApi.Extensions.DependencyInjection;
using System.Collections.Generic;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Returns the <see cref="IConfiguration"/> from within ApiConfig that matches the specified <see cref="apiKey"/>
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched in</param>
        /// <param name="apiKey">The <see cref="IConfiguration"/> name containing the API Configuration</param>
        public static IConfiguration GetApiConfig(this IConfiguration configuration, string apiKey)
        {
            if (!configuration.GetSection(ConfigurationConstants.ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationConstants.ApiConfigKey} inside the Configuration");
            }

            IConfiguration apiConfiguration = configuration.GetSection(ConfigurationConstants.ApiConfigKey);

            if (!apiConfiguration.GetSection(apiKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ConfigurationConstants.ApiConfigKey}:{apiKey} inside the Configuration");
            }

            return apiConfiguration.GetSection(apiKey);
        }

        internal static TValue Get<TValue>(this IConfiguration configuration, string key, bool required = true)
        {
            if (configuration.GetSection(key).Exists())
            {
                return configuration.GetSection(key).Get<TValue>();
            }

            if (required)
            {
                throw new KeyNotFoundException($"Could not find {key} inside the Configuration");
            }

            return default;
        }

        internal static bool ContainsKey(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Exists();
        }
    }
}
