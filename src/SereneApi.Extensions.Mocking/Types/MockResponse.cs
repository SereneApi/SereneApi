using DeltaWare.Dependencies;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Mocking.Enums;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SereneApi.Abstractions.Responses;

namespace SereneApi.Extensions.Mocking.Types
{
    /// <inheritdoc cref="IMockResponse"/>
    internal class MockResponse: IMockResponse
    {
        private readonly IDependencyProvider _dependencies;

        private readonly IApiRequestContent _responseContent;

        /// <inheritdoc cref="IMockResponse.Status"/>
        public Status Status { get; }

        /// <inheritdoc cref="IMockResponse.Message"/>
        public string Message { get; }

        /// <inheritdoc cref="IMockResponse.Serializer"/>
        public ISerializer Serializer { get; }

        public MockResponse(IDependencyProvider dependencies, Status status, string message, IApiRequestContent responseContent)
        {
            _dependencies = dependencies;
            _responseContent = responseContent;

            Message = message;
            Status = status;

            Serializer = dependencies.GetDependency<ISerializer>();
        }

        /// <inheritdoc cref="IWhitelist.Validate"/>
        public Validity Validate(object value)
        {
            List<IWhitelist> whitelistDependencies = _dependencies.GetDependencies<IWhitelist>();

            // If 0 or any whitelist items return true. True is returned.

            foreach(IWhitelist whitelistDependency in whitelistDependencies)
            {
                Validity validity = whitelistDependency.Validate(value);

                if(validity == Validity.NotApplicable)
                {
                    continue;
                }

                return validity;
            }

            return Validity.NotApplicable;
        }

        /// <inheritdoc cref="IMockResponse"/>
        public async Task<IApiRequestContent> GetResponseContentAsync(CancellationToken cancellationToken = default)
        {
            if(_dependencies.TryGetDependency(out DelayedResponseDependency delay))
            {
                await delay.DelayAsync(cancellationToken);
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
