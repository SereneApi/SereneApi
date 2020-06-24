using SereneApi.Enums;
using SereneApi.Interfaces;
using System;
using System.Diagnostics;

namespace SereneApi.Types
{
    [DebuggerDisplay("{Binding} - {Type.Name}")]
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

        /// <summary>
        /// Clones the <see cref="Dependency"/> the cloned <see cref="Dependency"/> will be UNBOUND.
        /// </summary>
        /// <remarks>The cloned <see cref="Dependency"/> will be disposed of if the source <see cref="Dependency"/> is disposed.</remarks>
        /// <exception cref="ObjectDisposedException">Thrown if the <see cref="Dependency"/> has been disposed of.</exception>
        public object Clone()
        {
            CheckIfDisposed();

            return new Dependency(Instance, Binding.Unbound);
        }

        #region IDisposable

        private volatile bool _disposed;

        protected void CheckIfDisposed()
        {
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
