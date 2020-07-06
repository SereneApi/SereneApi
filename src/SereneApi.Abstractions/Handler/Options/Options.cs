using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using System;
using System.Diagnostics;

namespace SereneApi.Abstractions.Handler.Options
{
    [DebuggerDisplay("Source: {Connection.BaseAddress.ToString()}")]
    public class Options: IOptions
    {
        #region Properties

        /// <inheritdoc cref="IOptions.Dependencies"/>
        public IDependencyProvider Dependencies { get; }

        /// <inheritdoc cref="IOptions.Connection"/>
        public IConnectionSettings Connection { get; set; }

        #endregion
        #region Constructors

        public Options(IDependencyProvider dependencies, IConnectionSettings connection)
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
