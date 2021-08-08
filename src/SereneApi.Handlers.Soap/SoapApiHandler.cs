using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Handlers.Soap.Configuration;

namespace SereneApi.Handlers.Soap
{
    [ConfigurationProvider(typeof(SoapConfigurationProvider))]
    public class SoapApiHandler : IApiHandler
    {
    }
}
