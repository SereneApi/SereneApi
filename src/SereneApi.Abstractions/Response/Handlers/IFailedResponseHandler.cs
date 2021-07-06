using SereneApi.Abstractions.Requests;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Response.Handlers
{
    public interface IFailedResponseHandler
    {
        Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, [AllowNull] HttpContent content);

        Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, [AllowNull] HttpContent content);
    }
}
