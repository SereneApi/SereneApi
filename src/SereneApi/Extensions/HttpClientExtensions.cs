using SereneApi;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
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
        public static TApiHandler CreateApiHandler<TApiHandler>(this HttpClient client, Action<IApiHandlerOptionsBuilder> optionsAction = null) where TApiHandler : ApiHandler
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

        internal static Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, Uri requestUri, StringContent content)
        {
            return client.PostAsync(requestUri, content);
        }

        internal static Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PutAsync(requestUri, null);
        }

        internal static Task<HttpResponseMessage> PutAsJsonAsync(this HttpClient client, Uri requestUri, StringContent content)
        {
            return client.PutAsync(requestUri, content);
        }

        internal static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, Uri requestUri)
        {
            return client.PatchAsync(requestUri, null);
        }

        internal static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, Uri requestUri, StringContent content)
        {
            return client.PatchAsync(requestUri, content);
        }

        #endregion
        #region Private Methods

        //private static StringContent ToStringContent<TContent>(this TContent content)
        //{
        //    return new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        //}

        #endregion
    }
}
