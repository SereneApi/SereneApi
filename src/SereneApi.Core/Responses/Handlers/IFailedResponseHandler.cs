using SereneApi.Core.Requests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Core.Responses.Handlers
{
    public interface IFailedResponseHandler
    {
        IApiResponse ProcessFailedRequest(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);

        IApiResponse<TResponse> ProcessFailedRequest<TResponse>(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);

        Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);

        Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, TimeSpan duration, [AllowNull] HttpContent content);
    }
}