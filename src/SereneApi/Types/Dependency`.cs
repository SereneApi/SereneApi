using SereneApi.Enums;
using SereneApi.Interfaces;
using System;
using System.Diagnostics;

namespace SereneApi.Types
{
    /// <summary>
    /// Stores the Dependencies Instance to be used at a later point.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    [DebuggerDisplay("Type:{Type}; Binding:{Binding}")]
    public class Dependency<TDependency> : IDependency<TDependency>, IDisposable
    {
        /// <inheritdoc cref="IDependency.Binding"/>
        public Binding Binding { get; }

        /// <inheritdoc cref="IDependency{TDependency}.Instance"/>
        public TDependency Instance { get; }

        /// <inheritdoc cref="IDependency{TDependency}.Type"/>
        public Type Type => typeof(TDependency);

        /// <summary>
        /// Creates a new Instance of the <see cref="Dependency{TDependency}"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="binding"></param>
        internal Dependency(TDependency instance, Binding binding = Binding.Bound)
        {
            Instance = instance;
            Binding = binding;
        }

        #region IDisposable

        private volatile bool _disposed;

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
            if (_disposed)
            {
                return;
            }

            if (disposing && Binding == Binding.Bound && Instance is IDisposable disposableImplementation)
            {
                disposableImplementation.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
