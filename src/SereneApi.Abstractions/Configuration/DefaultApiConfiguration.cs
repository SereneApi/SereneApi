using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    /// <inheritdoc cref="IDefaultApiConfiguration"/>.
    public class DefaultApiConfiguration: IDefaultApiConfiguration, IDefaultApiConfigurationBuilder
    {
        private Action<IDependencyCollection> _dependencyFactory;

        /// <inheritdoc cref="IDefaultApiConfiguration.ResourcePath"/>
        /// <remarks>Default: api/</remarks>
        public string ResourcePath { get; set; } = "api/";

        /// <inheritdoc cref="IDefaultApiConfiguration.Timeout"/>
        /// <remarks>Default: 30</remarks>
        public int Timeout { get; set; } = 30;

        /// <inheritdoc cref="IDefaultApiConfiguration.RetryCount"/>
        /// <remarks>Default: 0</remarks>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Builds a new instance of <see cref="DefaultApiConfiguration"/>
        /// </summary>
        public DefaultApiConfiguration([AllowNull] Action<IDependencyCollection> dependencyFactory = null)
        {
            _dependencyFactory = dependencyFactory;
        }

        /// <inheritdoc cref="IDefaultApiConfigurationBuilder.OverrideDependencies"/>
        public void OverrideDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _dependencyFactory += dependencies => dependencies.TryAddTransient<IDefaultApiConfiguration>(() => this);
        }

        /// <inheritdoc cref="IDefaultApiConfigurationBuilder.AddDependencies"/>
        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory ?? throw new ArgumentNullException(nameof(factory)); ;
        }

        /// <inheritdoc cref="IDefaultApiConfiguration.GetOptionsBuilder"/>
        public IApiOptionsBuilder GetOptionsBuilder()
        {
            ApiOptionsBuilder builder = new ApiOptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        /// <inheritdoc cref="IDefaultApiConfiguration.GetOptionsBuilder{TBuilder}"/>
        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            return builder;
        }

        /// <inheritdoc cref="IDefaultApiConfiguration.GetExtensions"/>
        public IDefaultApiConfigurationExtensions GetExtensions()
        {
            return this;
        }

        /// <summary>
        /// The default <see cref="IDefaultApiConfiguration"/>.
        /// </summary>
        public static IDefaultApiConfiguration Default
        {
            get
            {
                DefaultApiConfiguration configuration = new DefaultApiConfiguration(dependencies =>
                {
                    dependencies.TryAddScoped<IQueryFactory>(() => new QueryFactory());
                    dependencies.TryAddScoped<ISerializer>(() => new DefaultJsonSerializer());
                    dependencies.TryAddScoped(() => ContentType.Json);
                    dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
                    dependencies.TryAddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                    dependencies.TryAddScoped<IClientFactory>(p => new DefaultClientFactory(p));
                });

                configuration.AddDependencies(dependencies => dependencies.TryAddTransient<IDefaultApiConfiguration>(() => configuration));

                return configuration;
            }
        }
    }
}
