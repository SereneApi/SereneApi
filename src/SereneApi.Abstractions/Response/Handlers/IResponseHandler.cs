using SereneApi.Abstractions.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Response.Handlers
{
    public interface IResponseHandler
    {
        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        Task<IApiResponse> ProcessResponseAsync(IApiRequest request, [AllowNull] HttpResponseMessage responseMessage);

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(IApiRequest request, HttpResponseMessage responseMessage);
    }
}
