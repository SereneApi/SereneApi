using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Factories;
using SereneApi.Core.Options.Factories;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Configuration;
using SereneApi.Extensions.Mocking.Rest.Responses.Handlers;
using SereneApi.Extensions.Mocking.Rest.Responses.Manager;
using System;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Rest
{
    public static class ApiOptionsExtensions
    {
        public static void EnableRestMocking(this IApiOptionsExtensions extensions, Action<IMockingConfiguration> configurator, bool outgoingRequests = false)
        {
            MockingConfiguration configuration;

            using (IDependencyProvider dependencies = extensions.Dependencies.BuildProvider())
            {
                IConfiguration config = dependencies.GetDependency<IConfiguration>();

                if (config["ApiType"] != "RestApi")
                {
                    throw new MethodAccessException("This method can only be used if the ApiType is RestApi");
                }

                configuration = new MockingConfiguration(dependencies);

                configurator.Invoke(configuration);
            }

            extensions.Dependencies.AddScoped(p => configuration.BuildManager(p.GetDependency<ISerializer>()));

            extensions.Dependencies.AddScoped<HttpMessageHandler>(p =>
            {
                if (!outgoingRequests)
                {
                    return new MockMessageHandler(p.GetDependency<IMockResponseManager>());
                }

                HttpMessageHandler handler = p.GetDependency<IClientFactory>().BuildHttpMessageHandler();

                return new MockMessageHandler(p.GetDependency<IMockResponseManager>(), handler);
            });
        }
    }
}