using SereneApi.Abstractions.Request;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response
{
    /// <inheritdoc cref="IApiResponse{TResult}"/>
    public class ApiResponse<TResult>: IApiResponse<TResult>
    {
        public IApiRequest Request { get; }

        /// <inheritdoc cref="IApiResponse.Status"/>
        public Status Status { get; }

        /// <inheritdoc cref="IApiResponse.WasSuccessful"/>
        public bool WasSuccessful { get; }

        /// <inheritdoc cref="IApiResponse.HasException"/>
        public bool HasException => Exception != null;

        /// <inheritdoc cref="IApiResponse.Message"/>
        public string Message { get; }

        /// <inheritdoc cref="IApiResponse.Exception"/>
        public Exception Exception { get; }

        /// <inheritdoc cref="IApiResponse{TEntity}.Data"/>
        public TResult Data { get; }

        private ApiResponse([NotNull] IApiRequest request, Status status, [AllowNull] TResult result)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Data = result;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse([NotNull] IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Data = default;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse<TResult> Success([NotNull] IApiRequest request, Status status, [AllowNull] TResult result) => new ApiResponse<TResult>(request, status, result);

        public static IApiResponse<TResult> Failure([NotNull] IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new ApiResponse<TResult>(request, status, message, exception);
    }
}
