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
        #region Constructors

        public ApiHandlerOptionsBuilder()
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
            DependencyCollection.AddDependency(ContentType.Json);
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.Credentials);
        }

        protected internal ApiHandlerOptionsBuilder(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.QueryFactory);
            DependencyCollection.AddDependency(JsonSerializer.Default);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory());
            DependencyCollection.AddDependency(ContentType.Json);
            DependencyCollection.AddDependency(ApiHandlerOptionDefaults.Credentials);
        }

        #endregion

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseSource"/>
        public void UseSource(string source, string resource = null, string resourcePath = null)
        {
            ExceptionHelper.EnsureParameterIsNotNull(source, nameof(source));

            if(DependencyCollection.HasDependency<IConnectionInfo>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            IConnectionInfo connectionInfo = new ConnectionInfo(source, resource, resourcePath);

            DependencyCollection.AddDependency(connectionInfo);
            DependencyCollection.AddDependency<IRouteFactory>(new RouteFactory(connectionInfo));
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerOptionsBuilder.SetTimeoutPeriod</cref>
        /// </inheritdoc>
        public void SetTimeoutPeriod(int seconds)
        {
            if(!DependencyCollection.TryGetDependency(out IConnectionInfo connectionInfo))
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            if(connectionInfo.Timeout != default)
            {
                throw new MethodAccessException("A timeout has already been set.");
            }

            connectionInfo.SetTimeout(seconds);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.SetRetryAttempts"/>
        public void SetRetryAttempts(int attemptCount)
        {
            if(!DependencyCollection.TryGetDependency(out IConnectionInfo connectionInfo))
            {
                throw new MethodAccessException("Source information must be supplied fired.");
            }

            if(connectionInfo.RetryAttempts != default)
            {
                throw new MethodAccessException("The retry attempt count has already been set.");
            }

            connectionInfo.SetRetryAttempts(attemptCount);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddLogger"/>
        public void AddLogger(ILogger logger)
        {
            DependencyCollection.AddDependency(logger);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseQueryFactory"/>
        public void UseQueryFactory(IQueryFactory queryFactory)
        {
            DependencyCollection.AddDependency(queryFactory);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.UseCredentials"/>
        public void UseCredentials(ICredentials credentials)
        {
            DependencyCollection.AddDependency(credentials);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddAuthentication"/>
        public void AddAuthentication(IAuthentication authentication)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(authentication);
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBasicAuthentication"/>
        public void AddBasicAuthentication(string username, string password)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency<IAuthentication>(new BasicAuthentication(username, password));
        }

        /// <inheritdoc cref="IApiHandlerOptionsBuilder.AddBearerAuthentication"/>
        public void AddBearerAuthentication(string token)
        {
            if(DependencyCollection.HasDependency<IAuthentication>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency<IAuthentication>(new BearerAuthentication(token));
        }

        public void AcceptContentType(ContentType content)
        {
            DependencyCollection.AddDependency(content);
        }

        public IApiHandlerOptions BuildOptions()
        {
            // If no client factory has been provided the default will be used.
            DependencyCollection.TryAddDependency<IClientFactory>(new DefaultClientFactory(DependencyCollection));

            IConnectionInfo connectionInfo = DependencyCollection.GetDependency<IConnectionInfo>();

            IApiHandlerOptions options = new ApiHandlerOptions(DependencyCollection, connectionInfo);

            return options;
        }
    }
}
