using SereneApi.Core.Handler;
using SereneApi.Core.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Requests.Handler
{
    public interface IRequestHandler
    {
        Task<IApiResponse> PerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);

        Task<IApiResponse<TResponse>> PerformAsync<TResponse>(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);
    }
}
