using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Handler.Extensions;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Helpers;
using System;
using System.Collections.Generic;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiHandlerFactory"/>
    public class ApiHandlerFactory: IApiHandlerFactory
    {
        private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, IOptionsBuilder> _handlerOptions = new Dictionary<Type, IOptionsBuilder>();

        private readonly IConfiguration _configuration = Configuration.Default;

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApiDefinition Build<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(!_handlerOptions.TryGetValue(handlerType, out IOptionsBuilder builder))
            {
                throw new ArgumentException($"{nameof(TApiDefinition)} has not been registered.");
            }

            IOptions options = builder.BuildOptions();

            TApiDefinition handler = (TApiDefinition)Activator.CreateInstance(_handlers[handlerType], options);

            return handler;
        }

        /// <summary>
        /// Registers an <see cref="ApiHandler"/> implementation to the <see cref="ApiHandlerFactory"/>.
        /// The supplied <see cref="IOptionsConfigurator"/> will be used to build the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IOptionsConfigurator"/> that will be used to build the <see cref="ApiHandler"/>.</param>
        public IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(Action<IOptionsConfigurator> factory) where TApiImplementation : IApiHandler, TApiDefinition
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiDefinition)}");
            }

            IOptionsBuilder configurator = _configuration.ApiHandler.GetOptionsBuilder();

            configurator.Dependencies.AddSingleton<IApiHandlerFactory>(() => this, Binding.Unbound);

            factory.Invoke(configurator);

            _handlers.Add(handlerType, typeof(TApiImplementation));
            _handlerOptions.Add(handlerType, configurator);

            return new ApiHandlerExtensions(configurator.Dependencies);
        }

        public IApiHandlerExtensions ExtendApiHandler<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            if(!_handlerOptions.TryGetValue(typeof(TApiDefinition), out IOptionsBuilder builder))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            return new ApiHandlerExtensions(builder.Dependencies);
        }

        public void ExtendApiHandler<TApiDefinition>(Action<IApiHandlerExtensions> factory) where TApiDefinition : class
        {
            CheckIfDisposed();

            ExceptionHelper.EnsureParameterIsNotNull(factory, nameof(factory));

            IApiHandlerExtensions extensions = ExtendApiHandler<TApiDefinition>();

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
                foreach(IOptionsBuilder builder in _handlerOptions.Values)
                {
                    builder.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
