using SereneApi.Core.Http.Client.Handler;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Responses.Configuration;
using SereneApi.Extensions.Mocking.Rest.Responses.Handlers;
using DeltaWare.Dependencies.Abstractions;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class MockingApiConfiguration
    {
        public static void EnableMocking(this IApiConfiguration configuration, Action<IMockingConfiguration> configurator, bool enableOutgoing = false)
        {
            configuration.Dependencies.Configure<IHandlerFactory>((p, f) =>
            {
                MockingConfiguration mockingConfiguration = new MockingConfiguration(p.GetRequiredDependency<ISerializer>());

                configurator.Invoke(mockingConfiguration);

                f.AddHandler(new MockMessageHandler(mockingConfiguration.BuildManager(), enableOutgoing));
            });
        }
    }
}