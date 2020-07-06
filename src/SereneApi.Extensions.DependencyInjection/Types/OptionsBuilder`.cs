using DeltaWare.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Types;
using SereneApi.Extensions.DependencyInjection.Helpers;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <inheritdoc cref="IOptionsConfigurator{TApiDefinition}"/>
    internal class OptionsBuilder<TApiDefinition>: OptionsBuilder, IOptionsBuilder<TApiDefinition>, IOptionsConfigurator<TApiDefinition> where TApiDefinition : class
    {
        public Type HandlerType => typeof(TApiDefinition);

        /// <inheritdoc cref="IOptionsConfigurator{TApiDefinition}.UseConfiguration"/>
        public void UseConfiguration(IConfiguration configuration)
        {
            if(Dependencies.HasDependency<IConnectionSettings>())
            {
                throw new MethodAccessException("This method cannot be called twice");
            }

            string source = configuration.Get<string>(ConfigurationConstants.SourceKey, ConfigurationConstants.SourceIsRequired);
            string resource = configuration.Get<string>(ConfigurationConstants.ResourceKey, ConfigurationConstants.ResourceIsRequired);
            string resourcePath = configuration.Get<string>(ConfigurationConstants.ResourcePathKey, ConfigurationConstants.ResourcePathIsRequired);

            using IDependencyProvider provider = Dependencies.BuildProvider();

            IApiHandlerConfiguration handlerConfiguration = provider.GetDependency<IApiHandlerConfiguration>();

            if(string.IsNullOrWhiteSpace(resourcePath))
            {
                if(resourcePath != string.Empty)
                {
                    resourcePath = handlerConfiguration.ResourcePath;
                }
            }

            Connection = new Connection(source, resource, resourcePath)
            {
                Timeout = handlerConfiguration.Timeout,
                RetryAttempts = handlerConfiguration.RetryCount
            };

            #region Timeout

            if(configuration.ContainsKey(ConfigurationConstants.TimeoutKey))
            {
                int timeout = configuration.Get<int>(ConfigurationConstants.TimeoutKey, ConfigurationConstants.TimeoutIsRequired);

                if(timeout < 0)
                {
                    throw new ArgumentException("The Timeout value must be greater than 0");
                }

                if(timeout != default)
                {
                    Connection.Timeout = timeout;
                }
            }

            #endregion
            #region Retry Count

            if(!configuration.ContainsKey(ConfigurationConstants.RetryCountKey))
            {
                return;
            }

            int retryCount = configuration.Get<int>(ConfigurationConstants.RetryCountKey, ConfigurationConstants.RetryIsRequired);

            if(retryCount == default)
            {
                return;
            }

            Rules.ValidateRetryAttempts(retryCount);

            Connection.RetryAttempts = retryCount;

            #endregion
        }

        /// <inheritdoc cref="IOptionsConfigurator{TApiDefinition}.AddLoggerFactory"/>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            ExceptionHelper.EnsureParameterIsNotNull(loggerFactory, nameof(loggerFactory));

            Dependencies.AddScoped(loggerFactory.CreateLogger<TApiDefinition>);
        }

        /// <summary>
        /// Builds the <see cref="IOptions"/> for the specified <see cref="ApiHandler"/>.
        /// </summary>
        public new IOptions<TApiDefinition> BuildOptions()
        {
            Dependencies.AddScoped<IConnectionSettings>(() => Connection);

            IOptions<TApiDefinition> options = new Options<TApiDefinition>(Dependencies.BuildProvider(), Connection);

            return options;
        }
    }
}
