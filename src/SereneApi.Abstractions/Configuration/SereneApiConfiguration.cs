using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    /// <inheritdoc cref="ISereneApiConfiguration"/>.
    public class SereneApiConfiguration: ISereneApiConfiguration, ISereneApiConfigurationBuilder
    {
        private Action<IDependencyCollection> _dependencyFactory;

        /// <inheritdoc cref="ISereneApiConfiguration.ResourcePath"/>
        /// <remarks>Default: api/</remarks>
        public string ResourcePath { get; set; } = "api/";

        /// <inheritdoc cref="ISereneApiConfiguration.Timeout"/>
        /// <remarks>Default: 30</remarks>
        public int Timeout { get; set; } = 30;

        /// <inheritdoc cref="ISereneApiConfiguration.RetryCount"/>
        /// <remarks>Default: 0</remarks>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Builds a new instance of <see cref="SereneApiConfiguration"/>
        /// </summary>
        public SereneApiConfiguration([AllowNull] Action<IDependencyCollection> dependencyFactory = null)
        {
            _dependencyFactory = dependencyFactory;
        }

        /// <inheritdoc cref="ISereneApiConfigurationBuilder.OverrideDependencies"/>
        public void OverrideDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _dependencyFactory += dependencies => dependencies.TryAddTransient<ISereneApiConfiguration>(() => this);
        }

        /// <inheritdoc cref="ISereneApiConfigurationBuilder.AddDependencies"/>
        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory ?? throw new ArgumentNullException(nameof(factory)); ;
        }

        /// <inheritdoc cref="ISereneApiConfiguration.GetOptionsBuilder"/>
        public IApiOptionsBuilder GetOptionsBuilder()
        {
            ApiOptionsBuilder builder = new ApiOptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        /// <inheritdoc cref="ISereneApiConfiguration.GetOptionsBuilder{TBuilder}"/>
        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        /// <inheritdoc cref="ISereneApiConfiguration.GetExtensions"/>
        public ISereneApiExtensions GetExtensions()
        {
            return new SereneApiExtensions(this);
        }

        /// <summary>
        /// The default <see cref="ISereneApiConfiguration"/>.
        /// </summary>
        public static ISereneApiConfiguration Default
        {
            get
            {
                SereneApiConfiguration configuration = new SereneApiConfiguration(dependencies =>
                {
                    dependencies.TryAddScoped<IQueryFactory>(() => new DefaultQueryFactory());
                    dependencies.TryAddScoped<ISerializer>(() => new DefaultSerializer());
                    dependencies.TryAddScoped(() => ContentType.Json);
                    dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
                    dependencies.TryAddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                    dependencies.TryAddScoped<IClientFactory>(p => new DefaultClientFactory(p));
                });

                configuration.AddDependencies(dependencies => dependencies.TryAddTransient<ISereneApiConfiguration>(() => configuration));

                return configuration;
            }
        }
    }
}
