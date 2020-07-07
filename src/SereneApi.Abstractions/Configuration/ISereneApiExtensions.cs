using DeltaWare.Dependencies;
using System;

namespace SereneApi.Abstractions.Configuration
{
    public interface ISereneApiExtensions
    {
        void ExtendDependencyFactory(Action<IDependencyCollection> factory);
    }
}
