using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;

namespace SereneApi.Core.Configuration
{
    internal interface IHandlerConfigurationBuilder
    {
        ApiOptionsFactory<TApiHandler> BuildOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;
    }
}
