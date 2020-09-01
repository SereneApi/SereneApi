using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Response.Content;
using SereneApi.Abstractions.Serialization;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

        #endregion
        #region Properties

        /// <summary>
        /// The dependencies that may be used by this API.
        /// </summary>
        protected IApiOptions Options { get; }

        /// <inheritdoc cref="IApiHandler.Connection"/>
        public IConnectionSettings Connection { get; }

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

            _logger?.LogTrace($"{GetType()} has been instantiated");
        }

        #endregion
        #region Response Processing

        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private IApiResponse ProcessResponse([AllowNull] HttpResponseMessage responseMessage)
        {
            if(responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse.Failure(Status.None, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if(status.IsSuccessCode())
            {
                _logger?.LogTrace("The request received a successful response.");

                return ApiResponse.Success(status);
            }

            _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

            return ApiResponse.Failure(status, responseMessage.ReasonPhrase);
        }


        /// <summary>
        /// Processes the returned <see cref="HttpResponseMessage"/> deserializing the contained <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized from the response</typeparam>
        /// <param name="responseMessage">The <see cref="HttpResponseMessage"/> to process</param>
        private async Task<IApiResponse<TResponse>> ProcessResponseAsync<TResponse>(HttpResponseMessage responseMessage)
        {
            if(responseMessage == null)
            {
                _logger?.LogError("Received an Empty Http Response");

                return ApiResponse<TResponse>.Failure(Status.None, "Received an Empty Http Response");
            }

            Status status = responseMessage.StatusCode.ToStatus();

            if(!status.IsSuccessCode())
            {
                _logger?.LogWarning("Http Request was not successful, received:{statusCode} - {message}", responseMessage.StatusCode, responseMessage.ReasonPhrase);

                return ApiResponse<TResponse>.Failure(status, responseMessage.ReasonPhrase);
            }

            _logger?.LogTrace("The request received a successful response.");

            if(responseMessage.Content == null)
            {
                _logger.LogWarning("No content was received in the response.");

                return ApiResponse<TResponse>.Failure(status, "No content was received in the response.");
            }

            try
            {
                ISerializer serializer = Options.RetrieveRequiredDependency<ISerializer>();

                TResponse response = await serializer.DeserializeAsync<TResponse>(new HttpContentResponse(responseMessage.Content));

                return ApiResponse<TResponse>.Success(status, response);
            }
            catch(JsonException jsonException)
            {
                _logger?.LogError(jsonException, "Could not deserialize the returned value");

                return ApiResponse<TResponse>.Failure(status, "Could not deserialize returned value.", jsonException);
            }
            catch(Exception exception)
            {
                _logger?.LogError(exception, "An Exception occurred whilst processing the response.");

                return ApiResponse<TResponse>.Failure(status, "An Exception occurred whilst processing the response.", exception);
            }
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
            _logger?.LogTrace($"{GetType()} is being disposed");

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
    }
}
