using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Testing
{
    internal class TimeoutMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new TaskCanceledException();
        }
    }
}
