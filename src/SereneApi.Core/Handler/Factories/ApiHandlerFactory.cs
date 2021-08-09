using SereneApi.Core.Configuration;
using SereneApi.Core.Options.Factory;
using System;

namespace SereneApi.Core.Handler.Factories
{
    /// <inheritdoc cref="IApiFactory"/>
    public class ApiHandlerFactory : IApiFactory
    {
        /// <inheritdoc>
        ///     <cref>IApiHandlerFactory.Build</cref>
        /// </inheritdoc>
        public TApi Build<TApi>() where TApi : class
        {
            return null;
        }

        /// <summary>
        /// Registers an API definition to an API handler allowing for Dependency Injection of the specified API.
        /// </summary>
        /// <typeparam name="TApi">The API to be associated to a Handler.</typeparam>
        /// <typeparam name="TApiHandler">The Handler which will be configured and perform API calls.</typeparam>
        /// <param name="builder">Configures the API Handler using the provided configuration.</param>
        /// <exception cref="ArgumentException">Thrown when the specified API has already been registered.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value has been provided.</exception>
        public void RegisterApi<TApi, TApiHandler>(Action<IApiOptionsFactory> builder = null) where TApiHandler : IApiHandler, TApi
        {
            IApiOptionsBuilder<TApiHandler> optionsBuilder = ConfigurationManager.BuildApiOptionsFactory<TApiHandler>();
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
            }

            _disposed = true;
        }

        #endregion
    }
}
