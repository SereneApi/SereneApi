using SereneApi.Abstractions.Configuration.Adapters;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Options;

namespace SereneApi.Abstractions.Configuration
{
    public interface ISereneApiConfiguration
    {
        /// <summary>
        /// The default resource path if it has not been provided.
        /// </summary>
        string ResourcePath { get; }

        /// <summary>
        /// The default timeout value if it has not been provided.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// The default retry count if it has not been provided.
        /// </summary>
        int RetryCount { get; }

        IEventRelay Events { get; }

        /// <summary>
        /// Builds an <see cref="IApiOptionsFactory"/> providing the default values.
        /// </summary>
        IApiOptionsFactory BuildOptionsFactory();

        /// <summary>
        /// Builds the specified <see cref="IApiOptionsFactory"/> providing the default values.
        /// </summary>
        TBuilder BuildOptionsFactory<TBuilder>() where TBuilder : IApiOptionsFactory, new();

        IConfigurationExtensions GetExtensions();

        IApiAdapter GetAdapter();
    }
}
