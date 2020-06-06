using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Testing
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _mockResponseMessage;

        private readonly Uri _requestUri;

        public MockHttpMessageHandler(HttpResponseMessage mockResponseMessage, Uri requestUri = null)
        {
            _mockResponseMessage = mockResponseMessage;
            _requestUri = requestUri;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (_requestUri == null || request.RequestUri == _requestUri)
                {
                    return _mockResponseMessage;
                }

                throw new ArgumentException($"Received an Incorrect Uri - Received:{request.RequestUri} | Expected:{_requestUri}");
            }, cancellationToken);
        }
    }
}
