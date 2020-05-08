using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationExtensions
    {
        public static readonly string ApiConfigKey = "ApiConfig";

        public static TValue Get<TValue>(this IConfiguration configuration, string key, bool required = true)
        {
            if (configuration.GetSection(key).Exists())
            {
                return configuration.Get<TValue>();
            }

            if (required)
            {
                throw new KeyNotFoundException($"Could not find {key} inside appsettings.json");
            }

            return default;
        }

        /// <summary>
        /// Returns the <see cref="IConfigurationSection"/> from within ApiConfig that matches the specified <see cref="apiKey"/>
        /// </summary>
        /// <param name="configuration">The ROOT <see cref="IConfiguration"/> to be searched in</param>
        /// <param name="apiKey">The <see cref="IConfigurationSection"/> name containing the API Configuration</param>
        public static IConfigurationSection GetApiConfig(this IConfiguration configuration, string apiKey)
        {
            if (configuration.GetSection(ApiConfigKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey} inside appsettings.json");
            }

            IConfigurationSection apiConfiguration = configuration.GetSection(ApiConfigKey);

            if (apiConfiguration.GetSection(apiKey).Exists())
            {
                throw new KeyNotFoundException($"Could not find {ApiConfigKey}:{apiKey} inside appsettings.json");
            }

            return apiConfiguration.GetSection(apiKey);
        }
    }
}
