using System;
using System.Diagnostics;
using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;

namespace SereneApi.Abstractions.Options
{
    [DebuggerDisplay("Source: {Connection.BaseAddress.ToString()}")]
    public class ApiOptions: IApiOptions
    {
        #region Properties

        /// <inheritdoc cref="IApiOptions.Dependencies"/>
        public IDependencyProvider Dependencies { get; }

        /// <inheritdoc cref="IApiOptions.Connection"/>
        public IConnectionSettings Connection { get; set; }

        #endregion
        #region Constructors

        public ApiOptions(IDependencyProvider dependencies, IConnectionSettings connection)
        {
            Dependencies = dependencies;
            Connection = connection;
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
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
