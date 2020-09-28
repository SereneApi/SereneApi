using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Events;
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
    /// <inheritdoc cref="IApiConfiguration"/>.
    public class ApiConfiguration: IApiConfiguration, IApiConfigurationBuilder
    {
        /// <summary>
        /// Keep an Instance of EventManager within the ApiConfiguration.
        /// This is done so all APIs built from the same default configuration will share the same event manager.
        /// </summary>
        private IEventManager _eventManager;

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
            _dependencyFactory += dependencies => dependencies.TryAddTransient<IApiConfiguration>(() => this);
        }

        /// <inheritdoc cref="IApiConfigurationExtensions.AddDependencies"/>
        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void EnableEvents([AllowNull] IEventManager eventManagerOverride = null)
        {
            if(_eventManager != null && eventManagerOverride != null)
            {
                throw new MethodAccessException("An EventManager has already been added.");
            }

            // If EventManager is unassigned, assign either the override or create a new instance.
            _eventManager ??= eventManagerOverride ?? new EventManager();
        }

        /// <inheritdoc cref="IApiConfiguration.GetOptionsBuilder"/>
        public IApiOptionsBuilder GetOptionsBuilder()
        {
            ApiOptionsBuilder builder = new ApiOptionsBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            if(_eventManager != null)
            {
                builder.Dependencies.AddTransient(() => _eventManager, Binding.Unbound);
            }

            return builder;
        }

        /// <inheritdoc cref="IApiConfiguration.GetOptionsBuilder{TBuilder}"/>
        public TBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IApiOptionsBuilder, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            if(_eventManager != null)
            {
                builder.Dependencies.AddTransient(() => _eventManager, Binding.Unbound);
            }

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
                });

                configuration.AddDependencies(dependencies => dependencies.TryAddTransient<IApiConfiguration>(() => configuration));

                return configuration;
            }
        }
    }
}
