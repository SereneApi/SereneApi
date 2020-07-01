using DeltaWare.Dependencies;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;

namespace SereneApi.Types
{
    public class ApiHandlerExtensions: IApiHandlerExtensions, ICoreOptions
    {
        public IDependencyCollection Dependencies { get; }

        public ApiHandlerExtensions(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
