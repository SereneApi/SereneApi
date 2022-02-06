using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace SereneApi.Core.Http.Client.Handler
{
    internal sealed class HandlerFactory : IHandlerFactory, IHandlerBuilder, IDisposable
    {
        private readonly List<DelegatingHandler> _additionalHandlers = new();
        private readonly object _handlerBuilderLock = new();
        public IReadOnlyList<DelegatingHandler> AdditionalHandlers => _additionalHandlers;
        public HttpMessageHandler PrimaryHandler { get; set; }
        private HttpMessageHandler ConfiguredHandler { get; set; }

        public HandlerFactory(ICredentials credentials = null)
        {
            if (credentials == null)
            {
                PrimaryHandler = new HttpClientHandler();
            }
            else
            {
                PrimaryHandler = new HttpClientHandler
                {
                    Credentials = credentials
                };
            }
        }

        public void AddHandler(DelegatingHandler handler)
        {
            _additionalHandlers.Add(handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public void AddHandler<THandler>() where THandler : DelegatingHandler
        {
            AddHandler(Activator.CreateInstance<THandler>());
        }

        public HttpMessageHandler BuildHandler()
        {
            if (PrimaryHandler == null)
            {
                throw new ArgumentNullException(nameof(PrimaryHandler));
            }

            lock (_handlerBuilderLock)
            {
                return InternalBuildHandler();
            }
        }

        private HttpMessageHandler InternalBuildHandler()
        {
            if (ConfiguredHandler != null)
            {
                return ConfiguredHandler;
            }

            HttpMessageHandler innerHandler = PrimaryHandler;

            for (int i = 0; i < _additionalHandlers.Count; i++)
            {
                DelegatingHandler handler = _additionalHandlers[i];

                if (handler.InnerHandler != null)
                {
                    throw new Exception();
                }

                handler.InnerHandler = innerHandler;

                innerHandler = _additionalHandlers[i];
            }

            ConfiguredHandler = innerHandler;

            return ConfiguredHandler;
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            PrimaryHandler?.Dispose();

            foreach (DelegatingHandler handler in _additionalHandlers)
            {
                handler.Dispose();
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}