using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Response.Types;
using SereneApi.Core;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Responses.Types;
using SereneApi.Core.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Handlers
{
    public class FailedResponseHandler : IFailedResponseHandler
    {
        private readonly IDependencyProvider _dependencyProvider;

        private readonly ILogger _logger;

        public FailedResponseHandler(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider ?? throw new ArgumentNullException(nameof(dependencyProvider));

            _dependencyProvider.TryGetDependency(out _logger);
        }

        public async Task<IApiResponse> ProcessFailedRequestAsync(IApiRequest request, Status status, HttpContent content)
        {
            if (content == null)
            {
                return ApiResponse.Failure(request, status, string.Empty);
            }

            if (!_dependencyProvider.TryGetDependency(out ISerializer serializer))
            {
                _logger?.LogError(Logging.EventIds.DependencyNotFound, Logging.Messages.DependencyNotFound, nameof(ISerializer));

                return ApiResponse.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogInformation("Deserialization completed successfully.");

                    return ApiResponse.Failure(request, status, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return ApiResponse.Failure(request, status, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return ApiResponse.Failure(request, status, string.Empty);
        }

        public async Task<IApiResponse<TResponse>> ProcessFailedRequestAsync<TResponse>(IApiRequest request, Status status, HttpContent content)
        {
            if (content == null)
            {
                return ApiResponse<TResponse>.Failure(request, status, string.Empty);
            }

            if (!_dependencyProvider.TryGetDependency(out ISerializer serializer))
            {
                _logger?.LogError(Logging.EventIds.DependencyNotFound, Logging.Messages.DependencyNotFound, nameof(ISerializer));

                return ApiResponse<TResponse>.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                FailureResponse failureResponse = await serializer.DeserializeAsync<FailureResponse>(content);

                if (failureResponse != null)
                {
                    _logger?.LogInformation("Deserialization completed successfully.");

                    return ApiResponse<TResponse>.Failure(request, status, failureResponse.Message);
                }

                string message = await content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return ApiResponse<TResponse>.Failure(request, status, message);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return ApiResponse<TResponse>.Failure(request, status, string.Empty);
        }
    }
}
