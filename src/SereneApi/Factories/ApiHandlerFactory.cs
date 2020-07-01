using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiHandlerFactory"/>
    public class ApiHandlerFactory: IApiHandlerFactory
    {
        private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, ApiHandlerOptionsBuilder> _handlerOptions = new Dictionary<Type, ApiHandlerOptionsBuilder>();

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApiDefinition Build<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(!_handlerOptions.TryGetValue(handlerType, out ApiHandlerOptionsBuilder builder))
            {
                throw new ArgumentException($"{nameof(TApiDefinition)} has not been registered.");
            }

            IApiHandlerOptions options = builder.BuildOptions();

            TApiDefinition handler = (TApiDefinition)Activator.CreateInstance(_handlers[handlerType], options);

            return handler;
        }

        /// <summary>
        /// Registers an <see cref="ApiHandler"/> implementation to the <see cref="ApiHandlerFactory"/>.
        /// The supplied <see cref="IApiHandlerOptionsBuilder"/> will be used to build the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="optionsAction">The <see cref="IApiHandlerOptionsBuilder"/> that will be used to build the <see cref="ApiHandler"/>.</param>
        public IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder> optionsAction) where TApiImplementation : ApiHandler, TApiDefinition
        {
            CheckIfDisposed();

            Type handlerType = typeof(TApiDefinition);

            if(_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiDefinition)}");
            }

            ApiHandlerOptionsBuilder builder = new ApiHandlerOptionsBuilder();

            builder.Dependencies.AddSingleton<IApiHandlerFactory>(() => this, Binding.Unbound);

            optionsAction.Invoke(builder);

            _handlers.Add(handlerType, typeof(TApiImplementation));
            _handlerOptions.Add(handlerType, builder);

            return new ApiHandlerExtensions(builder.Dependencies);
        }

        public IApiHandlerExtensions ExtendApiHandler<TApiDefinition>() where TApiDefinition : class
        {
            CheckIfDisposed();

            if(!_handlerOptions.TryGetValue(typeof(TApiDefinition), out ApiHandlerOptionsBuilder builder))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            return new ApiHandlerExtensions(builder.Dependencies);
        }

        public void ExtendApiHandler<TApiDefinition>(Action<IApiHandlerExtensions> extensionsAction) where TApiDefinition : class
        {
            CheckIfDisposed();

            ExceptionHelper.EnsureParameterIsNotNull(extensionsAction, nameof(extensionsAction));

            IApiHandlerExtensions extensions = ExtendApiHandler<TApiDefinition>();

            extensionsAction.Invoke(extensions);
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
                foreach(ApiHandlerOptionsBuilder builder in _handlerOptions.Values)
                {
                    builder.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
