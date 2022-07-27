using SereneApi.Core.Configuration.Handler;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class ApiConfigurationExtensions
    {
        public static void SetRouteTemplate(this IApiConfiguration configuration, string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            configuration.SetHandlerConfiguration(config =>
            {
                config.SetRouteTemplate(template);
            });
        }
    }
}