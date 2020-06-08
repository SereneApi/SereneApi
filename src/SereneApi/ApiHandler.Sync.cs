using Microsoft.Extensions.Logging;
using SereneApi.Types;
using System;
using System.Net.Http;

namespace SereneApi
{
    public abstract partial class ApiHandler
    {
        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        protected virtual IApiResponse<TResponse> ProcessResponse<TResponse>(HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure("Received an Empty Http Response");
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(responseMessage.ReasonPhrase);
            }

            try
            {
                TResponse response = _serializer.Deserialize<TResponse>(responseMessage.Content);

                return ApiResponse<TResponse>.Success(response);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Could not deserialize the returned value");

                return ApiResponse<TResponse>.Failure("Could not deserialize returned value.", exception);
            }
        }
    }
}
