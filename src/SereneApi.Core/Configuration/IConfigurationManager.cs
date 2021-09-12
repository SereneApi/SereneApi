using SereneApi.Core.Handler;
using SereneApi.Core.Options.Factories;
using System;

namespace SereneApi.Core.Configuration
{
    public interface IConfigurationManager
    {
        void AmendConfiguration<THandler>(Action<IHandlerConfigurationFactory> factory) where THandler : ConfigurationFactory;

        ApiOptionsFactory<TApiHandler> BuildApiOptionsFactory<TApiHandler>() where TApiHandler : IApiHandler;
    }
}