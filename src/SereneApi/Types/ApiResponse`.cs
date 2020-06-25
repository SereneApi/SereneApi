using SereneApi.Abstraction.Enums;
using System;

namespace SereneApi.Types
{
    public class ApiResponse<TResult>: IApiResponse<TResult>
    {
        public Status Status { get; }

        public bool WasSuccessful { get; }

        public bool HasException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        public TResult Result { get; }

        private ApiResponse(Status status, TResult result)
        {
            WasSuccessful = true;
            Result = result;
            Message = null;
            Status = status;
            Exception = null;
        }

        private ApiResponse(Status status, string message, Exception exception)
        {
            WasSuccessful = false;
            Result = default;
            Message = message;
            Status = status;
            Exception = exception;
        }

        public static IApiResponse<TResult> Success(Status status, TResult result) => new ApiResponse<TResult>(status, result);

        public static IApiResponse<TResult> Failure(Status status, string message, Exception exception = null) => new ApiResponse<TResult>(status, message, exception);
    }
}
