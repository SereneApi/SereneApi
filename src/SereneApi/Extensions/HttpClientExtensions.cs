using SereneApi;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Creates a new <see cref="ApiHandler"/> using the <see cref="HttpClient"/> for the requests.
        /// The <see cref="HttpClient"/> will be disposed of by the <see cref="ApiHandler"/>.
        /// </summary>
        public static TApiHandler CreateApiHandler<TApiHandler>(this HttpClient client, Action<IApiHandlerOptionsBuilder> optionsAction = null) where TApiHandler : ApiHandler
        {
            // The base address of the HttpClient should not be change, so instead an exception will be thrown.
            SourceHelpers.CheckIfValid(client.BaseAddress.ToString());

            IClientFactory clientFactory = new OverrideClientFactory(client, true);

            DependencyCollection dependencyCollection = new DependencyCollection();

            dependencyCollection.AddDependency(clientFactory);
            dependencyCollection.AddDependency<IConnectionInfo>(new ConnectionInfo(client.BaseAddress));

            ApiHandlerOptionsBuilder builder = new ApiHandlerOptionsBuilder(dependencyCollection);

            optionsAction?.Invoke(builder);

            TApiHandler handler = (TApiHandler)Activator.CreateInstance(typeof(TApiHandler), builder.BuildOptions());

            return handler;
        }
    }
}
