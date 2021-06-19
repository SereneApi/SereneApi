using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using System;
using System.Collections.Generic;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiFactory"/>
    public class ApiFactory : IApiFactory
    {
        private readonly Dictionary<Type, Type> _apiHandlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, IApiOptionsFactory> _apiOptionsFactories = new Dictionary<Type, IApiOptionsFactory>();

        private ISereneApiConfiguration _configuration;

        /// <summary>
        /// Creates a new instance of <see cref="ApiFactory"/>.
        /// </summary>
        public ApiFactory()
        {
            _configuration = SereneApiConfiguration.Default;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ApiFactory"/>.
        /// </summary>
        /// <param name="configuration">The default configuration that will be provided to all APIs.</param>
        public ApiFactory(ISereneApiConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApi Build<TApi>() where TApi : class
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApi);

            if (!_apiOptionsFactories.TryGetValue(handlerType, out IApiOptionsFactory factory))
            {
                throw new ArgumentException($"{nameof(TApi)} has not been registered.");
            }

            IApiOptions apiOptions = factory.BuildOptions();

            TApi handler = (TApi)Activator.CreateInstance(_apiHandlers[handlerType], apiOptions);

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
        public IApiOptionsExtensions RegisterApi<TApi, TApiHandler>(Action<IApiOptionsBuilder> builder) where TApiHandler : IApiHandler, TApi
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            CheckIfDisposed();

            Type handlerType = typeof(TApi);

            if (_apiHandlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApi)}");
            }

            IApiOptionsFactory factory = _configuration.BuildOptionsFactory();

            factory.Dependencies.AddSingleton<IApiFactory>(() => this, Binding.Unbound);

            builder.Invoke(factory);

            _apiHandlers.Add(handlerType, typeof(TApiHandler));
            _apiOptionsFactories.Add(handlerType, factory);

            return factory;
        }

        public IApiOptionsExtensions ExtendApi<TApi>()
        {
            CheckIfDisposed();

            if (!_apiOptionsFactories.TryGetValue(typeof(TApi), out IApiOptionsFactory factory))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApi)}");
            }

            return factory;
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <param name="builder">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public void ExtendApi<TApi>(Action<IApiOptionsExtensions> builder) where TApi : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Invoke(ExtendApi<TApi>());
        }

        /// <summary>
        /// Configures the default configuration for all APIs.
        /// </summary>
        /// <param name="configuration">The prevalent configuration for all APIs.</param>
        /// <exception cref="ArgumentException">Thrown if this is called after API registration or if it is called twice.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>These values can be overriden during API Registration.</remarks>
        public void ConfigureSereneApi(ISereneApiConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Configures the default configuration for all APIs.
        /// </summary>
        /// <param name="builder">The prevalent configuration for all APIs.</param>
        /// <exception cref="ArgumentException">Thrown if this is called after API registration or if it is called twice.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>These values can be overriden during API Registration.</remarks>
        public void ConfigureSereneApi(Action<ISereneApiConfiguration> builder)
        {
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
                foreach (IApiOptionsFactory factory in _apiOptionsFactories.Values)
                {
                    factory.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
