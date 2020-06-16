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
    }
}
