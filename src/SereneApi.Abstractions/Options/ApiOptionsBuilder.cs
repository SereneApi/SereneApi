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
        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        protected ConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            using IDependencyProvider provider = Dependencies.BuildProvider();

            ISereneApiConfiguration configuration = provider.GetDependency<ISereneApiConfiguration>();

            if(string.IsNullOrWhiteSpace(resourcePath))
            {
                if(resourcePath != string.Empty)
                {
                    resourcePath = configuration.ResourcePath;
                }
            }

            ConnectionSettings = new ConnectionSettings(source, resource, resourcePath)
            {
                Timeout = configuration.Timeout,
                RetryAttempts = configuration.RetryCount
            };
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeout(int seconds)
        {
            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            ConnectionSettings.Timeout = seconds;
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.SetRetryAttempts"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Rules.ValidateRetryAttempts(attemptCount);

            ConnectionSettings.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            Dependencies.AddScoped(() => logger);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            Dependencies.AddScoped(() => queryFactory);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Dependencies.AddScoped(() => credentials);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddScoped(() => authentication);
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddTransient<IAuthentication>(() => new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiOptionsConfigurator.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddTransient<IAuthentication>(() => new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType content)
        {
            Dependencies.AddScoped(() => content);
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
