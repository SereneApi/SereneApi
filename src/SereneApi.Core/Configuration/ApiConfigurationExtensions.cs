using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Helpers;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Authorization;
using SereneApi.Core.Http.Authorization.Types;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Http.Responses.Handlers;
using SereneApi.Core.Serialization;
using System;
using System.Net;

namespace SereneApi.Core.Configuration
{
    public static class ApiConfigurationExtensions
    {
        public static void AcceptContentType(this IApiConfiguration apiConfiguration, ContentType type)
        {
            apiConfiguration.SetHandlerConfiguration(c =>
            {
                c.SetContentType(type);
            });
        }

        public static void AddAuthentication(this IApiConfiguration apiConfiguration, IAuthorization authorization)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            apiConfiguration.Dependencies.AddScoped(() => authorization);
        }

        public static void AddBasicAuthentication(this IApiConfiguration apiConfiguration, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            apiConfiguration.Dependencies.AddTransient<IAuthorization>(() => new BasicAuthorization(username, password));
        }

        public static void AddBearerAuthentication(this IApiConfiguration configuration, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            configuration.Dependencies.AddTransient<IAuthorization>(() => new BearerAuthorization(token));
        }

        [Obsolete("This has been superseded by AddConnectionSettings")]
        public static void AddConfiguration(this IApiConfiguration apiConfiguration, IConnectionSettings connectionSettings)
        {
            apiConfiguration.AddConnectionSettings(connectionSettings);
        }

        public static void AddConnectionSettings(this IApiConfiguration apiConfiguration, IConnectionSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException(nameof(connectionSettings));
            }

            apiConfiguration.Dependencies.AddSingleton<IConnectionSettings>(p =>
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

                ConnectionSettings connection = new ConnectionSettings(connectionSettings.BaseAddress, connectionSettings.Resource, resourcePath)
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
            });
        }

        public static void AddCredentials(this IApiConfiguration apiConfiguration, ICredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            apiConfiguration.Dependencies.AddSingleton(() => credentials);
        }

        public static void SetHandlerConfiguration(this IApiConfiguration apiConfiguration, Action<HandlerConfiguration> configuration)
        {
            apiConfiguration.Dependencies.Configure(configuration);
        }

        public static void SetRetryAttempts(this IApiConfiguration apiConfiguration, int attemptCount)
        {
            if (attemptCount < 0)
            {
                throw new ArgumentException("Retry attempts must be greater or equal to 0.");
            }

            if (!apiConfiguration.Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this method is called.");
            }

            apiConfiguration.Dependencies.Configure<IConnectionSettings>(c =>
            {
                ConnectionSettings connection = (ConnectionSettings)c;

                Rules.ValidateRetryAttempts(attemptCount);

                connection.RetryAttempts = attemptCount;
            });
        }

        public static void SetSource(this IApiConfiguration apiConfiguration, string baseAddress, string resource = null, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            apiConfiguration.Dependencies.AddSingleton<IConnectionSettings>(p =>
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
            });
        }

        public static void SetTimeout(this IApiConfiguration apiConfiguration, int seconds)
        {
            if (seconds <= 0)
            {
                throw new ArgumentException("A timeout value must be greater than 0.");
            }

            if (!apiConfiguration.Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this method is called.");
            }

            apiConfiguration.Dependencies.Configure<IConnectionSettings>(c =>
            {
                ConnectionSettings connection = (ConnectionSettings)c;

                connection.Timeout = seconds;
            });
        }

        public static void SetTimeout(this IApiConfiguration apiConfiguration, int seconds, int attempts)
        {
            if (!apiConfiguration.Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("Source must be provided before this method is called.");
            }

            apiConfiguration.Dependencies.Configure<IConnectionSettings>(c =>
            {
                ConnectionSettings connection = (ConnectionSettings)c;

                connection.Timeout = seconds;
                connection.RetryAttempts = attempts;
            });
        }

        public static void ThrowExceptions(this IApiConfiguration apiConfiguration)
        {
            apiConfiguration.SetHandlerConfiguration(c =>
            {
                c.SetThrowExceptions(true);
            });
        }

        public static void UseFailedResponseHandler(this IApiConfiguration apiConfiguration, IFailedResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            apiConfiguration.Dependencies.AddScoped(() => handler);
        }

        public static void UseFailedResponseHandler(this IApiConfiguration apiConfiguration, Func<IDependencyProvider, IFailedResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            apiConfiguration.Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        public static void UseLogger(this IApiConfiguration apiConfiguration, ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            apiConfiguration.Dependencies.AddScoped(() => logger);
        }

        public static void UseRequestHandler(this IApiConfiguration apiConfiguration, IRequestHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            apiConfiguration.Dependencies.AddScoped(() => handler);
        }

        public static void UseRequestHandler(this IApiConfiguration apiConfiguration, Func<IDependencyProvider, IRequestHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            apiConfiguration.Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        public static void UseResponseHandler(this IApiConfiguration apiConfiguration, IResponseHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            apiConfiguration.Dependencies.AddScoped(() => handler);
        }

        public static void UseResponseHandler(this IApiConfiguration apiConfiguration, Func<IDependencyProvider, IResponseHandler> handlerBuilder)
        {
            if (handlerBuilder == null)
            {
                throw new ArgumentNullException(nameof(handlerBuilder));
            }

            apiConfiguration.Dependencies.AddScoped(handlerBuilder.Invoke);
        }

        public static void UseSerializer(this IApiConfiguration apiConfiguration, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            apiConfiguration.Dependencies.AddScoped(() => serializer);
        }
    }
}