﻿using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Handlers.Rest.Responses
{
    /// <inheritdoc cref="IApiResponse"/>
    public class RestApiResponse : IApiResponse
    {
        public TimeSpan Duration { get; }

        /// <inheritdoc cref="IApiResponse.Exception"/>
        public Exception Exception { get; }

        /// <inheritdoc cref="IApiResponse.HasException"/>
        public bool HasException => Exception != null;

        /// <inheritdoc cref="IApiResponse.Message"/>
        public string Message { get; }

        /// <inheritdoc cref="IApiRequest"/>
        public IApiRequest Request { get; }

        /// <inheritdoc cref="IApiResponse.Status"/>
        public Status Status { get; }

        /// <inheritdoc cref="IApiResponse.WasNotSuccessful"/>
        public bool WasNotSuccessful => !WasSuccessful;

        /// <inheritdoc cref="IApiResponse.WasSuccessful"/>
        public bool WasSuccessful { get; }

        private RestApiResponse(IApiRequest request, Status status, TimeSpan duration)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = true;
            Message = null;
            Status = status;
            Exception = null;
            Duration = duration;
        }

        private RestApiResponse(IApiRequest request, Status status, TimeSpan duration, [AllowNull] string message, [AllowNull] Exception exception = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            WasSuccessful = false;
            Message = message;
            Status = status;
            Exception = exception;
            Duration = duration;
        }

        public static IApiResponse Failure(IApiRequest request, Status status, TimeSpan duration, [AllowNull] string message, [AllowNull] Exception exception = null) => new RestApiResponse(request, status, duration, message, exception);

        public static IApiResponse Success(IApiRequest request, Status status, TimeSpan duration) => new RestApiResponse(request, status, duration);
    }
}