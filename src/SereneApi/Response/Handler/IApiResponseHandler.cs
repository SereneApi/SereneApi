using SereneApi.Request;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Response.Handler
{
    public interface IApiResponseHandler
    {
        Task<IApiResponse> HandleSuccessfulResponseAsync(IApiRequest apiRequest, HttpResponseMessage httpResponse, CancellationToken cancellationToken = default);

        Task<IApiResponse> HandleFailedResponseAsync(IApiRequest apiRequest, HttpResponseMessage httpResponse, CancellationToken cancellationToken = default);
    }
}
