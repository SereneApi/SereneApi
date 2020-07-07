using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Extensions
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
