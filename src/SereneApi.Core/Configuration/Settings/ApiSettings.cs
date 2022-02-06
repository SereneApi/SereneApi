using SereneApi.Core.Http;
using DeltaWare.Dependencies.Abstractions;
using System;
using System.Diagnostics;

namespace SereneApi.Core.Configuration.Settings
{
    [DebuggerDisplay("Source: {Connection.Source}")]
    public class ApiSettings : IApiSettings
    {
        #region Properties

        /// <inheritdoc cref="IApiSettings.Connection"/>
        public IConnectionSettings Connection { get; }

        public IDependencyProvider Dependencies { get; }

        #endregion Properties

        #region Constructors

        public ApiSettings(IDependencyProvider dependencies)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Connection = Dependencies.GetRequiredDependency<IConnectionSettings>();
        }

        #endregion Constructors

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