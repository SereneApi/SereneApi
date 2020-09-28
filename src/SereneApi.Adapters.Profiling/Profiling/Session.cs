using SereneApi.Adapters.Profiling.Profiling.Api;
using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Adapters.Profiling.Profiling
{
    internal class Session: ISession
    {
        public IRequestProfile this[Guid identity] => Requests.Single(r => r.Identity == identity);

        public IReadOnlyList<IRequestProfile> Requests { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Session"/>.
        /// </summary>
        /// <param name="requests">The requests captured during the lifetime of the session.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public Session([NotNull] IReadOnlyList<IRequestProfile> requests)
        {
            Requests = requests ?? throw new ArgumentNullException(nameof(requests));
        }

        public IApiProfile<TApi> ByApi<TApi>()
        {
            List<IRequestProfile> requests = Requests.Where(r => r.Source == typeof(TApi)).ToList();

            return new ApiProfile<TApi>(requests);
        }
    }
}
