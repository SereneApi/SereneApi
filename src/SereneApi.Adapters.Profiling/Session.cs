using SereneApi.Adapters.Profiling.Api;
using SereneApi.Adapters.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Adapters.Profiling
{
    [DebuggerDisplay("Duration: {Duration}")]
    internal class Session : ISession
    {
        public TimeSpan Duration { get; private set; }

        public IReadOnlyList<IRequestProfile> Requests { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Session"/>.
        /// </summary>
        /// <param name="requests">The requests captured during the lifetime of the session.</param>
        /// <param name="duration">The duration of the session.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public Session([NotNull] IReadOnlyList<IRequestProfile> requests, TimeSpan duration)
        {
            Requests = requests ?? throw new ArgumentNullException(nameof(requests));

            Duration = duration;
        }

        public IApiProfile ByApi<TApi>()
        {
            List<IRequestProfile> requests = Requests.Where(r => r.Source == typeof(TApi)).ToList();

            return new ApiProfile(requests);
        }
    }
}
