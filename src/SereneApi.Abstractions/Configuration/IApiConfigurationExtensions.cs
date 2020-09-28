using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Extends <see cref="IApiConfiguration"/>.
    /// </summary>
    public interface IApiConfigurationExtensions
    {
        /// <summary>
        /// Subscribe and UnSubscribe events.
        /// </summary>
        /// <remarks>If null, EnableEvents should be called.</remarks>
        IEventRelay EventRelay { get; }

        void EnableEvents([AllowNull] IEventManager eventManagerOverride = null);

        /// <summary>
        /// Adds the specified dependencies.
        /// </summary>
        /// <param name="factory">Builds the dependencies.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddDependencies([NotNull] Action<IDependencyCollection> factory);
    }
}
