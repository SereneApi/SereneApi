using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response
{
    /// <inheritdoc cref="IApiResponse"/>
    public class ApiResponse: IApiResponse
    {
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

        private ApiResponse(Status status)
        {
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse(Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse Success(Status status) => new ApiResponse(status);

        public static IApiResponse Failure(Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new ApiResponse(status, message, exception);
    }
}
