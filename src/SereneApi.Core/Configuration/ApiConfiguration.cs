using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Core.Configuration
{
    internal sealed class ApiConfiguration : IApiConfiguration
    {
        public IDependencyCollection Dependencies { get; }

        public ApiConfiguration(IDependencyCollection dependencies, Type handlerType)
        {
            Dependencies = dependencies;
            Dependencies.Configure<HandlerConfiguration>(c =>
            {
                c.SetHandlerType(handlerType);
            });
        }

        public ApiConfigurationScope CreateScope()
        {
            return new ApiConfigurationScope(Dependencies.CreateScope());
        }
    }
}