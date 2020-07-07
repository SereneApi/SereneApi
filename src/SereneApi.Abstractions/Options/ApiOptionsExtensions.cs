using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Options
{
    public class ApiOptionsExtensions: IApiOptionsExtensions, ICoreOptions
    {
        public IDependencyCollection Dependencies { get; }

        public ApiOptionsExtensions(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
