using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factory;
using System;

namespace SereneApi.Core.Configuration
{
    public interface IConfigurationManager
    {
        ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;

        void AmendConfiguration<TApiHandler>(Action<IHandlerConfigurationFactory> factory) where TApiHandler : IApiHandler;
    }
}
