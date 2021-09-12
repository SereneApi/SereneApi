using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Authorization;
using SereneApi.Core.Authorization.Types;
using SereneApi.Core.Configuration;
using SereneApi.Core.Connection;
using SereneApi.Core.Content;
using SereneApi.Core.Handler;
using SereneApi.Core.Helpers;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using System;
using System.Net;

namespace SereneApi.Core.Options.Factories
{
    public class ApiOptionsFactory<TApiHandler> : ApiOptionsFactory, IApiOptionsBuilder, IApiOptionsFactory, IApiOptionsExtensions, IDisposable where TApiHandler : IApiHandler
    {
        public override Type HandlerType { get; }

        public ApiOptionsFactory(IDependencyCollection dependencies) : base(dependencies)
        {
            HandlerType = typeof(TApiHandler);
        }

        public void AcceptContentType(ContentType type)
        {
            Dependencies.AddScoped(() => type);
        }

        public void AddAuthentication(IAuthorization authorization)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            Dependencies.AddScoped(() => authorization);
        }

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

        public void AddBearerAuthentication(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Dependencies.AddTransient<IAuthorization>(() => new BearerAuthorization(token));
        }

        public void AddConfiguration(IConnectionSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException(nameof(connectionSettings));
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IConfiguration configuration = provider.GetDependency<IConfiguration>();

            string resourcePath = connectionSettings.ResourcePath;

            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                if (resourcePath != string.Empty)
                {
                    resourcePath = configuration["ResourcePath"];
                }
            }

            ConnectionSettings connection =
                new ConnectionSettings(connectionSettings.BaseAddress, connectionSettings.Resource, resourcePath)
                {
                    Timeout = configuration.Get<int>("Timeout"),
                    RetryAttempts = configuration.Get<int>("RetryAttempts")
                };

            #region Timeout

            int timeout = connectionSettings.Timeout;

            if (timeout < 0)
            {
                throw new ArgumentException("The Timeout value must be greater than 0");
            }

            if (timeout != default)
            {
                connection.Timeout = timeout;
            }

            #endregion Timeout

            #region Retry Count

            int retryCount = connectionSettings.RetryAttempts;

            if (retryCount != default)
            {
                Rules.ValidateRetryAttempts(retryCount);

                connection.RetryAttempts = retryCount;
            }

            #endregion Retry Count

            ConnectionSettings = connection;
        }

        public void AddCredentials(ICredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            Dependencies.AddScoped(() => credentials);
        }

        public IApiOptions BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => ConnectionSettings);

            IApiOptions<TApiHandler> apiOptions = new ApiOptions<TApiHandler>(Dependencies.BuildProvider(), ConnectionSettings);

            return apiOptions;
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

        public void SetSource(string baseAddress, string resource = null, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IConfiguration configuration = provider.GetDependency<IConfiguration>();

            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                if (resourcePath != string.Empty)
                {
                    resourcePath = configuration["ResourcePath"];
                }
            }

            ConnectionSettings = new ConnectionSettings(baseAddress, resource, resourcePath)
            {
                Timeout = configuration.Get<int>("Timeout"),
                RetryAttempts = configuration.Get<int>("RetryAttempts")
            };
        }

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

        public void SetTimeout(int seconds, int attempts)
        {
            ConnectionSettings.Timeout = seconds;
            ConnectionSettings.RetryAttempts = attempts;
        }

        public void ThrowExceptions()
        {
            using IDependencyProvider dependencies = Dependencies.BuildProvider();

            Configuration.Configuration configuration = (Configuration.Configuration)dependencies.GetDependency<IConfiguration>();

            configuration.Add("ThrowExceptions", true);
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

        public void UseLogger(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Dependencies.AddScoped(() => logger);
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

        public void UseSerializer(ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            Dependencies.AddScoped(() => serializer);
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

        #endregion IDisposable
    }
}