using System.Collections.Generic;

namespace SereneApi.Adapters.Testing.Profiling
{
    public interface IEndpointProfile
    {
        /// <summary>
        /// All requests made against the specific endpoint.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
