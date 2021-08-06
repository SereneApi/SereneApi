using System.Threading;
using System.Threading.Tasks;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Response;

namespace SereneApi.Abstractions.Requests.Handler
{
    public interface IRequestHandler
    {
        Task<IApiResponse> PerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);

        Task<IApiResponse<TResponse>> PerformAsync<TResponse>(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);
    }
}
