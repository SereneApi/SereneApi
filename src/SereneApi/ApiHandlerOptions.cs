using DeltaWare.SereneApi.Interfaces;
using System;
using System.Diagnostics;

namespace DeltaWare.SereneApi
{
    [DebuggerDisplay("Source: {Source.ToString()}")]
    public class ApiHandlerOptions : IApiHandlerOptions, IDisposable
    {
        #region Properties

        /// <inheritdoc cref="IApiHandlerOptions.Dependencies"/>
        public IDependencyCollection Dependencies { get; }

        /// <inheritdoc cref="IApiHandlerOptions.Source"/>
        public Uri Source { get; }

        #endregion
        #region Constructors

        public ApiHandlerOptions(IDependencyCollection dependencyCollection, Uri source)
        {
            Dependencies = dependencyCollection;
            Source = source;
        }

        #endregion
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
                if (Dependencies is IDisposable disposableDependencyCollection)
                {
                    disposableDependencyCollection.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
