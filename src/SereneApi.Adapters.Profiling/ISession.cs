using SereneApi.Adapters.Profiling.Api;
using SereneApi.Adapters.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Profiling
{
    /// <summary>
    /// Contains all statics collect during the profiling session.
    /// </summary>
    public interface ISession
    {
        TimeSpan Duration { get; }

        /// <summary>
        /// All requests made during the session.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }

        /// <summary>
        /// Gets all statics related to the specified API.
        /// </summary>
        /// <typeparam name="TApi"></typeparam>
        /// <returns></returns>
        IApiProfile ByApi<TApi>();
    }
}
