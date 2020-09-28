using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Profiling.Profiling.Api
{
    public interface IEndpointProfile
    {
        IRequestProfile this[Guid identity] { get; }

        /// <summary>
        /// All requests made against the specific endpoint.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
