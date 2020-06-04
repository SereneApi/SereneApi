using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Factories
{
    public class ApiHandlerFactory : IApiHandlerFactory, IDisposable
    {
        private readonly Dictionary<Type, IApiHandlerOptions> _handlers = new Dictionary<Type, IApiHandlerOptions>();

        private readonly Dictionary<Type, HttpClient> _clients = new Dictionary<Type, HttpClient>();

        /// <summary>
        /// Builds an Instance of the <see cref="ApiHandler"/>
        /// </summary>
        /// <typeparam name="TApiHandler"></typeparam>
        /// <returns></returns>
        public TApiHandler Build<TApiHandler>() where TApiHandler : ApiHandler
        {
            if (!_handlers.TryGetValue(typeof(TApiHandler), out IApiHandlerOptions options))
            {
                throw new ArgumentException($"{nameof(TApiHandler)} has not been registered.");
            }

            TApiHandler handler = (TApiHandler)Activator.CreateInstance(typeof(TApiHandler), options);

            return handler;
        }

        /// <summary>
        /// Registers an <see cref="IApiHandlerOptionsBuilder"/> against the <see cref="ApiHandler"/> which will be used when built.
        /// </summary>
        /// <typeparam name="TApiHandler"></typeparam>
        /// <param name="optionsAction"></param>
        public void RegisterHandlerOptions<TApiHandler>(Action<ApiHandlerOptionsBuilder> optionsAction) where TApiHandler : ApiHandler
        {
            Type handlerType = typeof(TApiHandler);

            if (_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiHandler)}");
            }

            HttpClient client = new HttpClient();

            // Disable client disposal for this ApiHandler as this factory has ownership of the client.
            ApiHandlerOptionsBuilder optionsBuilder = new ApiHandlerOptionsBuilder(client, false);

            optionsAction.Invoke(optionsBuilder);

            _handlers.Add(handlerType, optionsBuilder.BuildOptions());
            _clients.Add(handlerType, client);
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
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose of options anyway to be sure there are no other disposables contained.
                foreach (IApiHandlerOptions options in _handlers.Values)
                {
                    if (options is IDisposable disposableOptions)
                    {
                        disposableOptions.Dispose();
                    }
                }

                // Now dispose the clients.
                foreach (HttpClient client in _clients.Values)
                {
                    client.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
