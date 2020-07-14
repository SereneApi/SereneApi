using DeltaWare.Dependencies;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Options
{
    public class ApiOptionsBuilder: IApiOptionsBuilder
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

            ISereneApiConfiguration configuration = provider.GetDependency<ISereneApiConfiguration>();

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
        public void AddAuthentication(IAuthentication authentication)
        {
            if(authentication == null)
            {
                throw new ArgumentNullException(nameof(authentication));
            }

            Dependencies.AddScoped(() => authentication);
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

            Dependencies.AddTransient<IAuthentication>(() => new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Dependencies.AddTransient<IAuthentication>(() => new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType type)
        {
            Dependencies.AddScoped(() => type);
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

        public void SetTimeout([NotNull] int seconds, [NotNull] int attempts)
        {
            throw new NotImplementedException();
        }

        public void UseRouteFactory([NotNull] IRouteFactory routeFactory)
        {
            throw new NotImplementedException();
        }

        public void UseSerializer([NotNull] ISerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
