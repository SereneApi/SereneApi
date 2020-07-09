using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Helpers;
using System;
using System.Collections.Generic;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiHandlerFactory"/>
    public class ApiHandlerFactory: IApiHandlerFactory
    {
        private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, IApiOptionsBuilder> _handlerOptions = new Dictionary<Type, IApiOptionsBuilder>();

        private readonly ISereneApiConfiguration _configuration;

        public ApiHandlerFactory()
        {
            _configuration = SereneApiConfiguration.Default;
        }

        public ApiHandlerFactory(ISereneApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApiDefinition Build<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(!_handlerOptions.TryGetValue(handlerType, out IApiOptionsBuilder builder))
            {
                throw new ArgumentException($"{nameof(TApiDefinition)} has not been registered.");
            }

            IApiOptions apiOptions = builder.BuildOptions();

            TApiDefinition handler = (TApiDefinition)Activator.CreateInstance(_handlers[handlerType], apiOptions);

            return handler;
        }

        /// <summary>
        /// Registers an <see cref="ApiHandler"/> implementation to the <see cref="ApiHandlerFactory"/>.
        /// The supplied <see cref="IApiOptionsConfigurator"/> will be used to build the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IApiOptionsConfigurator"/> that will be used to build the <see cref="ApiHandler"/>.</param>
        public IApiOptionsExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(Action<IApiOptionsConfigurator> factory) where TApiImplementation : IApiHandler, TApiDefinition
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiDefinition)}");
            }

            IApiOptionsBuilder configurator = _configuration.GetOptionsBuilder();

            configurator.Dependencies.AddSingleton<IApiHandlerFactory>(() => this, Binding.Unbound);

            factory.Invoke(configurator);

            _handlers.Add(handlerType, typeof(TApiImplementation));
            _handlerOptions.Add(handlerType, configurator);

            return new ApiOptionsExtensions(configurator.Dependencies);
        }

        public IApiOptionsExtensions ExtendApi<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            if(!_handlerOptions.TryGetValue(typeof(TApiDefinition), out IApiOptionsBuilder builder))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            return new ApiOptionsExtensions(builder.Dependencies);
        }

        public void ExtendApi<TApiDefinition>(Action<IApiOptionsExtensions> factory) where TApiDefinition : class
        {
            CheckIfDisposed();

            ExceptionHelper.EnsureParameterIsNotNull(factory, nameof(factory));

            IApiOptionsExtensions extensions = ExtendApi<TApiDefinition>();

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
