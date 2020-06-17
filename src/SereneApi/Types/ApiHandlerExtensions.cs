using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public class ApiHandlerExtensions: CoreOptions, IApiHandlerExtensions, IDisposable
    {
        public ApiHandlerExtensions()
        {
        }

        public ApiHandlerExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
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
                DependencyCollection.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
