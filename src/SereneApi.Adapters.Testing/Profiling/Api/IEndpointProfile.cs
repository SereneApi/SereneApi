using System.Collections.Generic;
using SereneApi.Adapters.Testing.Profiling.Request;

namespace SereneApi.Adapters.Testing.Profiling.Api
{
    public interface IEndpointProfile
    {
        /// <summary>
        /// All requests made against the specific endpoint.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
