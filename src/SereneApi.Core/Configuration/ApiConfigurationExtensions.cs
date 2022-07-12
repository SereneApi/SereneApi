using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Helpers;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization.Types;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SereneApi.Core.Configuration
{
    public static class ApiConfigurationExtensions
    {
        public static void AddBearerAuthenticator<TApi, TDto>(this IApiConfiguration configuration, Func<TApi, Task<IApiResponse<TDto>>> apiCall, Func<TDto, TokenAuthResult> retrieveToken) where TApi : class, IDisposable where TDto : class
        {
            configuration.Dependencies
                .Register(p => new BearerAuthenticator<TApi, TDto>(p, apiCall, retrieveToken))
                .DefineAs<IAuthenticator>()
                .AsSingleton();
        }

        /// <summary>
        /// Sets the Accept ContentType Header.
        /// </summary>
        public static void AcceptContentType(this IApiConfiguration configuration, ContentType type)
        {
            configuration.SetHandlerConfiguration(c =>
            {
                c.SetContentType(type);
            });
        }

        public static void AddAuthentication(this IApiConfiguration configuration, IAuthentication authentication)
        {
            if (authentication == null)
            {
                throw new ArgumentNullException(nameof(authentication));
            }

            configuration.Dependencies
                .Register(() => authentication)
                .AsScoped();
        }

        public static void AddBasicAuthentication(this IApiConfiguration configuration, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            configuration.Dependencies
                .Register(() => new BasicAuthentication(username, password))
                .DefineAs<IAuthentication>()
                .AsTransient();
        }

        public static void AddBearerAuthentication(this IApiConfiguration configuration, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            configuration.Dependencies
                .Register(() => new BearerAuthentication(token))
                .DefineAs<IAuthentication>()
                .AsTransient();
        }

        [Obsolete("This has been superseded by AddConnectionSettings")]
        public static void AddConfiguration(this IApiConfiguration apiConfiguration, IConnectionSettings connectionSettings)
        {
            apiConfiguration.AddConnectionSettings(connectionSettings);
        }

        public static void AddConnectionSettings(this IApiConfiguration configuration, IConnectionSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException(nameof(connectionSettings));
            }

            configuration.Dependencies
                .Register(p =>
                {
                    HandlerConfiguration handlerConfiguration = p.GetRequiredDependency<HandlerConfiguration>();

                    string resourcePath = connectionSettings.ResourcePath;

                    if (string.IsNullOrWhiteSpace(resourcePath))
                    {
                        if (resourcePath != string.Empty)
                        {
                            resourcePath = handlerConfiguration.GetResourcePath();
                        }
                    }

                    ConnectionSettings connection =
                        new ConnectionSettings(connectionSettings.BaseAddress, connectionSettings.Resource, resourcePath)
                        {
                            Timeout = handlerConfiguration.GetTimeout(),
                            RetryAttempts = handlerConfiguration.GetRetryAttempts(),
                        };

                    int timeout = connectionSettings.Timeout;

                    if (timeout < 0)
                    {
                        throw new ArgumentException("The Timeout value must be greater than 0");
                    }

                    if (timeout != default)
                    {
                        connection.Timeout = timeout;
                    }

                    int retryCount = connectionSettings.RetryAttempts;

                    if (retryCount != default)
                    {
                        Rules.ValidateRetryAttempts(retryCount);

                        connection.RetryAttempts = retryCount;
                    }

                    return connection;
                })
                //.DefineAs<ConnectionSettings>()
                .DefineAs<IConnectionSettings>()
                .AsSingleton();

            //configuration.Dependencies
            //    .Register(p => p.GetRequiredDependency<ConnectionSettings>())
            //    .DefineAs<IConnectionSettings>()
            //    .AsTransient()
            //    .DoNotBind();
        }

        public static void AddCredentials(this IApiConfiguration configuration, ICredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            configuration.Dependencies.Register(() => credentials).AsSingleton();
        }

        public static void SetHandlerConfiguration(this IApiConfiguration configuration, Action<HandlerConfiguration> configurationAction)
        {
            configuration.Dependencies.Configure(configurationAction);
        }

        public static void SetRetryAttempts(this IApiConfiguration configuration, int attemptCount)
        {
            if (attemptCount < 0)
            {
                throw new ArgumentException("Retry attempts must be greater or equal to 0.");
            }

            if (!configuration.Dependencies.HasDependency<ConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this httpMethod is called.");
            }

            configuration.Dependencies.Configure<ConnectionSettings>(connection =>
            {
                Rules.ValidateRetryAttempts(attemptCount);

                connection.RetryAttempts = attemptCount;
            });
        }

        public static void SetSource(this IApiConfiguration configuration, string baseAddress, string resource = null, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            configuration.Dependencies
                .Register(p =>
                {
                    HandlerConfiguration handlerConfiguration = p.GetRequiredDependency<HandlerConfiguration>();

                    if (string.IsNullOrWhiteSpace(resourcePath))
                    {
                        if (resourcePath != string.Empty)
                        {
                            resourcePath = handlerConfiguration.GetResourcePath();
                        }
                    }

                    return new ConnectionSettings(baseAddress, resource, resourcePath)
                    {
                        Timeout = handlerConfiguration.GetTimeout(),
                        RetryAttempts = handlerConfiguration.GetRetryAttempts()
                    };
                })
                .DefineAs<IConnectionSettings>()
                .AsSingleton();

            //configuration.Dependencies
            //    .Register(p => p.GetRequiredDependency<ConnectionSettings>())
            //    .DefineAs<IConnectionSettings>()
            //    .AsTransient()
            //    .DoNotBind();
        }

        public static void SetTimeout(this IApiConfiguration configuration, int seconds)
        {
            if (seconds <= 0)
            {
                throw new ArgumentException("A timeout value must be greater than 0.");
            }

            if (!configuration.Dependencies.HasDependency<ConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this httpMethod is called.");
            }

            configuration.Dependencies.Configure<ConnectionSettings>(connection =>
            {
                connection.Timeout = seconds;
            });
        }

        public static void SetTimeout(this IApiConfiguration configuration, int seconds, int attempts)
        {
            if (!configuration.Dependencies.HasDependency<ConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this httpMethod is called.");
            }

            configuration.Dependencies.Configure<ConnectionSettings>(connection =>
            {
                connection.Timeout = seconds;
                connection.RetryAttempts = attempts;
            });
        }

        public static void ThrowExceptions(this IApiConfiguration configuration)
        {
            configuration.SetHandlerConfiguration(c =>
            {
                c.SetThrowExceptions(true);
            });
        }

        public static void UseFailedResponseHandler(this IApiConfiguration configuration, IFailedResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            configuration.Dependencies
                .Register(() => handler)
                .AsScoped();
        }

        public static void UseFailedResponseHandler(this IApiConfiguration configuration, Func<IDependencyProvider, IFailedResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            configuration.Dependencies
                .Register(handlerBuilder.Invoke)
                .AsScoped();
        }

        public static void UseLogger(this IApiConfiguration configuration, ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            configuration.Dependencies
                .Register(() => logger)
                .AsScoped();
        }

        public static void UseRequestHandler(this IApiConfiguration configuration, IRequestHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            configuration.Dependencies
                .Register(() => handler)
                .AsScoped();
        }

        public static void UseRequestHandler(this IApiConfiguration configuration, Func<IDependencyProvider, IRequestHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            configuration.Dependencies
                .Register(handlerBuilder.Invoke)
                .AsScoped();
        }

        public static void UseResponseHandler(this IApiConfiguration configuration, IResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            configuration.Dependencies
                .Register(() => handler)
                .AsScoped();
        }

        public static void UseResponseHandler(this IApiConfiguration configuration, Func<IDependencyProvider, IResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            configuration.Dependencies
                .Register(handlerBuilder.Invoke)
                .AsScoped();
        }

        public static void UseSerializer(this IApiConfiguration apiConfiguration, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            apiConfiguration.Dependencies
                .Register(() => serializer)
                .AsScoped();
        }
    }
}