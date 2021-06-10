using SereneApi.Abstractions.Requests;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response
{
    /// <inheritdoc cref="IApiResponse"/>
    public class ApiResponse : IApiResponse
    {
        /// <inheritdoc cref="IApiRequest"/>
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

        private ApiResponse(IApiRequest request, Status status)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse Success(IApiRequest request, Status status) => new ApiResponse(request, status);

        public static IApiResponse Failure(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new ApiResponse(request, status, message, exception);
    }
}
