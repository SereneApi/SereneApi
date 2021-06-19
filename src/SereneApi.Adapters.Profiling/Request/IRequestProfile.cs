using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Adapters.Profiling.Request
{
    /// <summary>
    /// Contains all statics related to a request.
    /// </summary>
    public interface IRequestProfile : IApiRequest
    {
        /// <summary>
        /// Species the source API.
        /// </summary>
        Type Source { get; }

        /// <summary>
        /// Specifies the duration of a request.
        /// </summary>
        TimeSpan RequestDuration { get; }

        /// <summary>
        /// Species the response of the request.
        /// </summary>
        IApiResponse Response { get; }

        /// <summary>
        /// Specifies how many times the request was retried.
        /// </summary>
        int RetryAttempts { get; }

        /// <summary>
        /// Specifies when the request was initialized.
        /// </summary>
        DateTimeOffset Sent { get; }

        /// <summary>
        /// Specifies when the request was finalized.
        /// </summary>
        DateTimeOffset Received { get; }
    }
}
