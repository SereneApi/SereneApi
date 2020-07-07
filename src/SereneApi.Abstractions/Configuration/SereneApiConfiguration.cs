using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System;
using System.Net;
using SereneApi.Abstractions.Options;

namespace SereneApi.Abstractions.Configuration
{
    public class SereneApiConfiguration: ISereneApiConfiguration, ISereneApiConfigurationBuilder
    {
        private Action<IDependencyCollection> _dependencyFactory;

        public int Timeout { get; set; } = 30;

        public string ResourcePath { get; set; } = "api/";

        public int RetryCount { get; set; } = 0;

        public SereneApiConfiguration()
        {
            _dependencyFactory = dependencies =>
            {
                dependencies.TryAddScoped<IQueryFactory>(() => new DefaultQueryFactory());
                dependencies.TryAddScoped<ISerializer>(() => new DefaultSerializer());
                dependencies.TryAddScoped(() => ContentType.Json);
                dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
                dependencies.TryAddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                dependencies.TryAddScoped<IClientFactory>(p => new DefaultClientFactory(p));
                dependencies.TryAddTransient<ISereneApiConfiguration>(() => this);
            };
        }

        public void SetDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory;
            _dependencyFactory += dependencies => dependencies.TryAddTransient<ISereneApiConfiguration>(() => this);
        }

        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory;
        }

        public IApiOptionsBuilder GetOptionsBuilder()
        {
            ApiOptionsBuilder builder = new ApiOptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        public ISereneApiExtensions GetExtensions()
        {
            return new SereneApiExtensions(_dependencyFactory);
        }

        public static ISereneApiConfiguration Default { get; } = new SereneApiConfiguration();
    }
}
