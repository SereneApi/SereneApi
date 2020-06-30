using DeltaWare.Dependencies;
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

        protected internal ApiHandlerOptionsBuilder(IDependencyCollection dependencies) : base(dependencies)
        {
            Dependencies.AddScoped(() => ApiHandlerOptionDefaults.QueryFactory);
            Dependencies.AddScoped(() => JsonSerializer.Default);
            Dependencies.AddScoped(() => ContentType.Json);
            Dependencies.AddScoped(() => ApiHandlerOptionDefaults.Credentials);
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
            Dependencies.TryAddScoped<IConnectionSettings>(() => Connection);
            Dependencies.TryAddScoped<IRouteFactory>(p => new RouteFactory(p));
            Dependencies.TryAddScoped<IClientFactory>(p => new DefaultClientFactory(p));

            IApiHandlerOptions options = new ApiHandlerOptions(Dependencies.BuildProvider(), Connection);

            return options;
        }
    }
}
