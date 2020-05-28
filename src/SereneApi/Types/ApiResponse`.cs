using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public readonly struct ApiResponse<TResult> : IApiResponse<TResult>
    {
        public bool WasSuccessful { get; }

        public bool HasException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        public TResult Result { get; }

        private ApiResponse(TResult result)
        {
            WasSuccessful = true;

            Result = result;

            Message = string.Empty;

            Exception = null;
        }

        private ApiResponse(string message, Exception exception)
        {
            WasSuccessful = false;

            Result = default;

            Message = message;

            Exception = exception;
        }

        public static IApiResponse<TResult> Success(TResult result) => new ApiResponse<TResult>(result);

        public static IApiResponse<TResult> Failure(string message, Exception exception = null) => new ApiResponse<TResult>(message, exception);
    }
}
