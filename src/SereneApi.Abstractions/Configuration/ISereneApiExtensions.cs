using System;
using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Configuration
{
    public interface ISereneApiExtensions
    {
        void ExtendDependencyFactory(Action<IDependencyCollection> factory);
    }
}
