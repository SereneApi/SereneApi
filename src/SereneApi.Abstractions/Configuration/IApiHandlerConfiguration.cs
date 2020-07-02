using System;
using System.Collections.Generic;
using System.Text;
using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Configuration
{
    public interface IApiHandlerConfiguration
    {
        void ConfigureDefaultDependencies(IDependencyCollection dependencies);
    }
}
