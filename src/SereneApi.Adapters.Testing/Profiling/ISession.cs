using SereneApi.Adapters.Testing.Profiling.Api;
using SereneApi.Adapters.Testing.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Testing.Profiling
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
