using SereneApi.Adapters.Testing.Profiling.Request;
using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Testing.Profiling.Api
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
