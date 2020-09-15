using System;

namespace SereneApi.Adapters.Testing.Profiling
{
    internal class ApiProfiler: IApiProfiler
    {
        public void StartSession()
        {
            throw new NotImplementedException();
        }

        public void EndSession()
        {
            throw new NotImplementedException();
        }

        public IApiProfile GetResults<TApi>()
        {
            throw new NotImplementedException();
        }
    }
}
