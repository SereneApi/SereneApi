using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Http.Client.Handler;
using SereneApi.Extensions.Mocking.Rest.Configuration;
using SereneApi.Extensions.Mocking.Rest.Handler.Manager;
using SereneApi.Extensions.Mocking.Rest.Responses.Handlers;
using System;
using SereneApi.Extensions.Mocking.Rest.Responses.Manager;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class MockingApiConfiguration
    {
        /// <summary>
        /// Enables Mocking.
        /// </summary>
        /// <param name="configuration">The Mocking configuration, used to configure Mock Responses.</param>
        /// <param name="enableOutgoing">Specifies if the Mocking Middleware will enable outgoing requests if a Mock response is not found.</param>
        public static void EnableMocking(this IApiConfiguration apiConfiguration, Action<IMockingConfiguration> configuration, bool enableOutgoing = false)
        {
            apiConfiguration.Dependencies
                .Register<MockHandlerManager>()
                .DefineAs<IMockHandlerManager>()
                .AsScoped();

            apiConfiguration.Dependencies.Register(p =>
            {
                MockingConfiguration mockingConfiguration = p.CreateInstance<MockingConfiguration>();

                configuration.Invoke(mockingConfiguration);

                return mockingConfiguration.BuildManager();
            }).AsScoped();

            apiConfiguration.Dependencies.Configure<IHandlerFactory>((p, f) =>
            {
                f.AddHandler(new MockMessageHandler(enableOutgoing, p.GetRequiredDependency<IMockResponseManager>(), p.GetDependency<ILogger>()));
            });
        }
    }
}