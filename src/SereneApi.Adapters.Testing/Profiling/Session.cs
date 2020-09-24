using System;
using System.Collections.Generic;
using System.Linq;
using SereneApi.Adapters.Testing.Profiling.Api;
using SereneApi.Adapters.Testing.Profiling.Request;

namespace SereneApi.Adapters.Testing.Profiling
{
    internal class Session: ISession
    {
        private readonly List<RequestProfile> _requests = new List<RequestProfile>();

        public RequestProfile this[Guid identity] => _requests.Single(r => r.Identity == identity);

        public IReadOnlyList<IRequestProfile> Requests => _requests;

        public IApiProfile<TApi> ByApi<TApi>()
        {
            throw new NotImplementedException();
        }

        public void AddRequest(RequestProfile request)
        {
            _requests.Add(request);
        }
    }
}
