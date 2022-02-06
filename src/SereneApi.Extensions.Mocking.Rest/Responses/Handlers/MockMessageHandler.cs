using SereneApi.Extensions.Mocking.Rest.Responses.Manager;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Handlers
{
    internal class MockMessageHandler : DelegatingHandler
    {
        private readonly bool _enableOutgoing;
        private readonly IMockResponseManager _responseManager;

        public MockMessageHandler(IMockResponseManager manager, bool enableOutgoing)
        {
            _responseManager = manager ?? throw new ArgumentNullException(nameof(manager));

            _enableOutgoing = enableOutgoing;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            HttpResponseMessage response = await _responseManager.GetMockResponseAsync(request, cancellationToken);

            if (response != null)
            {
                return response;
            }

            if (_enableOutgoing)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // As outgoing requests are not enabled and we could not find a mock response. A 404
            // status will be returned.
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}