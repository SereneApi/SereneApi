using DeltaWare.Dependencies.Abstractions;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IApiHandlerFactory"/>
    public class ApiHandlerFactory: IApiHandlerFactory, IDisposable
    {
        private readonly Dictionary<Type, Type> _handlers = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, Action<IApiHandlerOptionsBuilder>> _handlerOptions = new Dictionary<Type, Action<IApiHandlerOptionsBuilder>>();

        private readonly Dictionary<Type, IApiHandlerExtensions> _handlerExtensions = new Dictionary<Type, IApiHandlerExtensions>();

        public ApiHandlerFactory()
        {
        }

        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApiDefinition Build<TApiDefinition>() where TApiDefinition : class
        {
            Type handlerType = typeof(TApiDefinition);

            if(!_handlerOptions.TryGetValue(handlerType, out Action<IApiHandlerOptionsBuilder> optionsBuilderAction))
            {
                throw new ArgumentException($"{nameof(TApiDefinition)} has not been registered.");
            }

            ApiHandlerExtensions extensions = (ApiHandlerExtensions)_handlerExtensions[handlerType];

            ApiHandlerOptionsBuilder options = new ApiHandlerOptionsBuilder(extensions.Dependencies);

            optionsBuilderAction.Invoke(options);

            TApiDefinition handler = (TApiDefinition)Activator.CreateInstance(_handlers[handlerType], options.BuildOptions());

            return handler;
        }

        /// <summary>
        /// Registers an <see cref="ApiHandler"/> implementation to the <see cref="ApiHandlerFactory"/>.
        /// The supplied <see cref="IApiHandlerOptionsBuilder"/> will be used to build the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="optionsAction">The <see cref="IApiHandlerOptionsBuilder"/> that will be used to build the <see cref="ApiHandler"/>.</param>
        public IApiHandlerExtensions RegisterApiHandler<TApiDefinition, TApiImplementation>(Action<IApiHandlerOptionsBuilder> optionsAction) where TApiImplementation : ApiHandler, TApiDefinition
        {
            Type handlerType = typeof(TApiDefinition);

            if(_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiDefinition)}");
            }

            ApiHandlerExtensions extensions = new ApiHandlerExtensions();

            extensions.Dependencies.AddDependency(() => this, Binding.Unbound);

            _handlers.Add(handlerType, typeof(TApiImplementation));
            _handlerExtensions.Add(handlerType, extensions);
            _handlerOptions.Add(handlerType, optionsAction);

            return extensions;
        }

        public IApiHandlerExtensions ExtendApiHandler<TApiDefinition>() where TApiDefinition : class
        {
            if(!_handlerExtensions.TryGetValue(typeof(TApiDefinition), out IApiHandlerExtensions extensions))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            return extensions;
        }

        public void ExtendApiHandler<TApiDefinition>(Action<IApiHandlerExtensions> extensionsAction) where TApiDefinition : class
        {
            ExceptionHelper.EnsureParameterIsNotNull(extensionsAction, nameof(extensionsAction));

            if(!_handlerExtensions.TryGetValue(typeof(TApiDefinition), out IApiHandlerExtensions extensions))
            {
                throw new ArgumentException($"Could not find any registered extensions to {typeof(TApiDefinition)}");
            }

            extensionsAction.Invoke(extensions);
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
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                foreach(IApiHandlerExtensions handlerExtensions in _handlerExtensions.Values)
                {
                    if(handlerExtensions is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
