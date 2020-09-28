using SereneApi.Adapters.Profiling.Profiling.Api;
using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Profiling.Profiling
{
    public interface ISession
    {
        IRequestProfile this[Guid identity] { get; }

        /// <summary>
        /// All requests made during the session.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }

        IApiProfile<TApi> ByApi<TApi>();
    }
}
