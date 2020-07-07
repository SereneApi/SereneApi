using System;
using DeltaWare.Dependencies;

namespace SereneApi.Abstractions.Configuration
{
    public class SereneApiExtensions: ISereneApiExtensions
    {
        private Action<IDependencyCollection> _dependencyFactory;

        public SereneApiExtensions(Action<IDependencyCollection> dependencyFactory)
        {
            _dependencyFactory = dependencyFactory;
        }

        public void ExtendDependencyFactory(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory;
        }
    }
}
