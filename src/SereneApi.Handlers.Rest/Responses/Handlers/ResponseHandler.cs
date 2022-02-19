using Microsoft.Extensions.Logging;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Responses.Handlers
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

                return RestApiResponse.Failure(request, Status.None, duration, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if (status.IsSuccessCode())
            {
                _logger?.LogTrace("The request received a successful response.");

                return RestApiResponse.Success(request, status, duration);
            }

            _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

            return await _failedResponseHandler.ProcessFailedRequestAsync(request, status, duration, responseMessage.Content);
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

                return RestApiResponse<TResponse>.Failure(request, Status.None, duration, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if (!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}",
                    responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return await _failedResponseHandler.ProcessFailedRequestAsync<TResponse>(request, status, duration, responseMessage.Content);
            }

            _logger?.LogTrace("The request received a successful response.");

            try
            {
                TResponse responseData;

                if (typeof(TResponse).IsAssignableFrom(typeof(MemoryStream)))
                {
                    Stream stream = await responseMessage.Content.ReadAsStreamAsync();

                    MemoryStream memoryStream = new MemoryStream();

                    await stream.CopyToAsync(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    responseData = (TResponse)(object)memoryStream;
                }
                else
                {
                    responseData = await _serializer.DeserializeAsync<TResponse>(responseMessage.Content);
                }

                return RestApiResponse<TResponse>.Success(request, status, duration, responseData);
            }
            catch (JsonException jsonException)
            {
                _logger?.LogError(jsonException, "Could not deserialize the returned value");

                return RestApiResponse<TResponse>
                    .Failure(request, status, duration, "Could not deserialize returned value.", jsonException);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                return RestApiResponse<TResponse>
                    .Failure(request, status, duration, "An Exception occurred whilst processing the response.", exception);
            }
        }
    }
}