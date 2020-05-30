using SereneApi.Abstraction;
using System;

namespace SereneApi.Types
{
    public readonly struct ApiResponse : IApiResponse
    {
        public bool WasSuccessful { get; }

        public bool HasException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        private ApiResponse(string message)
        {
            WasSuccessful = true;

            Message = message;

            Exception = null;
        }

        private ApiResponse(string message, Exception exception)
        {
            WasSuccessful = false;

            Message = message;

            Exception = exception;
        }

        public static IApiResponse Success() => new ApiResponse(string.Empty);

        public static IApiResponse Failure(string message, Exception exception = null) => new ApiResponse(message, exception);
    }
}
