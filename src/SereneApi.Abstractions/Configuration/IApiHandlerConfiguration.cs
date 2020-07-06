using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Requests.Content;
using SereneApi.Abstractions.Serializers;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    public interface IApiHandlerConfiguration
    {
        ISerializer Serializer { get; }

        ContentType ContentType { get; }

        int Timeout { get; }

        string ResourcePath { get; }

        ICredentials Credentials { get; }

        int RetryCount { get; }

        IQueryFactory QueryFactory { get; }

        IOptionsBuilder GetOptionsBuilder();

        TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IOptionsBuilder, new();
    }
}
