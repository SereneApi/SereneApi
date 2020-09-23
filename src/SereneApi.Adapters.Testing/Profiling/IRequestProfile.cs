using SereneApi.Abstractions.Request;
using System;

namespace SereneApi.Adapters.Testing.Profiling
{
    public interface IRequestProfile
    {
        /// <summary>
        /// Specifies the duration of a request.
        /// </summary>
        TimeSpan RequestDuration { get; }

        IApiRequest Request { get; }
    }
}
