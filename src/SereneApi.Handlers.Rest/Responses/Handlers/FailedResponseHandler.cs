using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Http.Responses.Types;
using SereneApi.Core.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Responses.Handlers
{
    public class FailedResponseHandler : IFailedResponseHandler
    {
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;

        public FailedResponseHandler(ISerializer serializer, ILogger logger = null)
        {
            _serializer = serializer;

            _logger = logger;
        }

        public async Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, TimeSpan duration, HttpContent content)
        {
            if (content == null)
            {
                return RestApiResponse.Failure(request, status, duration, string.Empty);
            }

            _logger?.LogDebug("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await _serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogDebug("Deserialization completed successfully.");

                    return RestApiResponse.Failure(request, status, duration, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return RestApiResponse.Failure(request, status, duration, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return RestApiResponse.Failure(request, status, duration, string.Empty);
        }

        public async Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, TimeSpan duration, HttpContent content)
        {
            if (content == null)
            {
                return RestApiResponse<TResponse>.Failure(request, status, duration, string.Empty);
            }

            _logger?.LogDebug("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await _serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogDebug("Deserialization completed successfully.");

                    return RestApiResponse<TResponse>.Failure(request, status, duration, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return RestApiResponse<TResponse>.Failure(request, status, duration, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return RestApiResponse<TResponse>.Failure(request, status, duration, string.Empty);
        }
    }
}