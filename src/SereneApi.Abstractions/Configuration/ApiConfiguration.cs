using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Response.Handlers;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using DeltaWare.Dependencies.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Configuration
{
    /// <inheritdoc cref="IApiConfiguration"/>.
    public class ApiConfiguration: IApiConfiguration, IApiConfigurationBuilder
    {
        private readonly IEventManager _eventManager = new EventManager();

        private Action<IDependencyCollection> _dependencyFactory;

        /// <inheritdoc cref="IApiConfiguration.ResourcePath"/>
        /// <remarks>Default: api/</remarks>
        public string ResourcePath { get; set; } = "api/";

        /// <inheritdoc cref="IApiConfiguration.Timeout"/>
        /// <remarks>Default: 30</remarks>
        public int Timeout { get; set; } = 30;

        /// <inheritdoc cref="IApiConfiguration.RetryCount"/>
        /// <remarks>Default: 0</remarks>
        public int RetryCount { get; set; } = 0;

        public IEventRelay EventRelay => _eventManager;

        /// <summary>
        /// Builds a new instance of <see cref="ApiConfiguration"/>
        /// </summary>
        public ApiConfiguration([AllowNull] Action<IDependencyCollection> dependencyFactory = null)
        {
            _dependencyFactory = dependencyFactory;
        }

        /// <inheritdoc cref="IApiConfigurationBuilder.OverrideDependencies"/>
        public void OverrideDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc cref="IApiConfigurationExtensions.AddDependencies"/>
        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void AddCredentials(ICredentials credentials)
        {
            if(credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            _dependencyFactory += d => d.AddSingleton(() => credentials);
        }

        /// <inheritdoc cref="IApiConfiguration.GetOptionsBuilder"/>
        public IApiOptionsBuilder GetOptionsBuilder()
        {
            ApiOptionsBuilder builder = new ApiOptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            builder.Dependencies.TryAddTransient<IApiConfiguration>(() => this);
            builder.Dependencies.TryAddTransient(() => _eventManager);

            return builder;
        }

        /// <inheritdoc cref="IApiConfiguration.GetOptionsBuilder{TBuilder}"/>
        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            builder.Dependencies.TryAddTransient<IApiConfiguration>(() => this);
            builder.Dependencies.TryAddTransient(() => _eventManager);

            return builder;
        }

        /// <inheritdoc cref="IApiConfiguration.GetExtensions"/>
        public IApiConfigurationExtensions GetExtensions()
        {
            return this;
        }

        /// <summary>
        /// The default <see cref="IApiConfiguration"/>.
        /// </summary>
        public static IApiConfiguration Default
        {
            get
            {
                ApiConfiguration configuration = new ApiConfiguration(dependencies =>
                {
                    dependencies.TryAddScoped<IQueryFactory>(() => new QueryFactory());
                    dependencies.TryAddScoped<ISerializer>(() => new DefaultJsonSerializer());
                    dependencies.TryAddScoped(() => ContentType.Json);
                    dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
                    dependencies.TryAddScoped<IRouteFactory>(p => new DefaultRouteFactory(p));
                    dependencies.TryAddScoped<IClientFactory>(p => new DefaultClientFactory(p));
                    dependencies.TryAddScoped<IResponseHandler>(p => new DefaultResponseHandler(p));
                    dependencies.TryAddScoped<IFailedResponseHandler>(p => new DefaultFailedResponseHandler(p));
                });

                return configuration;
            }
        }
    }
}
