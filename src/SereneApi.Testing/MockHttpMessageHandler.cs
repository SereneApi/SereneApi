using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Testing
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _mockResponseMessage;

        public MockHttpMessageHandler(HttpResponseMessage mockResponseMessage)
        {
            _mockResponseMessage = mockResponseMessage;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => _mockResponseMessage, cancellationToken);
        }
    }
}
