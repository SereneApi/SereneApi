using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Manager
{
    public interface IMockResponseManager
    {
        HttpResponseMessage GetMockResponse(HttpRequestMessage request, CancellationToken cancellationToken);

        Task<HttpResponseMessage> GetMockResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}