using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Connection;
using System;

namespace SereneApi.Core.Options.Factories
{
    public abstract class ApiOptionsFactory : IDisposable
    {
        public IDependencyCollection Dependencies { get; }
        public abstract Type HandlerType { get; }

        /// <summary>
        /// Specifies the connection settings for the API.
        /// </summary>
        protected ConnectionSettings ConnectionSettings { get; set; }

        protected ApiOptionsFactory(IDependencyCollection dependencies)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
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
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion IDisposable
    }
}