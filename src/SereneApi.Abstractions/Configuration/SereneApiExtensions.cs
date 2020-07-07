using DeltaWare.Dependencies;
using System;

namespace SereneApi.Abstractions.Configuration
{
    public class SereneApiExtensions: ISereneApiExtensions
    {
        private readonly ISereneApiConfigurationBuilder _builder;

        public SereneApiExtensions(ISereneApiConfigurationBuilder builder)
        {
            _builder = builder;
        }

        public void ExtendDependencyFactory(Action<IDependencyCollection> factory)
        {
            _builder.AddDependencies(factory);
        }
    }
}
