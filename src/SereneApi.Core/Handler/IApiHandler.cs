using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Handler
{
    public interface IApiHandler : IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiSettings Settings { get; }

        Task<IApiResponse> PerformRequestAsync(IApiRequest request, IApiRequestOptions options, CancellationToken cancellationToken = default);

        Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(IApiRequest request, IApiRequestOptions options, CancellationToken cancellationToken = default);
    }
}