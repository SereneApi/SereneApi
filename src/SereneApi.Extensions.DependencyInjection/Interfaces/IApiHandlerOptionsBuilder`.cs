using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SereneApi.Interfaces;

namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    public interface IApiHandlerOptionsBuilder<TApiHandler> : IApiHandlerOptionsBuilder where TApiHandler : ApiHandler
    {
        /// <summary>
        /// Gets the Source, Resource, ResourcePrecursor and Timeout period from the <see cref="IConfiguration"/>.
        /// Note: Source and Resource MUST be supplied if this is used, ResourcePrecursor and Timeout are optional
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> the values will be retrieved from</param>
        void UseConfiguration(IConfiguration configuration);

        /// <summary>
        /// Adds an <see cref="ILoggerFactory"/> to the <see cref="ApiHandler"/> allowing built in Logging
        /// </summary>
        /// <param name="loggerFactory">The <see cref="IQueryFactory"/> to be used for Logging</param>
        void AddLoggerFactory(ILoggerFactory loggerFactory);
    }
}
