using SereneApi.Adapters.Profiling.Request;
using System.Collections.Generic;

namespace SereneApi.Adapters.Profiling.Api
{
    /// <summary>
    /// Contains all statistics related to a specific API.
    /// </summary>
    public interface IApiProfile
    {
        /// <summary>
        /// All requests made against the specific API.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
