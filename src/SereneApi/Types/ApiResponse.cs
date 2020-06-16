using SereneApi.Abstraction.Enums;
using System;

namespace SereneApi.Types
{
    public readonly struct ApiResponse: IApiResponse
    {
        public Status Status { get; }

        public bool WasSuccessful { get; }

        public bool HasException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        private ApiResponse(Status status)
        {
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse(Status status, string message, Exception exception)
        {
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse Success(Status status) => new ApiResponse(status);

        public static IApiResponse Failure(Status status, string message, Exception exception = null) => new ApiResponse(status, message, exception);
    }
}
