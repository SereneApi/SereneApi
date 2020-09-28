using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Adapters.Profiling.Profiling.Request
{
    public interface IRequestProfile: IApiRequest
    {
        Type Source { get; }

        /// <summary>
        /// Specifies the duration of a request.
        /// </summary>
        TimeSpan RequestDuration { get; }

        IApiResponse Response { get; }

        int RetryAttempts { get; }

        DateTime Sent { get; }

        DateTime Received { get; }
    }
}
