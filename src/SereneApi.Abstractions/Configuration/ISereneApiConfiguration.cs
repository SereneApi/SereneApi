using SereneApi.Abstractions.Options;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// The default provider for API configuration.
    /// </summary>
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

        /// <summary>
        /// Builds an <see cref="IApiOptionsBuilder"/> providing the default values.
        /// </summary>
        IApiOptionsBuilder GetOptionsBuilder();

        /// <summary>
        /// Builds the specified <see cref="IApiOptionsBuilder"/> providing the default values.
        /// </summary>
        TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new();

        /// <summary>
        /// Gets <see cref="ISereneApiExtensions"/> allowing extension of default values.
        /// </summary>
        /// <returns></returns>
        ISereneApiExtensions GetExtensions();
    }
}
