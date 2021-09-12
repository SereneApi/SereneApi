using Microsoft.Extensions.Logging;
using SereneApi.Core.Requests;
using SereneApi.Core.Response;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using SereneApi.Handlers.Soap.Responses.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Responses.Handlers
{
    public class ResponseHandler : IResponseHandler
    {
        private readonly IFailedResponseHandler _failedResponseHandler;
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;

        public ResponseHandler(ISerializer serializer, IFailedResponseHandler failedResponseHandler, ILogger logger = null)
        {
            _serializer = serializer;
            _failedResponseHandler = failedResponseHandler;

            _logger = logger;
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        public async Task<IApiResponse> ProcessResponseAsync(IApiRequest request, TimeSpan duration, [AllowNull] HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return SoapApiResponse.Failure(request, Status.None, duration, "Received an Empty Http Response");
            }

            IApiResponse response;

            Status status = responseMessage.StatusCode.ToStatus();

            if (status.IsSuccessCode())
            {
                _logger?.LogInformation("The request received a successful response.");

                response = SoapApiResponse.Success(request, status, duration);
            }
            else
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = await _failedResponseHandler.ProcessFailedRequestAsync(request, status, duration, responseMessage.Content);
            }

            return response;
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained
        /// <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        public async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(IApiRequest request, TimeSpan duration, HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return SoapApiResponse<TResponse>.Failure(request, Status.None, duration, "Received an Empty Http Response");
            }

            IApiResponse<TResponse> response;

            Status status = responseMessage.StatusCode.ToStatus();

            if (!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}",
                    responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = await _failedResponseHandler.ProcessFailedRequestAsync<TResponse>(request, status, duration, responseMessage.Content);
            }
            else
            {
                _logger?.LogInformation("The request received a successful response.");

                try
                {
                    TResponse responseData = await _serializer.DeserializeAsync<TResponse>(responseMessage.Content);

                    response = SoapApiResponse<TResponse>.Success(request, status, duration, responseData);
                }
                catch (JsonException jsonException)
                {
                    _logger?.LogError(jsonException, "Could not deserialize the returned value");

                    response = SoapApiResponse<TResponse>
                        .Failure(request, status, duration, "Could not deserialize returned value.", jsonException);
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                    response = SoapApiResponse<TResponse>
                        .Failure(request, status, duration, "An Exception occurred whilst processing the response.", exception);
                }
            }

            return response;
        }
    }
}