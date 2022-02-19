using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Configuration
{
    public interface IApiOnInitialization
    {
        public IDependencyProvider Dependencies { get; }
    }
}