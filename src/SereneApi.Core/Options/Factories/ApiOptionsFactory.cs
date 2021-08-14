using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Connection;
using System;

namespace SereneApi.Core.Options.Factories
{
    public abstract class ApiOptionsFactory
    {
        public IDependencyCollection Dependencies { get; }

        /// <summary>
        /// Specifies the connection settings for the API.
        /// </summary>
        protected ConnectionSettings ConnectionSettings { get; set; }

        protected ApiOptionsFactory(IDependencyCollection dependencies)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }
    }
}
