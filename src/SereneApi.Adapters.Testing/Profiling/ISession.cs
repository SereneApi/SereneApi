using System.Collections.Generic;
using SereneApi.Adapters.Testing.Profiling.Api;
using SereneApi.Adapters.Testing.Profiling.Request;

namespace SereneApi.Adapters.Testing.Profiling
{
    public interface ISession
    {
        /// <summary>
        /// All requests made during the session.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }

        IApiProfile<TApi> ByApi<TApi>();
    }
}
