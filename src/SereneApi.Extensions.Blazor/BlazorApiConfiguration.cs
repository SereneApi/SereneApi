using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Serialization;
using System.Net;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration
{
    public static class BlazorApiConfiguration
    {
        private const string ImpersonatingRequestHandler = "ImpersonatingRequestHandler";
        private const string NewtonsoftSerializer = "NewtonsoftSerializer";

        /// <summary>
        /// Enables support for Blazor WebAssembly.
        /// </summary>
        /// <remarks>Support is enable by disabling features and extensions.</remarks>
        public static void EnableBlazorWebAssemblySupport(this IApiConfiguration configuration)
        {
            // Credentials is not supported by blazor, BrowserHttpHandler throws a NotSupportedException.
            configuration.Dependencies.Remove<ICredentials>();

            // Newtonsoft is not support by blazor, as a JS Interop is called to process JSON.
            if (configuration.Dependencies.Get<ISerializer>().ImplementationType.Name.Equals(NewtonsoftSerializer))
            {
                configuration.Dependencies
                    .Register<JsonSerializer>()
                    .DefineAs<ISerializer>()
                    .AsSingleton();
            }

            // Windows impersonation is not supported by blazor.
            if (configuration.Dependencies.Get<ISerializer>().ImplementationType.Name.Equals(ImpersonatingRequestHandler))
            {
                configuration.Dependencies
                    .Register<RetryingRequestHandler>()
                    .DefineAs<IRequestHandler>()
                    .AsScoped();
            }
        }
    }
}