using DeltaWare.SereneApi.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace DeltaWare.SereneApi
{
    public class ApiHandlerOptions : IApiHandlerOptions, IDisposable
    {
        private readonly bool _disposeClient;

        public ILogger Logger { get; }

        public IQueryFactory QueryFactory { get; }

        public HttpClient HttpClient { get; }

        public uint RetryCount { get; }

        public ApiHandlerOptions(HttpClient httpClient, uint retryCount = 0, bool disposeClient = true)
        {
            HttpClient = httpClient;

            RetryCount = retryCount;

            _disposeClient = disposeClient;
        }

        public ApiHandlerOptions(HttpClient httpClient, ILogger logger, uint retryCount = 0, bool disposeClient = true) : this(httpClient, retryCount, disposeClient)
        {
            Logger = logger;
        }

        public ApiHandlerOptions(HttpClient httpClient, ILogger logger, IQueryFactory queryFactory, uint retryCount = 0, bool disposeClient = true) : this(httpClient, logger, retryCount, disposeClient)
        {
            QueryFactory = queryFactory;
        }

        #region IDisposable

        private bool _disposed;

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

            if (disposing && _disposeClient)
            {
                HttpClient.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
