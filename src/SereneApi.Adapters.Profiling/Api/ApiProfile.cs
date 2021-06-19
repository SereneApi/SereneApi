using SereneApi.Adapters.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Adapters.Profiling.Api
{
    internal class ApiProfile : IApiProfile
    {
        public IReadOnlyList<IRequestProfile> Requests { get; }

        /// <summary>
        /// Creates a new instance of an APIs statistics profile.
        /// </summary>
        /// <param name="requests">The requests made with the specific API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiProfile([NotNull] IReadOnlyList<IRequestProfile> requests)
        {
            Requests = requests ?? throw new ArgumentNullException(nameof(requests));
        }
    }
}
