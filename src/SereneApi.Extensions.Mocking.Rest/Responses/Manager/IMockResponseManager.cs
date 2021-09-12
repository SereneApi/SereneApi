using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Manager
{
    public interface IMockResponseManager
    {
        Task<HttpResponseMessage> GetMockResponseAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}