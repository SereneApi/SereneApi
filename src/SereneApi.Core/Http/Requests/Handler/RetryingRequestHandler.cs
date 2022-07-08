using Microsoft.Extensions.Logging;
using SereneApi.Core.Events;
using SereneApi.Core.Events.Types;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Responses.Handlers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Requests.Handler
{
    public class RetryingRequestHandler : RequestHandler
    {
        private readonly IConnectionSettings _connection;
        private readonly IEventManager _eventManager;
        private readonly ILogger _logger;

        public RetryingRequestHandler(IClientFactory clientFactory, IConnectionSettings connection, IResponseHandler responseHandler, IEventManager eventManager = null, ILogger logger = null) : base(clientFactory, connection, responseHandler, eventManager, logger)
        {
            _connection = connection;
            _eventManager = eventManager;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> InternalPerformAsync(IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            _eventManager?.PublishAsync(new RequestEvent(caller, request)).FireAndForget();

            HttpClient client = await ClientFactory.BuildClientAsync(out bool disposeClient);

            try
            {
                return await PerformRetryingRequestAsync(client, request, caller, cancellationToken);
            }
            finally
            {
                if (disposeClient)
                {
                    client?.Dispose();
                }
            }
        }

        private async Task<HttpResponseMessage> PerformRetryingRequestAsync(HttpClient client, IApiRequest request, IApiHandler caller, CancellationToken cancellationToken = default)
        {
            bool retryingRequest;
            int requestsAttempted = 0;

            do
            {
                try
                {
                    HttpResponseMessage response = await HandleRequestAsync(client, request, cancellationToken);

                    _logger?.LogInformation(Logging.EventIds.ResponseReceivedEvent,
                        Logging.Messages.ReceivedResponse,
                        request.HttpMethod.ToString(), GetRequestRoute(request), response.StatusCode);

                    return response;
                }
                catch (TaskCanceledException canceledException)
                {
                    // TODO: This may be thrown if a task is cancelled by the CancellationToken.

                    requestsAttempted++;

                    if (_connection.RetryAttempts == 0 || requestsAttempted == _connection.RetryAttempts)
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, canceledException,
                            Logging.Messages.TimeoutNoRetry, request.HttpMethod, GetRequestRoute(request),
                            requestsAttempted);

                        retryingRequest = false;
                    }
                    else
                    {
                        _logger?.LogWarning(Logging.EventIds.RetryEvent, Logging.Messages.TimeoutRetry,
                            request.HttpMethod,
                            GetRequestRoute(request), _connection.RetryAttempts - requestsAttempted);

                        _eventManager?.PublishAsync(new RetryEvent(caller, request)).FireAndForget();

                        retryingRequest = true;
                    }
                }
            } while (retryingRequest);

            throw new TimeoutException($"The [{request.HttpMethod}] request to \"{GetRequestRoute(request)}\" has Timed out; The retry limit has been reached after attempting {requestsAttempted} times");
        }
    }
}