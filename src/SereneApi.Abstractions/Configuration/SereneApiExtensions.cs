using DeltaWare.Dependencies;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <inheritdoc cref="ISereneApiExtensions"/>
    public class SereneApiExtensions: ISereneApiExtensions
    {
        private readonly ISereneApiConfigurationBuilder _builder;

        /// <summary>
        /// Creates a new instance of <see cref="SereneApiExtensions"/>.
        /// </summary>
        /// <param name="builder">The builder that will be extended.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public SereneApiExtensions([NotNull] ISereneApiConfigurationBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        /// <inheritdoc cref="ISereneApiExtensions.AddDependency"/>
        public void AddDependency([NotNull] Action<IDependencyCollection> factory)
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _builder.AddDependencies(factory);
        }
    }
}
