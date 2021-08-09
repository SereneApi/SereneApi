using SereneApi.Core.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Core.Responses.Handlers
{
    public interface IFailedResponseHandler
    {
        Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, [AllowNull] HttpContent content);

        Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, [AllowNull] HttpContent content);
    }
}
