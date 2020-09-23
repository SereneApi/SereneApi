using System.Collections.Generic;

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
