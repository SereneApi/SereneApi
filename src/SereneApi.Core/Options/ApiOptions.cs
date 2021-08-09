﻿using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Handler;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SereneApi.Core.Connection;

namespace SereneApi.Core.Options
{
    [DebuggerDisplay("Source: {Connection.Source}")]
    public class ApiOptions<TApiHandler> : IApiOptions<TApiHandler> where TApiHandler : IApiHandler
    {
        #region Properties

        /// <inheritdoc cref="IApiOptions.Connection"/>
        public IConnectionSettings Connection { get; }

        public IDependencyProvider Dependencies { get; }

        public bool ThrowExceptions { get; set; }

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
