using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Types
{
    public abstract class CoreOptions
    {
        /// <summary>
        /// The Dependencies required for the Options to function.
        /// </summary>
        public IDependencyCollection Dependencies { get; }

        protected CoreOptions()
        {
            Dependencies = new DependencyCollection();
        }

        protected CoreOptions(IDependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
