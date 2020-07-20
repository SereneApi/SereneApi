using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Authorization.Types;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Options
{
    /// <inheritdoc cref="IApiOptionsConfigurator"/>
    public class ApiOptionsBuilder: IApiOptionsBuilder, IApiOptionsExtensions
    {
        /// <inheritdoc cref="ICoreOptions.Dependencies"/>
        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        /// <summary>
        /// Specifies the connection settings for the API.
        /// </summary>
        protected ConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseSource"/>
        public void UseSource([NotNull] string baseAddress, string resource = null, string resourcePath = null)
        {
            if(string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IDefaultApiConfiguration configuration = provider.GetDependency<IDefaultApiConfiguration>();

            if(string.IsNullOrWhiteSpace(resourcePath))
            {
                if(resourcePath != string.Empty)
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

        /// <inheritdoc cref="IApiOptionsConfigurator.SetTimeout(int)"/>
        public void SetTimeout(int seconds)
        {
            if(seconds <= 0)
            {
                throw new ArgumentException("A timeout value must be greater than 0.");
            }

            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            ConnectionSettings.Timeout = seconds;
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.SetRetryAttempts(int)"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(attemptCount < 0)
            {
                throw new ArgumentException("Retry attempts must be greater or equal to 0.");
            }

            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Rules.ValidateRetryAttempts(attemptCount);

            ConnectionSettings.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseLogger"/>
        public void UseLogger([NotNull] ILogger logger)
        {
            if(logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Dependencies.AddScoped(() => logger);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseQueryFactory"/>
        public void UseQueryFactory([NotNull] IQueryFactory queryFactory)
        {
            if(queryFactory == null)
            {
                throw new ArgumentNullException(nameof(queryFactory));
            }

            Dependencies.AddScoped(() => queryFactory);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseCredentials"/>
        public void UseCredentials([NotNull] ICredentials credentials)
        {
            if(credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            Dependencies.AddScoped(() => credentials);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddAuthentication"/>
        public void AddAuthentication(IAuthorization authorization)
        {
            if(authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            Dependencies.AddScoped(() => authorization);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if(string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            Dependencies.AddTransient<IAuthorization>(() => new BasicAuthorization(username, password));
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Dependencies.AddTransient<IAuthorization>(() => new BearerAuthorization(token));
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AcceptContentType"/>
        public void AcceptContentType(ContentType type)
        {
            Dependencies.AddScoped(() => type);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.SetTimeout(int,int)"/>
        public void SetTimeout([NotNull] int seconds, [NotNull] int attempts)
        {
            ConnectionSettings.Timeout = seconds;
            ConnectionSettings.RetryAttempts = attempts;
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseRouteFactory"/>
        public void UseRouteFactory([NotNull] IRouteFactory routeFactory)
        {
            if(routeFactory == null)
            {
                throw new ArgumentNullException(nameof(routeFactory));
            }

            Dependencies.AddScoped(() => routeFactory);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseSerializer"/>
        public void UseSerializer([NotNull] ISerializer serializer)
        {
            if(serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            Dependencies.AddScoped(() => serializer);
        }

        public IApiOptions BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IApiOptions apiOptions = new ApiOptions(Dependencies.BuildProvider(), ConnectionSettings);

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
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
