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
        #region Conclassors

        public ApiHandlerOptionsBuilder()
        {
            Dependencies.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            Dependencies.AddDependency(JsonSerializer.Default);
            Dependencies.AddDependency<IRouteFactory>(new RouteFactory());
            Dependencies.AddDependency(ContentType.Json);
            Dependencies.AddDependency(ApiHandlerOptionDefaults.Credentials);
        }

        protected internal ApiHandlerOptionsBuilder(DependencyCollection dependencies) : base(dependencies)
        {
            Dependencies.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            Dependencies.AddDependency(JsonSerializer.Default);
            Dependencies.AddDependency<IRouteFactory>(new RouteFactory());
            Dependencies.AddDependency(ContentType.Json);
            Dependencies.AddDependency(ApiHandlerOptionDefaults.Credentials);
        }

        #endregion

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            if(Dependencies.HasDependency<IConnectionSettings>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            IConnectionSettings connection = new Connection(source, resource, resourcePath);

            Dependencies.AddDependency(connection);
            Dependencies.AddDependency<IRouteFactory>(new RouteFactory(connection));
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeoutPeriod(int seconds)
        {
            if(!Dependencies.TryGetDependency(out IConnectionSettings connection))
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Dependencies.AddDependency(connection.SetTimeout(seconds));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.SetRetryAttempts"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(!Dependencies.TryGetDependency(out IConnectionSettings connection))
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            Dependencies.AddDependency(connection.SetRetryAttempts(attemptCount));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            Dependencies.AddDependency(logger);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            Dependencies.AddDependency(queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            Dependencies.AddDependency(credentials);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency(authentication);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency<IAuthentication>(new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(Dependencies.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency<IAuthentication>(new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType content)
        {
            Dependencies.AddDependency(content);
        }

        public IApiHandlerOptions BuildOptions()
        {
            // If no client factory has been provided the default will be used.
            if(!Dependencies.HasDependency<IClientFactory>())
            {
                Dependencies.AddDependency<IClientFactory>(new DefaultClientFactory(Dependencies));
            }

            IConnectionSettings connection = Dependencies.GetDependency<IConnectionSettings>();

            IApiHandlerOptions options = new ApiHandlerOptions(Dependencies, connection);

            return options;
        }
    }
}
