﻿using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration.Adapters;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response.Handlers;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using System;
using System.Net;
using SereneApi.Abstractions.Options.Builder;
using SereneApi.Abstractions.Requests.Handler;

namespace SereneApi.Abstractions.Configuration
{
    public class SereneApiConfiguration : ISereneApiConfiguration, ISereneApiConfigurationBuilder
    {
        private readonly IEventManager _eventManager = new EventManager();

        private Action<IDependencyCollection> _dependencyFactory;

        public string ResourcePath { get; set; } = "api";

        public int Timeout { get; set; } = 30;

        public int RetryCount { get; set; } = 0;

        public IEventRelay Events => _eventManager;

        public void AddCredentials(ICredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            _dependencyFactory += d => d.AddSingleton(() => credentials);
        }

        /// <summary>
        /// Builds a new instance of <see cref="SereneApiConfiguration"/>
        /// </summary>
        public SereneApiConfiguration(Action<IDependencyCollection> dependencyFactory = null)
        {
            _dependencyFactory = dependencyFactory ?? Default._dependencyFactory;
        }


        public IApiOptionsFactory BuildOptionsFactory()
        {
            return BuildOptionsFactory<ApiOptionsFactory>();
        }

        public TBuilder BuildOptionsFactory<TBuilder>() where TBuilder : IApiOptionsFactory, new()
        {
            TBuilder builder = new TBuilder();

            _dependencyFactory.Invoke(builder.Dependencies);

            builder.Dependencies.TryAddTransient<ISereneApiConfiguration>(() => this);
            builder.Dependencies.TryAddTransient(() => _eventManager);

            return builder;
        }

        public IConfigurationExtensions GetExtensions()
        {
            return this;
        }

        public IApiAdapter GetAdapter()
        {
            return new ApiAdapter(Events);
        }

        /// <summary>
        /// The default <see cref="ISereneApiConfiguration"/>.
        /// </summary>
        public static SereneApiConfiguration Default
        {
            get
            {
                SereneApiConfiguration configuration = new SereneApiConfiguration(dependencies =>
                {
                    dependencies.TryAddScoped<IQueryFactory>(() => new QueryFactory());
                    dependencies.TryAddScoped<ISerializer>(() => new JsonSerializer());
                    dependencies.TryAddScoped(() => ContentType.Json);
                    dependencies.TryAddScoped(() => CredentialCache.DefaultCredentials);
                    dependencies.TryAddScoped<IRouteFactory>(p => new RouteFactory(p));
                    dependencies.TryAddScoped<IClientFactory>(p => new ClientFactory(p));
                    dependencies.TryAddScoped<IResponseHandler>(p => new ResponseHandler(p));
                    dependencies.TryAddScoped<IFailedResponseHandler>(p => new FailedResponseHandler(p));
                    dependencies.TryAddScoped<IRequestHandler>(p => new RetryingRequestHandler(p));
                });

                return configuration;
            }
        }

        public void AddDependencies(Action<IDependencyCollection> factory)
        {
            _dependencyFactory += factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}
