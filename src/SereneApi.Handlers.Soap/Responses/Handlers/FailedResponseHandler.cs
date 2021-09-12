using Microsoft.Extensions.Logging;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Responses.Types;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Soap.Responses.Types;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Responses.Handlers
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
                return SoapApiResponse.Failure(request, status, duration, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await _serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogInformation("Deserialization completed successfully.");

                    return SoapApiResponse.Failure(request, status, duration, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return SoapApiResponse.Failure(request, status, duration, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return SoapApiResponse.Failure(request, status, duration, string.Empty);
        }

        public async Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, TimeSpan duration, HttpContent content)
        {
            if (content == null)
            {
                return SoapApiResponse<TResponse>.Failure(request, status, duration, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await _serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogInformation("Deserialization completed successfully.");

                    return SoapApiResponse<TResponse>.Failure(request, status, duration, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return SoapApiResponse<TResponse>.Failure(request, status, duration, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return SoapApiResponse<TResponse>.Failure(request, status, duration, string.Empty);
        }
    }
}