using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Response.Handlers
{
    public class DefaultFailedResponseHandler : IFailedResponseHandler
    {
        private readonly IDependencyProvider _dependencyProvider;

        private readonly ILogger _logger;

        public DefaultFailedResponseHandler(IDependencyProvider dependencyProvider)
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
                _logger?.LogWarning("Could not retrieve ISerializer dependency.");

                return ApiResponse.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                DefaultFailureResponse defaultFailureResponse = await serializer.DeserializeAsync<DefaultFailureResponse>(content);

                _logger?.LogInformation("Deserialization completed successfully.");

                return ApiResponse.Failure(request, status, defaultFailureResponse.Message);
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
                _logger?.LogWarning("Could not retrieve ISerializer dependency.");

                return ApiResponse<TResponse>.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                DefaultFailureResponse defaultFailureResponse = await serializer.DeserializeAsync<DefaultFailureResponse>(content);

                _logger?.LogInformation("Deserialization completed successfully.");

                return ApiResponse<TResponse>.Failure(request, status, defaultFailureResponse.Message);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return ApiResponse<TResponse>.Failure(request, status, string.Empty);
        }
    }
}
