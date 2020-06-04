using SereneApi;
using SereneApi.Helpers;
using SereneApi.Types;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="ApiHandler"/> using the <see cref="HttpClient"/> for the requests.
        /// The <see cref="HttpClient"/> will be disposed of by the <see cref="ApiHandler"/>.
        /// </summary>
        public static TApiHandler CreateApiHandler<TApiHandler>(this HttpClient client, Action<ApiHandlerOptionsBuilder> optionsAction = null) where TApiHandler : ApiHandler
        {
            // The base address of the HttpClient should not be change, so instead an exception will be thrown.
            SourceHelpers.CheckIfValid(client.BaseAddress.ToString());

            ApiHandlerOptionsBuilder builder = new ApiHandlerOptionsBuilder();

            builder.UseClientOverride(client, true);

            optionsAction?.Invoke(builder);

            TApiHandler handler = (TApiHandler)Activator.CreateInstance(typeof(TApiHandler), builder.BuildOptions());

            return handler;
        }

        #endregion
        #region Internal Methods

        internal static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PostAsync(requestUri, null);
        }

        internal static Task<HttpResponseMessage> PostAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PostAsync(requestUri, content.ToStringContent());
        }

        internal static Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PutAsync(requestUri, null);
        }

        internal static Task<HttpResponseMessage> PutAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PutAsync(requestUri, content.ToStringContent());
        }

        internal static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PatchAsync(requestUri, null);
        }

        internal static Task<HttpResponseMessage> PatchAsJsonAsync<TContent>(this HttpClient client, Uri requestUri, TContent content)
        {
            return client.PatchAsync(requestUri, content.ToStringContent());
        }

        #endregion
        #region Private Methods

        private static StringContent ToStringContent<TContent>(this TContent content)
        {
            return new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        }

        #endregion
    }
}
