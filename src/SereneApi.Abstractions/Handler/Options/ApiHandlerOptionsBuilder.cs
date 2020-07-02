using System;
using System.Net;
using DeltaWare.Dependencies;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Requests.Content;
using SereneApi.Abstractions.Types;

namespace SereneApi.Abstractions.Handler.Options
{
    public class ApiHandlerOptionsBuilder: IApiHandlerOptionsBuilder, ICoreOptions
    {
        public IDependencyCollection Dependencies { get; }

        protected Connection Connection { get; set; }

        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            Dependencies = new DependencyCollection();

            Dependencies.AddScoped(() => Defaults.Factories.QueryFactory);
            Dependencies.AddScoped(() => Defaults.Serializer);
            Dependencies.AddScoped(() => Defaults.ContentType);
            Dependencies.AddScoped(() => Defaults.Handler.Credentials);
        }

        #endregion

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            Connection = new Connection(source, resource, resourcePath);
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeoutPeriod(int seconds)
        {
            if(Connection == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Connection.Timeout = seconds;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.SetRetryAttempts"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(Connection == null)
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Rules.ValidateRetryAttempts(attemptCount);

            Connection.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            Dependencies.AddScoped(() => logger);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            Dependencies.AddScoped(() => queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Dependencies.AddScoped(() => credentials);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddScoped(() => authentication);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddTransient<IAuthentication>(() => new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBearerAuthentication"/>
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

        public IApiHandlerOptions BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => Connection);

            IApiHandlerOptions options = new ApiHandlerOptions(Dependencies.BuildProvider(), Connection);

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
