using SereneApi.Enums;
using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    /// <summary>
    /// Stores the Dependencies Instance to be used at a later point.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    public class Dependency<TDependency>: Dependency, IDependency<TDependency>
    {
        /// <inheritdoc cref="IDependency{TDependency}.Instance"/>
        public new TDependency Instance => (TDependency)base.Instance;

        /// <inheritdoc cref="IDependency{TDependency}.Type"/>
        public new Type Type => typeof(TDependency);

        /// <summary>
        /// Creates a new Instance of the <see cref="Dependency{TDependency}"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="binding"></param>
        internal Dependency(TDependency instance, Binding binding = Binding.Bound) : base(instance, binding)
        {
        }

        /// <summary>
        /// Clones the <see cref="Dependency{TDependecny}"/> the cloned <see cref="Dependency{TDependecny}"/> will be UNBOUND.
        /// </summary>
        /// <remarks>The cloned <see cref="Dependency{TDependecny}"/> will be disposed of if the source <see cref="Dependency{TDependecny}"/> is disposed.</remarks>
        /// <exception cref="ObjectDisposedException">Thrown if the <see cref="Dependency{TDependecny}"/> has been disposed of.</exception>
        public new object Clone()
        {
            CheckIfDisposed();

            return new Dependency<TDependency>(Instance, Binding.Unbound);
        }
    }
}
