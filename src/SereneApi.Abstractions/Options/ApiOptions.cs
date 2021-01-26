using DeltaWare.Dependencies.Abstractions;
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
        private readonly IDependencyProvider _dependencies;

        #region Properties

        /// <inheritdoc cref="IApiOptions.Connection"/>
        public IConnectionConfiguration Connection { get; set; }

        public bool ThrowExceptions { get; set; }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ApiOptions"/>
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used when making an API request.</param>
        /// <param name="connection">The <see cref="IConnectionConfiguration"/> used to make requests to the API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiOptions([NotNull] IDependencyProvider dependencies, [NotNull] IConnectionConfiguration connection)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #endregion

        /// <inheritdoc cref="IApiOptions.RetrieveRequiredDependency{TDependency}"/>
        public TDependency RetrieveRequiredDependency<TDependency>()
        {
            return _dependencies.GetDependency<TDependency>();
        }

        /// <inheritdoc cref="IApiOptions.RetrieveRequiredDependency{TDependency}"/>
        public bool RetrieveDependency<TDependency>(out TDependency dependency)
        {
            return _dependencies.TryGetDependency(out dependency);
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
            if(_disposed)
            {
                return;
            }

            if(disposing)
            {
                _dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
