using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public class ApiHandlerExtensions: CoreOptions, IApiHandlerExtensions, IDisposable
    {
        public ApiHandlerExtensions()
        {
        }

        public ApiHandlerExtensions(DependencyCollection dependencies) : base(dependencies)
        {
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
                Dependencies.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
