using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Connection;
using System;

namespace SereneApi.Abstractions.Options
{
    public class ApiOptions<TApi> : IApiOptions<TApi> where TApi : class
    {
        #region Properties

        /// <inheritdoc cref="IApiOptions.Connection"/>
        public IConnectionSettings Connection { get; }

        public IDependencyProvider Dependencies { get; }

        public bool ThrowExceptions { get; set; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ApiOptions{TApi}"/>
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used when making an API request.</param>
        /// <param name="connection">The <see cref="IConnectionSettings"/> used to make requests to the API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiOptions(IDependencyProvider dependencies, IConnectionSettings connection)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
