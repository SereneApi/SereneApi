namespace SereneApi.Types
{
    /// <summary>
    /// The Core of all Options, contains the <see cref="Dependencies"/> required for them to run.
    /// </summary>
    public abstract class CoreOptions
    {
        /// <summary>
        /// The Dependencies required for the Options to function.
        /// </summary>
        public DependencyCollection Dependencies { get; }

        protected CoreOptions()
        {
            Dependencies = new DependencyCollection();
        }

        protected CoreOptions(DependencyCollection dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
