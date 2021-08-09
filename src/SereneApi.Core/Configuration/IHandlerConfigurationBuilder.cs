using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factory;

namespace SereneApi.Core.Configuration
{
    internal interface IHandlerConfigurationBuilder
    {
        ApiOptionsFactory<TApiHandler> BuildOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;
    }
}
