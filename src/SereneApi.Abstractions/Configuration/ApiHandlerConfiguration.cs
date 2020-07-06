using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    public class ApiHandlerConfiguration: IApiHandlerConfiguration
    {
        private Action<IDependencyCollection> _dependencyFactory;

        public ISerializer Serializer { get; set; } = new DefaultSerializer();

        public ContentType ContentType { get; set; } = ContentType.Json;

        public int Timeout { get; set; } = 30;

        public string ResourcePath { get; set; } = "api/";

        public ICredentials Credentials { get; set; } = CredentialCache.DefaultCredentials;

        public int RetryCount { get; set; } = 0;

        public IQueryFactory QueryFactory { get; set; } = new DefaultQueryFactory();

        public ApiHandlerConfiguration()
        {
            _dependencyFactory = dependencies =>
            {
                dependencies.AddScoped(() => QueryFactory);
                dependencies.AddScoped(() => Serializer);
                dependencies.AddScoped(() => ContentType);
                dependencies.AddScoped(() => Credentials);
                dependencies.AddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                dependencies.AddScoped<IClientFactory>(p => new DefaultClientFactory(p));
            };
        }

        public void SetDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory;
        }

        public IOptionsBuilder GetOptionsBuilder()
        {
            OptionsBuilder builder = new OptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            builder.Dependencies.TryAddTransient<IApiHandlerConfiguration>(() => this);

            return builder;
        }

        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            builder.Dependencies.TryAddTransient<IApiHandlerConfiguration>(() => this);

            return builder;
        }

        public static IApiHandlerConfiguration Default { get; } = new ApiHandlerConfiguration();
    }
}
