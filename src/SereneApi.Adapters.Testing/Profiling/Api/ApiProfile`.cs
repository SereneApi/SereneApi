using SereneApi.Adapters.Testing.Profiling.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Adapters.Testing.Profiling.Api
{
    internal class ApiProfile<TApi>: IApiProfile<TApi>
    {
        public IRequestProfile this[Guid identity] => Requests.Single(r => r.Identity == identity);

        public IReadOnlyList<IRequestProfile> Requests { get; }

        public ApiProfile(IReadOnlyList<IRequestProfile> requests)
        {
            Requests = requests;
        }

        public IEndpointProfile ByEndpoint(Func<TApi, string> endpointName)
        {
            throw new NotImplementedException();
        }

    }
}
