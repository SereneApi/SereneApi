using SereneApi.Enums;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public class Dependency: IDependency, IDisposable
    {
        /// <inheritdoc cref="IDependency.Binding"/>
        public Binding Binding { get; }

        /// <inheritdoc cref="IDependency.Instance"/>
        public object Instance { get; }

        /// <inheritdoc cref="IDependency.Type"/>
        public Type Type { get; }

        /// <summary>
        /// Creates a new Instance of the <see cref="Dependency"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="binding"></param>
        internal Dependency(object instance, Binding binding = Binding.Bound)
        {
            Instance = instance;
            Type = instance.GetType();
            Binding = binding;
        }

        public object Clone()
        {
            CheckIfDisposed();

            return new Dependency(Instance, Binding.Unbound);
        }

        #region IDisposable

        private volatile bool _disposed;

        protected void CheckIfDisposed()
        {
            // TODO: Throw an exception if the HttpClient has been disposed of, at present there is no way to do this.
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(GetType));
            }
        }

        /// <summary>
        /// Disposes the current instance of the <see cref="Dependency{TDependency}"/>.
        /// </summary>
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

            if(disposing && Binding == Binding.Bound && Instance is IDisposable disposableImplementation)
            {
                disposableImplementation.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
