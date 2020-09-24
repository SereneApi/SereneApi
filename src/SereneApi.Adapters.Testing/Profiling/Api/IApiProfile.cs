using System.Collections.Generic;
using SereneApi.Adapters.Testing.Profiling.Request;

namespace SereneApi.Adapters.Testing.Profiling.Api
{
    public interface IApiProfile
    {
        /// <summary>
        /// All requests made against the specific API.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
