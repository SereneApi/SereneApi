using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;
using System;

namespace SereneApi.Core.Configuration
{
    public interface IConfigurationManager
    {
        ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;

        void AmendConfiguration<THandler>(Action<IHandlerConfigurationFactory> factory) where THandler : ConfigurationProvider;
    }
}
