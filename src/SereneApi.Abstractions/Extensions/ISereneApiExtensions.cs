using System;
using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Extensions
{
    public interface ISereneApiExtensions
    {
        void ExtendDependencyFactory(Action<IDependencyCollection> factory);
    }
}
