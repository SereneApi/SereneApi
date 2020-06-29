using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Serializers;
using SereneApi.Types.Headers.Accept;
using SereneApi.Types.Headers.Authentication;
using System;
using System.Net;

namespace SereneApi.Types
{
    public class ApiHandlerOptionsBuilder: CoreOptions, IApiHandlerOptionsBuilder
    {
        protected Connection Connection { get; set; }

        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            Dependencies.AddDependency(() => ApiHandlerOptionDefaults.QueryFactory);
            Dependencies.AddDependency(() => JsonSerializer.Default);
            Dependencies.AddDependency(() => ContentType.Json);
            Dependencies.AddDependency(() => ApiHandlerOptionDefaults.Credentials);
            Dependencies.AddDependency<IClientFactory>(() => new DefaultClientFactory(Dependencies));
        }

        protected internal ApiHandlerOptionsBuilder(IDependencyCollection dependencies) : base(dependencies)
        {
            Dependencies.AddDependency(() => ApiHandlerOptionDefaults.QueryFactory);
            Dependencies.AddDependency(() => JsonSerializer.Default);
            Dependencies.AddDependency(() => ContentType.Json);
            Dependencies.AddDependency(() => ApiHandlerOptionDefaults.Credentials);
            Dependencies.AddDependency<IClientFactory>(() => new DefaultClientFactory(Dependencies));
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

            ApiHandlerOptionsRules.ValidateRetryAttempts(attemptCount);

            Connection.RetryAttempts = attemptCount;
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            Dependencies.AddDependency(() => logger);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            Dependencies.AddDependency(() => queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Dependencies.AddDependency(() => credentials);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency(() => authentication);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency<IAuthentication>(() => new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency<IAuthentication>(() => new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType content)
        {
            Dependencies.AddDependency(() => content);
        }

        public IApiHandlerOptions BuildOptions()
        {
            Dependencies.AddDependency<IConnectionSettings>(() => Connection);
            Dependencies.AddDependency<IRouteFactory>(() => new RouteFactory(Connection));

            IApiHandlerOptions options = new ApiHandlerOptions(Dependencies.BuildProvider(), Connection);

            return options;
        }
    }
}
