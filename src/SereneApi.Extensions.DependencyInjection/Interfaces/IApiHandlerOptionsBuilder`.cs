using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    /// <summary>
    /// Builds <see cref="IApiHandlerOptions"/> for the specified <see cref="ApiHandler"/>.
    /// </summary>
    /// <typeparam name="TApiHandler">The <see cref="ApiHandler"/> the options are intended for.</typeparam>
    public interface IApiHandlerOptionsBuilder<TApiHandler>: IApiHandlerOptionsBuilder where TApiHandler : class
    {
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
