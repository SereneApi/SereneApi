﻿using System;
using System.Net;
using System.Net.Mime;
using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Authorization.Types;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Options.Types;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Requests.Handler;
using SereneApi.Abstractions.Response.Handlers;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;

namespace SereneApi.Abstractions.Options.Builder
{
    public class ApiOptionsFactory : IApiOptionsFactory
    {
        private bool _throwExceptions = false;

        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        /// <summary>
        /// Specifies the connection settings for the API.
        /// </summary>
        protected ConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc cref="IApiOptionsBuilder.AddConfiguration"/>
        public void AddConfiguration(IConnectionSettings configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            ISereneApiConfiguration apiConfiguration = provider.GetDependency<ISereneApiConfiguration>();

            string resourcePath = configuration.ResourcePath;

            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                if (resourcePath != string.Empty)
                {
                    resourcePath = apiConfiguration.ResourcePath;
                }
            }

            ConnectionSettings connection =
                new ConnectionSettings(configuration.BaseAddress, configuration.Resource, resourcePath)
                {
                    Timeout = apiConfiguration.Timeout,
                    RetryAttempts = apiConfiguration.RetryCount
                };

            #region Timeout

            int timeout = configuration.Timeout;

            if (timeout < 0)
            {
                throw new ArgumentException("The Timeout value must be greater than 0");
            }

            if (timeout != default)
            {
                connection.Timeout = timeout;
            }

            #endregion
            #region Retry Count

            int retryCount = configuration.RetryAttempts;

            if (retryCount != default)
            {
                Rules.ValidateRetryAttempts(retryCount);

                connection.RetryAttempts = retryCount;
            }

            #endregion

            ConnectionSettings = connection;
        }

        /// <inheritdoc cref="IApiOptionsBuilder.SetSource"/>
        public void SetSource(string baseAddress, string resource = null, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            ISereneApiConfiguration configuration = provider.GetDependency<ISereneApiConfiguration>();

            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                if (resourcePath != string.Empty)
                {
                    resourcePath = configuration.ResourcePath;
                }
            }

            ConnectionSettings = new ConnectionSettings(baseAddress, resource, resourcePath)
            {
                Timeout = configuration.Timeout,
                RetryAttempts = configuration.RetryCount
            };
        }

        /// <inheritdoc cref="IApiOptionsBuilder.SetTimeout(int)"/>
        public void SetTimeout(int seconds)
        {
            if (seconds <= 0)
            {
                throw new ArgumentException("A timeout value must be greater than 0.");
            }

            if (ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            ConnectionSettings.Timeout = seconds;
        }

        /// <inheritdoc cref="IApiOptionsBuilder.SetRetryAttempts(int)"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if (attemptCount < 0)
            {
                throw new ArgumentException("Retry attempts must be greater or equal to 0.");
            }

            if (ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Rules.ValidateRetryAttempts(attemptCount);

            ConnectionSettings.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IApiOptionsBuilder.UseLogger"/>
        public void UseLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Dependencies.AddScoped(() => logger);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            if (queryFactory == null)
            {
                throw new ArgumentNullException(nameof(queryFactory));
            }

            Dependencies.AddScoped(() => queryFactory);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.AddCredentials"/>
        public void AddCredentials(ICredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            Dependencies.AddScoped(() => credentials);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthorization authorization)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            Dependencies.AddScoped(() => authorization);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            Dependencies.AddTransient<IAuthorization>(() => new BasicAuthorization(username, password));
        }

        /// <inheritdoc cref="IApiOptionsBuilder.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Dependencies.AddTransient<IAuthorization>(() => new BearerAuthorization(token));
        }

        /// <inheritdoc cref="IApiOptionsBuilder.AcceptContentType"/>
        public void AcceptContentType(ContentType type)
        {
            Dependencies.AddScoped(() => type);
        }

        public void ThrowExceptions()
        {
            _throwExceptions = true;
        }

        public void UseRequestHandler(IRequestHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Dependencies.AddScoped(() => handler);
        }

        public void UseRequestHandler(Func<IDependencyProvider, IRequestHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        public void UseResponseHandler(IResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Dependencies.AddScoped(() => handler);
        }

        public void UseResponseHandler(Func<IDependencyProvider, IResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        public void UseFailedResponseHandler(IFailedResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Dependencies.AddScoped(() => handler);
        }

        public void UseFailedResponseHandler(Func<IDependencyProvider, IFailedResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.SetTimeout(int,int)"/>
        public void SetTimeout(int seconds, int attempts)
        {
            ConnectionSettings.Timeout = seconds;
            ConnectionSettings.RetryAttempts = attempts;
        }

        /// <inheritdoc cref="IApiOptionsBuilder.UseRouteFactory"/>
        public void UseRouteFactory(IRouteFactory routeFactory)
        {
            if (routeFactory == null)
            {
                throw new ArgumentNullException(nameof(routeFactory));
            }

            Dependencies.AddScoped(() => routeFactory);
        }

        /// <inheritdoc cref="IApiOptionsBuilder.UseSerializer"/>
        public void UseSerializer(ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            Dependencies.AddScoped(() => serializer);
        }

        public IApiOptions BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IApiOptions apiOptions = new ApiOptions(Dependencies.BuildProvider(), ConnectionSettings)
            {
                ThrowExceptions = _throwExceptions
            };

            return apiOptions;
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
