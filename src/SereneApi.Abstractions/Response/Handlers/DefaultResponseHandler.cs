using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Response.Handlers
{
    public class DefaultResponseHandler : IResponseHandler
    {
        private readonly IDependencyProvider _dependencyProvider;

        private readonly IFailedResponseHandler _failedResponseHandler;

        private readonly ILogger _logger;

        public DefaultResponseHandler(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider ?? throw new ArgumentNullException(nameof(dependencyProvider));

            _failedResponseHandler = _dependencyProvider.GetDependency<IFailedResponseHandler>();

            _dependencyProvider.TryGetDependency(out _logger);
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        public async Task<IApiResponse> ProcessResponseAsync(IApiRequest request, [AllowNull] HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse.Failure(request, Status.None, "Received an Empty Http Response");
            }

            IApiResponse response;

            Status status = responseMessage.StatusCode.ToStatus();

            if (status.IsSuccessCode())
            {
                _logger?.LogInformation("The request received a successful response.");

                response = ApiResponse.Success(request, status);
            }
            else
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = await _failedResponseHandler.ProcessFailedRequestAsync(request, status, responseMessage.Content);
            }

            return response;
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        public async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(IApiRequest request, HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(request, Status.None, "Received an Empty Http Response");
            }

            IApiResponse<TResponse> response;

            Status status = responseMessage.StatusCode.ToStatus();

            if (!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}",
                    responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = await _failedResponseHandler.ProcessFailedRequestAsync<TResponse>(request, status, responseMessage.Content);
            }
            else
            {
                _logger?.LogInformation("The request received a successful response.");

                if (responseMessage.Content == null)
                {
                    _logger.LogWarning("No content was received in the response.");

                    response = ApiResponse<TResponse>.Failure(request, status, "No content was received in the response.");
                }
                else
                {
                    try
                    {
                        ISerializer serializer = _dependencyProvider.GetDependency<ISerializer>();

                        TResponse responseData = await serializer
                            .DeserializeAsync<TResponse>(responseMessage.Content);

                        response = ApiResponse<TResponse>.Success(request, status, responseData);
                    }
                    catch (JsonException jsonException)
                    {
                        _logger?.LogError(jsonException, "Could not deserialize the returned value");

                        response = ApiResponse<TResponse>
                            .Failure(request, status, "Could not deserialize returned value.", jsonException);
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                        response = ApiResponse<TResponse>
                            .Failure(request, status, "An Exception occurred whilst processing the response.", exception);
                    }
                }
            }

            return response;
        }
    }
}
