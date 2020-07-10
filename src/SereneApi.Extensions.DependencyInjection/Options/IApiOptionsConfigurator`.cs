using Microsoft.Extensions.Configuration;
using SereneApi.Abstractions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <summary>
    /// Configures the <see cref="IApiOptionsBuilder{TApi}"/>.
    /// </summary>
    /// <typeparam name="TApi">The API the <see cref="IApiOptions{TApi}"/> are intended for.</typeparam>
    public interface IApiOptionsConfigurator<TApi>: IApiOptionsConfigurator where TApi : class
    {
        /// <summary>
        /// Gets the APIs connection information from the provided <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> that the source information will be retrieved from.</param>
        /// <remarks>
        /// Configurable Values<br/>
        /// Source - <see cref="Uri"/> - Required<br/>
        /// Resource - <see cref="string"/> - Optional<br/>
        /// ResourcePath - <see cref="string"/> - Optional<br/>
        /// Timeout - <see cref="int"/> - Optional; Specified in seconds.<br/>
        /// Retries - <see cref="int"/> - Optional; How many times a timed out request will be re-attempted.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when an invalid value is provided.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when the method is called twice.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the specified value cannot be found and requited is set to true.</exception>
        void UseConfiguration([NotNull] IConfiguration configuration);
    }
}