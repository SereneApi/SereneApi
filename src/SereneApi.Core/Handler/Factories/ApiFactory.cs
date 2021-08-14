using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Options;
using SereneApi.Core.Options.Factories;
using System;
using System.Collections.Generic;

namespace SereneApi.Core.Handler.Factories
{
    /// <inheritdoc cref="IApiFactory"/>
    public class ApiFactory : IApiFactory
    {
        private readonly IConfigurationManager _configurationManager = new ConfigurationManager();

        private readonly Dictionary<Type, Type> _registeredHandlers = new();

        private readonly Dictionary<Type, IApiOptionsBuilder> _optionsBuilders = new();

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApi Build<TApi>() where TApi : class
        {
            CheckIfDisposed();

            if (!_optionsBuilders.TryGetValue(typeof(TApi), out IApiOptionsBuilder builder))
            {
                throw new ArgumentException($"{typeof(TApi).Name} has not been registered.");
            }

            IApiOptions options = builder.BuildOptions();

            TApi handler = (TApi)Activator.CreateInstance(_registeredHandlers[typeof(TApi)], options);

            return handler;
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">The Handler which will be configured and perform API calls.</typeparam>
        /// <param name="builder">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">Thrown when the specified API has already been registered.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        public IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(Action<IApiOptionsFactory> builder = null) where TApiHandler : IApiHandler, TApi
        {
            CheckIfDisposed();

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ApiOptionsFactory<TApiHandler> factory = _configurationManager.BuildApiOptionsFactory<TApiHandler>();

            factory.Dependencies.AddSingleton<IApiFactory>(() => this, Binding.Unbound);

            builder.Invoke(factory);

            _registeredHandlers.Add(typeof(TApi), typeof(TApiHandler));
            _optionsBuilders.Add(typeof(TApi), factory);

            return factory;
        }

        public IApiOptionsExtensions ExtendApi<TApi>()
        {
            CheckIfDisposed();

            if (!_optionsBuilders.TryGetValue(typeof(TApi), out IApiOptionsBuilder factory))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApi)}");
            }

            return (IApiOptionsExtensions)factory;
        }

        public void ExtendApi<TApi>(Action<IApiOptionsExtensions> builder) where TApi : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Invoke(ExtendApi<TApi>());
        }

        #region IDisposable

        private volatile bool _disposed;

        protected void CheckIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

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
                foreach (IApiOptionsBuilder builder in _optionsBuilders.Values)
                {
                    if (builder is IDisposable disposableBuilder)
                    {
                        disposableBuilder.Dispose();
                    }
                }

                if (_configurationManager is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
