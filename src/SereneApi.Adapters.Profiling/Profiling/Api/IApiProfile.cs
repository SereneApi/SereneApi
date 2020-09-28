using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Profiling.Profiling.Api
{
    public interface IApiProfile
    {
        IRequestProfile this[Guid identity] { get; }

        /// <summary>
        /// All requests made against the specific API.
        /// </summary>
        IReadOnlyList<IRequestProfile> Requests { get; }
    }
}
