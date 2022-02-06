using SereneApi.Core.Configuration.Provider;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Core.Configuration
{
    /// <summary>
    /// Contains the handlerConfiguration scope of a <see cref="HandlerConfigurationProvider"/>
    /// </summary>
    internal sealed class ApiConfigurationScope : IDisposable
    {
        /// <summary>
        /// The current handlerConfiguration scope.
        /// </summary>
        internal IDependencyScope Scope { get; }

        public ApiConfigurationScope(IDependencyScope scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <summary>
        /// Creates a new instance of <see cref="IApiSettings"/>.
        /// </summary>
        /// <typeparam name="TApiHandler">
        /// The handler the <see cref="IApiSettings"/> is intended for.
        /// </typeparam>
        /// <returns>A new instance of <see cref="ApiSettings"/>.</returns>
        public IApiSettings<TApiHandler> BuildApiSettings<TApiHandler>() where TApiHandler : IApiHandler
        {
            return new ApiSettings<TApiHandler>(Scope.BuildProvider());
        }

        /// <summary>
        /// Creates a new instance of <see cref="IApiSettings"/>.
        /// </summary>
        /// <param name="handlerType">The handler the <see cref="IApiSettings"/> is intended for.</param>
        /// <returns>A new instance of <see cref="ApiSettings"/>.</returns>
        public IApiSettings BuildApiSettings(Type handlerType)
        {
            Type genericOptions = typeof(ApiSettings<>).MakeGenericType(handlerType);

            return (IApiSettings)Activator.CreateInstance(genericOptions, Scope.BuildProvider());
        }

        #region IDisposable

        private volatile bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Scope.Dispose();

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}