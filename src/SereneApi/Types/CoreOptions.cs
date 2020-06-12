namespace SereneApi.Types
{
    /// <summary>
    /// The Core of all Options, contains the <see cref="DependencyCollection"/> required for them to run.
    /// </summary>
    public abstract class CoreOptions
    {
        /// <summary>
        /// The Dependencies required for the Options to function.
        /// </summary>
        public DependencyCollection DependencyCollection { get; }

        protected CoreOptions()
        {
            DependencyCollection = new DependencyCollection();
        }

        protected CoreOptions(DependencyCollection dependencyCollection)
        {
            DependencyCollection = dependencyCollection;
        }
    }
}
