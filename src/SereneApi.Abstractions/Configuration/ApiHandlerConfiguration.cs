using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Requests.Content;
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
            _dependencyFactory = collection =>
            {
                collection.AddTransient<IApiHandlerConfiguration>(() => this);
                collection.AddScoped(() => QueryFactory);
                collection.AddScoped(() => Serializer);
                collection.AddScoped(() => ContentType);
                collection.AddScoped(() => Credentials);
                collection.AddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                collection.AddScoped<IClientFactory>(p => new DefaultClientFactory(p));
            };
        }

        public void SetDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory;
        }

        public IOptionsBuilder GetOptionsBuilder()
        {
            OptionsBuilder optionsBuilder = new OptionsBuilder();

            _dependencyFactory.Invoke(optionsBuilder.Dependencies);

            return optionsBuilder;
        }

        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        public static IApiHandlerConfiguration Default { get; } = new ApiHandlerConfiguration();
    }
}
