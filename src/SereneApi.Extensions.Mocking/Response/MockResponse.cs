using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Response;
using SereneApi.Extensions.Mocking.Dependencies;
using SereneApi.Extensions.Mocking.Dependencies.Whitelist;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Response
{
    /// <inheritdoc cref="IMockResponse"/>
    internal class MockResponse : IMockResponse
    {
        private readonly IDependencyProvider _dependencies;

        private readonly IRequestContent _responseContent;

        /// <inheritdoc cref="IMockResponse.Status"/>
        public Status Status { get; }

        /// <summary>
        /// Creates a new instance of <seealso cref="MockResponse"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies available to the <see cref="MockResponse"/>.</param>
        /// <param name="status">The status of the response.</param>
        /// <param name="responseContent">,The content of the response.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        public MockResponse([NotNull] IDependencyProvider dependencies, Status status, IRequestContent responseContent)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _responseContent = responseContent;

            Status = status;
        }

        /// <inheritdoc cref="IWhitelist.Validate"/>
        public Validity Validate([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            List<IWhitelist> whitelistDependencies = _dependencies.GetDependencies<IWhitelist>();

            // If 0 or any whitelist items return true. True is returned.

            return whitelistDependencies
                .Select(whitelistDependency => whitelistDependency.Validate(value))
                .FirstOrDefault(validity => validity != Validity.NotApplicable);
        }

        /// <inheritdoc cref="IMockResponse.GetResponseContentAsync"/>
        public async Task<IRequestContent> GetResponseContentAsync(CancellationToken cancellationToken = default)
        {
            if (_dependencies.TryGetDependency(out DelayedResponseDependency delay))
            {
                await delay.DelayResponseAsync(cancellationToken);
            }

            return _responseContent;
        }

        #region IDisposeble

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
                _dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
