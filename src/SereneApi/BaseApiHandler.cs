using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Response.Events;
using SereneApi.Abstractions.Serialization;
using SereneApi.Extensions;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SereneApi.Abstractions.Request;

namespace SereneApi
{
    /// <summary>
    /// When Inherited; Provides the methods required for implementing a RESTful Api consumer.
    /// </summary>
    [DebuggerDisplay("Source:{Connection.Source}; Timeout:{Connection.Timeout}")]
    public abstract partial class BaseApiHandler: IApiHandler
    {
        #region Variables

        private readonly ILogger _logger;

        private readonly IEventManager _eventManager;

        #endregion
        #region Properties

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        protected IApiOptions Options { get; }

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionConfiguration Connection { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="BaseApiHandler"/>.
        /// </summary>
        /// <param name="options">The options to be used when making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        protected BaseApiHandler([NotNull] IApiOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Connection = options.Connection;

            Options = options;
            Options.RetrieveDependency(out _logger);
            Options.RetrieveDependency(out _eventManager);

            _logger?.LogInformation($"{GetType()} has been instantiated");
        }

        #endregion
        #region Response Processing

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private IApiResponse ProcessResponse([NotNull] IApiRequest request, [AllowNull] HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse.Failure(request, Status.None, "Received an Empty Http Response");
            }

            IApiResponse response;

            Status status = responseMessage.StatusCode.ToStatus();

            if(status.IsSuccessCode())
            {
                _logger?.LogInformation("The request received a successful response.");

                response = ApiResponse.Success(request, status);
            }
            else
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = ProcessFailedRequest(request, status, responseMessage.Content);
            }

            _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

            return response;
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private IApiResponse<TResponse> ProcessResponse<TResponse>([NotNull] IApiRequest request, HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(responseMessage == null)
            {
                _logger?.LogWarning("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(request, Status.None, "Received an Empty Http Response");
            }

            IApiResponse<TResponse> response;

            Status status = responseMessage.StatusCode.ToStatus();

            if(!responseMessage.IsSuccessStatusCode)
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = ProcessFailedRequest<TResponse>(request, status, responseMessage.Content);
            }
            else
            {
                if(responseMessage.Content == null)
                {
                    _logger.LogWarning("No content was received in the response.");

                    response = ApiResponse<TResponse>.Failure(request, status, "No content was received in the response.");
                }
                else
                {
                    try
                    {
                        ISerializer serializer = Options.RetrieveRequiredDependency<ISerializer>();

                        TResponse responseData =
                            serializer.Deserialize<TResponse>(responseMessage.Content);

                        response = ApiResponse<TResponse>.Success(request, status, responseData);
                    }
                    catch(JsonException jsonException)
                    {
                        _logger?.LogError(jsonException, "Could not deserialize the returned value");

                        response = ApiResponse<TResponse>.Failure(request, status, "Could not deserialize returned value.",
                            jsonException);
                    }
                    catch(Exception exception)
                    {
                        _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                        response = ApiResponse<TResponse>.Failure(request, status,
                            "An Exception occurred whilst processing the response.", exception);
                    }
                }
            }

            _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

            return response;
        }

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>([NotNull] IApiRequest request, HttpResponseMessage responseMessage)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(request, Status.None, "Received an Empty Http Response");
            }

            IApiResponse<TResponse> response;

            Status status = responseMessage.StatusCode.ToStatus();

            if(!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}",
                    responseMessage.StatusCode, responseMessage.ReasonPhrase);

                response = ProcessFailedRequest<TResponse>(request, status, responseMessage.Content);
            }
            else
            {
                _logger?.LogInformation("The request received a successful response.");

                if(responseMessage.Content == null)
                {
                    _logger.LogWarning("No content was received in the response.");

                    response = ApiResponse<TResponse>.Failure(request, status, "No content was received in the response.");
                }
                else
                {
                    try
                    {
                        ISerializer serializer = Options.RetrieveRequiredDependency<ISerializer>();

                        TResponse responseData = await serializer
                            .DeserializeAsync<TResponse>(responseMessage.Content);

                        response = ApiResponse<TResponse>.Success(request, status, responseData);
                    }
                    catch(JsonException jsonException)
                    {
                        _logger?.LogError(jsonException, "Could not deserialize the returned value");

                        response = ApiResponse<TResponse>
                            .Failure(request, status, "Could not deserialize returned value.", jsonException);
                    }
                    catch(Exception exception)
                    {
                        _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                        response = ApiResponse<TResponse>
                            .Failure(request, status, "An Exception occurred whilst processing the response.", exception);
                    }
                }
            }

            _eventManager?.PublishAsync(new ResponseEvent(this, response)).FireAndForget();

            return response;
        }

        private IApiResponse ProcessFailedRequest([NotNull] IApiRequest request, Status status, [AllowNull] HttpContent content)
        {
            if(content == null)
            {
                return ApiResponse.Failure(request, status, string.Empty);
            }

            if(!Options.RetrieveDependency(out ISerializer serializer))
            {
                _logger?.LogWarning("Could not retrieve ISerializer dependency.");

                return ApiResponse.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                HttpError httpError = serializer.Deserialize<HttpError>(content);

                _logger?.LogInformation("Deserialization completed successfully.");

                return ApiResponse.Failure(request, status, httpError.Message);
            }
            catch(Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return ApiResponse.Failure(request, status, string.Empty);
        }

        private IApiResponse<TResponse> ProcessFailedRequest<TResponse>([NotNull]IApiRequest request, Status status, [AllowNull] HttpContent content)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(content == null)
            {
                return ApiResponse<TResponse>.Failure(request, status, string.Empty);
            }

            if(!Options.RetrieveDependency(out ISerializer serializer))
            {
                _logger?.LogWarning("Could not retrieve ISerializer dependency.");

                return ApiResponse<TResponse>.Failure(request, status, string.Empty);
            }

            _logger?.LogInformation("Endpoint returned in body content for failed request, attempting deserialization.");

            try
            {
                HttpError httpError = serializer.Deserialize<HttpError>(content);

                _logger?.LogInformation("Deserialization completed successfully.");

                return ApiResponse<TResponse>.Failure(request, status, httpError.Message);
            }
            catch(Exception exception)
            {
                _logger?.LogWarning(exception, "Could not deserialize in body content.");
            }

            return ApiResponse<TResponse>.Failure(request, status, string.Empty);
        }

        #endregion
        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Throws an Object Disposed Exception if the <see cref="BaseApiHandler"/> has been disposed.
        /// </summary>
        protected void CheckIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="BaseApiHandler"/>.
        /// </summary>
        public void Dispose()
        {
            _logger?.LogInformation($"{GetType()} is being disposed");

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this method to implement <see cref="BaseApiHandler"/> disposal.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                Options.Dispose();
            }

            _disposed = true;
        }

        #endregion

        private string GetRequestRoute(Uri endpoint)
        {
            return $"{Options.Connection.BaseAddress}{endpoint}";
        }
    }
}
