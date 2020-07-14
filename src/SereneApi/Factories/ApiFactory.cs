using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiFactory"/>
    public class ApiFactory: IApiFactory
    {
        private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, IApiOptionsBuilder> _handlerOptions = new Dictionary<Type, IApiOptionsBuilder>();

        private readonly ISereneApiConfiguration _configuration;

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
        public ApiFactory([NotNull] ISereneApiConfiguration configuration)
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

            if(!_handlerOptions.TryGetValue(handlerType, out IApiOptionsBuilder builder))
            {
                throw new ArgumentException($"{nameof(TApi)} has not been registered.");
            }

            IApiOptions apiOptions = builder.BuildOptions();

            TApi handler = (TApi)Activator.CreateInstance(_handlers[handlerType], apiOptions);

            return handler;
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">The Handler which will be configured and perform API calls.</typeparam>
        /// <param name="factory">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">Thrown when the specified API has already been registered.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        public IApiOptionsExtensions RegisterApi<TApi, TApiHandler>([NotNull] Action<IApiOptionsConfigurator> factory) where TApiHandler : IApiHandler, TApi
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            CheckIfDisposed();

            Type handlerType = typeof(TApi);

            if(_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApi)}");
            }

            IApiOptionsBuilder configurator = _configuration.GetOptionsBuilder();

            configurator.Dependencies.AddSingleton<IApiFactory>(() => this, Binding.Unbound);

            factory.Invoke(configurator);

            _handlers.Add(handlerType, typeof(TApiHandler));
            _handlerOptions.Add(handlerType, configurator);

            return new ApiOptionsExtensions(configurator.Dependencies);
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public IApiOptionsExtensions ExtendApi<TApi>() where TApi : class
        {
            CheckIfDisposed();

            if(!_handlerOptions.TryGetValue(typeof(TApi), out IApiOptionsBuilder builder))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApi)}");
            }

            return new ApiOptionsExtensions(builder.Dependencies);
        }

        /// <summary>
        /// Allows extensions to be implemented for the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API that will be extended.</typeparam>
        /// <param name="factory">Configures the API extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        public void ExtendApi<TApi>([NotNull] Action<IApiOptionsExtensions> factory) where TApi : class
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            CheckIfDisposed();

            IApiOptionsExtensions extensions = ExtendApi<TApi>();

            factory.Invoke(extensions);
        }

        #region IDisposable

        private volatile bool _disposed;

        protected void CheckIfDisposed()
        {
            if(_disposed)
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
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                foreach(IApiOptionsBuilder builder in _handlerOptions.Values)
                {
                    builder.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
