using SereneApi.Adapters.Profiling.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Adapters.Profiling.Profiling.Api
{
    internal class ApiProfile<TApi>: IApiProfile<TApi>
    {
        public IRequestProfile this[Guid identity] => Requests.Single(r => r.Identity == identity);

        public IReadOnlyList<IRequestProfile> Requests { get; }

        /// <summary>
        /// Creates a new instance of an Api's statistics profile.
        /// </summary>
        /// <param name="requests">The requests made with the specific API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiProfile([NotNull] IReadOnlyList<IRequestProfile> requests)
        {
            Requests = requests ?? throw new ArgumentNullException(nameof(requests));
        }

        public IEndpointProfile ByEndpoint(Func<TApi, string> endpointName)
        {
            throw new NotImplementedException();
        }

    }
}
