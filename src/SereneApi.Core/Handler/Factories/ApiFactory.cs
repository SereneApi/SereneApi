using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Settings;
using DeltaWare.Dependencies.Abstractions;
using System;
using System.Collections.Generic;

namespace SereneApi.Core.Handler.Factories
{
    /// <inheritdoc cref="IApiFactory"/>
    public class ApiFactory : IApiFactory
    {
        private readonly ApiConfigurationManager _configurationManager;

        private readonly Dictionary<Type, Type> _registeredHandlers = new();

        public ApiFactory()
        {
            _configurationManager = new ApiConfigurationManager(r => r.Dependencies.AddSingleton<IApiFactory>(() => this));
        }

        public TApi Build<TApi>() where TApi : class
        {
            CheckIfDisposed();

            Type handlerType = GetApiHandlerType<TApi>();

            IApiSettings settings = _configurationManager.BuildApiOptions(handlerType);

            TApi handler = (TApi)Activator.CreateInstance(_registeredHandlers[typeof(TApi)], settings);

            return handler;
        }

        public void ExtendApi<TApi>(Action<IApiConfiguration> configuration) where TApi : class
        {
            CheckIfDisposed();

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Type handlerType = GetApiHandlerType<TApi>();

            _configurationManager.AddApiPostConfiguration(handlerType, configuration);
        }

        public void RegisterApi<TApi, TApiHandler>(Action<IApiConfiguration> configuration) where TApiHandler : IApiHandler, TApi
        {
            CheckIfDisposed();

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configurationManager.AddApiConfiguration<TApiHandler>(configuration.Invoke);

            _registeredHandlers.Add(typeof(TApi), typeof(TApiHandler));
        }

        private Type GetApiHandlerType<TApi>() where TApi : class
        {
            Type apiType = typeof(TApi);

            if (!_registeredHandlers.TryGetValue(apiType, out Type handlerType))
            {
                throw new ArgumentException($"The specified API {apiType.Name} has not been registered.");
            }

            return handlerType;
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected void CheckIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _configurationManager.Dispose();
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}