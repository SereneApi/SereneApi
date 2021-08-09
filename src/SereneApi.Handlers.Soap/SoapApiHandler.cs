using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Handlers.Soap.Configuration;

namespace SereneApi.Handlers.Soap
{
    [ConfigurationProvider(typeof(SoapConfigurationProvider))]
    public abstract class SoapApiHandler : IApiHandler
    {
        private readonly IApiOptions _options;

        protected SoapApiHandler(IApiOptions options)
        {
            _options = options;
        }
    }
}
