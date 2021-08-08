using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Handlers.Rest.Configuration;

namespace SereneApi.Handlers.Rest
{
    [ConfigurationProvider(typeof(RestConfigurationProvider))]
    public abstract class RestApiHandler : IApiHandler
    {
        private readonly IApiOptions _options;

        protected RestApiHandler(IApiOptions options)
        {
            _options = options;
        }
    }
}
