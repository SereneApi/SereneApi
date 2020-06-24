using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;
using System.Net.Http;

namespace SereneApi.Factories
{
    public class DefaultClientFactory: IClientFactory, IDisposable
    {
        private readonly HttpClient _client;

        public DefaultClientFactory(IDependencyCollection dependencies)
        {
            _client = HttpClientHelper.BuildHttpClient(dependencies);
        }

        public HttpClient BuildClient()
        {
            return _client;
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
                _client.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
