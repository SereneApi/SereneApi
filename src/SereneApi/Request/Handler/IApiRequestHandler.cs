using SereneApi.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Request.Handler
{
    internal interface IApiRequestHandler
    {
        Task<IApiResponse> ExecuteAsync(IApiRequest apiRequest, CancellationToken cancellationToken = default);
    }
}
