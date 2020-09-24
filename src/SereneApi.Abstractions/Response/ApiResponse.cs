using System;
using System.Diagnostics.CodeAnalysis;
using SereneApi.Abstractions.Request;

namespace SereneApi.Abstractions.Response
{
    /// <inheritdoc cref="IApiResponse"/>
    public class ApiResponse: IApiResponse
    {
        /// <inheritdoc cref="IApiResponse.IApiRequest"/>
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

        private ApiResponse([NotNull] IApiRequest request, Status status)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse([NotNull] IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse Success([NotNull] IApiRequest request, Status status) => new ApiResponse(request, status);

        public static IApiResponse Failure([NotNull] IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new ApiResponse(request, status, message, exception);
    }
}
