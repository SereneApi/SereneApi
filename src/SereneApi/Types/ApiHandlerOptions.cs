using SereneApi.Interfaces;
using System;
using System.Diagnostics;

namespace SereneApi.Types
{
    [DebuggerDisplay("Source: {ConnectionInfo.BaseAddress.ToString()}")]
    public class ApiHandlerOptions: IApiHandlerOptions, IDisposable
    {
        #region Properties

        /// <inheritdoc cref="IApiHandlerOptions.Dependencies"/>
        public IDependencyCollection Dependencies { get; }

        /// <inheritdoc cref="IApiHandlerOptions.ConnectionInfo"/>
        public IConnectionInfo ConnectionInfo { get; set; }

        #endregion
        #region Constructors

        public ApiHandlerOptions(IDependencyCollection dependencyCollection, IConnectionInfo connectionInfo)
        {
            Dependencies = dependencyCollection;
            ConnectionInfo = connectionInfo;
        }

        #endregion
        #region IDisposable

        private volatile bool _disposed;

        internal bool IsDisposed()
        {
            return _disposed;
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
                if(Dependencies is IDisposable disposableDependencyCollection)
                {
                    disposableDependencyCollection.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
