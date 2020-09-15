using System;
using System.Collections.Generic;
using System.Text;

namespace SereneApi.Adapters.Testing.Profiling
{
    public interface IApiProfiler
    {
        void StartSession();

        void EndSession();

        IApiProfile GetResults<TApi>();
    }
}
