using SereneApi.Interfaces;
using System;
using System.Diagnostics;

namespace SereneApi.Types
{
    [DebuggerDisplay("Source: {Source.ToString()}")]
    public class ApiHandlerOptions : IApiHandlerOptions, IDisposable
    {
        #region Properties

        /// <inheritdoc cref="IApiHandlerOptions.Dependencies"/>
        public IDependencyCollection Dependencies { get; }

        /// <inheritdoc cref="IApiHandlerOptions.Source"/>
        public Uri Source { get; }

        /// <inheritdoc cref="IApiHandlerOptions.Resource"/>
        public string Resource { get; }

        #endregion
        #region Constructors

        public ApiHandlerOptions(IDependencyCollection dependencyCollection, Uri source, string resource)
        {
            Dependencies = dependencyCollection;
            Source = source;
            Resource = resource;
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
