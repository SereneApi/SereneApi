using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Handlers.Soap.Responses.Types
{
    /// <inheritdoc cref="IApiResponse"/>
    public class SoapApiResponse : IApiResponse
    {
        /// <inheritdoc cref="IApiRequest"/>
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

        private SoapApiResponse(IApiRequest request, Status status)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
        }

        private SoapApiResponse(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse Success(IApiRequest request, Status status) => new SoapApiResponse(request, status);

        public static IApiResponse Failure(IApiRequest request, Status status, [AllowNull] string message, [AllowNull] Exception exception = null) => new SoapApiResponse(request, status, message, exception);
    }
}
