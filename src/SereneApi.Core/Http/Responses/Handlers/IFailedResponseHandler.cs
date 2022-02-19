using SereneApi.Core.Http.Requests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Responses.Handlers
{
    public interface IFailedResponseHandler
    {
        Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);

        Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);
    }
}