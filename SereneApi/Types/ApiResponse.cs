using DeltaWare.SereneApi.Interfaces;
using System;

namespace DeltaWare.SereneApi.Types
{
    public class ApiResponse : IApiResponse
    {
        public bool WasSuccessful { get; }

        public bool HasException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        private ApiResponse()
        {
            WasSuccessful = true;
        }

        private ApiResponse(string message, Exception exception)
        {
            Message = message;

            Exception = exception;

            WasSuccessful = false;
        }

        public static IApiResponse Success() => new ApiResponse();

        public static IApiResponse Failure(string message, Exception exception = null) => new ApiResponse(message, exception);
    }
}
