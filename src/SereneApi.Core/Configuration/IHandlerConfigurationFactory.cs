using SereneApi.Core.Handler;
using SereneApi.Core.Options.Builder;

namespace SereneApi.Core.Configuration
{
    internal interface IHandlerConfigurationFactory
    {
        IApiOptionsFactory<TApiHandler> BuildOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;
    }
}
