using DeltaWare.Dependencies;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Request.Content;
using System;
using System.Net;

namespace SereneApi.Abstractions.Handler.Options
{
    public class OptionsBuilder: IOptionsBuilder
    {
        public IDependencyCollection Dependencies { get; } = new DependencyCollection();

        protected ConnectionSettings ConnectionSettings { get; set; }

        /// <inheritdoc cref="IOptionsConfigurator.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IApiHandlerConfiguration configuration = provider.GetDependency<IApiHandlerConfiguration>();

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
        public void SetTimeoutPeriod(int seconds)
        {
            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            ConnectionSettings.Timeout = seconds;
        }

        /// <inheritdoc cref="IOptionsConfigurator.SetRetryAttempts"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(ConnectionSettings == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Rules.ValidateRetryAttempts(attemptCount);

            ConnectionSettings.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IOptionsConfigurator.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            Dependencies.AddScoped(() => logger);
        }

        /// <inheritdoc cref="IOptionsConfigurator.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            Dependencies.AddScoped(() => queryFactory);
        }

        /// <inheritdoc cref="IOptionsConfigurator.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Dependencies.AddScoped(() => credentials);
        }

        /// <inheritdoc cref="IOptionsConfigurator.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddScoped(() => authentication);
        }

        /// <inheritdoc cref="IOptionsConfigurator.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddTransient<IAuthentication>(() => new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IOptionsConfigurator.AddBearerAuthentication"/>
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

        public IOptions BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IOptions options = new Options(Dependencies.BuildProvider(), ConnectionSettings);

            return options;
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
