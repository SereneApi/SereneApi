using System;
using System.Collections.Generic;

namespace SereneApi.Adapters.Testing.Profiling
{
    internal class Session: ISession
    {
        private readonly List<IRequestProfile> _requests = new List<IRequestProfile>();

        public IReadOnlyList<IRequestProfile> Requests => _requests;

        public IApiProfile<TApi> ByApi<TApi>()
        {
            throw new NotImplementedException();
        }

        public void AddRequest(IRequestProfile requestProfile)
        {
            _requests.Add(requestProfile);
        }
    }
}
