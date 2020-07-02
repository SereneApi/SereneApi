using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Handler;
using System;
using SereneApi.Abstractions.Handler.Options;

namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    public interface IApiHandlerOptionsBuilder<TApiDefinition>: IApiHandlerOptionsBuilder where TApiDefinition : class
    {
        /// <summary>
        /// The specific handler the <see cref="IApiHandlerOptionsBuilder"/> are for.
        /// </summary>
        Type HandlerType { get; }

        /// <summary>
        /// Gets the Source, Resource, ResourcePrecursor and Timeout period from the <see cref="IConfiguration"/>.
        /// </summary>
        /// <remarks>Source is the only required values. All other values are optional and can be provided at later stages.</remarks>
        /// <param name="configuration">The <see cref="IConfiguration"/> the values will be retrieved from.</param>
        /// <exception cref="MethodAccessException">Thrown if a client was provided or this methods was called twice.</exception>
        void UseConfiguration(IConfiguration configuration);

        /// <summary>
        /// Adds an <see cref="ILoggerFactory"/> to the <see cref="ApiHandler"/> allowing built in Logging.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> used to instantiate new instances of an <see cref="ILogger"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value was provided.</exception>
        void AddLoggerFactory(ILoggerFactory loggerFactory);
    }
}
