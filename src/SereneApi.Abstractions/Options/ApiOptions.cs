using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Options
{
    /// <inheritdoc cref="IApiOptions"/>
    [DebuggerDisplay("Source: {Connection.Source}")]
    public class ApiOptions: IApiOptions
    {
        #region Properties

        /// <inheritdoc cref="IApiOptions.Dependencies"/>
        public IDependencyProvider Dependencies { get; }

        /// <inheritdoc cref="IApiOptions.Connection"/>
        public IConnectionSettings Connection { get; set; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ApiOptions"/>
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used when making an API request.</param>
        /// <param name="connection">The <see cref="IConnectionSettings"/> used to make requests to the API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiOptions([NotNull] IDependencyProvider dependencies, [NotNull] IConnectionSettings connection)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
