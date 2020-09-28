using SereneApi.Adapters.Testing.Profiling.Api;
using SereneApi.Adapters.Testing.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Adapters.Testing.Profiling
{
    internal class Session: ISession
    {
        public IRequestProfile this[Guid identity] => Requests.Single(r => r.Identity == identity);

        public IReadOnlyList<IRequestProfile> Requests { get; }

        public Session(IReadOnlyList<IRequestProfile> requests)
        {
            Requests = requests;
        }

        public IApiProfile<TApi> ByApi<TApi>()
        {
            List<IRequestProfile> requests = Requests.Where(r => r.Source == typeof(TApi)).ToList();

            return new ApiProfile<TApi>(requests);
        }
    }
}
