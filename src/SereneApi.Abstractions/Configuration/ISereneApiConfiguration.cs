using SereneApi.Abstractions.Extensions;
using SereneApi.Abstractions.Options;

namespace SereneApi.Abstractions.Configuration
{
    public interface ISereneApiConfiguration
    {
        int Timeout { get; }

        string ResourcePath { get; }

        int RetryCount { get; }

        IApiOptionsBuilder GetOptionsBuilder();

        TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new();

        ISereneApiExtensions GetExtensions();
    }
}
