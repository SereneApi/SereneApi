using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Requests
{
    public interface IRequestHandler
    {
        Task<IApiResponse> PerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);

        Task<IApiResponse<TResponse>> PerformAsync<TResponse>(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default);
    }
}
