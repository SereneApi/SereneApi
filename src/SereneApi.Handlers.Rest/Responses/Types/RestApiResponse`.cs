using System;
using System.Diagnostics.CodeAnalysis;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;

namespace SereneApi.Handlers.Rest.Responses.Types
{
    /// <inheritdoc cref="IApiResponse"/>
    public class RestApiResponse<TResult> : IApiResponse<TResult>
    {
        public IApiRequest Request { get; }

        /// <inheritdoc cref="IApiResponse.Status"/>
        public Status Status { get; }

        /// <inheritdoc cref="IApiResponse.WasSuccessful"/>
        public bool WasSuccessful { get; }

        /// <inheritdoc cref="IApiResponse.WasNotSuccessful"/>
        public bool WasNotSuccessful => !WasSuccessful;

        /// <inheritdoc cref="IApiResponse.HasException"/>
        public bool HasException => Exception != null;

        /// <inheritdoc cref="IApiResponse.Message"/>
        public string Message { get; }

        /// <inheritdoc cref="IApiResponse.Exception"/>
        public Exception Exception { get; }

        /// <inheritdoc cref="IApiResponse{TEntity}.Data"/>
        public TResult Data { get; }

        private RestApiResponse(IApiRequest request, Status status, [AllowNull] TResult result)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Data = result;
            Message = null;
            Status = status;
            Exception = null;
        }

        private RestApiResponse(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Data = default;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse<TResult> Success(IApiRequest request, Status status, [AllowNull] TResult result) => new RestApiResponse<TResult>(request, status, result);

        public static IApiResponse<TResult> Failure(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new RestApiResponse<TResult>(request, status, message, exception);
    }
}
