using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public MockMessageHandler(IMockResponseManager manager, bool enableOutgoing, ILogger logger = null)
        {
            _responseManager = manager ?? throw new ArgumentNullException(nameof(manager));

            _enableOutgoing = enableOutgoing;
            _logger = logger;
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
                _logger?.LogInformation("No Mock Response found for {requestUri} - Outgoing Requests enabled, invoking authentic request", request.RequestUri.ToString());

                return await base.SendAsync(request, cancellationToken);
            }

            _logger?.LogInformation("No Mock Response found for {requestUri} - Outgoing Requests disabled, returning NotFound [404]", request.RequestUri.ToString());

            // As outgoing requests are not enabled and we could not find a mock response. A 404
            // status will be returned.
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}