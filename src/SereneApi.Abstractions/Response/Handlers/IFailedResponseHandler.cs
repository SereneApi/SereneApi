using SereneApi.Abstractions.Request;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Response.Handlers
{
    public interface IFailedResponseHandler
    {
        IApiResponse ProcessFailedRequest([NotNull] IApiRequest request, Status status, [AllowNull] HttpContent content);

        Task<IApiResponse> ProcessFailedRequestAsync([NotNull] IApiRequest request, Status status, [AllowNull] HttpContent content);

        IApiResponse<TResponse> ProcessFailedRequest<TResponse>([NotNull] IApiRequest request, Status status, [AllowNull] HttpContent content);

        Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>([NotNull] IApiRequest request, Status status, [AllowNull] HttpContent content);
    }
}
