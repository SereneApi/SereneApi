using System;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;

namespace SereneApi.Adapters.Testing.Profiling.Request
{
    public interface IRequestProfile
    {
        Type SourceApi { get; }

        Guid Identity { get; }

        /// <summary>
        /// Specifies the duration of a request.
        /// </summary>
        TimeSpan RequestDuration { get; }

        IApiRequest Request { get; }

        IApiResponse Response { get; }

        int RetryAttempts { get; }

        DateTime Sent { get; }

        DateTime Received { get; }
    }
}
